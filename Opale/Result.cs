namespace Opale;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T">Input params</typeparam>
/// <typeparam name="TError">Output error</typeparam>
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
    
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess 
        ? field!
        : throw new InvalidOperationException("Cannot access Value on a failed result.");
    
    public TError Error => IsFailure 
        ? field!
        : throw new InvalidOperationException("Cannot access Error on a successful result.");
    
    public static Result<T, TError> Ok(T value) => new(value);
    public static Result<T, TError> Fail(TError error) => new(error);
}