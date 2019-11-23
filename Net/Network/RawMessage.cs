using System;

namespace Network
{
    internal sealed class RawMessage : NetEventObject
    {
        public static RawMessage Clone(byte[] data, int offset, int length)
        {
            RawMessage message = new RawMessage();
            if (null == data)
            {
                message.length = 0;
                return message;
            }

            message.length = length;
            message.buffer = new byte[length];
            Buffer.BlockCopy(data, offset, message.buffer, 0, length);
            return message;
        }

        public sealed override void Dispose()
        {
            length = 0;
            buffer = null;
            base.Dispose();
        }

        public int length { get; private set; }
        public byte[] buffer { get; private set; }
    }
}