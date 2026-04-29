using Opale;

namespace Opale.Tests;

public class ResultTaskExtensionTests
{
    [Fact]
    public async Task TaskResultPipeline_CanMapBindTapAndMatch()
    {
        var tapped = false;

        var result = await GetNumberAsync()
            .BindAsync(value => Task.FromResult(Result<int, string>.Ok(value + 1)))
            .MapAsync(value => value * 2)
            .OnSuccessAsync(_ => tapped = true)
            .MatchAsync(
                onSuccess: value => value.ToString(),
                onFailure: error => error);

        result.Should().Be("8");
        tapped.Should().BeTrue();
    }

    [Fact]
    public async Task TaskResultPipeline_OnFailure_SkipsSuccessDelegates()
    {
        var called = false;

        var result = await Task.FromResult(Result<int, string>.Fail("failed"))
            .MapAsync(value => { called = true; return value * 2; })
            .BindAsync(value => Result<int, string>.Ok(value + 1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("failed");
        called.Should().BeFalse();
    }

    [Fact]
    public async Task TaskResultPipeline_CanMapError()
    {
        var result = await Task.FromResult(Result<int, string>.Fail("boom"))
            .MapErrorAsync(error => error.Length);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(4);
    }

    private static Task<Result<int, string>> GetNumberAsync()
        => Task.FromResult(Result<int, string>.Ok(3));
}
