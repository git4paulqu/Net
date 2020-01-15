using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Net.UDP
{
    public class UDPTest
    {
        public static void Start()
        {
            RUDPConnection s = new RUDPConnection();
            RUDPConnection c = new RUDPConnection();

            s.OnPacketReceived += (UDPPacket p) =>
            {
                Debug(true, "[SERVER]: {0}", Encoding.ASCII.GetString(p.Data));
                s.Send(p.Src, data: p.Data);
            };


            int clientCount = 1;
            c.OnConnected += (EndPoint ip) =>
            {
                for (int i = 0; i < clientCount; i++)
                {
                    c.Send(i.ToString());
                }
            };

            c.OnPacketReceived += (UDPPacket p) =>
            {
                Debug(false, "[CLIENT]: {0}", Encoding.ASCII.GetString(p.Data));
            };

            s.Listen("127.0.0.1", 50080);
            c.Connect("127.0.0.1", 50080);
        }

        private static void Debug(bool server, string content, params object[] args)
        {
            Console.ForegroundColor = server ? ConsoleColor.Red : ConsoleColor.Yellow;
            Console.WriteLine(string.Format(content, args));
            Console.ResetColor();
        }
    }
}
