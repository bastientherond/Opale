using Opale;

namespace Opale.Tests;

public class ResultEqualityTests
{
    // ── Equals ─────────────────────────────────────────────────────────────

    [Fact]
    public void Equals_TwoOkWithSameValue_ReturnsTrue()
    {
        var a = Result<int, string>.Ok(42);
        var b = Result<int, string>.Ok(42);
        a.Equals(b).Should().BeTrue();
    }

    [Fact]
    public void Equals_TwoOkWithDifferentValues_ReturnsFalse()
    {
        var a = Result<int, string>.Ok(1);
        var b = Result<int, string>.Ok(2);
        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_TwoFailWithSameError_ReturnsTrue()
    {
        var a = Result<int, string>.Fail("error");
        var b = Result<int, string>.Fail("error");
        a.Equals(b).Should().BeTrue();
    }

    [Fact]
    public void Equals_TwoFailWithDifferentErrors_ReturnsFalse()
    {
        var a = Result<int, string>.Fail("err1");
        var b = Result<int, string>.Fail("err2");
        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_OkAndFail_ReturnsFalse()
    {
        var ok = Result<int, string>.Ok(1);
        var fail = Result<int, string>.Fail("e");
        ok.Equals(fail).Should().BeFalse();
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var result = Result<int, string>.Ok(1);
        result.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        var result = Result<int, string>.Ok(7);
        result.Equals(result).Should().BeTrue();
    }

    // ── == / != operators ──────────────────────────────────────────────────

    [Fact]
    public void EqualityOperator_TwoEqualOk_ReturnsTrue()
    {
        var a = Result<int, string>.Ok(5);
        var b = Result<int, string>.Ok(5);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_DifferentOk_ReturnsTrue()
    {
        var a = Result<int, string>.Ok(1);
        var b = Result<int, string>.Ok(2);
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_BothNull_ReturnsTrue()
    {
        Result<int, string>? a = null;
        Result<int, string>? b = null;
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_LeftNullRightNotNull_ReturnsFalse()
    {
        Result<int, string>? a = null;
        var b = Result<int, string>.Ok(1);
        (a == b).Should().BeFalse();
    }

    // ── GetHashCode ────────────────────────────────────────────────────────

    [Fact]
    public void GetHashCode_TwoEqualOk_ReturnsSameHash()
    {
        var a = Result<int, string>.Ok(99);
        var b = Result<int, string>.Ok(99);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_OkAndFail_ReturnDifferentHashes()
    {
        var ok = Result<int, string>.Ok(1);
        var fail = Result<int, string>.Fail("1");
        ok.GetHashCode().Should().NotBe(fail.GetHashCode());
    }

    // ── ToString ───────────────────────────────────────────────────────────

    [Fact]
    public void ToString_Ok_ReturnsOkRepresentation()
    {
        var result = Result<int, string>.Ok(42);
        result.ToString().Should().Be("Ok(42)");
    }

    [Fact]
    public void ToString_Fail_ReturnsFailRepresentation()
    {
        var result = Result<int, string>.Fail("oops");
        result.ToString().Should().Be("Fail(oops)");
    }
}
