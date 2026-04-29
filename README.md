# Opale

A lightweight **Result type** for functional error handling in C#. Model success and failure explicitly, eliminate defensive exception handling, and compose operations safely with full async support.

```csharp
Result<User, string> result = await GetUserAsync(id)
    .BindAsync(user => ValidateEmailAsync(user))
    .MapAsync(user => user with { IsVerified = true });

result.Match(
    onSuccess: user => Console.WriteLine($"Welcome, {user.Name}"),
    onFailure: error => Console.WriteLine($"Error: {error}")
);
```

---

## Table of Contents

- [Installation](#installation)
- [Getting Started](#getting-started)
- [API Reference](#api-reference)
  - [Creating Results](#creating-results)
  - [Pattern Matching](#pattern-matching)
  - [Transformations](#transformations)
  - [Recovery](#recovery)
  - [Side Effects](#side-effects)
  - [Inspection](#inspection)
  - [Conversion](#conversion)
  - [Exception Interop](#exception-interop)
- [Async Support](#async-support)
- [License](#license)

---

## Installation

> Supports **.NET 8.0**, **.NET 9.0**, and **.NET 10.0**.

```bash
dotnet add package Opale
```

---

## Getting Started

The core type is `Result<T, TError>` — a discriminated union that holds either a success value or an error, never both.

```csharp
using Opale;

// Create results
Result<int, string> success = Result<int, string>.Ok(42);
Result<int, string> failure = Result<int, string>.Fail("Something went wrong");

// Implicit conversion from T
Result<int, string> implicit = 42; // equivalent to Ok(42)

// Check state
if (result.IsSuccess)
    Console.WriteLine(result.Value);

if (result.IsFailure)
    Console.WriteLine(result.Error);
```

### Wrapping exception-prone code

Use `Result.Try` to capture exceptions as typed failures:

```csharp
Result<int, Exception> result = Result.Try(() => int.Parse(userInput));
```

---

## API Reference

### Creating Results

| Method | Description |
|--------|-------------|
| `Result<T, TError>.Ok(value)` | Creates a successful result |
| `Result<T, TError>.Fail(error)` | Creates a failed result |
| `implicit operator Result<T, TError>(T)` | Implicit conversion from `T` to `Ok(value)` |
| `Result.Try(Func<T>)` | Wraps a delegate, catching exceptions as `Result<T, Exception>` |

---

### Pattern Matching

Execute different branches based on the result state.

```csharp
// With return value
string message = result.Match(
    onSuccess: value => $"Got {value}",
    onFailure: error => $"Failed: {error}"
);

// Void branches
result.Match(
    onSuccess: value => Console.WriteLine(value),
    onFailure: () => Console.WriteLine("Failed")
);
```

| Method | Description |
|--------|-------------|
| `Match<TResult>(onSuccess, onFailure)` | Pattern match returning a value |
| `Match(onSuccess, onFailure)` | Pattern match for side effects |

---

### Transformations

Chain operations without unwrapping.

```csharp
Result<string, string> result = Result<int, string>.Ok(42)
    .Map(n => n * 2)          // 84
    .Map(n => n.ToString())   // "84"
    .Bind(s => s.Length > 0
        ? Result<string, string>.Ok(s)
        : Result<string, string>.Fail("Empty"));

// Transform the error type
Result<int, int> mapped = failedResult.MapError(err => err.Length);
```

| Method | Description |
|--------|-------------|
| `Map<TNew>(Func<T, TNew>)` | Transforms the success value |
| `Bind<TNew>(Func<T, Result<TNew, TError>>)` | Chains a result-returning operation |
| `MapError<TNewError>(Func<TError, TNewError>)` | Transforms the error value |
| `ToException(Func<TError, Exception>)` | Unwraps value or throws a mapped exception |

---

### Recovery

Provide fallback values or results on failure.

```csharp
int value = result
    .Recover(error => 0)          // fallback value
    .GetValueOrDefault(-1);       // safe unwrap

Result<int, string> recovered = failedResult
    .RecoverWith(error => Result<int, string>.Ok(0));
```

| Method | Description |
|--------|-------------|
| `Recover(Func<TError, T>)` | Returns an `Ok` result from the fallback value |
| `RecoverWith(Func<TError, Result<T, TError>>)` | Chains a fallback result |
| `GetValueOrDefault(T)` | Returns the value or a default on failure |

---

### Side Effects

Execute actions without breaking the chain.

```csharp
Result<User, string> result = await GetUserAsync(id)
    .OnSuccess(user => logger.LogInfo($"Found user {user.Id}"))
    .OnFailure(error => logger.LogError(error));
```

| Method | Description |
|--------|-------------|
| `OnSuccess(Action<T>)` | Executes an action if the result is successful |
| `OnFailure(Action<TError>)` | Executes an action if the result is a failure |
| `Tap(Action<T>)` | Alias for `OnSuccess` |

All side-effect methods return `this`, preserving the original result for further chaining.

---

### Inspection

Evaluate predicates against the result's value or error.

```csharp
bool isAdult = result.IsSuccessAnd(user => user.Age >= 18);
bool isNotFound = result.IsFailureAnd(err => err == "NOT_FOUND");
bool hasValue = result.IsContains(expectedUser);
```

| Method | Description |
|--------|-------------|
| `IsSuccessAnd(Func<T, bool>)` | `true` if successful and predicate holds |
| `IsFailureAnd(Func<TError, bool>)` | `true` if failed and predicate holds |
| `IsContains(T)` | `true` if successful and value equals the given value |

---

### Conversion

```csharp
// Returns the value or null/default on failure
User? user = result.ToOption();
```

| Method | Description |
|--------|-------------|
| `ToOption()` | Returns `T?` — the value on success, `default` on failure |

---

### Exception Interop

Bridge between `Result` and exception-based APIs.

```csharp
// Throws if the result is a failure
User user = result.ToException(error => new NotFoundException(error));

// Wrap exception-prone code
Result<Data, Exception> safe = Result.Try(() => JsonSerializer.Deserialize<Data>(json));
```

---

## Async Support

Every operation has a full async counterpart. Async pipelines can start from either `Result<T, TError>` or `Task<Result<T, TError>>`, which keeps railway-oriented code fluent without manually awaiting each step.

```csharp
Result<UserDto, string> result = await GetUserAsync(id)           // Task<Result<User, string>>
    .BindAsync(user => FetchPermissionsAsync(user))               // async bind
    .MapAsync(user => MapToDto(user))                             // sync map in async context
    .OnSuccessAsync(dto => auditLog.WriteAsync($"Read {dto.Id}")) // async side effect
    .MatchAsync(
        onSuccess: dto => dto,
        onFailure: error => UserDto.Empty
    );
```

Most async variants accept both `Func<T, Task<TResult>>` and `Func<T, TResult>` so you can mix sync and async delegates freely.

---

## License

MIT — see [LICENSE](LICENSE) for details.
