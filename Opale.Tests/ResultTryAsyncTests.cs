using Opale.Static;

namespace Opale.Tests;

public class ResultTryAsyncTests
{
    [Fact]
    public async Task TryAsync_SuccessfulFunc_ReturnsOk()
    {
        var result = await Result.TryAsync(() => Task.FromResult(42));
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public async Task TryAsync_ThrowingFunc_ReturnsFail()
    {
        var result = await Result.TryAsync<int>(() => throw new InvalidOperationException("boom"));
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidOperationException>();
        result.Error.Message.Should().Be("boom");
    }

    [Fact]
    public async Task TryAsync_FuncThatAwaitsAndSucceeds_ReturnsOk()
    {
        var result = await Result.TryAsync(async () =>
        {
            await Task.Delay(1);
            return "hello";
        });
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public async Task TryAsync_FuncThatAwaitsAndThrows_ReturnsFail()
    {
        var result = await Result.TryAsync<string>(async () =>
        {
            await Task.Delay(1);
            throw new ArgumentException("async-boom");
        });
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ArgumentException>();
    }
}
