namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Projects the result into a value of type <typeparamref name="TResult"/> by executing
    /// <paramref name="onSuccess"/> when the result is successful, or <paramref name="onFailure"/>
    /// when it is a failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected value.</typeparam>
    /// <param name="onSuccess">Function executed with the success value.</param>
    /// <param name="onFailure">Function executed with the error value.</param>
    /// <returns>The value returned by whichever branch was executed.</returns>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<TError, TResult> onFailure)
        => IsSuccess ? onSuccess(Value) : onFailure(Error);

    /// <summary>
    /// Executes a side-effecting action based on the result state.
    /// </summary>
    /// <param name="onSuccess">Action executed with the success value.</param>
    /// <param name="onFailure">Action executed with the error value.</param>
    public void Match(Action<T> onSuccess, Action<TError> onFailure)
    {
        if (IsSuccess)
            onSuccess(Value);
        else
            onFailure(Error);
    }
}