using Net.TCP;

namespace Net
{
    public class TestDefine
    {
        public static string host = "127.0.0.1";
        public static int tcp_port = 50010;


        public static TCPSetting GetTCPSetting()
        {
            return new TCPSetting(host, tcp_port);
        }
    }
}
