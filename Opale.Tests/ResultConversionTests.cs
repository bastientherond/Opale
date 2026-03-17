using Opale;

namespace Opale.Tests;

public class ResultConversionTests
{
    // ── ToOption ───────────────────────────────────────────────────────────

    [Fact]
    public void ToOption_OnOk_ReturnsValue()
    {
        Result<int, string>.Ok(42).ToOption().Should().Be(42);
    }

    [Fact]
    public void ToOption_OnFail_ReturnsDefault()
    {
        Result<int, string>.Fail("e").ToOption().Should().Be(default(int));
    }

    [Fact]
    public void ToOption_OnOk_WithReferenceType_ReturnsObject()
    {
        var obj = new object();
        Result<object, string>.Ok(obj).ToOption().Should().BeSameAs(obj);
    }

    [Fact]
    public void ToOption_OnFail_WithReferenceType_ReturnsNull()
    {
        Result<object, string>.Fail("e").ToOption().Should().BeNull();
    }

    // ── ToOptionAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task ToOptionAsync_OnOk_ReturnsValue()
    {
        var result = await Result<int, string>.Ok(7).ToOptionAsync();
        result.Should().Be(7);
    }

    [Fact]
    public async Task ToOptionAsync_OnFail_ReturnsDefault()
    {
        var result = await Result<int, string>.Fail("e").ToOptionAsync();
        result.Should().Be(default(int));
    }

    [Fact]
    public async Task ToOptionAsync_OnOk_WithReferenceType_ReturnsObject()
    {
        var obj = new object();
        var result = await Result<object, string>.Ok(obj).ToOptionAsync();
        result.Should().BeSameAs(obj);
    }

    [Fact]
    public async Task ToOptionAsync_OnFail_WithReferenceType_ReturnsNull()
    {
        var result = await Result<object, string>.Fail("e").ToOptionAsync();
        result.Should().BeNull();
    }
}
