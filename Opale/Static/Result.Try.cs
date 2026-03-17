namespace Opale.Static;

/// <summary>
/// Static factory helpers for creating <see cref="Result{T, TError}"/> instances
/// from code that may throw exceptions.
/// </summary>
public static class Result
{
    /// <summary>
    /// Executes <paramref name="func"/> and wraps the return value in a successful result.
    /// If an exception is thrown it is caught and returned as a failed result.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by <paramref name="func"/>.</typeparam>
    /// <param name="func">The delegate to execute.</param>
    /// <returns>
    /// <see cref="Result{T, Exception}.Ok"/> when <paramref name="func"/> completes without throwing;
    /// <see cref="Result{T, Exception}.Fail"/> containing the caught exception otherwise.
    /// </returns>
    public static Result<T, Exception> Try<T>(Func<T> func)
    {
        try
        {
            var value = func.Invoke();
            return Result<T, Exception>.Ok(value);
        }
        catch (Exception e)
        {
            return Result<T, Exception>.Fail(e);
        }
    }

    /// <summary>
    /// Asynchronously executes <paramref name="func"/> and wraps the return value in a
    /// successful result. If an exception is thrown it is caught and returned as a failed result.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by <paramref name="func"/>.</typeparam>
    /// <param name="func">The async delegate to execute.</param>
    /// <returns>
    /// A task that resolves to <see cref="Result{T, Exception}.Ok"/> when <paramref name="func"/>
    /// completes without throwing, or <see cref="Result{T, Exception}.Fail"/> containing the
    /// caught exception otherwise.
    /// </returns>
    public static async Task<Result<T, Exception>> TryAsync<T>(Func<Task<T>> func)
    {
        try
        {
            var value = await func().ConfigureAwait(false);
            return Result<T, Exception>.Ok(value);
        }
        catch (Exception e)
        {
            return Result<T, Exception>.Fail(e);
        }
    }
}