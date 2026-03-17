namespace Opale;

public partial class Result<T, TError>
{
    public async Task<T?> ToOptionAsync()
        => IsSuccess
            ? await Task.FromResult(Value) 
            : default;
}