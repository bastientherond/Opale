namespace Opale;

public partial class Result<T, TError>
{
    public T? ToOption()
        => IsSuccess
            ? Value 
            : default;
}