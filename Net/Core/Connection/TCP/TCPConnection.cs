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
        }

        protected virtual void OnReceiveCallback(RawMessage message)
        {
            onReceiveCallback.SafeInvoke(message);
        }

        protected sealed override void OnBeginRecevieCallback(RawMessage message)
        {
            OnReceiveCallback(message);
        }

        public NetRecevieEventCallback onReceiveCallback { get; set; }

        protected TCPSetting setting;
    }
}
