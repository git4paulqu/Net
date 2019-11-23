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

        public TCPConnection(Socket socket)
        {
            this.socket = socket;
            Initialize(NetDefine.DEFAUT_IONUM);
        }

        protected override bool EncodeSend(byte[] buffer, int offset, int count, byte[] data, out int packCount)
        {
            int MESSAGE_LENGTH_SIZE = NetDefine.MESSAGE_LENGTH_SIZE;
            byte[] length = System.BitConverter.GetBytes(data.Length);
            System.Buffer.BlockCopy(length, 0, buffer, offset, MESSAGE_LENGTH_SIZE);
            System.Buffer.BlockCopy(data, 0, buffer, offset + MESSAGE_LENGTH_SIZE, count);
            packCount = count + MESSAGE_LENGTH_SIZE;
            return true;
        }

        protected override bool DecodeReceive(byte[] buffer, int offset, int count, out int error)
        {
            error = 0;
            receiveBuffer.Copy(buffer, offset, count);

            int MESSAGE_LENGTH_SIZE = NetDefine.MESSAGE_LENGTH_SIZE;
            while (receiveBuffer.dataLength > MESSAGE_LENGTH_SIZE)
            {
                int contentLength = System.BitConverter.ToInt32(receiveBuffer.buffer, 0);

                if ((contentLength < 0) ||
                    (contentLength > NetDefine.MAX_MESSAGE_LENGTH) ||
                    (receiveBuffer.dataLength > NetDefine.MAX_MESSAGE_LENGTH))
                {
                    error = 1;
                    return false;
                }

                if ((receiveBuffer.dataLength - MESSAGE_LENGTH_SIZE) >= contentLength)
                {
                    RawMessage message = RawMessage.Clone(receiveBuffer.buffer, MESSAGE_LENGTH_SIZE, contentLength);
                    OnReceiveAsyncCallback(message);
                    receiveBuffer.Clear(MESSAGE_LENGTH_SIZE + contentLength);
                }
                else
                {
                    return true;
                }
            }

            return true;
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

        protected override bool ready4Send
        {
            get {
                return connected;
            }
        }

        protected TCPSetting setting;
    }
}