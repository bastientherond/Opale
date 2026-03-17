namespace Opale;

public partial class Result<T, TError>
{
    /// <summary>
    /// Converts the result to a nullable value.
    /// Returns the success value when successful; <see langword="default"/> otherwise.
    /// </summary>
    /// <returns>The success value, or <see langword="default"/> for <typeparamref name="T"/>.</returns>
    public T? ToOption()
        => IsSuccess
            ? Value
            : default;
}