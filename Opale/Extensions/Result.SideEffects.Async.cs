namespace Opale;

public partial class Result<T, TError>
{
    public async Task<Result<T, TError>> OnSuccessAsync(Func<T, Task> action)
    {
        if (IsSuccess)
            await action(Value);
        return this;
    }
    
    public async Task<Result<T, TError>> TapAsync(Func<T, Task> action)
        => await OnSuccessAsync(action);

    public async Task<Result<T, TError>> OnFailureAsync(Func<TError, Task> action)
    {
        if(IsFailure)
            await action(Error);
        return this;
    }
}