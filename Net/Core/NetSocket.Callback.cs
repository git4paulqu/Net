using System;
using System.Net.Sockets;

namespace Net
{
    public partial class NetSocket
    {
        protected void OnSAEACompleted(object sender, SocketAsyncEventArgs saea)
        {
            switch (saea.LastOperation)
            {
                case SocketAsyncOperation.Disconnect:
                    DisConnectAsyncCallback(saea);
                    break;

                case SocketAsyncOperation.Receive:
                    ReceiveAsyncCallback(saea);
                    break;

                case SocketAsyncOperation.ReceiveFrom:
                    ReceiveFromAsyncCallback(saea);
                    break;

                case SocketAsyncOperation.Send:
                    SendAsyncCallback(saea);
                    break;

                case SocketAsyncOperation.SendTo:
                    SendToAsyncCallback(saea);
                    break;

                default:
                    OnSAEACompletedCallback(sender, saea);
                    break;
            }
        }

        #region event

        protected virtual void OnSAEACompletedCallback(object sender, SocketAsyncEventArgs saea)
        {

        }

        protected virtual void OnDisConnectAsyncCallback(Socket socket)
        {
            NetDebug.Log("OnDisConnectAsyncCallback");
        }

        protected virtual void OnSendAsyncCallback()
        {

        }

        protected virtual void OnSendToAsyncCallback()
        {

        }

        protected virtual void OnCloseCallback()
        {

        }

        #endregion

        #region callback

        protected void ReceiveAsync()
        {
            SocketAsyncEventArgs saea = AllocReceiveSAEA();
            ReceiveAsync(saea);
        }

        protected virtual void ReceiveAsync(SocketAsyncEventArgs saea)
        {
            try
            {
                bool willRaiseEvent = socket.ReceiveAsync(saea);
                if (!willRaiseEvent)
                {
                    ReceiveAsyncCallback(saea);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected virtual void ReceiveFromAsync()
        {

        }

        protected virtual void SendAsync(byte[] data)
        {
            if (null == data || data.Length == 0)
            {
                NetDebug.Log("[NetSocket] SendAsync, the data can not be null.");
                return;
            }

            SocketAsyncEventArgs saea = AllocSendSAEA();
            try
            {
                OnSend(saea, data);
                bool willRaiseEvent = socket.SendAsync(saea);
                //bool willRaiseEvent = socket.Send(data,  SocketFlags.None);
                if (!willRaiseEvent)
                {
                    SendAsyncCallback(saea);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected virtual void SendToAsync(byte[] data)
        {
            if (null == data || data.Length == 0)
            {
                NetDebug.Log("[NetSocket] SendAsync, the data can not be null.");
                return;
            }
        }

        private void DisConnectAsyncCallback(SocketAsyncEventArgs saea)
        {
            OnDisConnectAsyncCallback(saea.AcceptSocket);
        }

        private void ReceiveAsyncCallback(SocketAsyncEventArgs saea)
        {
            try
            {
                int read = saea.BytesTransferred;
                if (read > 0 && saea.SocketError == SocketError.Success)
                {
                    OnReceive(saea);
                    ReceiveAsync(saea);
                }
                else
                {
                    CloseSAEA(saea, receiveSAEAPool);
                }
            }
            catch { }
        }

        private void ReceiveFromAsyncCallback(SocketAsyncEventArgs saea)
        {

        }

        private void SendAsyncCallback(SocketAsyncEventArgs saea)
        {
            try
            {
                int send = saea.BytesTransferred;
                if (send <= 0 || saea.SocketError != SocketError.Success)
                {
                    CloseSAEA(saea, sendSAEAPool);
                }
                else
                {
                    RecycleSendSAEA(saea);
                }
            }
            catch { }
        }

        private void SendToAsyncCallback(SocketAsyncEventArgs saea)
        {

        }

        #endregion

        protected SocketAsyncEventArgs AllocSendSAEA()
        {
            return sendSAEAPool.Alloc();
        }

        protected SocketAsyncEventArgs AllocReceiveSAEA()
        {
            return receiveSAEAPool.Alloc();
        }

        protected void RecycleSendSAEA(SocketAsyncEventArgs saea)
        {
            sendSAEAPool.Recycle(saea);
            NetDebug.Log("Recycle Send SAEA count:{0} --{1}", sendSAEAPool.count, GetType().ToString());
        }

        protected void RecycleReceiveSAEA(SocketAsyncEventArgs saea)
        {
            receiveSAEAPool.Recycle(saea);
            NetDebug.Log("Recycle Receive SAEA count:{0} --{1}", sendSAEAPool.count, GetType().ToString());
        }

        private void OnSend(SocketAsyncEventArgs saea, byte[] data)
        {
            try
            {
                byte[] buffer = saea.Buffer;
                int offset = saea.Offset;
                int count = data.Length;
                int packCount = count;
                if (OnSend(buffer, offset, count, data, out packCount))
                {
                    saea.SetBuffer(offset, packCount);
                }
                else
                {
                    saea.SetBuffer(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                NetDebug.Error("[NetSocket] OnSend, error: {0}.", ex.Message.ToString());
            }
        }

        private void OnReceive(SocketAsyncEventArgs saea)
        {
            try
            {
                byte[] buffer = saea.Buffer;
                int offset = saea.Offset;
                int count = saea.BytesTransferred;

                int error = 0;
                if (!OnReceive(buffer, offset, count, out error))
                {
                    if (error > 0)
                    {
                        CloseSAEA(saea, receiveSAEAPool);
                        return;
                    }

                    RawMessage message = RawMessage.Clone(buffer, offset, count);
                    OnReceiveAsyncCallback(message);
                }
            }
            catch (Exception ex)
            {
                NetDebug.Error("[NetSocket] OnReceive, error: {0}.", ex.Message.ToString());
            }
        }
    }
}