namespace TheIsleEvrimaPlayerTracker.Core
{
    internal class Logging
    {
        internal static void WriteLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] ");
            Console.ResetColor();

            Console.WriteLine(message);
        }

        internal static void EmptyLine()
        {
            Console.WriteLine();
        }
    }
}
