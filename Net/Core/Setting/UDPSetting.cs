namespace Net
{
    public class UDPSetting : INetSetting
    {
        public UDPSetting(
          string host,
          int port,
          int ioNum = NetDefine.DEFAUT_IONUM,
          int defautBufferSize = NetDefine.DEFAUT_BUFFER_SIZE)
        {
            this.host = host;
            this.port = port;
            this.ioNum = ioNum;
            this.defautBufferSize = defautBufferSize;
        }

        public string host { get; private set; }
        public int port { get; private set; }
        public int ioNum { get; private set; }
        public int defautBufferSize { get; private set; }
    }
}
