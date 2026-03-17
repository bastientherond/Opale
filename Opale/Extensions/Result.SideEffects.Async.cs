namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Asynchronously executes <paramref name="action"/> with the success value when the result
    /// is successful, then returns the result unchanged. No-op on failure.
    /// </summary>
    /// <param name="action">Async side-effecting action executed with the success value.</param>
    /// <returns>This result, unchanged.</returns>
    public async Task<Result<T, TError>> OnSuccessAsync(Func<T, Task> action)
    {
        if (IsSuccess)
            await action(Value);
        return this;
    }

    /// <summary>
    /// Alias for <see cref="OnSuccessAsync"/>. Asynchronously executes <paramref name="action"/>
    /// with the success value and returns the result unchanged.
    /// </summary>
    /// <param name="action">Async side-effecting action executed with the success value.</param>
    /// <returns>This result, unchanged.</returns>
    public async Task<Result<T, TError>> TapAsync(Func<T, Task> action)
        => await OnSuccessAsync(action);

    /// <summary>
    /// Asynchronously executes <paramref name="action"/> with the error value when the result
    /// is a failure, then returns the result unchanged. No-op on success.
    /// </summary>
    /// <param name="action">Async side-effecting action executed with the error value.</param>
    /// <returns>This result, unchanged.</returns>
    public async Task<Result<T, TError>> OnFailureAsync(Func<TError, Task> action)
    {
        if (IsFailure)
            await action(Error);
        return this;
    }
}