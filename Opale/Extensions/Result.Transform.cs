namespace Opale;

public partial class Result<T, TError>
{
    public Result<TNew, TError> Map<TNew>(Func<T, TNew> map)
        => IsSuccess 
            ? Result<TNew, TError>.Ok(map(Value)) 
            : Result<TNew, TError>.Fail(Error);
    
    public Result<TNew, TError> Bind<TNew>(Func<T, Result<TNew, TError>> bind)
        => IsSuccess
            ? bind(Value)
            : Result<TNew, TError>.Fail(Error);

    public Result<T, TNewError> MapError<TNewError>(Func<TError, TNewError> map)
        => IsFailure
            ? Result<T, TNewError>.Fail(map(Error))
            : Result<T, TNewError>.Ok(Value);

    public T ToException(Func<TError, Exception> error)
        => IsSuccess
            ? Value
            : throw error(Error);
}