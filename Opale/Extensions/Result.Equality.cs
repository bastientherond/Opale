namespace Opale;

public partial class Result<T, TError> : IEquatable<Result<T, TError>>
{
    /// <summary>
    /// Determines whether two results are equal.
    /// Two results are equal when they have the same state (both success or both failure)
    /// and their respective values compare equal using the default equality comparer.
    /// </summary>
    /// <param name="other">The result to compare with.</param>
    /// <returns><see langword="true"/> if the results are equal; otherwise <see langword="false"/>.</returns>
    public bool Equals(Result<T, TError>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (IsSuccess != other.IsSuccess) return false;

        return IsSuccess
            ? EqualityComparer<T>.Default.Equals(_value, other._value)
            : EqualityComparer<TError>.Default.Equals(_error, other._error);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is Result<T, TError> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => IsSuccess
            ? HashCode.Combine(true, _value)
            : HashCode.Combine(false, _error);

    /// <summary>Returns a human-readable representation of the result.</summary>
    public override string ToString()
        => IsSuccess
            ? $"Ok({_value})"
            : $"Fail({_error})";

    /// <summary>Returns <see langword="true"/> when both results are equal.</summary>
    public static bool operator ==(Result<T, TError>? left, Result<T, TError>? right)
        => left is null ? right is null : left.Equals(right);

    /// <summary>Returns <see langword="true"/> when the results are not equal.</summary>
    public static bool operator !=(Result<T, TError>? left, Result<T, TError>? right)
        => !(left == right);
}
