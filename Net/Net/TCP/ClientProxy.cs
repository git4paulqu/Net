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
            client.onDisConnectCallback += OnDisConnectCallback;
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

            for (int i = 0; i < 5; i++)
            {
                string content = string.Format("hello form {0} - {1}.", uid, i);
                byte[] data = System.Text.Encoding.Default.GetBytes(content);
                client.Send(data);
            }
           
        }

        private void OnConnectedFail(INetEventObject message)
        {
            Util.ClientLog("Client:{0} OnConnectedFail", uid);
        }

        private void OnDisConnectCallback(INetEventObject message)
        {
            Util.ClientLog("Client:{0} OnDisConnect", uid);
        }

        private void OnRevecie(RawMessage rawMessage)
        {
            string remote = (string)rawMessage.data;
            string content = System.Text.Encoding.Default.GetString(rawMessage.buffer);

            Util.ClientLog("OnRevecie from:{0} content:{1}", remote, content);
        }

        public string uid { get; set; }

        private TCPClient client;
    }
}
