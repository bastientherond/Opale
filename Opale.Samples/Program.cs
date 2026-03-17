using Opale.Samples.Examples;

Console.WriteLine();
Console.WriteLine("╔══════════════════════════════════════════════════════╗");
Console.WriteLine("║             Opale — Usage Samples                   ║");
Console.WriteLine("╚══════════════════════════════════════════════════════╝");
Console.WriteLine();

UserRegistrationExample.Run();

CsvParsingExample.Run();

await HttpClientExample.RunAsync();

await OrderProcessingExample.RunAsync();

Console.WriteLine("All examples completed.");
