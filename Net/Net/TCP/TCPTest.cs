using System.Threading;
using Net.Test.TCP;
using Net.TCP;
using Net.Test.TCP;
using System.Collections.Generic;

namespace Net
{
    public class TCPTest
    {
        public static void Start()
        {

            Util.Log("=============TCP START=============");

            server = new ServerProxy();
            server.Start();

            InitClient(1);

            System.Console.ReadKey();
            Util.Log("=============TCP END=============");
            
        }

        private static void InitClient(int num)
        {
            for (int i = 0; i < num; i++)
            {
                ClientProxy client = new ClientProxy();
                client.Start();
                clients.Add(client);
            }
        }

        private static void ShutdownAllClients()
        {
            foreach (var item in clients)
            {
                item.Stop();
            }
        }

        private static ServerProxy server;
        private static List<ClientProxy> clients = new List<ClientProxy>();
    }
}