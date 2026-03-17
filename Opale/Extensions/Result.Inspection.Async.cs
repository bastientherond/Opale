namespace Opale;

public partial class Result<T, TError>
{
    public async Task<bool> IsSuccessAndAsync(Func<T, bool> predicate)
        => IsSuccess && await Task.FromResult(predicate(Value));
    
    public async Task<bool> IsSuccessAndAsync(Func<T, Task<bool>> predicate)
        => IsSuccess && await predicate(Value);
    
    public async Task<bool> IsFailureAndAsync(Func<TError, bool> predicate)
        => IsFailure && await Task.FromResult(predicate(Error));
    
    public async Task<bool> IsFailureAndAsync(Func<TError, Task<bool>> predicate)
        => IsFailure && await predicate(Error);
    
    public async Task<bool> IsContainsAsync(T value)
        => IsSuccess 
           && Value is not null 
           && value is not null 
           && await Task.FromResult(EqualityComparer<T>.Default.Equals(value, Value));

    public async Task<bool> IsContainsAsync(Task<T> value)
        => IsSuccess
           && Value is not null
           && await value is not null
           && EqualityComparer<T>.Default.Equals(await value, Value);
}