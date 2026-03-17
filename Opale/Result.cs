namespace Opale;

/// <summary>
/// Represents a discriminated union of either a successful value (<typeparamref name="T"/>)
/// or an error (<typeparamref name="TError"/>). Provides a type-safe alternative to
/// exception-based error handling following the railway-oriented programming pattern.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
public partial class Result<T, TError>
{
    private readonly T _value;
    private readonly TError  _error;

    private Result(T value)
    {
        _value = value;
        IsSuccess = true;
    }

    private Result(TError error)
    {
        _error = error;
        IsSuccess = false;
    }

    /// <summary>Gets a value indicating whether this result represents a success.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets a value indicating whether this result represents a failure.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the success value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the result is a failure.</exception>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value on a failed result.");

    /// <summary>
    /// Gets the error value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the result is a success.</exception>
    public TError Error => IsFailure
        ? _error!
        : throw new InvalidOperationException("Cannot access Error on a successful result.");

    /// <summary>Creates a successful <see cref="Result{T, TError}"/> with the given value.</summary>
    public static Result<T, TError> Ok(T value) => new(value);

    /// <summary>Creates a failed <see cref="Result{T, TError}"/> with the given error.</summary>
    public static Result<T, TError> Fail(TError error) => new(error);
}
