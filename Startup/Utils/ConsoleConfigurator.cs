namespace Startup.Utils;

internal static class ConsoleConfigurator
{
    public static void Configure()
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Clear();
    }
}