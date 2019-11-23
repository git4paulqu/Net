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

        protected virtual void OnSAEACompletedCallback(object sender, SocketAsyncEventArgs saea)
        {

        }

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
                NetDebug.Log("[NetSocket] ReceiveAsync, error:{0}.", ex.Message.ToString());
            }
        }

        protected virtual void ReceiveFromAsync()
        {

        }

        protected virtual void SendAsync(byte[] data)
        {
            if (!ready4Send)
            {
                NetDebug.Log("[NetSocket] SendAsync, not ready for send.");
                return;
            }

            if (null == data || data.Length == 0)
            {
                NetDebug.Log("[NetSocket] SendAsync, the data can not be null.");
                return;
            }

            if (data.Length > NetDefine.MAX_MESSAGE_LENGTH)
            {
                NetDebug.Error("[Netsocket] SendAsync, the data length:{0} is greater message max length:{1}.",
                               data.Length,
                               NetDefine.MAX_MESSAGE_LENGTH);
                return;
            }

            SocketAsyncEventArgs saea = AllocSendSAEA();
            try
            {
                EncodeSend(saea, data);
                bool willRaiseEvent = socket.SendAsync(saea);
                if (!willRaiseEvent)
                {
                    SendAsyncCallback(saea);
                }
            }
            catch (Exception ex)
            {
                NetDebug.Log("[NetSocket] SendAsync, error:{0}.", ex.Message.ToString());
            }
        }

        protected virtual void SendToAsync(byte[] data)
        {
            if (!ready4Send)
            {
                NetDebug.Log("[NetSocket] SendToAsync, not ready for send.");
                return;
            }

            if (null == data || data.Length == 0)
            {
                NetDebug.Log("[NetSocket] SendAsync, the data can not be null.");
                return;
            }

            if (data.Length > NetDefine.MAX_MESSAGE_LENGTH)
            {
                NetDebug.Error("[Netsocket] SendToAsync, the data length:{0} is greater message max length:{1}.",
                               data.Length,
                               NetDefine.MAX_MESSAGE_LENGTH);
                return;
            }
        }

        private void ReceiveAsyncCallback(SocketAsyncEventArgs saea)
        {
            try
            {
                int read = saea.BytesTransferred;
                if (read > 0 && saea.SocketError == SocketError.Success)
                {
                    DecodeReceive(saea);
                    ReceiveAsync(saea);
                }
                else
                {
                    CloseSAEA(saea, receiveSAEAPool);
                }
            }
            catch (Exception ex)
            {
                NetDebug.Log("[NetSocket] ReceiveAsyncCallback, error:{0}.", ex.Message.ToString());
            }
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

        private void EncodeSend(SocketAsyncEventArgs saea, byte[] data)
        {
            try
            {
                byte[] buffer = saea.Buffer;
                int offset = saea.Offset;
                int count = data.Length;
                int packCount = count;
                if (EncodeSend(buffer, offset, count, data, out packCount))
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

        private void DecodeReceive(SocketAsyncEventArgs saea)
        {
            try
            {
                byte[] buffer = saea.Buffer;
                int offset = saea.Offset;
                int count = saea.BytesTransferred;

                int error = 0;
                if (!DecodeReceive(buffer, offset, count, out error))
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