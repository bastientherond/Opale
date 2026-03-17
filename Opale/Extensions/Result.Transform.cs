namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Transforms the success value using <paramref name="map"/>.
    /// If the result is a failure the error is propagated unchanged.
    /// </summary>
    /// <typeparam name="TNew">The type of the transformed value.</typeparam>
    /// <param name="map">Projection applied to the success value.</param>
    /// <returns>A new result containing the mapped value, or the original error.</returns>
    public Result<TNew, TError> Map<TNew>(Func<T, TNew> map)
        => IsSuccess
            ? Result<TNew, TError>.Ok(map(Value))
            : Result<TNew, TError>.Fail(Error);

    /// <summary>
    /// Chains this result into another result-returning operation.
    /// If the result is a failure the error is propagated and <paramref name="bind"/> is not called.
    /// </summary>
    /// <typeparam name="TNew">The success type of the chained result.</typeparam>
    /// <param name="bind">The operation to chain.</param>
    /// <returns>The result of <paramref name="bind"/>, or the original failure.</returns>
    public Result<TNew, TError> Bind<TNew>(Func<T, Result<TNew, TError>> bind)
        => IsSuccess
            ? bind(Value)
            : Result<TNew, TError>.Fail(Error);

    /// <summary>
    /// Transforms the error value using <paramref name="map"/>.
    /// If the result is a success the value is propagated unchanged.
    /// </summary>
    /// <typeparam name="TNewError">The type of the transformed error.</typeparam>
    /// <param name="map">Projection applied to the error value.</param>
    /// <returns>A new result containing the mapped error, or the original value.</returns>
    public Result<T, TNewError> MapError<TNewError>(Func<TError, TNewError> map)
        => IsFailure
            ? Result<T, TNewError>.Fail(map(Error))
            : Result<T, TNewError>.Ok(Value);

    /// <summary>
    /// Returns the success value, or throws the exception produced by <paramref name="error"/>
    /// if the result is a failure.
    /// </summary>
    /// <param name="error">Factory that converts the error into an exception to throw.</param>
    /// <returns>The success value.</returns>
    /// <exception cref="Exception">The exception returned by <paramref name="error"/>.</exception>
    public T ToException(Func<TError, Exception> error)
        => IsSuccess
            ? Value
            : throw error(Error);
}