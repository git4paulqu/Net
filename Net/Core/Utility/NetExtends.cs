namespace Net
{
    public static class NetExtends
    {
        public static void SafeInvoke(this NetEventCallback callback, INetEventObject message)
        {
            if (callback != null)
            {
                callback.Invoke(message);
            }
        }

        public static void SafeInvoke(this NetRecevieEventCallback callback, RawMessage message)
        {
            if (callback != null)
            {
                callback.Invoke(message);
            }
        }
    }
}