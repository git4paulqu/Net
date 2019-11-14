using Net.Test.TCP;
using System.Collections.Generic;
using System.Threading;

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

            RegisterCommond();

            //Thread.Sleep(2000);
            //Shutdown();
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

        private static void Shutdown(string args)
        {
            foreach (var item in clients)
            {
                item.Stop();
            }
            //server.Stop();
        }

        private static void RegisterCommond()
        {
            InputHandle.Register(CommondMap.clientlist, PringAllClients);
            InputHandle.Register(CommondMap.shutdownclient, Shutdown);
        }

        #region commond callback

        private static void PringAllClients(string arg)
        {
            Util.Log("===========================");
            Util.Log("client count:{0}", clients.Count);
            foreach (var item in clients)
            {
                Util.Log(item.uid);
            }
            Util.Log("===========================\r\n");
        }

        #endregion

        private static ServerProxy server;
        private static List<ClientProxy> clients = new List<ClientProxy>();
    }
}