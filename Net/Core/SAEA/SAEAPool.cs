using System;
using System.Net.Sockets;

namespace Net
{
    public class SAEAPool
    {
        public SAEAPool(int ioNum,
                        EventHandler<SocketAsyncEventArgs> callback,
                        int bufferSize = NetDefine.DEFAUT_BUFFER_SIZE)
        {
            saeaCallback = callback;
            pool = new ThreadSafedStack<SocketAsyncEventArgs>();
            buffer = new SAEABuffer(ioNum, bufferSize);
        }

        public SocketAsyncEventArgs Alloc()
        {
            SocketAsyncEventArgs saea;
            if (!pool.TryPop(out saea))
            {
                saea = InitSAEA();
            }

            buffer.Bind(saea);
            return saea;
        }

        public void Recycle(SocketAsyncEventArgs saea)
        {
            buffer.UnBind(saea);
            if (null != pool)
            {
                pool.Push(saea);
            }
        }

        public virtual void Dispose()
        {
            int count = pool.Count;
            for (int i = 0; i < count; i++)
            {
                SocketAsyncEventArgs item = null;
                if (pool.TryPop(out item))
                {
                    item.Dispose();
                }
            }
            pool.Clear();

            if (null != buffer)
            {
                buffer.Dispose();
            }

            buffer = null;
            pool = null;
            saeaCallback = null;
        }

        private SocketAsyncEventArgs InitSAEA()
        {
            SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
            saea.Completed += saeaCallback;
            return saea;
        }

        public int count
        {
            get
            {
                if (null == pool)
                {
                    return 0;
                }
                return pool.Count;
            }
        }

        private ThreadSafedStack<SocketAsyncEventArgs> pool;
        private EventHandler<SocketAsyncEventArgs> saeaCallback;
        public SAEABuffer buffer;
    }
}