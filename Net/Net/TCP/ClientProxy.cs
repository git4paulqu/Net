using Net.TCP;

namespace Net.Test.TCP
{
    public class ClientProxy
    {
        public void Start()
        {
            TCPSetting clientSetting = TestDefine.GetTCPSetting();
            client = new TCPClient(clientSetting);
            client.onConnectCallback += OnConnected;
            client.onConnectFailedCallback += OnConnectedFail;
            client.onClosedCallback += OnCloseCallback;
            client.onReceiveCallback += OnRevecie;
            client.Connect();
        }

        public void Stop()
        {
            client.Close();
        }

        private void OnConnected(INetEventObject message)
        {
            Util.ClientLog("Client:{0} OnConnected", uid);
        }

        private void OnConnectedFail(INetEventObject message)
        {
            Util.ClientLog("Client:{0} OnConnectedFail", uid);
        }

        private void OnCloseCallback(INetEventObject message)
        {
            RawMessage msg = message as RawMessage;
            string port = msg.data as string;
            Util.ClientLog("Client OnClose port:{0} uid:{1} ", port, uid);
            if (port != string.Empty)
            {
                TCPTest.OnClientClose(uid);
            }
        }

        private void OnRevecie(RawMessage rawMessage)
        {
            string remote = (string)rawMessage.data;
            string content = System.Text.Encoding.Default.GetString(rawMessage.buffer);

            Util.ClientLog("OnRevecie from:{0} content:{1}", remote, content);
        }

        public string uid { get; set; }

        public string local
        {
            get
            {
                if (null == client)
                {
                    return string.Empty;
                }
                return client.local;
            }
        }

        public TCPClient client;
    }
}
