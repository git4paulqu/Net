using System;
using System.Net;
using System.Net.Sockets;

namespace Net
{
    public abstract partial class NetSocket
    {
        public void Close()
        {
            OnClose();
        }

        protected void Initialize(int ioNum, int bufferSize = NetDefine.DEFAUT_BUFFER_SIZE)
        {
            sendSAEAPool = new SAEAPool(ioNum, 
                                        new EventHandler<SocketAsyncEventArgs>(OnSAEACompleted),
                                        bufferSize);

            receiveSAEAPool = new SAEAPool(ioNum,
                                           new EventHandler<SocketAsyncEventArgs>(OnSAEACompleted),
                                           bufferSize);

            receiveBuffer = new DynamicBuffer(ioNum * bufferSize);
        }

        protected virtual void OnClose()
        {
            ResetSocket();

            if (null != sendSAEAPool)
            {
                sendSAEAPool.Dispose();
                sendSAEAPool = null;
            }

            if (null != receiveSAEAPool)
            {
                receiveSAEAPool.Dispose();
                receiveSAEAPool = null;
            }

            if (null != receiveBuffer)
            {
                receiveBuffer.Dispose();
                receiveBuffer = null;
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

        protected virtual bool OnSend(byte[] buffer, int offset, int count, byte[] data, out int packCount)
        {
            packCount = count;
            return false;
        }

        protected virtual bool OnReceive(byte[] buffer, int offset, int count, out int error)
        {
            error = 0;
            return false;
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

        protected virtual void CloseSAEA(SocketAsyncEventArgs saea, SAEAPool pool = null)
        {
            CloseSocket(saea.AcceptSocket);
            if (null != pool)
            {
                pool.Recycle(saea);
            }
            OnClose();
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

        protected Socket socket { get; set; }
        protected SAEAPool sendSAEAPool { get; private set; }
        protected SAEAPool receiveSAEAPool { get; private set; }
        protected DynamicBuffer receiveBuffer { get; private set; }

        private EndPoint remoterPoint = new IPEndPoint(IPAddress.Any, 0);
    }
}
