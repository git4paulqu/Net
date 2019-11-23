namespace Network
{
    internal interface INetEvent
    {
        NetRecevieEventCallback onReceiveCallback { get; set; }
    }

    internal delegate void NetEventCallback(INetEventObject data);
    internal delegate void NetRecevieEventCallback(RawMessage message);
}