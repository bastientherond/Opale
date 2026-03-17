using Opale;

namespace Opale.Tests;

public class ResultSideEffectsTests
{
    // ── OnSuccess ──────────────────────────────────────────────────────────

    [Fact]
    public void OnSuccess_OnOk_ExecutesAction()
    {
        var seen = 0;
        Result<int, string>.Ok(7).OnSuccess(v => seen = v);
        seen.Should().Be(7);
    }

    [Fact]
    public void OnSuccess_OnFail_DoesNotExecuteAction()
    {
        var called = false;
        Result<int, string>.Fail("e").OnSuccess(_ => called = true);
        called.Should().BeFalse();
    }

    [Fact]
    public void OnSuccess_ReturnsOriginalResult()
    {
        var original = Result<int, string>.Ok(3);
        var returned = original.OnSuccess(_ => { });
        returned.Should().BeSameAs(original);
    }

    [Fact]
    public void OnSuccess_OnFail_ReturnsOriginalFailure()
    {
        var original = Result<int, string>.Fail("err");
        var returned = original.OnSuccess(_ => { });
        returned.Should().BeSameAs(original);
    }

    // ── OnFailure ──────────────────────────────────────────────────────────

    [Fact]
    public void OnFailure_OnFail_ExecutesAction()
    {
        string? seen = null;
        Result<int, string>.Fail("boom").OnFailure(e => seen = e);
        seen.Should().Be("boom");
    }

    [Fact]
    public void OnFailure_OnOk_DoesNotExecuteAction()
    {
        var called = false;
        Result<int, string>.Ok(1).OnFailure(_ => called = true);
        called.Should().BeFalse();
    }

    [Fact]
    public void OnFailure_ReturnsOriginalResult()
    {
        var original = Result<int, string>.Fail("e");
        var returned = original.OnFailure(_ => { });
        returned.Should().BeSameAs(original);
    }

    // ── Tap ────────────────────────────────────────────────────────────────

    [Fact]
    public void Tap_OnOk_ExecutesAction()
    {
        var seen = 0;
        Result<int, string>.Ok(9).Tap(v => seen = v);
        seen.Should().Be(9);
    }

    [Fact]
    public void Tap_OnFail_DoesNotExecuteAction()
    {
        var called = false;
        Result<int, string>.Fail("e").Tap(_ => called = true);
        called.Should().BeFalse();
    }

    [Fact]
    public void Tap_ReturnsOriginalResult()
    {
        var original = Result<int, string>.Ok(5);
        var returned = original.Tap(_ => { });
        returned.Should().BeSameAs(original);
    }

    // ── Chaining ───────────────────────────────────────────────────────────

    [Fact]
    public void SideEffects_CanBeChained()
    {
        var log = new List<string>();
        Result<int, string>.Ok(1)
            .OnSuccess(_ => log.Add("success1"))
            .OnSuccess(_ => log.Add("success2"))
            .OnFailure(_ => log.Add("failure"));
        log.Should().Equal("success1", "success2");
    }

    // ── OnSuccessAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task OnSuccessAsync_OnOk_ExecutesAction()
    {
        var seen = 0;
        await Result<int, string>.Ok(4).OnSuccessAsync(v => { seen = v; return Task.CompletedTask; });
        seen.Should().Be(4);
    }

    [Fact]
    public async Task OnSuccessAsync_OnFail_DoesNotExecuteAction()
    {
        var called = false;
        await Result<int, string>.Fail("e")
            .OnSuccessAsync(_ => { called = true; return Task.CompletedTask; });
        called.Should().BeFalse();
    }

    [Fact]
    public async Task OnSuccessAsync_ReturnsOriginalResult()
    {
        var original = Result<int, string>.Ok(3);
        var returned = await original.OnSuccessAsync(_ => Task.CompletedTask);
        returned.IsSuccess.Should().BeTrue();
        returned.Value.Should().Be(3);
    }

    // ── OnFailureAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task OnFailureAsync_OnFail_ExecutesAction()
    {
        string? seen = null;
        await Result<int, string>.Fail("oops")
            .OnFailureAsync(e => { seen = e; return Task.CompletedTask; });
        seen.Should().Be("oops");
    }

    [Fact]
    public async Task OnFailureAsync_OnOk_DoesNotExecuteAction()
    {
        var called = false;
        await Result<int, string>.Ok(1)
            .OnFailureAsync(_ => { called = true; return Task.CompletedTask; });
        called.Should().BeFalse();
    }

    [Fact]
    public async Task OnFailureAsync_ReturnsOriginalFailure()
    {
        var returned = await Result<int, string>.Fail("e")
            .OnFailureAsync(_ => Task.CompletedTask);
        returned.IsFailure.Should().BeTrue();
        returned.Error.Should().Be("e");
    }

    // ── TapAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task TapAsync_OnOk_ExecutesAction()
    {
        var seen = 0;
        await Result<int, string>.Ok(11).TapAsync(v => { seen = v; return Task.CompletedTask; });
        seen.Should().Be(11);
    }

    [Fact]
    public async Task TapAsync_OnFail_DoesNotExecuteAction()
    {
        var called = false;
        await Result<int, string>.Fail("e")
            .TapAsync(_ => { called = true; return Task.CompletedTask; });
        called.Should().BeFalse();
    }
}
