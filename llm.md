# Opale LLM Usage Guide

This file explains how an LLM should use Opale in C# code.
Opale is a lightweight Result type for explicit success/failure handling.

Package:

```bash
dotnet add package Opale --version 1.0.0-alpha.5
```

Import:

```csharp
using Opale;
```

Do not use `using Opale.Static;`. `Result.Try` and `Result.TryAsync` are in the `Opale` namespace.

## Core Type

Use `Result<T, TError>` to represent either:

- success: contains a value of type `T`
- failure: contains an error of type `TError`

Create results:

```csharp
Result<User, string> ok = Result<User, string>.Ok(user);
Result<User, string> fail = Result<User, string>.Fail("User not found");
```

Implicit success conversion is supported:

```csharp
Result<int, string> result = 42;
```

Inspect state:

```csharp
if (result.IsSuccess)
{
    var value = result.Value;
}

if (result.IsFailure)
{
    var error = result.Error;
}
```

Important:

- Accessing `Value` on a failure throws `InvalidOperationException`.
- Accessing `Error` on a success throws `InvalidOperationException`.
- Prefer `Match`, `Map`, `Bind`, or recovery methods instead of directly reading `Value`/`Error`.

## Pattern Matching

Use `Match` to convert a result into another value:

```csharp
string message = result.Match(
    onSuccess: user => $"Welcome {user.Name}",
    onFailure: error => $"Error: {error}");
```

Use void `Match` for side effects:

```csharp
result.Match(
    onSuccess: user => Console.WriteLine(user.Name),
    onFailure: error => Console.WriteLine(error));
```

Async matching:

```csharp
string message = await result.MatchAsync(
    onSuccess: user => Task.FromResult($"Welcome {user.Name}"),
    onFailure: error => $"Error: {error}");
```

Available async match forms:

- `MatchAsync(Func<T, Task<TResult>>, Func<TError, TResult>)`
- `MatchAsync(Func<T, TResult>, Func<TError, Task<TResult>>)`
- `MatchAsync(Func<T, Task<TResult>>, Func<TError, Task<TResult>>)`
- `MatchAsync(Func<T, Task>, Func<TError, Task>)`

## Transformations

Use `Map` to transform a success value while preserving the error type:

```csharp
Result<UserDto, string> dto = result.Map(user => new UserDto(user.Id, user.Name));
```

If the result is a failure, `Map` does not call the mapper and propagates the error.

Use `Bind` when the next operation also returns a `Result`:

```csharp
Result<UserDto, string> dto = GetUser(id)
    .Bind(ValidateUser)
    .Map(user => new UserDto(user.Id, user.Name));
```

Use `MapError` to transform the error:

```csharp
Result<User, ErrorResponse> mapped = result.MapError(error => new ErrorResponse(error));
```

Async variants:

```csharp
Result<UserDto, string> dto = await result
    .BindAsync(user => ValidateUserAsync(user))
    .MapAsync(user => new UserDto(user.Id, user.Name))
    .MapErrorAsync(error => error.ToUpperInvariant());
```

Available async transformation methods on `Result<T, TError>`:

- `MapAsync(Func<T, TNew>)`
- `MapAsync(Func<T, Task<TNew>>)`
- `BindAsync(Func<T, Result<TNew, TError>>)`
- `BindAsync(Func<T, Task<Result<TNew, TError>>>)`
- `MapErrorAsync(Func<TError, TNewError>)`
- `MapErrorAsync(Func<TError, Task<TNewError>>)`

## Task Result Pipelines

Opale supports fluent pipelines that start with `Task<Result<T, TError>>`.
Use this when the first operation is async.

```csharp
Result<UserDto, string> result = await GetUserAsync(id)
    .BindAsync(user => ValidateUserAsync(user))
    .MapAsync(user => new UserDto(user.Id, user.Name))
    .OnSuccessAsync(dto => audit.WriteAsync(dto.Id))
    .OnFailureAsync(error => logger.WriteAsync(error));
```

Available methods on `Task<Result<T, TError>>`:

- `MapAsync(Func<T, TNew>)`
- `MapAsync(Func<T, Task<TNew>>)`
- `BindAsync(Func<T, Result<TNew, TError>>)`
- `BindAsync(Func<T, Task<Result<TNew, TError>>>)`
- `MapErrorAsync(Func<TError, TNewError>)`
- `MapErrorAsync(Func<TError, Task<TNewError>>)`
- `OnSuccessAsync(Action<T>)`
- `OnSuccessAsync(Func<T, Task>)`
- `TapAsync(Action<T>)`
- `TapAsync(Func<T, Task>)`
- `OnFailureAsync(Action<TError>)`
- `OnFailureAsync(Func<TError, Task>)`
- `MatchAsync(...)`

Prefer chaining these extensions instead of writing repeated `var result = await ...; if (result.IsFailure) ...`.

## Recovery

Use `Recover` to replace a failure with a success value:

```csharp
Result<int, string> recovered = result.Recover(error => 0);
```

Use `RecoverWith` to replace a failure with another result:

```csharp
Result<int, string> recovered = result.RecoverWith(error =>
    error == "not_found"
        ? Result<int, string>.Ok(0)
        : Result<int, string>.Fail(error));
```

Use `GetValueOrDefault` to unwrap with a fallback:

```csharp
int value = result.GetValueOrDefault(-1);
```

Async variants:

```csharp
Result<int, string> recovered = await result.RecoverAsync(error => 0);
Result<int, string> recoveredAsync = await result.RecoverWithAsync(error => LoadFallbackAsync(error));
int value = await result.GetValueOrDefaultAsync(-1);
```

