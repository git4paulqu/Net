using System;

namespace Network
{
    internal class NetDebug
    {
        public static void Log(string format, params object[] arg)
        {
            lock (lockMutex)
            {
                Console.WriteLine("[NET LOG] " + string.Format(format, arg));
            }
        }

        public static void Error(string format, params object[] arg)
        {
            lock (lockMutex)
            {
                Console.WriteLine("[NET ERROR] " + string.Format(format, arg));
            }
        }

        private static object lockMutex = new object();
    }
}