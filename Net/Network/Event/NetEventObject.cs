namespace Network
{
    internal class NetEventObject : INetEventObject
    {
        public virtual void Dispose()
        {
            userData = null;
        }

        public object userData { get; set; }
    }
}