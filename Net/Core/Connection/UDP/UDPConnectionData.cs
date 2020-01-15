using System.Collections.Generic;
using System.Net;

namespace Net.UDP
{
    public class UDPConnectionData
    {
        public EndPoint EndPoint { get; set; }
        public int Local { get; set; }
        public int? Remote { get; set; }
        public int PacketId { get; set; }
        public List<UDPPacket> ReceivedPackets { get; set; }
        public List<UDPPacket> Pending { get; set; }

        public UDPConnectionData()
        {
            PacketId = 0;
            ReceivedPackets = new List<UDPPacket>();
            Pending = new List<UDPPacket>();
        }

        public override string ToString()
        {
            return string.Format("[{0}] Local: {1} | Remote: {2}", EndPoint, Local, Remote);
        }
    }
}
