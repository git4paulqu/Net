namespace Net
{
    public class DynamicBuffer
    {
        public DynamicBuffer(int totalBytes)
        {
            dataLength = 0;
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
                System.Buffer.BlockCopy(data, offset, buffer, dataLength, count);
            }
            else
            {
                int size = buffer.Length + count - reserveBytesLength;
                byte[] alloc = new byte[size];
                System.Buffer.BlockCopy(buffer, 0, alloc, 0, dataLength);
                System.Buffer.BlockCopy(data, offset, alloc, dataLength, count);
                buffer = alloc;
            }
            dataLength += count;
        }

        public void Clear(int count = int.MaxValue)
        {
            if (count >= dataLength)
            {
                dataLength = 0;
                return;
            }

            for (int i = 0; i < dataLength - count; ++i)
            {
                buffer[i] = buffer[i + count];
            }

            dataLength -= count;
        }

        public void Dispose()
        {
            dataLength = 0;
            buffer = null;
        }

        private int GetReserveBytesLength()
        {
            if (null == buffer)
            {
                return 0;
            }
            return buffer.Length - dataLength;
        }

        public int dataLength { get; private set; }
        public byte[] buffer { get; private set; }
    }
}