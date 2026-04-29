namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Asynchronously returns <see langword="true"/> when the result is successful and the
    /// success value satisfies the synchronous <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">Synchronous condition tested against the success value.</param>
    public Task<bool> IsSuccessAndAsync(Func<T, bool> predicate)
        => Task.FromResult(IsSuccess && predicate(Value));

    /// <summary>
    /// Asynchronously returns <see langword="true"/> when the result is successful and the
    /// success value satisfies the async <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">Async condition tested against the success value.</param>
    public async Task<bool> IsSuccessAndAsync(Func<T, Task<bool>> predicate)
        => IsSuccess && await predicate(Value);

    /// <summary>
    /// Asynchronously returns <see langword="true"/> when the result is a failure and the
    /// error value satisfies the synchronous <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">Synchronous condition tested against the error value.</param>
    public Task<bool> IsFailureAndAsync(Func<TError, bool> predicate)
        => Task.FromResult(IsFailure && predicate(Error));

    /// <summary>
    /// Asynchronously returns <see langword="true"/> when the result is a failure and the
    /// error value satisfies the async <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">Async condition tested against the error value.</param>
    public async Task<bool> IsFailureAndAsync(Func<TError, Task<bool>> predicate)
        => IsFailure && await predicate(Error);

    /// <summary>
    /// Asynchronously returns <see langword="true"/> when the result is successful and its
    /// value equals <paramref name="value"/> using the default equality comparer.
    /// </summary>
    /// <param name="value">The value to compare against the success value.</param>
    public Task<bool> IsContainsAsync(T value)
        => Task.FromResult(IsSuccess && EqualityComparer<T>.Default.Equals(value, Value));

    /// <summary>
    /// Asynchronously returns <see langword="true"/> when the result is successful and its
    /// value equals the value resolved from the <paramref name="value"/> task.
    /// </summary>
    /// <param name="value">Task that resolves to the value to compare against.</param>
    public async Task<bool> IsContainsAsync(Task<T> value)
        => IsSuccess && EqualityComparer<T>.Default.Equals(await value, Value);
}
