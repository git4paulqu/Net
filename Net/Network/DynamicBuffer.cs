namespace Network
{
    internal class DynamicBuffer
    {
        public DynamicBuffer(int totalBytes)
        {
            usedLength = 0;
            buffer = new byte[totalBytes];
        }

        public void Copy(byte[] data)
        {
            Copy(data, 0, data.Length);
        }

        public void Copy(byte[] data, int offset, int count)
        {
            if (null == data)
            {
                NetDebug.Error("[DynamicBuffer] Copy, the data can not be null.");
                return;
            }

            int reserveBytesLength = GetReserveBytesLength();
            if (reserveBytesLength >= count)
            {
                System.Buffer.BlockCopy(data, offset, buffer, usedLength, count);
            }
            else
            {
                int size = buffer.Length + count - reserveBytesLength;
                byte[] alloc = new byte[size];
                System.Buffer.BlockCopy(buffer, 0, alloc, 0, usedLength);
                System.Buffer.BlockCopy(data, offset, alloc, usedLength, count);
                buffer = alloc;
            }
            usedLength += count;
        }

        public void Clear(int count = int.MaxValue)
        {
            if (count >= usedLength)
            {
                usedLength = 0;
                return;
            }

            for (int i = 0; i < usedLength - count; ++i)
            {
                buffer[i] = buffer[i + count];
            }

            usedLength -= count;
        }

        public void Dispose()
        {
            usedLength = 0;
            buffer = null;
        }

        private int GetReserveBytesLength()
        {
            if (null == buffer)
            {
                return 0;
            }
            return buffer.Length - usedLength;
        }

        public int usedLength { get; private set; }
        public byte[] buffer { get; private set; }
    }
}