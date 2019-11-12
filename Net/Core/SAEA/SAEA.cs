using System;
using System.Net.Sockets;

namespace Net
{
    public class SAEA
    {
        public SAEA(EventHandler<SocketAsyncEventArgs> callback)
        {
            data = new SocketAsyncEventArgs();
            data.UserToken = this;
            data.Completed += callback;
        }

        public void Dispose()
        {

        }

        public SocketAsyncEventArgs data { get; private set; }
    }
}
