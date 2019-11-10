using System;

namespace Net
{
    public class StateObject
    {
        public StateObject(int bufferSize)
        {
            this.length = bufferSize;
            buffer = new byte[bufferSize];
        }

        public void Dispose()
        {
            length = 0;
            buffer = null;
        }

        public int length { get; private set; }
        public byte[] buffer { get; private set; }
    }
}
