namespace Opale.Static;

public static class Result
{
    public static Result<T, Exception> Try<T>(Func<T> func)
    {
        try
        {
            var value = func.Invoke();
            return Result<T, Exception>.Ok(value);
        }
        catch(Exception e)
        {
            return Result<T, Exception>.Fail(e);
        }
    }
}