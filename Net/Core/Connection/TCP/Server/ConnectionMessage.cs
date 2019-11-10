using System.Net.Sockets;

namespace Net.TCP.Server
{
    public class ConnectionMessage : INetEventObject
    {
        public ConnectionMessage(Socket socket)
        {
            this.socket = socket;
        }

        public Socket socket;
    }
}
