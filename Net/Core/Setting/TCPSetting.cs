namespace Net
{
    public class TCPSetting : INetSetting
    {
        public TCPSetting(
            string host,
            int port,
            int defautBufferSize = NetDefine.DEFAUT_BUFFER_SIZE,
            bool bloking = false,
            bool noDelay = true,
            int backlog = 0,
            int acceptThreadCount = 1)
        {
            this.host = host;
            this.port = port;
            this.defautBufferSize = defautBufferSize;
            this.blocking = blocking;
            this.noDelay = noDelay;
            this.backlog = backlog;
            this.acceptThreadCount = acceptThreadCount;
        }

        public string host { get; private set; }
        public int port { get; private set; }
        public int defautBufferSize { get; private set; }
        public bool blocking { get; private set; }
        public bool noDelay { get; private set; }
        public int backlog { get; private set; }
        public int acceptThreadCount { get; private set; }
    }
}
