namespace Net
{
    public interface INetSetting
    {
        string host { get; }
        int port { get; }
        int defautBufferSize { get; }
    }
}
