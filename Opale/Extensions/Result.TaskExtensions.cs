namespace Opale;

/// <summary>
/// Provides async pipeline helpers for tasks that resolve to <see cref="Result{T, TError}"/>.
/// </summary>
public static class ResultTaskExtensions
{
    /// <summary>
    /// Transforms the success value of an asynchronously produced result.
    /// </summary>
    public static async Task<Result<TNew, TError>> MapAsync<T, TError, TNew>(
        this Task<Result<T, TError>> resultTask,
        Func<T, TNew> map)
        => await (await resultTask).MapAsync(map);

    /// <summary>
    /// Transforms the success value of an asynchronously produced result using an async projection.
    /// </summary>
    public static async Task<Result<TNew, TError>> MapAsync<T, TError, TNew>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Task<TNew>> map)
        => await (await resultTask).MapAsync(map);

    /// <summary>
    /// Chains an asynchronously produced result into a result-returning operation.
    /// </summary>
    public static async Task<Result<TNew, TError>> BindAsync<T, TError, TNew>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Result<TNew, TError>> bind)
        => await (await resultTask).BindAsync(bind);

    /// <summary>
    /// Chains an asynchronously produced result into an async result-returning operation.
    /// </summary>
    public static async Task<Result<TNew, TError>> BindAsync<T, TError, TNew>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Task<Result<TNew, TError>>> bind)
        => await (await resultTask).BindAsync(bind);

    /// <summary>
    /// Transforms the error value of an asynchronously produced result.
    /// </summary>
    public static async Task<Result<T, TNewError>> MapErrorAsync<T, TError, TNewError>(
        this Task<Result<T, TError>> resultTask,
        Func<TError, TNewError> map)
        => await (await resultTask).MapErrorAsync(map);

    /// <summary>
    /// Transforms the error value of an asynchronously produced result using an async projection.
    /// </summary>
    public static async Task<Result<T, TNewError>> MapErrorAsync<T, TError, TNewError>(
        this Task<Result<T, TError>> resultTask,
        Func<TError, Task<TNewError>> map)
        => await (await resultTask).MapErrorAsync(map);

    /// <summary>
    /// Executes an action when an asynchronously produced result is successful.
    /// </summary>
    public static async Task<Result<T, TError>> OnSuccessAsync<T, TError>(
        this Task<Result<T, TError>> resultTask,
        Action<T> action)
    {
        var result = await resultTask;
        if (result.IsSuccess)
            action(result.Value);
        return result;
    }

    /// <summary>
    /// Executes an async action when an asynchronously produced result is successful.
    /// </summary>
    public static async Task<Result<T, TError>> OnSuccessAsync<T, TError>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Task> action)
        => await (await resultTask).OnSuccessAsync(action);

    /// <summary>
    /// Alias for <see cref="OnSuccessAsync{T,TError}(Task{Result{T,TError}},Action{T})"/>.
    /// </summary>
    public static Task<Result<T, TError>> TapAsync<T, TError>(
        this Task<Result<T, TError>> resultTask,
        Action<T> action)
        => resultTask.OnSuccessAsync(action);

    /// <summary>
    /// Alias for <see cref="OnSuccessAsync{T,TError}(Task{Result{T,TError}},Func{T,Task})"/>.
    /// </summary>
    public static Task<Result<T, TError>> TapAsync<T, TError>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Task> action)
        => resultTask.OnSuccessAsync(action);

    /// <summary>
    /// Executes an action when an asynchronously produced result is a failure.
    /// </summary>
    public static async Task<Result<T, TError>> OnFailureAsync<T, TError>(
        this Task<Result<T, TError>> resultTask,
        Action<TError> action)
    {
        var result = await resultTask;
        if (result.IsFailure)
            action(result.Error);
        return result;
    }

    /// <summary>
    /// Executes an async action when an asynchronously produced result is a failure.
    /// </summary>
    public static async Task<Result<T, TError>> OnFailureAsync<T, TError>(
        this Task<Result<T, TError>> resultTask,
        Func<TError, Task> action)
        => await (await resultTask).OnFailureAsync(action);

    /// <summary>
    /// Projects an asynchronously produced result into a value.
    /// </summary>
    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Task<Result<T, TError>> resultTask,
        Func<T, TResult> onSuccess,
        Func<TError, TResult> onFailure)
        => (await resultTask).Match(onSuccess, onFailure);

    /// <summary>
    /// Projects an asynchronously produced result into a value using an async success branch.
    /// </summary>
    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Task<TResult>> onSuccess,
        Func<TError, TResult> onFailure)
        => await (await resultTask).MatchAsync(onSuccess, onFailure);

    /// <summary>
    /// Projects an asynchronously produced result into a value using an async failure branch.
    /// </summary>
    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Task<Result<T, TError>> resultTask,
        Func<T, TResult> onSuccess,
        Func<TError, Task<TResult>> onFailure)
        => await (await resultTask).MatchAsync(onSuccess, onFailure);

    /// <summary>
    /// Projects an asynchronously produced result into a value using async branches.
    /// </summary>
    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Task<TResult>> onSuccess,
        Func<TError, Task<TResult>> onFailure)
        => await (await resultTask).MatchAsync(onSuccess, onFailure);
}
