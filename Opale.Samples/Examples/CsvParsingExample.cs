using Opale;

namespace Opale.Samples.Examples;

// ── Domain models ─────────────────────────────────────────────────────────────

public record Product(int Id, string Name, decimal Price, int Stock);

public record ParseError(int Line, string Reason);

// ── CSV Parser ────────────────────────────────────────────────────────────────

file static class CsvParser
{
    // Parse a single CSV line into a Product
    public static Result<Product, ParseError> ParseLine(string line, int lineNumber)
    {
        var parts = line.Split(',');

        if (parts.Length != 4)
            return Result<Product, ParseError>.Fail(
                new ParseError(lineNumber, $"Expected 4 columns, got {parts.Length}"));

        if (!int.TryParse(parts[0].Trim(), out var id))
            return Result<Product, ParseError>.Fail(
                new ParseError(lineNumber, $"Invalid id: '{parts[0].Trim()}'"));

        var name = parts[1].Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result<Product, ParseError>.Fail(
                new ParseError(lineNumber, "Name cannot be empty"));

        if (!decimal.TryParse(parts[2].Trim(), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var price))
            return Result<Product, ParseError>.Fail(
                new ParseError(lineNumber, $"Invalid price: '{parts[2].Trim()}'"));

        if (!int.TryParse(parts[3].Trim(), out var stock))
            return Result<Product, ParseError>.Fail(
                new ParseError(lineNumber, $"Invalid stock: '{parts[3].Trim()}'"));

        return Result<Product, ParseError>.Ok(new Product(id, name, price, stock));
    }

    // Parse all lines, collecting both successes and failures
    public static (List<Product> Products, List<ParseError> Errors) ParseAll(string[] lines)
    {
        var products = new List<Product>();
        var errors = new List<ParseError>();

        foreach (var (line, index) in lines.Select((l, i) => (l, i + 1)))
        {
            ParseLine(line, index)
                .OnSuccess(p => products.Add(p))
                .OnFailure(e => errors.Add(e));
        }

        return (products, errors);
    }
}

// ── Example entry point ───────────────────────────────────────────────────────

public static class CsvParsingExample
{
    // Simulated CSV content (would normally come from File.ReadAllLines)
    private static readonly string[] CsvLines =
    [
        "1, Widget A, 9.99, 100",
        "2, Widget B, 24.50, 0",       // stock = 0 is valid
        "3, , 5.00, 10",               // empty name
        "4, Gadget X, not-a-price, 5", // invalid price
        "5, Gadget Y, 49.99, 3",
        "6, Broken Row",               // too few columns
        "7, Widget C, 14.99, 50",
    ];

    public static void Run()
    {
        Console.WriteLine("=== Example 2: CSV Parsing ===");
        Console.WriteLine();

        // ── Basic per-line parsing ────────────────────────────────────────────

        Console.WriteLine("  Per-line parsing with Match:");
        foreach (var (line, i) in CsvLines.Select((l, i) => (l, i + 1)))
        {
            CsvParser.ParseLine(line, i)
                .OnSuccess(p => Console.WriteLine($"  [OK]   Line {i}: {p.Name} @ {p.Price:C} (stock={p.Stock})"))
                .OnFailure(e => Console.WriteLine($"  [FAIL] Line {e.Line}: {e.Reason}"));
        }

        Console.WriteLine();

        // ── Batch parsing: separate valid rows from errors ────────────────────

        var (products, errors) = CsvParser.ParseAll(CsvLines);

        Console.WriteLine($"  Batch result: {products.Count} valid rows, {errors.Count} errors");

        // ── Further processing using Map/Bind on successful results ───────────

        Console.WriteLine();
        Console.WriteLine("  In-stock products with price > 10 (chained transforms):");

        var inStockLines = CsvLines
            .Select((l, i) => CsvParser.ParseLine(l, i + 1))
            .Where(r => r.IsSuccessAnd(p => p.Stock > 0 && p.Price > 10m))
            .Select(r => r.Map(p => $"{p.Name} — {p.Price:C}"))
            .Select(r => r.GetValueOrDefault("(error)"));

        foreach (var label in inStockLines)
            Console.WriteLine($"  • {label}");

        // ── Result.Try wrapping exception-prone I/O ───────────────────────────

        Console.WriteLine();
        Console.WriteLine("  Result.Try wrapping file I/O:");

        var fileResult = Result.Try(() => File.ReadAllLines("missing-file.csv"));
        fileResult
            .OnSuccess(lines => Console.WriteLine($"  [OK]   Read {lines.Length} lines"))
            .OnFailure(ex => Console.WriteLine($"  [FAIL] {ex.GetType().Name}: {ex.Message}"));

        Console.WriteLine();
    }
}
