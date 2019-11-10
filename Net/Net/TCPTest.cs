using System.Threading;
using Net.TCP;

namespace Net
{
    public class TCPTest
    {
        public static void Start()
        {

            Util.Log("=============TCP START=============");

            string host = "127.0.0.1";
            int port = 50010;

            TCPSetting serverSetting = new TCPSetting(host, port);
            TCPServer server = new TCPServer(serverSetting);
            server.onConnectionConnect += OnAccepted;
            server.onConnectionDropped += OnConnectionDropped;
            server.onReceiveCallback += OnServerRevecie;
            server.Listen();

            TCPSetting clientSetting = new TCPSetting(host, port);
            TCPClient client = new TCPClient(clientSetting);
            client.onConnectCallback += OnConnected;
            client.onConnectFailedCallback += OnConnectedFail;
            client.onReceiveCallback += OnClienRevecie;
            client.Connect();

            System.Console.ReadKey();
            Util.Log("=============TCP END=============");
            client.Close();
            server.Close();
        }

        private static void OnConnected(INetEventObject message)
        {
            Util.ClientLog("OnConnected");
        }

        private static void OnConnectedFail(INetEventObject message)
        {
            Util.ClientLog("OnConnectedFail");
        }

        private static void OnClienRevecie(RawMessage rawMessage)
        {

        }

        private static void OnAccepted(INetEventObject message)
        {
            TCP.Server.ConnectionMessage connectionMessage = message as TCP.Server.ConnectionMessage;
            Util.ServerLog("accepted form:{0}", connectionMessage.socket.RemoteEndPoint.ToString());
        }

        private static void OnConnectionDropped(INetEventObject message)
        {
            TCP.Server.ConnectionMessage connectionMessage = message as TCP.Server.ConnectionMessage;
            Util.ServerLog("OnConnectionDropped form:{0}", connectionMessage.socket.RemoteEndPoint.ToString());
        }


        private static void OnServerRevecie(RawMessage rawMessage)
        {

        }
    }
}