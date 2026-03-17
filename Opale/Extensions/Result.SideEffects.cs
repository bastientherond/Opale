namespace Opale;

public partial class Result<T, TError>
{
    public Result<T, TError> OnSuccess(Action<T> action)
    {
        if (IsSuccess)
            action(Value);
        return this;
    }
    
    public Result<T, TError> Tap(Action<T> action)
        => OnSuccess(action);

    public Result<T, TError> OnFailure(Action<TError> action)
    {
        if(IsFailure)
            action(Error);
        return this;
    }
}