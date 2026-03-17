using Opale;
using Opale.Static;

namespace Opale.Samples.Examples;

// ── Domain models ─────────────────────────────────────────────────────────────

public record UserRegistrationRequest(string Username, string Email, string Password);

public record User(Guid Id, string Username, string Email, string PasswordHash);

public enum RegistrationError
{
    UsernameTooShort,
    UsernameAlreadyTaken,
    InvalidEmail,
    PasswordTooWeak,
}

// ── Validation helpers ────────────────────────────────────────────────────────

file static class Validate
{
    public static Result<string, RegistrationError> Username(string username) =>
        username.Length >= 3
            ? Result<string, RegistrationError>.Ok(username)
            : Result<string, RegistrationError>.Fail(RegistrationError.UsernameTooShort);

    public static Result<string, RegistrationError> Email(string email) =>
        email.Contains('@') && email.Contains('.')
            ? Result<string, RegistrationError>.Ok(email)
            : Result<string, RegistrationError>.Fail(RegistrationError.InvalidEmail);

    public static Result<string, RegistrationError> Password(string password) =>
        password.Length >= 8 && password.Any(char.IsDigit)
            ? Result<string, RegistrationError>.Ok(password)
            : Result<string, RegistrationError>.Fail(RegistrationError.PasswordTooWeak);
}

// ── Fake infrastructure ───────────────────────────────────────────────────────

file static class UserRepository
{
    private static readonly HashSet<string> _takenUsernames = ["alice", "bob"];

    public static Result<string, RegistrationError> EnsureUsernameAvailable(string username) =>
        _takenUsernames.Contains(username.ToLower())
            ? Result<string, RegistrationError>.Fail(RegistrationError.UsernameAlreadyTaken)
            : Result<string, RegistrationError>.Ok(username);

    public static User Save(UserRegistrationRequest req) =>
        new(Guid.NewGuid(), req.Username, req.Email, $"hashed:{req.Password}");
}

// ── Registration service ──────────────────────────────────────────────────────

file static class RegistrationService
{
    public static Result<User, RegistrationError> Register(UserRegistrationRequest req) =>
        Validate.Username(req.Username)
            .Bind(UserRepository.EnsureUsernameAvailable)
            .Bind(_ => Validate.Email(req.Email))
            .Bind(_ => Validate.Password(req.Password))
            .Map(_ => UserRepository.Save(req));
}

// ── Example entry point ───────────────────────────────────────────────────────

public static class UserRegistrationExample
{
    public static void Run()
    {
        Console.WriteLine("=== Example 1: User Registration ===");
        Console.WriteLine();

        var scenarios = new[]
        {
            new UserRegistrationRequest("al", "alice@example.com", "Password1"),   // username too short
            new UserRegistrationRequest("alice", "alice@example.com", "Password1"), // username taken
            new UserRegistrationRequest("charlie", "not-an-email", "Password1"),   // invalid email
            new UserRegistrationRequest("charlie", "charlie@example.com", "weak"),  // weak password
            new UserRegistrationRequest("charlie", "charlie@example.com", "Secure1!"), // valid
        };

        foreach (var req in scenarios)
        {
            var result = RegistrationService.Register(req);

            result
                .OnSuccess(user => Console.WriteLine($"  [OK]   Registered '{user.Username}' (id={user.Id})"))
                .OnFailure(error => Console.WriteLine($"  [FAIL] '{req.Username}' -> {error}"));
        }

        Console.WriteLine();

        // ── Demonstrate Map / Bind chaining + side effects ────────────────────

        Console.WriteLine("  Pipeline with side effects and recovery:");

        var result2 = RegistrationService.Register(
                new UserRegistrationRequest("charlie", "charlie@example.com", "Secure1!"))
            .OnSuccess(u => Console.WriteLine($"  [LOG]  User saved: {u.Email}"))
            .OnFailure(e => Console.WriteLine($"  [LOG]  Registration failed: {e}"))
            .Map(u => $"Welcome, {u.Username}!");

        Console.WriteLine($"  Response: {result2.GetValueOrDefault("Registration failed.")}");
        Console.WriteLine();
    }
}
