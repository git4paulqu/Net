using System;
using System.Net;
using System.Net.Sockets;

namespace Net
{
    public abstract class NetSocket
    {
        public void Close()
        {
            OnClose();
        }

        protected void Start(Socket socket)
        {
            this.socket = socket;
            state = new StateObject(defautBufferSize);
        }

        #region User Callback

        protected virtual void OnClose()
        {
            if (null != socket)
            {
                socket.Close();
                socket = null;
            }

            if (null != state)
            {
                state.Dispose();
                state = null;
            }
        }

        protected virtual void OnBeginRecevieCallback(RawMessage message)
        {

        }

        protected virtual void OnBeginReceiveFromCallbak(IPEndPoint remotePoint, RawMessage message)
        {

        }

        #endregion

        #region Socket Callback

        protected void BeginSend(byte[] data)
        {
            if (!IsCanSend())
            {
                NetDebug.Log("[NetSocket] BeginSend, can not send.");
                return;
            }

            socket.BeginSend(data,
                0,
                data.Length,
                SocketFlags.None,
                BeginSendCallback,
                null);
        }

        protected void BeginSendTo(IPEndPoint remotePoint, byte[] data)
        {
            if (!IsCanSend())
            {
                NetDebug.Log("[NetSocket] BeginSendTo, can not send.");
                return;
            }

            socket.BeginSendTo(data,
                0,
                data.Length,
                SocketFlags.None,
                remotePoint,
                BeginSendToCallback,
                null);
        }

        protected void BeginRecevie()
        {
            socket.BeginReceive(state.buffer,
                0,
                state.length,
                SocketFlags.None,
                BeginRecevieCallback,
                state);
        }

        protected void BeginRecevieFrom()
        {
            socket.BeginReceiveFrom(state.buffer,
                0,
                state.length,
                SocketFlags.None,
                ref remoterPoint,
                BeginRecevieFromCallback,
                state);
        }

        private void BeginSendCallback(IAsyncResult ar)
        {
            try
            {
                lock (socket)
                {
                    int send = socket.EndSend(ar);
                    if (send == 0)
                    {
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                NetDebug.Log("[NetSocket] BeginSendCallback, error: {0}.", ex.ToString());
            }
        }

        private void BeginSendToCallback(IAsyncResult ar)
        {
            try
            {
                lock (socket)
                {
                    int send = socket.EndSendTo(ar);
                    if (send == 0)
                    {
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                NetDebug.Log("[NetSocket] BeginSendToCallback, error: {0}.", ex.ToString());
            }
        }

        private void BeginRecevieCallback(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            try
            {
                int read = socket.EndReceive(ar);
                if (read == 0)
                {
                    Close();
                    return;
                }

                RawMessage message = ReadMessage(so, read);
                BeginRecevieFrom();
                OnBeginRecevieCallback(message);
            }
            catch (Exception ex)
            {
                NetDebug.Log("[NetSocket] BeginRecevieCallback error:{0}.", ex.ToString());
            }
        }

        private void BeginRecevieFromCallback(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            try
            {
                int read = socket.EndReceiveFrom(ar, ref remoterPoint);
                RawMessage message = ReadMessage(so, read);
                BeginRecevieFrom();
                OnBeginReceiveFromCallbak((IPEndPoint)remoterPoint, message);
            }
            catch (Exception ex)
            {
                NetDebug.Log("[NetSocket] BeginRecevieFromCallback error:{0}.", ex.ToString());
            }
        }

        #endregion

        protected virtual void Reset()
        {
            if (null != socket)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
        }

        private RawMessage ReadMessage(StateObject stateObject, int length)
        {
            return RawMessage.Clone(stateObject.buffer, length);
        }

        protected virtual bool IsCanSend()
        {
            return false;
        }

        protected int defautBufferSize { get; set; }
        protected Socket socket { get; private set; }
        protected StateObject state;

        private EndPoint remoterPoint = new IPEndPoint(IPAddress.Any, 0);
    }
}
