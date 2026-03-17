namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Asynchronously returns a successful result with the value produced by the synchronous
    /// <paramref name="fallback"/> when the result is a failure.
    /// </summary>
    /// <param name="fallback">Function that produces a recovery value from the error.</param>
    public async Task<Result<T, TError>> RecoverAsync(Func<TError, T> fallback)
        => IsFailure
            ? await Task.FromResult(Ok(fallback(Error)))
            : await Task.FromResult(Ok(Value));

    /// <summary>
    /// Asynchronously returns a successful result with the value produced by the async
    /// <paramref name="fallback"/> when the result is a failure.
    /// </summary>
    /// <param name="fallback">Async function that produces a recovery value from the error.</param>
    public async Task<Result<T, TError>> RecoverAsync(Func<TError, Task<T>> fallback)
        => IsFailure
            ? Ok(await fallback(Error))
            : await Task.FromResult(Ok(Value));

    /// <summary>
    /// Asynchronously replaces a failed result with the result returned by the synchronous
    /// <paramref name="fallback"/>.
    /// </summary>
    /// <param name="fallback">Function that produces a replacement result from the error.</param>
    public async Task<Result<T, TError>> RecoverWithAsync(Func<TError, Result<T, TError>> fallback)
        => IsFailure
            ? await Task.FromResult(fallback(Error))
            : await Task.FromResult(Ok(Value));

    /// <summary>
    /// Asynchronously replaces a failed result with the result returned by the async
    /// <paramref name="fallback"/>.
    /// </summary>
    /// <param name="fallback">Async function that produces a replacement result from the error.</param>
    public async Task<Result<T, TError>> RecoverWithAsync(Func<TError, Task<Result<T, TError>>> fallback)
        => IsFailure
            ? await fallback(Error)
            : await Task.FromResult(Ok(Value));

    /// <summary>
    /// Asynchronously returns the success value, or <paramref name="defaultValue"/> when the
    /// result is a failure.
    /// </summary>
    /// <param name="defaultValue">The value to return when the result is a failure.</param>
    public async Task<T> GetValueOrDefaultAsync(T defaultValue)
        => IsSuccess
            ? await Task.FromResult(Value)
            : await Task.FromResult(defaultValue);

    /// <summary>
    /// Asynchronously returns the success value, or the value produced by the async
    /// <paramref name="defaultValue"/> task when the result is a failure.
    /// </summary>
    /// <param name="defaultValue">Task that produces the default value.</param>
    public async Task<T> GetValueOrDefaultAsync(Task<T> defaultValue)
        => IsSuccess
            ? Value
            : await defaultValue;
}