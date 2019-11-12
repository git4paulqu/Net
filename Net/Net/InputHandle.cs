using System;
using System.Collections.Generic;
using System.Threading;

namespace Net
{
    public class InputHandle
    {
        public static void Start()
        {
            Thread inputThread = new Thread(HandleInput);
            inputThread.Start();
        }

        public static void Register(string commond, Action<string> callback)
        {
            commondCallback[commond] = callback;
        }

        public static void UnRegister(string commond)
        {
            commondCallback.Remove(commond);
        }

        private static void HandleInput()
        {
            while (true)
            {
                string input = System.Console.ReadLine();
                TryHandleInput(input);
            }
        }

        private static void TryHandleInput(string input)
        {
            string commond = string.Empty;
            int index = input.IndexOf(" ", 0);
            if (index < 1)
            {
                commond = input;
            }
            else {
                commond = input.Substring(0, index).Trim();
            }

            Action<string> callback = null;
            if (commondCallback.TryGetValue(commond, out callback))
            {
                string args = string.Empty;
                PareCommond(input, commond, out args);
                if (null != callback)
                {
                    callback(args);
                }
            }
        }

        private static void PareCommond(string input, string define, out string args)
        {
            args = string.Empty;
            if (input.StartsWith(define + " "))
            {
                args = input.Substring(define.Length).Trim();
            }
        }

        private static Dictionary<string, Action<string>> commondCallback = new Dictionary<string, Action<string>>();
    }
}
