using System;
using System.Net.Sockets;

namespace Net
{
    public class SAEABuffer
    {
        public SAEABuffer(int count, int bufferSize = NetDefine.DEFAUT_BUFFER_SIZE)
        {
            position = 0;
            this.bufferSize = bufferSize;
            ResetBuffer(count * bufferSize);
            freePosition = new ThreadSafedStack<int>();
        }

        public void Bind(SocketAsyncEventArgs saea)
        {
            if (null == saea)
            {
                return;
            }

            int pos;
            freePosition.TryPop(out pos);
            if (pos > 0)
            {
                saea.SetBuffer(buffer, pos, bufferSize);
                return;
            }

            if (totalSize - position < bufferSize)
            {
                ResetBuffer(totalSize * 2);
            }

            saea.SetBuffer(buffer, position, bufferSize);
            position += bufferSize;
        }

        public void UnBind(SocketAsyncEventArgs saea)
        {
            if (null == saea)
            {
                return;
            }
            int pos = saea.Offset;
            Array.Clear(buffer, pos, bufferSize);
            freePosition.Push(pos);
            saea.SetBuffer(null, 0, 0);
        }

        public void Write(byte[] data, int offset, int count)
        {
            if (offset > totalSize || count + offset > totalSize)
            {
                NetDebug.Error("[SAEABuffer] write error.");
                return;
            }
            Buffer.BlockCopy(data, 0, buffer, offset, count);
        }

        public void Dispose()
        {
            position = 0;
            bufferSize = 0;
            totalSize = 0;
            buffer = null;
            freePosition.Clear();
            freePosition = null;
        }

        private void ResetBuffer(int count)
        {
            totalSize = count;

            if (null == buffer)
            {
                buffer = new byte[totalSize];
                return;
            }

            lock (buffer)
            {
                byte[] alloc = new byte[count];
                Buffer.BlockCopy(buffer, 0, alloc, 0, buffer.Length);
                buffer = alloc;
            }
        }

        private int position;
        private int bufferSize;
        private int totalSize;
        private byte[] buffer;
        private ThreadSafedStack<int> freePosition;
    }
}