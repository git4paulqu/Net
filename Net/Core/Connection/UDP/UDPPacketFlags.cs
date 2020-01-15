using System;

namespace Net.UDP
{
    [Flags]
    public enum UDPPacketFlags
    {
        NUL = 0,
        ACK,
        RST
    }
}