using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Net.TCP.Server;

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

            IPEndPoint remote = NetUtility.GetIPEndPoint(setting.host, setting.port);
            if (null == remote)
            {
                NetDebug.Log("[TCPServer] ip port can not be null.");
                return;
            }

            socket = new Socket(remote.AddressFamily,
                                SocketType.Stream,
                                ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            socket.Blocking = setting.blocking;
            socket.Bind(remote);
            socket.Listen(setting.backlog);
            AcceptAsync();
        }

        public void Send(string remote, byte[] data)
        {
            CSConnection connection = null;
            if (!map_remote2Connection.TryGetValue(remote, out connection))
            {
                NetDebug.Log("[TCPServer] try send, but the remote:{0} is null.", remote);
                return;
            }
            connection.Send(data);
        }

        protected override void OnSAEACompletedCallback(object sender, SocketAsyncEventArgs saea)
        {
            switch (saea.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    AcceptAsyncCallback(saea);
                    break;
            }
        }

        protected virtual void AcceptAsync()
        {
            SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
            saea.Completed += new EventHandler<SocketAsyncEventArgs>(OnSAEACompleted);

            bool willRaiseEvent = socket.AcceptAsync(saea);
            if (!willRaiseEvent)
            {
                AcceptAsyncCallback(saea);
            }
        }

        private void AcceptAsyncCallback(SocketAsyncEventArgs saea)
        {
            try
            {
                if (saea.SocketError == SocketError.Success)
                {
                    OnAcceptCallback(saea.AcceptSocket);
                }
                AcceptAsync();
            }
            catch (Exception ex)
            {
                NetDebug.Error("[TCPServer] AcceptAsyncCallback error:{0}.", ex.Message.ToString());
            }
        }

        private void OnConnectionRecevie(RawMessage message)
        {
            onReceiveCallback.SafeInvoke(message);
        }

        private void OnAcceptCallback(Socket socket)
        {
            TryInitConnection(socket);
            Server.ConnectionMessage message = new Server.ConnectionMessage(socket.RemoteEndPoint.ToString());
            onConnectionConnect(message);
        }

        private void TryInitConnection(Socket socket)
        {
            if (null == socket)
            {
                return;
            }

            string remote = socket.RemoteEndPoint.ToString();
            if (!map_remote2Connection.ContainsKey(remote))
            {
                CSConnection connection = new CSConnection(socket, OnConnectionRecevie);
                map_remote2Connection.Add(remote, connection);
            }
        }

        private void TryRemoveConnection(Socket socket)
        {
            if (null == socket)
            {
                return;
            }

            string remote = socket.RemoteEndPoint.ToString();
            CSConnection connection = null;
            if (map_remote2Connection.TryGetValue(remote, out connection))
            {
                if (null != connection)
                {
                    connection.Close();
                }
                map_remote2Connection.Remove(remote);

                ClientDroppedMessage message = new ClientDroppedMessage();
                message.remote = remote;
                onConnectionDropped.SafeInvoke(message);
            }
        }

        public NetEventCallback onConnectionConnect { get; set; }
        public NetEventCallback onConnectionDropped { get; set; }

        private Dictionary<string, CSConnection> map_remote2Connection = new Dictionary<string, CSConnection>();
    }
}