using Opale;

namespace Opale.Tests;

public class ResultTransformTests
{
    // ── Map ────────────────────────────────────────────────────────────────

    [Fact]
    public void Map_OnOk_TransformsValue()
    {
        var result = Result<int, string>.Ok(5).Map(v => v * 2);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(10);
    }

    [Fact]
    public void Map_OnFail_PropagatesError()
    {
        var result = Result<int, string>.Fail("err").Map(v => v * 2);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("err");
    }

    [Fact]
    public void Map_CanChangeTType()
    {
        var result = Result<int, string>.Ok(42).Map(v => v.ToString());
        result.Value.Should().Be("42");
    }

    [Fact]
    public void Map_OnOk_MapFuncNotCalledOnFail()
    {
        var called = false;
        Result<int, string>.Fail("e").Map(v => { called = true; return v; });
        called.Should().BeFalse();
    }

    // ── Bind ───────────────────────────────────────────────────────────────

    [Fact]
    public void Bind_OnOk_ChainsNextResult()
    {
        var result = Result<int, string>.Ok(5)
            .Bind(v => Result<string, string>.Ok(v.ToString()));
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("5");
    }

    [Fact]
    public void Bind_OnOk_WhenBindReturnsFailure_PropagatesFailure()
    {
        var result = Result<int, string>.Ok(5)
            .Bind(_ => Result<string, string>.Fail("downstream"));
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("downstream");
    }

    [Fact]
    public void Bind_OnFail_PropagatesError_WithoutCallingBind()
    {
        var called = false;
        var result = Result<int, string>.Fail("orig")
            .Bind(v => { called = true; return Result<int, string>.Ok(v); });
        called.Should().BeFalse();
        result.Error.Should().Be("orig");
    }

    // ── MapError ───────────────────────────────────────────────────────────

    [Fact]
    public void MapError_OnFail_TransformsError()
    {
        var result = Result<int, string>.Fail("err").MapError(e => e.Length);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(3);
    }

    [Fact]
    public void MapError_OnOk_PropagatesValue()
    {
        var result = Result<int, string>.Ok(10).MapError(e => e.Length);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(10);
    }

    [Fact]
    public void MapError_OnOk_MapFuncNotCalled()
    {
        var called = false;
        Result<int, string>.Ok(1).MapError(e => { called = true; return e.Length; });
        called.Should().BeFalse();
    }

    // ── ToException ────────────────────────────────────────────────────────

    [Fact]
    public void ToException_OnOk_ReturnsValue()
    {
        var result = Result<int, string>.Ok(99);
        var value = result.ToException(_ => new Exception("should not throw"));
        value.Should().Be(99);
    }

    [Fact]
    public void ToException_OnFail_ThrowsMappedException()
    {
        var result = Result<int, string>.Fail("oh no");
        var act = () => result.ToException(e => new InvalidOperationException(e));
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("oh no");
    }

    // ── MapAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task MapAsync_AsyncDelegate_OnOk_TransformsValue()
    {
        var result = await Result<int, string>.Ok(3)
            .MapAsync(v => Task.FromResult(v * 4));
        result.Value.Should().Be(12);
    }

    [Fact]
    public async Task MapAsync_AsyncDelegate_OnFail_PropagatesError()
    {
        var result = await Result<int, string>.Fail("e")
            .MapAsync(v => Task.FromResult(v * 4));
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("e");
    }

    [Fact]
    public async Task MapAsync_SyncDelegate_OnOk_TransformsValue()
    {
        var result = await Result<int, string>.Ok(5)
            .MapAsync(v => v + 1);
        result.Value.Should().Be(6);
    }

    [Fact]
    public async Task MapAsync_SyncDelegate_OnFail_PropagatesError()
    {
        var result = await Result<int, string>.Fail("x")
            .MapAsync(v => v + 1);
        result.Error.Should().Be("x");
    }

    // ── BindAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task BindAsync_OnOk_ChainsNextResult()
    {
        var result = await Result<int, string>.Ok(7)
            .BindAsync(v => Task.FromResult(Result<string, string>.Ok(v.ToString())));
        result.Value.Should().Be("7");
    }

    [Fact]
    public async Task BindAsync_OnOk_WhenBindReturnsFailure_PropagatesIt()
    {
        var result = await Result<int, string>.Ok(1)
            .BindAsync(_ => Task.FromResult(Result<int, string>.Fail("down")));
        result.Error.Should().Be("down");
    }

    [Fact]
    public async Task BindAsync_OnFail_PropagatesError_WithoutCallingBind()
    {
        var called = false;
        var result = await Result<int, string>.Fail("orig")
            .BindAsync(v => { called = true; return Task.FromResult(Result<int, string>.Ok(v)); });
        called.Should().BeFalse();
        result.Error.Should().Be("orig");
    }

    // ── MapErrorAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task MapErrorAsync_OnFail_TransformsError()
    {
        var result = await Result<int, string>.Fail("hello")
            .MapErrorAsync(e => Task.FromResult(e.Length));
        result.Error.Should().Be(5);
    }

    [Fact]
    public async Task MapErrorAsync_OnOk_PropagatesValue()
    {
        var result = await Result<int, string>.Ok(42)
            .MapErrorAsync(e => Task.FromResult(e.Length));
        result.Value.Should().Be(42);
    }

    // ── ToExceptionAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task ToExceptionAsync_OnOk_ReturnsValue()
    {
        var result = Result<int, string>.Ok(55);
        var value = await result.ToExceptionAsync(_ => Task.FromResult<Exception>(new Exception()));
        value.Should().Be(55);
    }

    [Fact]
    public async Task ToExceptionAsync_OnFail_ThrowsMappedException()
    {
        var result = Result<int, string>.Fail("boom");
        var act = async () => await result.ToExceptionAsync(
            e => Task.FromResult<Exception>(new InvalidOperationException(e)));
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("boom");
    }
}
