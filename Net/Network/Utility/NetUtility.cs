using System.Net;

namespace Network
{
    internal class NetUtility
    {
        internal static IPEndPoint GetIPEndPoint(string host, int port)
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

        internal static string FormatIPEndPoint(string host, int port)
        {
            return string.Format("{0}:{1}", host, port);
        }
    }
}