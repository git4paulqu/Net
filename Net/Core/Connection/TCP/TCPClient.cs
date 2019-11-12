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

            Reset();
            ConnetAsync();
        }

        public void Send(byte[] data)
        {
            SendAsync(data);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        protected override void ResetSocket()
        {
            if (null != socket)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            base.ResetSocket();
        }

        protected override void OnRecevieFaild(Socket socket)
        {
            base.OnRecevieFaild(socket);
            Close();
        }

        protected override void OnSAEACompletedCallback(object sender, SocketAsyncEventArgs saea)
        {
            switch (saea.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ConnectAsyncCallback(saea);
                    break;
            }
        }

        private void ConnetAsync()
        {
            IPEndPoint remote = NetUtility.GetIPEndPoint(setting.host, setting.port);
            if (null == remote)
            {
                NetDebug.Log("[TCPClient] remote can not be null.");
                return;
            }

            socket = new Socket(remote.AddressFamily,
                                              SocketType.Stream,
                                              ProtocolType.Tcp);
            socket.Blocking = setting.blocking;
            socket.NoDelay = setting.noDelay;

            SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
            saea.Completed += new EventHandler<SocketAsyncEventArgs>(OnSAEACompleted);
            saea.AcceptSocket = socket;
            saea.RemoteEndPoint = remote;

            bool willRaiseEvent = socket.ConnectAsync(saea);
            if (!willRaiseEvent)
            {
                ConnectAsyncCallback(saea);
            }
        }

        private void ConnectAsyncCallback(SocketAsyncEventArgs saea)
        {
            try
            {
                if (saea.SocketError == SocketError.Success)
                {
                    OnConnectCallback(true, saea.AcceptSocket);
                    saea.AcceptSocket = null;
                }
                else
                {
                    OnConnectCallback(false, null);
                }
            }
            catch (Exception ex)
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
                ReceiveAsync();
                onConnectCallback(null);
                return;
            }

            onConnectFailedCallback(null);
        }

        public NetEventCallback onConnectCallback { get; set; }
        public NetEventCallback onConnectFailedCallback { get; set; }
        public NetEventCallback onDisConnectCallback { get; set; }
    }
}
