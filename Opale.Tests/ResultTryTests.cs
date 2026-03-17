using Opale.Static;

namespace Opale.Tests;

public class ResultTryTests
{
    [Fact]
    public void Try_WhenFuncSucceeds_ReturnsOkWithValue()
    {
        var result = Result.Try(() => int.Parse("42"));
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Try_WhenFuncThrows_ReturnsFailWithException()
    {
        var result = Result.Try(() => int.Parse("not-a-number"));
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<FormatException>();
    }

    [Fact]
    public void Try_CapturesExceptionMessage()
    {
        var result = Result.Try<int>(() => throw new InvalidOperationException("test error"));
        result.Error.Message.Should().Be("test error");
    }

    [Fact]
    public void Try_WhenFuncReturnsReferenceType_ReturnsOkWithObject()
    {
        var obj = new object();
        var result = Result.Try(() => obj);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(obj);
    }

    [Fact]
    public void Try_CatchesAnyExceptionType()
    {
        var result = Result.Try<int>(() => throw new ArgumentNullException("param"));
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ArgumentNullException>();
    }

    [Fact]
    public void Try_WhenFuncReturnsZero_TreatsAsSuccess()
    {
        var result = Result.Try(() => 0);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0);
    }
}
