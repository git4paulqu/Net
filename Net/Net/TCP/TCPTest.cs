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
                client.uid = Util.GetUID().ToString();
                clients.Add(client);
            }
        }

        public static void OnClientClose(string uid)
        {
            ClientProxy client = GetClient(uid);
            if (null != client)
            {
                clients.Remove(client);
            }
        }

        private static ClientProxy GetClient(string uid)
        {
            lock (clients)
            {
                foreach (var item in clients)
                {
                    if (IsMatchWithUID(uid, item.uid))
                    {
                        return item;
                    }
                }
                return null;
            }
        }

        private static void RegisterCommond()
        {
            InputHandle.Register(CommondMap.cls, PringAllClients);
            InputHandle.Register(CommondMap.scls, PringServerConnection);
            InputHandle.Register(CommondMap.killc, ShutdownClient);
            InputHandle.Register(CommondMap.sras, ServerReceiveAutoSend);

            InputHandle.Register(CommondMap.c2stcp, C2STCP);
            InputHandle.Register(CommondMap.s2ctcp, S2CTCP);
        }

        #region commond callback

        private static void PringAllClients(string arg)
        {
            Util.Log("===========================");
            Util.Log("client count:{0}", clients.Count);
            foreach (var item in clients)
            {
                Util.Log("uid:{0}, port:{1}", item.uid, item.local);
            }
            Util.Log("===========================\r\n");
        }

        private static void PringServerConnection(string arg)
        {
            Util.Log("===========================");
            Util.Log("server connection count:{0}", server.server.map_remote2Connection.Count);
            foreach (var item in server.server.map_remote2Connection)
            {
                Util.Log(item.Key);
            }
            Util.Log("===========================\r\n");
        }

        private static void ShutdownClient(string uid)
        {
            ClientProxy client = GetClient(uid);
            if (null != client)
            {
                client.Stop();
            }
        }

        private static void ServerReceiveAutoSend(string art)
        {
            serverReceiveSend = !serverReceiveSend;
            Util.Log("serverReceiveSend:{0}.", serverReceiveSend);
        }

        private static void C2STCP(string arg)
        {
            string[] args = arg.Split('@');
            if (args.Length < 2)
            {
                return;
            }

            string uid = args[0];
            ClientProxy client = GetClient(uid);
            if (null == client)
            {
                Util.Log("[C2STCP] uid:{0} is null", uid);
                return;
            }

            byte[] data = System.Text.Encoding.Default.GetBytes(args[1]);
            client.client.Send(data);
        }

        private static void S2CTCP(string arg)
        {
            string[] args = arg.Split('@');
            if (args.Length < 2)
            {
                return;
            }

            string uid = args[0];
            byte[] data = System.Text.Encoding.Default.GetBytes(args[1]);
            server.server.Send(uid, data);
        }

        private static bool IsMatchWithUID(string src, string dest)
        {
            return src == dest;
        }

        #endregion

        private static ServerProxy server;
        private static List<ClientProxy> clients = new List<ClientProxy>();

        public static bool serverReceiveSend = false;
    }
}