using System.Net.Sockets;

namespace Net.TCP.Server
{
    public class CSConnection : TCPConnection
    {
        public CSConnection(Socket socket) : base (socket)
        {
            this.socket = socket;
            socket.Blocking = false;
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
            message.data = remote;
            base.OnReceiveAsyncCallback(message);
        }
    }
}