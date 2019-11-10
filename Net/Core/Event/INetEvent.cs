namespace Net
{
    public interface INetEvent
    {
        NetRecevieEventCallback onReceiveCallback { get; set; }
    }

    public delegate void NetEventCallback(INetEventObject data);
    public delegate void NetRecevieEventCallback(RawMessage message);
}
