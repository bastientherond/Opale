namespace Opale;

public partial class Result<T, TError>
{
    public Result<T, TError> Recover(Func<TError, T> fallback)
        => IsFailure 
            ? Ok(fallback(Error))
            : Ok(Value);

    public Result<T, TError> RecoverWith(Func<TError, Result<T, TError>> fallback)
        => IsFailure
            ? fallback(Error)
            : Ok(Value);
    
    public T GetValueOrDefault(T defaultValue)
        => IsSuccess 
            ? Value 
            : defaultValue;
}