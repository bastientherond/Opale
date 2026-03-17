using Opale;

namespace Opale.Samples.Examples;

// ── Domain models ─────────────────────────────────────────────────────────────

public record Order(Guid Id, string CustomerId, List<OrderLine> Lines);
public record OrderLine(string ProductId, int Quantity, decimal UnitPrice);
public record Invoice(Guid OrderId, string CustomerId, decimal Total, DateTime IssuedAt);

public enum OrderError
{
    OrderNotFound,
    CustomerNotFound,
    EmptyOrder,
    InsufficientStock,
    PaymentDeclined,
}

// ── Fake repositories ─────────────────────────────────────────────────────────

file static class OrderRepository
{
    private static readonly Dictionary<Guid, Order> _store = new()
    {
        [Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001")] = new Order(
            Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
            "cust-1",
            [
                new OrderLine("prod-a", 2, 15.00m),
                new OrderLine("prod-c", 1, 45.00m),
            ]),
        [Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002")] = new Order(
            Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"),
            "cust-unknown",   // customer doesn't exist
            [new OrderLine("prod-a", 1, 15.00m)]),
        [Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003")] = new Order(
            Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"),
            "cust-2",
            []),  // empty order
    };

    public static Result<Order, OrderError> FindById(Guid id) =>
        _store.TryGetValue(id, out var order)
            ? Result<Order, OrderError>.Ok(order)
            : Result<Order, OrderError>.Fail(OrderError.OrderNotFound);
}

file static class CustomerRepository
{
    private static readonly HashSet<string> _customers = ["cust-1", "cust-2"];

    public static Result<string, OrderError> FindById(string customerId) =>
        _customers.Contains(customerId)
            ? Result<string, OrderError>.Ok(customerId)
            : Result<string, OrderError>.Fail(OrderError.CustomerNotFound);
}

file static class StockService
{
    private static readonly Dictionary<string, int> _stock = new()
    {
        ["prod-a"] = 100,
        ["prod-b"] = 0,   // out of stock
        ["prod-c"] = 50,
    };

    public static Result<Order, OrderError> CheckStock(Order order)
    {
        foreach (var line in order.Lines)
        {
            var available = _stock.GetValueOrDefault(line.ProductId, 0);
            if (available < line.Quantity)
                return Result<Order, OrderError>.Fail(OrderError.InsufficientStock);
        }
        return Result<Order, OrderError>.Ok(order);
    }
}

file static class PaymentGateway
{
    public static async Task<Result<Invoice, OrderError>> ChargeAsync(Order order)
    {
        await Task.Delay(10); // simulate payment latency

        var total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);

        // Simulate payment declined for very large orders
        if (total > 500m)
            return Result<Invoice, OrderError>.Fail(OrderError.PaymentDeclined);

        return Result<Invoice, OrderError>.Ok(
            new Invoice(order.Id, order.CustomerId, total, DateTime.UtcNow));
    }
}

// ── Order processing service ──────────────────────────────────────────────────

file static class OrderService
{
    public static async Task<Result<Invoice, OrderError>> ProcessAsync(Guid orderId)
    {
        var orderResult = OrderRepository.FindById(orderId);

        // Validate order is non-empty
        var validated = orderResult.Bind(order =>
            order.Lines.Count == 0
                ? Result<Order, OrderError>.Fail(OrderError.EmptyOrder)
                : Result<Order, OrderError>.Ok(order));

        // Validate customer exists
        var customerChecked = validated.Bind(order =>
            CustomerRepository.FindById(order.CustomerId)
                .Map(_ => order));

        // Check stock for all lines
        var stockChecked = customerChecked.Bind(StockService.CheckStock);

        if (stockChecked.IsFailure)
            return Result<Invoice, OrderError>.Fail(stockChecked.Error);

        // Process payment (async)
        return await PaymentGateway.ChargeAsync(stockChecked.Value);
    }
}

// ── Example entry point ───────────────────────────────────────────────────────

public static class OrderProcessingExample
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Example 4: Order Processing Pipeline ===");
        Console.WriteLine();

        var scenarios = new[]
        {
            (Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"), "Order 1 — valid"),
            (Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"), "Order 2 — unknown customer"),
            (Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"), "Order 3 — empty order"),
            (Guid.NewGuid(),                                     "Order 4 — not found"),
        };

        foreach (var (id, label) in scenarios)
        {
            var result = await OrderService.ProcessAsync(id);

            result
                .OnSuccess(inv => Console.WriteLine($"  [OK]   {label}: Invoice #{inv.OrderId} — Total {inv.Total:C}"))
                .OnFailure(err => Console.WriteLine($"  [FAIL] {label}: {err}"));
        }

        Console.WriteLine();

        // ── Demonstrate Recover: fallback to manual review on stock failure ────

        Console.WriteLine("  Recovery: route InsufficientStock to manual review queue:");

        // Temporarily simulate a stock-problem order
        var stockFailure = Result<Invoice, OrderError>.Fail(OrderError.InsufficientStock);

        var handled = stockFailure
            .Recover(err => err == OrderError.InsufficientStock
                ? new Invoice(Guid.Empty, "manual-review", 0m, DateTime.UtcNow)
                : throw new InvalidOperationException("Unhandled error"));

        Console.WriteLine(handled.IsSuccess
            ? $"  Routed to manual review (customer={handled.Value.CustomerId})"
            : "  Could not recover.");

        Console.WriteLine();

        // ── Demonstrate MapError: translate domain errors to HTTP status codes ──

        Console.WriteLine("  MapError: translate OrderError → HTTP status codes:");

        var errors = new[]
        {
            OrderError.OrderNotFound,
            OrderError.CustomerNotFound,
            OrderError.PaymentDeclined,
            OrderError.InsufficientStock,
        };

        foreach (var err in errors)
        {
            var httpStatus = Result<Invoice, OrderError>.Fail(err)
                .MapError(e => e switch
                {
                    OrderError.OrderNotFound    => 404,
                    OrderError.CustomerNotFound => 422,
                    OrderError.PaymentDeclined  => 402,
                    _                           => 500,
                });

            Console.WriteLine($"  {err,-20} → HTTP {httpStatus.Error}");
        }

        Console.WriteLine();
    }
}
