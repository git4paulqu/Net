using System;
using System.Net;
using System.Net.Sockets;

namespace Net.TCP
{
    public class TCPClient : TCPConnection
    {
        public TCPClient(TCPSetting setting) : base(setting)
        {

        }

        public void Connect()
        {
            NetDebug.Log("[TCPClient] Try to Connect {0}:{1}.", setting.host, setting.port);
            BeginConnect();
        }

        protected virtual void BeginConnect()
        {
            Reset();

            Socket connectSocket = new Socket(AddressFamily.InterNetwork,
                                 SocketType.Stream,
                                 ProtocolType.Tcp);
            connectSocket.Blocking = setting.blocking;
            connectSocket.NoDelay = setting.noDelay;

            IPEndPoint remote = NetUtility.GetIPEndPoint(setting.host, setting.port);
            if (null == remote)
            {
                NetDebug.Log("[TCPClient] remote can not be null.");
                return;
            }

            connectSocket.BeginConnect(remote, BeginConnectCallback, connectSocket);
        }

        private void BeginConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket connectSocket = ar.AsyncState as Socket;
                lock (connectSocket)
                {
                    connectSocket.EndConnect(ar);
                }

                OnConnectCallback(connectSocket.Connected, connectSocket);
            }
            catch (SocketException ex)
            {
                NetDebug.Error("[TCPClient] conncet callback error:{0}.", ex.Message.ToString());
                OnConnectCallback(false, null);
            }
        }

        private void OnConnectCallback(bool connect, Socket connectSocket)
        {
            NetDebug.Log("[TCPClient] connect to {0}:{1} result:{2}.",
                setting.host,
                setting.port,
                connect);

            if (connect)
            {
                Start(connectSocket);
                BeginRecevie();
                onConnectCallback(null);
                return;
            }

            onConnectFailedCallback(null);
        }

        protected override void OnClose()
        {
            base.OnClose();
            onDisConnectCallback.SafeInvoke(null);
        }

        public bool connected
        {
            get
            {
                if (null == socket)
                {
                    return false;
                }
                return socket.Connected;
            }
        }

        protected override bool IsCanSend()
        {
            return connected;
        }

        public NetEventCallback onConnectCallback { get; set; }
        public NetEventCallback onConnectFailedCallback { get; set; }
        public NetEventCallback onDisConnectCallback { get; set; }
    }
}
