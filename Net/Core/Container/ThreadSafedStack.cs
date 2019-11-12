using System;
using System.Collections.Generic;

namespace Net
{
    public class ThreadSafedStack<T>
    {

        public ThreadSafedStack()
        {
            _stack = new Stack<T>();
        }

        public void Push(T item)
        {
            if (item == null) throw new ArgumentNullException("Items added to a ThreadSafedStack cannot be null.");
            lock (_stack)
            {
                _stack.Push(item);
            }
        }

        public bool TryPop(out T item)
        {
            item = default(T);
            lock (_stack)
            {
                if (_stack.Count > 0)
                {
                    item = _stack.Pop();
                }
            }

            return (item != null);
        }

        public bool TryPeek(out T item)
        {
            item = default(T);
            if (_stack.Count > 0)
            {
                item = _stack.Peek();
            }

            return (item != null);
        }

        public void Clear()
        {
            lock (_stack)
            {
                _stack.Clear();
            }
        }

        public int Count { get { return _stack.Count; } }

        Stack<T> _stack;
    }
}

