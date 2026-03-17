namespace Opale;

public partial class Result<T, TError>
{
    public async Task<Result<TNew, TError>> MapAsync<TNew>(Func<T, Task<TNew>> map)
        => IsSuccess 
            ? Result<TNew, TError>.Ok(await map(Value)) 
            : Result<TNew, TError>.Fail(Error);
    
    public async Task<Result<TNew, TError>> MapAsync<TNew>(Func<T, TNew> map)
        => IsSuccess 
            ? await Task.FromResult(Result<TNew, TError>.Ok(map(Value))) 
            : await Task.FromResult(Result<TNew, TError>.Fail(Error));
    
    public async Task<Result<TNew, TError>> BindAsync<TNew>(Func<T, Task<Result<TNew, TError>>> bind)
        => IsSuccess
            ? await bind(Value)
            : Result<TNew, TError>.Fail(Error);

    public async Task<Result<T, TNewError>> MapErrorAsync<TNewError>(Func<TError, Task<TNewError>> map)
        => IsFailure
            ? Result<T, TNewError>.Fail(await map(Error))
            : Result<T, TNewError>.Ok(Value);

    public async Task<T> ToExceptionAsync(Func<TError, Task<Exception>> error)
        => IsSuccess
            ? Value
            : throw await error(Error);
    
}