Available recovery methods:

- `Recover(Func<TError, T>)`
- `RecoverWith(Func<TError, Result<T, TError>>)`
- `GetValueOrDefault(T)`
- `RecoverAsync(Func<TError, T>)`
- `RecoverAsync(Func<TError, Task<T>>)`
- `RecoverWithAsync(Func<TError, Result<T, TError>>)`
- `RecoverWithAsync(Func<TError, Task<Result<T, TError>>>)`
- `GetValueOrDefaultAsync(T)`
- `GetValueOrDefaultAsync(Task<T>)`

## Side Effects

Use side-effect methods for logging, metrics, auditing, or debugging without changing the result.

```csharp
Result<User, string> result = GetUser(id)
    .OnSuccess(user => logger.LogInformation("Found {UserId}", user.Id))
    .OnFailure(error => logger.LogWarning("Failed: {Error}", error));
```

`Tap` is an alias for `OnSuccess`.

```csharp
Result<User, string> result = GetUser(id)
    .Tap(user => metrics.Count("user.found"));
```

Async side effects:

```csharp
Result<User, string> result = await GetUser(id)
    .OnSuccessAsync(user => audit.WriteAsync(user.Id))
    .OnFailureAsync(error => audit.WriteAsync(error));
```

Available side-effect methods:

- `OnSuccess(Action<T>)`
- `Tap(Action<T>)`
- `OnFailure(Action<TError>)`
- `OnSuccessAsync(Func<T, Task>)`
- `TapAsync(Func<T, Task>)`
- `OnFailureAsync(Func<TError, Task>)`

## Inspection

Use inspection methods when a boolean is needed.

```csharp
bool isAdmin = result.IsSuccessAnd(user => user.Role == "admin");
bool isNotFound = result.IsFailureAnd(error => error == "not_found");
bool containsUser = result.IsContains(expectedUser);
```

`IsContains` uses `EqualityComparer<T>.Default`, including null comparisons.

Async variants:

```csharp
bool isAdmin = await result.IsSuccessAndAsync(user => user.Role == "admin");
bool isAllowed = await result.IsSuccessAndAsync(user => permissionService.IsAllowedAsync(user));
bool contains = await result.IsContainsAsync(expectedUser);
```

Available inspection methods:

- `IsSuccessAnd(Func<T, bool>)`
- `IsFailureAnd(Func<TError, bool>)`
- `IsContains(T)`
- `IsSuccessAndAsync(Func<T, bool>)`
- `IsSuccessAndAsync(Func<T, Task<bool>>)`
- `IsFailureAndAsync(Func<TError, bool>)`
- `IsFailureAndAsync(Func<TError, Task<bool>>)`
- `IsContainsAsync(T)`
- `IsContainsAsync(Task<T>)`

## Conversion

Use `ToOption` to get the success value or `default` on failure:

```csharp
User? user = result.ToOption();
```

Async:

```csharp
User? user = await result.ToOptionAsync();
```

Available conversion methods:

- `ToOption()`
- `ToOptionAsync()`

## Exception Interop

Use `Result.Try` to capture exceptions as `Result<T, Exception>`:

```csharp
Result<int, Exception> parsed = Result.Try(() => int.Parse(input));
```

Use `Result.TryAsync` for async delegates:

```csharp
Result<User, Exception> user = await Result.TryAsync(() => client.GetUserAsync(id));
```

Use `ToException` only when calling exception-based APIs or crossing a boundary that expects exceptions:

```csharp
User user = result.ToException(error => new InvalidOperationException(error));
```

Async:

```csharp
User user = await result.ToExceptionAsync(error =>
    Task.FromResult<Exception>(new InvalidOperationException(error)));
```

Available exception interop methods:

- `Result.Try(Func<T>)`
- `Result.TryAsync(Func<Task<T>>)`
- `ToException(Func<TError, Exception>)`
- `ToExceptionAsync(Func<TError, Task<Exception>>)`

## Equality And Display

Results support value equality:

```csharp
Result<int, string>.Ok(1) == Result<int, string>.Ok(1);       // true
Result<int, string>.Fail("e") == Result<int, string>.Fail("e"); // true
```

`ToString()` returns:

- `Ok(value)` for success
- `Fail(error)` for failure

## Recommended LLM Patterns

Prefer:

```csharp
return await LoadUserAsync(id)
    .BindAsync(ValidateUserAsync)
    .MapAsync(MapToDto)
    .MapErrorAsync(ToApiError);
```

Avoid:

```csharp
var result = await LoadUserAsync(id);
if (result.IsFailure)
{
    return Result<UserDto, ApiError>.Fail(ToApiError(result.Error));
}

var validated = await ValidateUserAsync(result.Value);
...
```

Prefer typed errors instead of strings for application code:

```csharp
public enum RegistrationError
{
    InvalidEmail,
    UsernameTaken,
    PasswordTooWeak
}
```

Use strings only for simple samples or throwaway code.

## Common Mistakes

- Do not access `Value` unless `IsSuccess` is true.
- Do not access `Error` unless `IsFailure` is true.
- Do not catch exceptions around a whole Result pipeline; use `Result.Try` or `Result.TryAsync` at exception boundaries.
- Do not use `Map` when the mapper returns a `Result`; use `Bind`.
- Do not use `Bind` when the mapper returns a plain value; use `Map`.
- Do not use `Opale.Static`; import `Opale`.
- Do not manually unwrap and rewrap failures when `MapError`, `Bind`, `Recover`, or `Match` expresses the intent.

