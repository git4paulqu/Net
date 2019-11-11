using System;
namespace Net
{
    public class Util
    {

        #region log

        public static void Log(string format, params object[] args)
        {
            Console.WriteLine(string.Format(format, args));
        }

        public static void ClientLog(string format, params object[] args)
        {
            lock (logLockObject)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Log("[CLIENT] " + format, args);
                Console.ResetColor();
            }
        }

        public static void ServerLog(string format, params object[] args)
        {
            lock (logLockObject)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Log("[SERVER] " + format, args);
                Console.ResetColor();
            }
        }

        private static object logLockObject = new object();

        #endregion

        #region uid

        public static ulong GetUID()
        {
            return uid++;
        }

        public static ulong uid = 0;
        #endregion
    }
}
