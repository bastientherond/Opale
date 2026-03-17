namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Returns a successful result with the value produced by <paramref name="fallback"/>
    /// when the result is a failure. If the result is already a success it is returned unchanged.
    /// </summary>
    /// <param name="fallback">Function that produces a recovery value from the error.</param>
    /// <returns>A successful result.</returns>
    public Result<T, TError> Recover(Func<TError, T> fallback)
        => IsFailure
            ? Ok(fallback(Error))
            : Ok(Value);

    /// <summary>
    /// Replaces a failed result with the result returned by <paramref name="fallback"/>.
    /// If the result is already a success it is returned unchanged.
    /// </summary>
    /// <param name="fallback">Function that produces a replacement result from the error.</param>
    /// <returns>The original success or the result of <paramref name="fallback"/>.</returns>
    public Result<T, TError> RecoverWith(Func<TError, Result<T, TError>> fallback)
        => IsFailure
            ? fallback(Error)
            : Ok(Value);

    /// <summary>
    /// Returns the success value, or <paramref name="defaultValue"/> when the result is a failure.
    /// </summary>
    /// <param name="defaultValue">The value to return when the result is a failure.</param>
    /// <returns>The success value or <paramref name="defaultValue"/>.</returns>
    public T GetValueOrDefault(T defaultValue)
        => IsSuccess
            ? Value
            : defaultValue;
}