using System.Net.Sockets;

namespace Net.TCP.Server
{
    public class CSConnection : NetSocket
    {
        public CSConnection(Socket socket, NetRecevieEventCallback recevieEventCallback)
        {
            this.socket = socket;
            socket.Blocking = false;
            this.recevieEventCallback = recevieEventCallback;
            remote = socket.RemoteEndPoint.ToString();
            Initialize(NetDefine.DEFAUT_IONUM);
            ReceiveAsync();
        }

        public void Send(byte[] data)
        {
            SendAsync(data);
        }

        protected override void OnReceiveAsyncCallback(RawMessage message)
        {
            base.OnReceiveAsyncCallback(message);

            message.data = remote;
            recevieEventCallback.SafeInvoke(message);
        }

        public string remote { get; private set; }
        private NetRecevieEventCallback recevieEventCallback;
    }
}