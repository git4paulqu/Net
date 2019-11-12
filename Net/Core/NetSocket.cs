using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Net
{
    public abstract partial class NetSocket
    {
        public void Close()
        {
            OnClose();
        }

        protected void Initialize(int ioNum)
        {
            saeaPool = new SAEAPool(ioNum, new EventHandler<SocketAsyncEventArgs>(OnSAEACompleted));
        }

        protected virtual void OnClose()
        {
            ResetSocket();

            if (null != saeaPool)
            {
                saeaPool.Dispose();
                saeaPool = null;
            }
        }

        protected virtual void OnReceiveAsyncCallback(RawMessage message)
        {

        }

        protected virtual void OnReceiveFromAsyncCallback(IPEndPoint remotePoint, RawMessage message)
        {

        }

        protected virtual void OnRecevieFaild(Socket socket)
        {

        }

        protected virtual void Reset()
        {
            ResetSocket();
        }

        protected virtual void ResetSocket()
        {
            CloseSocket(socket);
            socket = null;
        }

        protected virtual bool IsCanSend()
        {
            return false;
        }

        private void CloseSocket(SocketAsyncEventArgs saea)
        {
            OnClose();
            CloseSocket(saea.AcceptSocket);
            CloseSAEA(saea);
        }

        private void CloseSocket(Socket closeSocket)
        {
            try
            {
                if (null != closeSocket)
                {
                    closeSocket.Shutdown(SocketShutdown.Both);
                    closeSocket.Close();
                }
            }
            catch { }
        }

        private void CloseSAEA(SocketAsyncEventArgs saea)
        {
            saeaPool.Recycle(saea.UserToken as SAEA);
        }

        protected Socket socket { get; set; }
        protected SAEAPool saeaPool { get; private set; }

        private EndPoint remoterPoint = new IPEndPoint(IPAddress.Any, 0);
    }
}
