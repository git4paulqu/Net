using System;
using System.Net.Sockets;

namespace Net.TCP
{
    public class TCPConnection : NetSocket, INetEvent
    {
        public TCPConnection(TCPSetting setting)
        {
            if (null == setting)
            {
                NetDebug.Error("[TCPConnection] the setting can not be null.");
                return;
            }

            this.setting = setting;
            Initialize(this.setting.ioNum);
        }

        protected virtual void OnReceiveCallback(RawMessage message)
        {
            onReceiveCallback.SafeInvoke(message);
        }

        protected sealed override void OnReceiveAsyncCallback(RawMessage message)
        {
            OnReceiveCallback(message);
        }

        protected override bool IsCanSend()
        {
            return connected;
        }

        public bool connected
        {
            get
            {
                if (null == socket)
                {
                    return false;
                }
                return socket.Connected;
            }
        }

        public NetRecevieEventCallback onReceiveCallback { get; set; }

        protected TCPSetting setting;
    }
}
