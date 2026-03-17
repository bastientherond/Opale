namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Asynchronously converts the result to a nullable value.
    /// Returns the success value when successful; <see langword="default"/> otherwise.
    /// </summary>
    /// <returns>
    /// A task resolving to the success value, or <see langword="default"/> for
    /// <typeparamref name="T"/>.
    /// </returns>
    public async Task<T?> ToOptionAsync()
        => IsSuccess
            ? await Task.FromResult(Value)
            : default;
}