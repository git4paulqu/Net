using System;
using System.Net;
using System.Net.Sockets;

namespace Net.TCP
{
    public class TCPServer : TCPConnection
    {
        public TCPServer(TCPSetting setting) : base(setting)
        {

        }

        public void Listen()
        {
            NetDebug.Log("[TCPServer] Listen.");
            Socket listenSocket = new Socket(AddressFamily.InterNetwork,
                                 SocketType.Stream,
                                 ProtocolType.Tcp);
            listenSocket.Blocking = setting.blocking;
            listenSocket.NoDelay = setting.noDelay;
            IPEndPoint remotePoint = NetUtility.GetIPEndPoint(setting.host, setting.port);
            listenSocket.Bind(remotePoint);
            listenSocket.Listen(setting.backlog);

            Start(listenSocket);
            for (int i = 0; i < setting.acceptThreadCount; i++)
            {
                BeginAccept();
            }
        }

        private void BeginAccept()
        {
            socket.BeginAccept(BeginAcceptCallback, null);
        }

        private void BeginAcceptCallback(IAsyncResult ar)
        {
            Socket connectSocket = null;
            try
            {
                lock (socket)
                {
                    connectSocket = socket.EndAccept(ar);
                    OnAcceptCallback(connectSocket);
                }

                BeginAccept();
            }
            catch (Exception ex)
            {
                NetDebug.Log("[TCPServer] BeginAcceptCallback, error: {0}.", ex.ToString());
            }
        }

        private void OnAcceptCallback(Socket socket)
        {
            Server.ConnectionMessage message = new Server.ConnectionMessage(socket);
            onConnectionConnect(message);
        }

        public NetEventCallback onConnectionConnect { get; set; }
        public NetEventCallback onConnectionDropped { get; set; }
    }
}
