namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="T"/> to a successful
    /// <see cref="Result{T, TError}"/>.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    public static implicit operator Result<T, TError>(T value)
        => Ok(value);
}