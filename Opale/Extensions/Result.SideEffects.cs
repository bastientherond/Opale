namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Executes <paramref name="action"/> with the success value when the result is successful,
    /// then returns the result unchanged. No-op on failure.
    /// </summary>
    /// <param name="action">Side-effecting action executed with the success value.</param>
    /// <returns>This result, unchanged.</returns>
    public Result<T, TError> OnSuccess(Action<T> action)
    {
        if (IsSuccess)
            action(Value);
        return this;
    }

    /// <summary>
    /// Alias for <see cref="OnSuccess"/>. Executes <paramref name="action"/> with the success
    /// value and returns the result unchanged.
    /// </summary>
    /// <param name="action">Side-effecting action executed with the success value.</param>
    /// <returns>This result, unchanged.</returns>
    public Result<T, TError> Tap(Action<T> action)
        => OnSuccess(action);

    /// <summary>
    /// Executes <paramref name="action"/> with the error value when the result is a failure,
    /// then returns the result unchanged. No-op on success.
    /// </summary>
    /// <param name="action">Side-effecting action executed with the error value.</param>
    /// <returns>This result, unchanged.</returns>
    public Result<T, TError> OnFailure(Action<TError> action)
    {
        if (IsFailure)
            action(Error);
        return this;
    }
}