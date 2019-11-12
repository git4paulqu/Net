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

            freePosition.Push(saea.Offset);
            saea.SetBuffer(null, 0, 0);
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
            buffer = new byte[totalSize];
        }

        private int position;
        private int bufferSize;
        private int totalSize;
        private byte[] buffer;
        private ThreadSafedStack<int> freePosition;
    }
}