namespace Net
{
    public class NetDefine
    {
        /// <summary>
        /// 默认的IO数量
        /// </summary>
        public const int DEFAUT_IONUM = 2;

        /// <summary>
        /// 最大收发包字节数
        /// </summary>
        public const int MAX_MESSAGE_LENGTH = 64000;

        /// <summary>
        /// 消息包长度字节数
        /// </summary>
        public const int MESSAGE_LENGTH_SIZE = 4;

        /// <summary>
        /// 默认的buffer长度字节数
        /// </summary>
        public const int DEFAUT_BUFFER_SIZE = MAX_MESSAGE_LENGTH + MESSAGE_LENGTH_SIZE;
    }
}
