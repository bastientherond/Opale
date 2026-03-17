namespace Opale;

public partial class Result<T, TError>
{
    public static implicit operator Result<T, TError>(T value)
        => Ok(value);
}