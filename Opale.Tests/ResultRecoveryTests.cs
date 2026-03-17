using Opale;

namespace Opale.Tests;

public class ResultRecoveryTests
{
    // ── Recover ────────────────────────────────────────────────────────────

    [Fact]
    public void Recover_OnFail_ReturnsOkWithFallback()
    {
        var result = Result<int, string>.Fail("err").Recover(_ => 0);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0);
    }

    [Fact]
    public void Recover_OnOk_ReturnsSameValue()
    {
        var result = Result<int, string>.Ok(5).Recover(_ => 0);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(5);
    }

    [Fact]
    public void Recover_OnOk_FallbackNotCalled()
    {
        var called = false;
        Result<int, string>.Ok(1).Recover(_ => { called = true; return 0; });
        called.Should().BeFalse();
    }

    [Fact]
    public void Recover_OnFail_FallbackReceivesError()
    {
        string? received = null;
        Result<int, string>.Fail("oops").Recover(e => { received = e; return 0; });
        received.Should().Be("oops");
    }

    // ── RecoverWith ────────────────────────────────────────────────────────

    [Fact]
    public void RecoverWith_OnFail_ReturnsFallbackResult()
    {
        var result = Result<int, string>.Fail("e")
            .RecoverWith(_ => Result<int, string>.Ok(42));
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void RecoverWith_OnFail_FallbackCanReturnFailure()
    {
        var result = Result<int, string>.Fail("first")
            .RecoverWith(_ => Result<int, string>.Fail("second"));
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("second");
    }

    [Fact]
    public void RecoverWith_OnOk_ReturnsSameResult()
    {
        var result = Result<int, string>.Ok(7)
            .RecoverWith(_ => Result<int, string>.Ok(0));
        result.Value.Should().Be(7);
    }

    [Fact]
    public void RecoverWith_OnOk_FallbackNotCalled()
    {
        var called = false;
        Result<int, string>.Ok(1).RecoverWith(_ => { called = true; return Result<int, string>.Ok(0); });
        called.Should().BeFalse();
    }

    // ── GetValueOrDefault ──────────────────────────────────────────────────

    [Fact]
    public void GetValueOrDefault_OnOk_ReturnsValue()
    {
        var result = Result<int, string>.Ok(10).GetValueOrDefault(-1);
        result.Should().Be(10);
    }

    [Fact]
    public void GetValueOrDefault_OnFail_ReturnsDefault()
    {
        var result = Result<int, string>.Fail("e").GetValueOrDefault(-1);
        result.Should().Be(-1);
    }

    // ── RecoverAsync (sync fallback) ───────────────────────────────────────

    [Fact]
    public async Task RecoverAsync_SyncFallback_OnFail_ReturnsOkWithFallback()
    {
        var result = await Result<int, string>.Fail("e").RecoverAsync(_ => 99);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(99);
    }

    [Fact]
    public async Task RecoverAsync_SyncFallback_OnOk_ReturnsSameValue()
    {
        var result = await Result<int, string>.Ok(5).RecoverAsync(_ => 99);
        result.Value.Should().Be(5);
    }

    // ── RecoverAsync (async fallback) ──────────────────────────────────────

    [Fact]
    public async Task RecoverAsync_AsyncFallback_OnFail_ReturnsOkWithFallback()
    {
        var result = await Result<int, string>.Fail("e")
            .RecoverAsync(_ => Task.FromResult(77));
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(77);
    }

    [Fact]
    public async Task RecoverAsync_AsyncFallback_OnOk_ReturnsSameValue()
    {
        var result = await Result<int, string>.Ok(3)
            .RecoverAsync(_ => Task.FromResult(77));
        result.Value.Should().Be(3);
    }

    // ── RecoverWithAsync (sync fallback) ──────────────────────────────────

    [Fact]
    public async Task RecoverWithAsync_SyncFallback_OnFail_ReturnsFallbackResult()
    {
        var result = await Result<int, string>.Fail("e")
            .RecoverWithAsync(_ => Result<int, string>.Ok(55));
        result.Value.Should().Be(55);
    }

    [Fact]
    public async Task RecoverWithAsync_SyncFallback_OnOk_ReturnsSameValue()
    {
        var result = await Result<int, string>.Ok(8)
            .RecoverWithAsync(_ => Result<int, string>.Ok(55));
        result.Value.Should().Be(8);
    }

    // ── RecoverWithAsync (async fallback) ─────────────────────────────────

    [Fact]
    public async Task RecoverWithAsync_AsyncFallback_OnFail_ReturnsFallbackResult()
    {
        var result = await Result<int, string>.Fail("e")
            .RecoverWithAsync(_ => Task.FromResult(Result<int, string>.Ok(33)));
        result.Value.Should().Be(33);
    }

    [Fact]
    public async Task RecoverWithAsync_AsyncFallback_OnOk_ReturnsSameValue()
    {
        var result = await Result<int, string>.Ok(2)
            .RecoverWithAsync(_ => Task.FromResult(Result<int, string>.Ok(33)));
        result.Value.Should().Be(2);
    }

    // ── GetValueOrDefaultAsync ────────────────────────────────────────────

    [Fact]
    public async Task GetValueOrDefaultAsync_SyncDefault_OnOk_ReturnsValue()
    {
        var result = await Result<int, string>.Ok(10).GetValueOrDefaultAsync(-1);
        result.Should().Be(10);
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_SyncDefault_OnFail_ReturnsDefault()
    {
        var result = await Result<int, string>.Fail("e").GetValueOrDefaultAsync(-1);
        result.Should().Be(-1);
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_AsyncDefault_OnOk_ReturnsValue()
    {
        var result = await Result<int, string>.Ok(10)
            .GetValueOrDefaultAsync(Task.FromResult(-1));
        result.Should().Be(10);
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_AsyncDefault_OnFail_ReturnsDefault()
    {
        var result = await Result<int, string>.Fail("e")
            .GetValueOrDefaultAsync(Task.FromResult(-1));
        result.Should().Be(-1);
    }
}
