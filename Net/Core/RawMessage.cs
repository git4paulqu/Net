using System;

namespace Net
{
    public class RawMessage : INetEventObject
    {
        public static RawMessage Clone(byte[] data, int start, int length)
        {
            RawMessage message = new RawMessage();
            if (null == data)
            {
                message.size = 0;
                return message;
            }

            message.size = length;
            message.buffer = new byte[length];
            Buffer.BlockCopy(data, start, message.buffer, 0, length);
            return message;
        }

        public void Dispose()
        {
            size = 0;
            buffer = null;
            data = null;
        }

        public int size { get; private set; }
        public byte[] buffer { get; private set; }
        public object data { get; set; }
    }
}