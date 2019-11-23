namespace Network
{
    internal static class NetExtends
    {
        internal static void SafeInvoke(this NetEventCallback callback, INetEventObject message)
        {
            if (callback != null)
            {
                callback.Invoke(message);
            }
        }

        internal static void SafeInvoke(this NetRecevieEventCallback callback, RawMessage message)
        {
            if (callback != null)
            {
                callback.Invoke(message);
            }
        }
    }
}