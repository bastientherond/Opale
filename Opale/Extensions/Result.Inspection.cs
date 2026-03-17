namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Returns <see langword="true"/> when the result is successful and the success value
    /// satisfies <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">Condition tested against the success value.</param>
    public bool IsSuccessAnd(Func<T, bool> predicate)
        => IsSuccess && predicate(Value);

    /// <summary>
    /// Returns <see langword="true"/> when the result is a failure and the error value
    /// satisfies <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">Condition tested against the error value.</param>
    public bool IsFailureAnd(Func<TError, bool> predicate)
        => IsFailure && predicate(Error);

    /// <summary>
    /// Returns <see langword="true"/> when the result is successful and its value equals
    /// <paramref name="value"/> using the default equality comparer.
    /// </summary>
    /// <param name="value">The value to compare against the success value.</param>
    public bool IsContains(T value)
        => IsSuccess
           && Value is not null
           && value is not null
           && EqualityComparer<T>.Default.Equals(value, Value);
}