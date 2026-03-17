using Opale;

namespace Opale.Tests;

public class ResultMatchTests
{
    // ── Match<TResult> ─────────────────────────────────────────────────────

    [Fact]
    public void Match_OnOk_InvokesOnSuccess()
    {
        var result = Result<int, string>.Ok(10);
        var output = result.Match(
            onSuccess: v => v * 2,
            onFailure: _ => -1);
        output.Should().Be(20);
    }

    [Fact]
    public void Match_OnFail_InvokesOnFailure()
    {
        var result = Result<int, string>.Fail("err");
        var output = result.Match(
            onSuccess: v => v * 2,
            onFailure: e => e.Length);
        output.Should().Be(3); // "err".Length
    }

    [Fact]
    public void Match_OnOk_DoesNotInvokeOnFailure()
    {
        var result = Result<int, string>.Ok(1);
        var failureCalled = false;
        result.Match(
            onSuccess: _ => 0,
            onFailure: _ => { failureCalled = true; return 0; });
        failureCalled.Should().BeFalse();
    }

    [Fact]
    public void Match_OnFail_DoesNotInvokeOnSuccess()
    {
        var result = Result<int, string>.Fail("e");
        var successCalled = false;
        result.Match(
            onSuccess: _ => { successCalled = true; return 0; },
            onFailure: _ => 0);
        successCalled.Should().BeFalse();
    }

    // ── Match (void) ───────────────────────────────────────────────────────

    [Fact]
    public void MatchVoid_OnOk_InvokesOnSuccess()
    {
        var result = Result<int, string>.Ok(5);
        var seen = 0;
        result.Match(
            onSuccess: v => seen = v,
            onFailure: _ => seen = -1);
        seen.Should().Be(5);
    }

    [Fact]
    public void MatchVoid_OnFail_InvokesOnFailure()
    {
        var result = Result<int, string>.Fail("x");
        var seen = string.Empty;
        result.Match(
            onSuccess: _ => seen = "success",
            onFailure: e => seen = e);
        seen.Should().Be("x");
    }

    // ── MatchAsync — all overloads ─────────────────────────────────────────

    [Fact]
    public async Task MatchAsync_AsyncOnSuccess_OnOk_ReturnsTransformed()
    {
        var result = Result<int, string>.Ok(3);
        var output = await result.MatchAsync(
            onSuccess: v => Task.FromResult(v * 10),
            onFailure: _ => -1);
        output.Should().Be(30);
    }

    [Fact]
    public async Task MatchAsync_AsyncOnSuccess_OnFail_InvokesOnFailure()
    {
        var result = Result<int, string>.Fail("nope");
        var output = await result.MatchAsync(
            onSuccess: v => Task.FromResult(v * 10),
            onFailure: e => e.Length);
        output.Should().Be(4); // "nope".Length
    }

    [Fact]
    public async Task MatchAsync_AsyncOnFailure_OnOk_InvokesOnSuccess()
    {
        var result = Result<int, string>.Ok(7);
        var output = await result.MatchAsync(
            onSuccess: v => v + 1,
            onFailure: _ => Task.FromResult(-1));
        output.Should().Be(8);
    }

    [Fact]
    public async Task MatchAsync_AsyncOnFailure_OnFail_ReturnsTransformed()
    {
        var result = Result<int, string>.Fail("err");
        var output = await result.MatchAsync(
            onSuccess: v => v,
            onFailure: e => Task.FromResult(e.Length));
        output.Should().Be(3);
    }

    [Fact]
    public async Task MatchAsync_BothAsync_OnOk_InvokesOnSuccess()
    {
        var result = Result<int, string>.Ok(4);
        var output = await result.MatchAsync(
            onSuccess: v => Task.FromResult(v * 3),
            onFailure: _ => Task.FromResult(-1));
        output.Should().Be(12);
    }

    [Fact]
    public async Task MatchAsync_BothAsync_OnFail_InvokesOnFailure()
    {
        var result = Result<int, string>.Fail("ab");
        var output = await result.MatchAsync(
            onSuccess: _ => Task.FromResult(-1),
            onFailure: e => Task.FromResult(e.Length));
        output.Should().Be(2);
    }

    [Fact]
    public async Task MatchAsync_Void_OnOk_InvokesOnSuccess()
    {
        var result = Result<int, string>.Ok(9);
        var seen = 0;
        await result.MatchAsync(
            onSuccess: v => { seen = v; return Task.CompletedTask; },
            onFailure: _ => { seen = -1; return Task.CompletedTask; });
        seen.Should().Be(9);
    }

    [Fact]
    public async Task MatchAsync_Void_OnFail_InvokesOnFailure()
    {
        var result = Result<int, string>.Fail("x");
        var seen = 0;
        await result.MatchAsync(
            onSuccess: _ => { seen = 1; return Task.CompletedTask; },
            onFailure: _ => { seen = -99; return Task.CompletedTask; });
        seen.Should().Be(-99);
    }
}
