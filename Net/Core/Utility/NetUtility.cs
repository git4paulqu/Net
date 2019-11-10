using System.Net;

namespace Net
{
    public class NetUtility
    {
        public static IPEndPoint GetIPEndPoint(string host, int port)
        {
            IPEndPoint remotePoint = null;
            IPAddress ipAddress;
            if (IPAddress.TryParse(host, out ipAddress))
            {
                remotePoint = new IPEndPoint(ipAddress, port);
            }
            else
                foreach (IPAddress ip in Dns.GetHostEntry(host).AddressList)
                {
                    remotePoint = new IPEndPoint(ip, port);
                    break;
                }
            return remotePoint;
        }
    }
}
