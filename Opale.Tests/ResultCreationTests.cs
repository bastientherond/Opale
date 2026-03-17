using Opale;

namespace Opale.Tests;

public class ResultCreationTests
{
    // ── Ok ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Ok_SetsIsSuccessTrue()
    {
        var result = Result<int, string>.Ok(42);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Ok_SetsIsFailureFalse()
    {
        var result = Result<int, string>.Ok(42);
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void Ok_ValueReturnsCorrectValue()
    {
        var result = Result<int, string>.Ok(42);
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Ok_AccessingErrorThrows()
    {
        var result = Result<int, string>.Ok(42);
        var act = () => result.Error;
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*successful*");
    }

    [Fact]
    public void Ok_WithReferenceType_ValueReturnsObject()
    {
        var obj = new object();
        var result = Result<object, string>.Ok(obj);
        result.Value.Should().BeSameAs(obj);
    }

    // ── Fail ───────────────────────────────────────────────────────────────

    [Fact]
    public void Fail_SetsIsSuccessFalse()
    {
        var result = Result<int, string>.Fail("error");
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Fail_SetsIsFailureTrue()
    {
        var result = Result<int, string>.Fail("error");
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Fail_ErrorReturnsCorrectError()
    {
        var result = Result<int, string>.Fail("error");
        result.Error.Should().Be("error");
    }

    [Fact]
    public void Fail_AccessingValueThrows()
    {
        var result = Result<int, string>.Fail("error");
        var act = () => result.Value;
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*failed*");
    }

    [Fact]
    public void Fail_WithReferenceErrorType_ErrorReturnsObject()
    {
        var ex = new Exception("boom");
        var result = Result<int, Exception>.Fail(ex);
        result.Error.Should().BeSameAs(ex);
    }

    // ── Implicit operator ──────────────────────────────────────────────────

    [Fact]
    public void ImplicitConversion_FromValue_CreatesOkResult()
    {
        Result<int, string> result = 99;
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(99);
    }

    [Fact]
    public void ImplicitConversion_FromReferenceType_CreatesOkResult()
    {
        var obj = new object();
        Result<object, string> result = obj;
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(obj);
    }
}
