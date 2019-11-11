using Net.TCP;

namespace Net.Test.TCP
{
    public class ClientProxy
    {
        public void Start()
        {
            uid = Util.GetUID().ToString();

            TCPSetting clientSetting = TestDefine.GetTCPSetting();
            client = new TCPClient(clientSetting);
            client.onConnectCallback += OnConnected;
            client.onConnectFailedCallback += OnConnectedFail;
            client.onReceiveCallback += OnRevecie;
            client.Connect();
        }

        public void Stop()
        {
            client.Close();
            Util.ClientLog("Client:{0} closed.", uid);
        }

        private void OnConnected(INetEventObject message)
        {
            Util.ClientLog("Client:{0} OnConnected", uid);
        }

        private void OnConnectedFail(INetEventObject message)
        {
            Util.ClientLog("Client:{0} OnConnectedFail", uid);
        }

        private void OnRevecie(RawMessage rawMessage)
        {

        }

        public string uid { get; set; }

        private TCPClient client;
    }
}
