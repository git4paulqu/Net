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
            SAEA saea = saeaPool.Alloc();
            ReceiveAsync(saea.data);
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
            socket.Send(data, data.Length, SocketFlags.None);
        }

        protected virtual void SendToAsync()
        {

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
                    RawMessage message = RawMessage.Clone(saea.Buffer, saea.Offset, read);
                    OnReceiveAsyncCallback(message);
                }
                else
                {
                    CloseSAEA(saea);
                }
            }
            catch { }
        }

        private void ReceiveFromAsyncCallback(SocketAsyncEventArgs saea)
        {

        }

        private void SendAsyncCallback(SocketAsyncEventArgs saea)
        {

        }

        private void SendToAsyncCallback(SocketAsyncEventArgs saea)
        {

        }

        #endregion
    }
}