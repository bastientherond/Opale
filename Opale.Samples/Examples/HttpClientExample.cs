using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Opale;

namespace Opale.Samples.Examples;

// ── Domain models ─────────────────────────────────────────────────────────────

public record WeatherForecast(
    [property: JsonPropertyName("name")] string City,
    [property: JsonPropertyName("main")] WeatherMain Main,
    [property: JsonPropertyName("weather")] WeatherCondition[] Weather
);

public record WeatherMain(
    [property: JsonPropertyName("temp")] double Temp,
    [property: JsonPropertyName("feels_like")] double FeelsLike,
    [property: JsonPropertyName("humidity")] int Humidity
);

public record WeatherCondition(
    [property: JsonPropertyName("description")] string Description
);

public record WeatherSummary(string City, string Description, double TempCelsius);

public enum WeatherError
{
    CityNotFound,
    ServiceUnavailable,
    InvalidResponse,
    NetworkError,
}

// ── HTTP Service ──────────────────────────────────────────────────────────────

file class WeatherService(HttpClient http)
{
    // Simulate the call without a real API key — uses a stub instead
    public async Task<Result<WeatherForecast, WeatherError>> GetForecastAsync(string city)
    {
        // In production this would be: http.GetFromJsonAsync<WeatherForecast>($"...{city}...")
        // Here we simulate responses to keep the sample self-contained.
        await Task.Delay(10); // simulate latency

        return city.ToLowerInvariant() switch
        {
            "paris"  => Result<WeatherForecast, WeatherError>.Ok(
                            new WeatherForecast("Paris",
                                new WeatherMain(18.5, 17.0, 62),
                                [new WeatherCondition("partly cloudy")])),
            "london" => Result<WeatherForecast, WeatherError>.Ok(
                            new WeatherForecast("London",
                                new WeatherMain(12.1, 10.5, 80),
                                [new WeatherCondition("light rain")])),
            "unknown" => Result<WeatherForecast, WeatherError>.Fail(WeatherError.CityNotFound),
            _         => Result<WeatherForecast, WeatherError>.Fail(WeatherError.ServiceUnavailable),
        };
    }
}

// ── Application layer ─────────────────────────────────────────────────────────

file class WeatherApp(WeatherService service)
{
    public async Task<Result<WeatherSummary, string>> GetSummaryAsync(string city)
    {
        var result = await service.GetForecastAsync(city);

        return result
            .MapError(e => e switch
            {
                WeatherError.CityNotFound       => $"City '{city}' was not found.",
                WeatherError.ServiceUnavailable => "Weather service is currently unavailable.",
                WeatherError.InvalidResponse    => "Received an unexpected response from the service.",
                WeatherError.NetworkError       => "A network error occurred. Check your connection.",
                _                               => "An unknown error occurred.",
            })
            .Map(f => new WeatherSummary(
                f.City,
                f.Weather.FirstOrDefault()?.Description ?? "unknown",
                Math.Round(f.Main.Temp, 1)));
    }
}

// ── Example entry point ───────────────────────────────────────────────────────

public static class HttpClientExample
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Example 3: Async HTTP / External Service ===");
        Console.WriteLine();

        var service = new WeatherService(new HttpClient());
        var app = new WeatherApp(service);

        var cities = new[] { "Paris", "London", "Unknown", "Atlantis" };

        // ── Sequential async pipeline ─────────────────────────────────────────

        foreach (var city in cities)
        {
            var result = await app.GetSummaryAsync(city);

            result
                .OnSuccess(s => Console.WriteLine($"  [OK]   {s.City}: {s.Description}, {s.TempCelsius}°C"))
                .OnFailure(e => Console.WriteLine($"  [FAIL] {e}"));
        }

        Console.WriteLine();

        // ── Parallel fetch + collect ──────────────────────────────────────────

        Console.WriteLine("  Parallel fetch for Paris & London:");

        var tasks = new[] { "Paris", "London" }
            .Select(city => app.GetSummaryAsync(city));

        var results = await Task.WhenAll(tasks);

        var summaries = results
            .Where(r => r.IsSuccess)
            .Select(r => r.Value);

        var warmest = summaries.MaxBy(s => s.TempCelsius);
        Console.WriteLine(warmest is not null
            ? $"  Warmest city: {warmest.City} ({warmest.TempCelsius}°C)"
            : "  No data available.");

        Console.WriteLine();

        // ── Chaining async transforms ─────────────────────────────────────────

        Console.WriteLine("  Async chain: fetch → validate temperature → format alert:");

        var alert = await app.GetSummaryAsync("London")
            .ContinueWith(t =>
                t.Result
                    .Bind(s => s.TempCelsius < 15
                        ? Result<WeatherSummary, string>.Ok(s)
                        : Result<WeatherSummary, string>.Fail("Temperature is comfortable, no alert needed."))
                    .Map(s => $"COLD ALERT: {s.City} is only {s.TempCelsius}°C — wear a coat!")
                    .GetValueOrDefault("No alert."));

        Console.WriteLine($"  {alert}");
        Console.WriteLine();
    }
}
