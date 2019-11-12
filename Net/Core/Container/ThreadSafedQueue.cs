using System;
using System.Collections.Generic;


namespace Net
{
    public class ThreadSafedQueue<T>
    {

        public ThreadSafedQueue()
        {
            _queue = new Queue<T>();
        }

        public void Enqueue(T item)
        {
            if (item == null) throw new ArgumentNullException("Items added to a ThreadSafedQueue cannot be null.");
            lock (_queue)
            {
                _queue.Enqueue(item);
            }
        }

        public bool TryDequeue(out T item)
        {
            item = default(T);
            lock (_queue)
            {
                if (_queue.Count > 0)
                {
                    item = _queue.Dequeue();
                }
            }

            return (item != null);
        }

        public bool TryPeek(out T item)
        {
            item = default(T);
            if (_queue.Count > 0)
            {
                item = _queue.Peek();
            }

            return (item != null);
        }

        public void Clear()
        {
            lock (_queue)
            {
                _queue.Clear();
            }
        }

        public int Count { get { return _queue.Count; } }

        Queue<T> _queue;
    }
}
