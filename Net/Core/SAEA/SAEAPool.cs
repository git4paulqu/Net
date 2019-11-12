using System;
using System.Net.Sockets;

namespace Net
{
    public class SAEAPool
    {
        public SAEAPool(int ioNum, EventHandler<SocketAsyncEventArgs> callback)
        {
            saeaCallback = callback;
            pool = new ThreadSafedStack<SAEA>();
            buffer = new SAEABuffer(ioNum);
        }

        public SAEA Alloc()
        {
            SAEA saea;
            if (!pool.TryPop(out saea))
            {
                saea = InitSAEA();
            }

            buffer.Bind(saea.data);
            return saea;
        }

        public void Recycle(SAEA saea)
        {
            buffer.UnBind(saea.data);
            if (null != pool)
            {
                pool.Push(saea);
            }
        }

        public void Dispose()
        {
            int count = pool.Count;
            for (int i = 0; i < count; i++)
            {
                SAEA item = null;
                if (pool.TryPop(out item))
                {
                    item.Dispose();
                }
            }
            pool.Clear();
            pool = null;
        }

        private SAEA InitSAEA()
        {
            SAEA saea = new SAEA(saeaCallback);
            return saea;
        }

        private ThreadSafedStack<SAEA> pool;
        private SAEABuffer buffer;
        private EventHandler<SocketAsyncEventArgs> saeaCallback;
    }
}