namespace Opale;

public partial class Result<T, TError>
{
    #region MatchAsync
    public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> onSuccess, Func<TError, TResult> onFailure)
        => IsSuccess ? await onSuccess(Value) : onFailure(Error);
    
    public async Task<TResult> MatchAsync<TResult>(Func<T, TResult> onSuccess, Func<TError, Task<TResult>> onFailure)
        => IsSuccess ? onSuccess(Value) : await onFailure(Error);
    
    public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> onSuccess, Func<TError, Task<TResult>> onFailure)
        => IsSuccess ? await onSuccess(Value) : await onFailure(Error);
    
    public async Task MatchAsync(
        Func<T, Task> onSuccess,
        Func<TError, Task> onFailure)
    {
        if (IsSuccess) await onSuccess(Value);
        else await onFailure(Error);
    }
    #endregion
}