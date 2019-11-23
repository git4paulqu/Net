using System.Net.Sockets;

namespace Network
{
    internal abstract partial class NetSocket
    {
        public void Dispose()
        {

        }

        protected virtual void Encode()
        {
        }

        protected virtual void Decode()
        {

        }



        protected Socket socket { get; set; }
        protected SAEAPool saeas { get; private set; }
        protected DynamicBuffer messages { get; private set; }

        private string LOG_PREFIX = "[NetSocket]";
    }
}