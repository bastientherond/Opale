using Opale;

namespace Opale.Tests;

public class ResultInspectionTests
{
    // ── IsSuccessAnd ───────────────────────────────────────────────────────

    [Fact]
    public void IsSuccessAnd_OnOk_PredicateTrue_ReturnsTrue()
    {
        Result<int, string>.Ok(10).IsSuccessAnd(v => v > 5).Should().BeTrue();
    }

    [Fact]
    public void IsSuccessAnd_OnOk_PredicateFalse_ReturnsFalse()
    {
        Result<int, string>.Ok(3).IsSuccessAnd(v => v > 5).Should().BeFalse();
    }

    [Fact]
    public void IsSuccessAnd_OnFail_ReturnsFalse()
    {
        Result<int, string>.Fail("e").IsSuccessAnd(_ => true).Should().BeFalse();
    }

    [Fact]
    public void IsSuccessAnd_OnFail_PredicateNotCalled()
    {
        var called = false;
        Result<int, string>.Fail("e").IsSuccessAnd(_ => { called = true; return true; });
        called.Should().BeFalse();
    }

    // ── IsFailureAnd ───────────────────────────────────────────────────────

    [Fact]
    public void IsFailureAnd_OnFail_PredicateTrue_ReturnsTrue()
    {
        Result<int, string>.Fail("err").IsFailureAnd(e => e == "err").Should().BeTrue();
    }

    [Fact]
    public void IsFailureAnd_OnFail_PredicateFalse_ReturnsFalse()
    {
        Result<int, string>.Fail("err").IsFailureAnd(e => e == "other").Should().BeFalse();
    }

    [Fact]
    public void IsFailureAnd_OnOk_ReturnsFalse()
    {
        Result<int, string>.Ok(1).IsFailureAnd(_ => true).Should().BeFalse();
    }

    [Fact]
    public void IsFailureAnd_OnOk_PredicateNotCalled()
    {
        var called = false;
        Result<int, string>.Ok(1).IsFailureAnd(_ => { called = true; return true; });
        called.Should().BeFalse();
    }

    // ── IsContains ─────────────────────────────────────────────────────────

    [Fact]
    public void IsContains_OnOk_ValueMatches_ReturnsTrue()
    {
        Result<int, string>.Ok(42).IsContains(42).Should().BeTrue();
    }

    [Fact]
    public void IsContains_OnOk_ValueDiffers_ReturnsFalse()
    {
        Result<int, string>.Ok(42).IsContains(7).Should().BeFalse();
    }

    [Fact]
    public void IsContains_OnFail_ReturnsFalse()
    {
        Result<int, string>.Fail("e").IsContains(42).Should().BeFalse();
    }

    [Fact]
    public void IsContains_OnOk_WithReferenceType_ByEquality()
    {
        Result<string, int>.Ok("hello").IsContains("hello").Should().BeTrue();
        Result<string, int>.Ok("hello").IsContains("world").Should().BeFalse();
    }

    // ── IsSuccessAndAsync (sync predicate) ─────────────────────────────────

    [Fact]
    public async Task IsSuccessAndAsync_SyncPredicate_OnOk_PredicateTrue_ReturnsTrue()
    {
        var result = await Result<int, string>.Ok(10).IsSuccessAndAsync(v => v > 5);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsSuccessAndAsync_SyncPredicate_OnOk_PredicateFalse_ReturnsFalse()
    {
        var result = await Result<int, string>.Ok(2).IsSuccessAndAsync(v => v > 5);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsSuccessAndAsync_SyncPredicate_OnFail_ReturnsFalse()
    {
        var result = await Result<int, string>.Fail("e").IsSuccessAndAsync(_ => true);
        result.Should().BeFalse();
    }

    // ── IsSuccessAndAsync (async predicate) ────────────────────────────────

    [Fact]
    public async Task IsSuccessAndAsync_AsyncPredicate_OnOk_PredicateTrue_ReturnsTrue()
    {
        var result = await Result<int, string>.Ok(10)
            .IsSuccessAndAsync(v => Task.FromResult(v > 5));
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsSuccessAndAsync_AsyncPredicate_OnFail_ReturnsFalse()
    {
        var result = await Result<int, string>.Fail("e")
            .IsSuccessAndAsync(_ => Task.FromResult(true));
        result.Should().BeFalse();
    }

    // ── IsFailureAndAsync (sync predicate) ─────────────────────────────────

    [Fact]
    public async Task IsFailureAndAsync_SyncPredicate_OnFail_PredicateTrue_ReturnsTrue()
    {
        var result = await Result<int, string>.Fail("err").IsFailureAndAsync(e => e == "err");
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFailureAndAsync_SyncPredicate_OnOk_ReturnsFalse()
    {
        var result = await Result<int, string>.Ok(1).IsFailureAndAsync(_ => true);
        result.Should().BeFalse();
    }

    // ── IsFailureAndAsync (async predicate) ────────────────────────────────

    [Fact]
    public async Task IsFailureAndAsync_AsyncPredicate_OnFail_PredicateTrue_ReturnsTrue()
    {
        var result = await Result<int, string>.Fail("err")
            .IsFailureAndAsync(e => Task.FromResult(e == "err"));
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFailureAndAsync_AsyncPredicate_OnOk_ReturnsFalse()
    {
        var result = await Result<int, string>.Ok(1)
            .IsFailureAndAsync(_ => Task.FromResult(true));
        result.Should().BeFalse();
    }

    // ── IsContainsAsync (sync value) ───────────────────────────────────────

    [Fact]
    public async Task IsContainsAsync_OnOk_ValueMatches_ReturnsTrue()
    {
        var result = await Result<int, string>.Ok(5).IsContainsAsync(5);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsContainsAsync_OnOk_ValueDiffers_ReturnsFalse()
    {
        var result = await Result<int, string>.Ok(5).IsContainsAsync(99);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsContainsAsync_OnFail_ReturnsFalse()
    {
        var result = await Result<int, string>.Fail("e").IsContainsAsync(5);
        result.Should().BeFalse();
    }

    // ── IsContainsAsync (async value) ──────────────────────────────────────

    [Fact]
    public async Task IsContainsAsync_AsyncValue_OnOk_ValueMatches_ReturnsTrue()
    {
        var result = await Result<int, string>.Ok(5).IsContainsAsync(Task.FromResult(5));
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsContainsAsync_AsyncValue_OnOk_ValueDiffers_ReturnsFalse()
    {
        var result = await Result<int, string>.Ok(5).IsContainsAsync(Task.FromResult(99));
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsContainsAsync_AsyncValue_OnFail_ReturnsFalse()
    {
        var result = await Result<int, string>.Fail("e").IsContainsAsync(Task.FromResult(5));
        result.Should().BeFalse();
    }
}
