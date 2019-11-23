using System.Net.Sockets;
using SAEA = System.Net.Sockets.SocketAsyncEventArgs;

namespace Network
{
    internal partial class NetSocket
    {
        protected void SendAsync(byte[] data, int offset, int count)
        {

        }

        protected void SendToAsync(byte[] data, int offset, int count)
        {

        }

        protected void ReceiveAsync(SAEA saea)
        {

        }

        protected void ReceiveFromAsync(SAEA saea)
        {

        }

        protected virtual bool OnSAEACompletedCallback(object sender, SAEA saea)
        {
            return false;
        }

        private void ProcessSAEA(object sender, SAEA saea)
        {
            switch (saea.LastOperation)
            {
                case SocketAsyncOperation.Send:
                    ProcessSendAsync(saea);
                    break;

                case SocketAsyncOperation.SendTo:
                    ProcessSendToAsync(saea);
                    break;

                case SocketAsyncOperation.Receive:
                    ProcessReceiveAsync(saea);
                    break;

                case SocketAsyncOperation.ReceiveFrom:
                    ProcessReceiveFromAsync(saea);
                    break;

                default:
                    if (!OnSAEACompletedCallback(sender, saea))
                    {
                        NetDebug.Log("{0} ProcessSAEA, not process!", LOG_PREFIX);
                    }
                    break;
            }
        }

        private void ProcessSendAsync(SAEA saea)
        {

        }

        private void ProcessSendToAsync(SAEA saea)
        {

        }

        private void ProcessReceiveAsync(SAEA saea)
        {

        }

        private void ProcessReceiveFromAsync(SAEA saea)
        {

        }
    }
}