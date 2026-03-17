namespace Opale;

public partial class Result<T, TError>
{
    public async Task<Result<T, TError>> RecoverAsync(Func<TError, T> fallback)
        => IsFailure 
            ? await Task.FromResult(Ok(fallback(Error)))
            : await Task.FromResult(Ok(Value));

    public async Task<Result<T, TError>> RecoverAsync(Func<TError, Task<T>> fallback)
        => IsFailure 
            ? Ok(await fallback(Error))
            : await Task.FromResult(Ok(Value));

    
    public async Task<Result<T, TError>> RecoverWithAsync(Func<TError, Result<T, TError>> fallback)
        => IsFailure
            ? await Task.FromResult(fallback(Error))
            : await Task.FromResult(Ok(Value));
    
    public async Task<Result<T, TError>> RecoverWithAsync(Func<TError, Task<Result<T, TError>>> fallback)
        => IsFailure
            ? await fallback(Error)
            : await Task.FromResult(Ok(Value));
    
    public async Task<T> GetValueOrDefaultAsync(T defaultValue)
        => IsSuccess 
            ? await Task.FromResult(Value) 
            : await Task.FromResult(defaultValue);
    
    public async Task<T> GetValueOrDefaultAsync(Task<T> defaultValue)
        => IsSuccess 
            ? Value 
            : await defaultValue;
}