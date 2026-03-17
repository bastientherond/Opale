namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Asynchronously projects the result using an async <paramref name="onSuccess"/> delegate
    /// and a synchronous <paramref name="onFailure"/> delegate.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected value.</typeparam>
    /// <param name="onSuccess">Async function executed with the success value.</param>
    /// <param name="onFailure">Synchronous function executed with the error value.</param>
    public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> onSuccess, Func<TError, TResult> onFailure)
        => IsSuccess ? await onSuccess(Value) : onFailure(Error);

    /// <summary>
    /// Asynchronously projects the result using a synchronous <paramref name="onSuccess"/> delegate
    /// and an async <paramref name="onFailure"/> delegate.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected value.</typeparam>
    /// <param name="onSuccess">Synchronous function executed with the success value.</param>
    /// <param name="onFailure">Async function executed with the error value.</param>
    public async Task<TResult> MatchAsync<TResult>(Func<T, TResult> onSuccess, Func<TError, Task<TResult>> onFailure)
        => IsSuccess ? onSuccess(Value) : await onFailure(Error);

    /// <summary>
    /// Asynchronously projects the result using async delegates for both branches.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected value.</typeparam>
    /// <param name="onSuccess">Async function executed with the success value.</param>
    /// <param name="onFailure">Async function executed with the error value.</param>
    public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> onSuccess, Func<TError, Task<TResult>> onFailure)
        => IsSuccess ? await onSuccess(Value) : await onFailure(Error);

    /// <summary>
    /// Asynchronously executes a side-effecting action based on the result state.
    /// </summary>
    /// <param name="onSuccess">Async action executed with the success value.</param>
    /// <param name="onFailure">Async action executed with the error value.</param>
    public async Task MatchAsync(Func<T, Task> onSuccess, Func<TError, Task> onFailure)
    {
        if (IsSuccess) await onSuccess(Value);
        else await onFailure(Error);
    }
}
