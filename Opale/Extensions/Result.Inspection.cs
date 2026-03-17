namespace Opale;

public partial class Result<T, TError>
{
    public bool IsSuccessAnd(Func<T, bool> predicate)
        => IsSuccess && predicate(Value);
    
    public bool IsFailureAnd(Func<TError, bool> predicate)
        => IsFailure && predicate(Error);
    
    public bool IsContains(T value)
        => IsSuccess 
           && Value is not null 
           && value is not null 
           && EqualityComparer<T>.Default.Equals(value, Value);
}