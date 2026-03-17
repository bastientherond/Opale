namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Asynchronously transforms the success value using an async <paramref name="map"/> delegate.
    /// If the result is a failure the error is propagated unchanged.
    /// </summary>
    /// <typeparam name="TNew">The type of the transformed value.</typeparam>
    /// <param name="map">Async projection applied to the success value.</param>
    public async Task<Result<TNew, TError>> MapAsync<TNew>(Func<T, Task<TNew>> map)
        => IsSuccess
            ? Result<TNew, TError>.Ok(await map(Value))
            : Result<TNew, TError>.Fail(Error);

    /// <summary>
    /// Transforms the success value using a synchronous <paramref name="map"/> delegate,
    /// returning the result wrapped in a completed task.
    /// </summary>
    /// <typeparam name="TNew">The type of the transformed value.</typeparam>
    /// <param name="map">Synchronous projection applied to the success value.</param>
    public async Task<Result<TNew, TError>> MapAsync<TNew>(Func<T, TNew> map)
        => IsSuccess
            ? await Task.FromResult(Result<TNew, TError>.Ok(map(Value)))
            : await Task.FromResult(Result<TNew, TError>.Fail(Error));

    /// <summary>
    /// Asynchronously chains this result into another result-returning async operation.
    /// If the result is a failure the error is propagated and <paramref name="bind"/> is not called.
    /// </summary>
    /// <typeparam name="TNew">The success type of the chained result.</typeparam>
    /// <param name="bind">The async operation to chain.</param>
    public async Task<Result<TNew, TError>> BindAsync<TNew>(Func<T, Task<Result<TNew, TError>>> bind)
        => IsSuccess
            ? await bind(Value)
            : Result<TNew, TError>.Fail(Error);

    /// <summary>
    /// Asynchronously transforms the error value using an async <paramref name="map"/> delegate.
    /// If the result is a success the value is propagated unchanged.
    /// </summary>
    /// <typeparam name="TNewError">The type of the transformed error.</typeparam>
    /// <param name="map">Async projection applied to the error value.</param>
    public async Task<Result<T, TNewError>> MapErrorAsync<TNewError>(Func<TError, Task<TNewError>> map)
        => IsFailure
            ? Result<T, TNewError>.Fail(await map(Error))
            : Result<T, TNewError>.Ok(Value);

    /// <summary>
    /// Returns the success value, or throws the exception produced by the async <paramref name="error"/>
    /// factory if the result is a failure.
    /// </summary>
    /// <param name="error">Async factory that converts the error into an exception to throw.</param>
    /// <exception cref="Exception">The exception returned by <paramref name="error"/>.</exception>
    public async Task<T> ToExceptionAsync(Func<TError, Task<Exception>> error)
        => IsSuccess
            ? Value
            : throw await error(Error);
}