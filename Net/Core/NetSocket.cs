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

        public void Dispose()
        {
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
        }

        protected virtual void OnReceiveAsyncCallback(RawMessage message)
        {
            onReceiveCallback.SafeInvoke(message);
        }

        protected virtual void OnReceiveFromAsyncCallback(IPEndPoint remotePoint, RawMessage message)
        {

        }

        protected virtual void Reset()
        {
            ResetSocket();
        }

        protected virtual bool EncodeSend(byte[] buffer, int offset, int count, byte[] data, out int packCount)
        {
            packCount = count;
            return false;
        }

        protected virtual bool DecodeReceive(byte[] buffer, int offset, int count, out int error)
        {
            error = 0;
            return false;
        }

        protected virtual void ResetSocket()
        {
            CloseSocket(socket);
            socket = null;
        }

        protected virtual void CloseSAEA(SocketAsyncEventArgs saea, SAEAPool pool = null)
        {
            if (null != pool)
            {
                pool.Recycle(saea);
            }

            RawMessage message = new RawMessage();
            message.data = remote;
            onClosedCallback.SafeInvoke(message);
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

        public string remote { get; protected set; }
        protected virtual bool ready4Send { get; }
        protected Socket socket { get; set; }
        protected SAEAPool sendSAEAPool { get; private set; }
        protected SAEAPool receiveSAEAPool { get; private set; }
        protected DynamicBuffer receiveBuffer { get; private set; }

        private EndPoint remoterPoint = new IPEndPoint(IPAddress.Any, 0);

        public NetRecevieEventCallback onReceiveCallback { get; set; }
        public NetRecevieEventCallback onClosedCallback { get; set; }
    }
}