using System;
namespace Net
{
    public class Util
    {
        public static void Reset()
        {
            Console.ResetColor();
        }

        public static void Log(string format, params object[] args)
        {
            Console.WriteLine(string.Format(format, args));
        }

        public static void ClientLog(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Log("[CLIENT] " + format, args);
            Console.ResetColor();
        }

        public static void ServerLog(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log("[SERVER] " + format, args);
            Console.ResetColor();
        }
    }
}
