namespace Opale;

public partial class Result<T, TError>
{
    #region Match
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<TError, TResult> onFailure)
        => IsSuccess ? onSuccess(Value) : onFailure(Error);

    public void Match(Action<T> onSuccess, Action onFailure)
    {
        if (IsSuccess)
            onSuccess(Value);
        else
            onFailure();
    }
    #endregion
}