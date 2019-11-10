using System;

namespace Net
{
    public class NetDebug
    {
        public static void Log(string format, params object[] arg)
        {
            Console.WriteLine("[NET LOG] " + string.Format(format, arg));
        }

        public static void Error(string format, params object[] arg)
        {
            Console.WriteLine("[NET ERROR] " + string.Format(format, arg));
        }
    }
}
