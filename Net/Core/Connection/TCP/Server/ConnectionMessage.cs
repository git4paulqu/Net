using System.Net.Sockets;

namespace Net.TCP.Server
{
    public class ConnectionMessage : INetEventObject
    {
        public ConnectionMessage(string remote)
        {
            this.remote = remote;
        }

        public string remote;
    }
}
