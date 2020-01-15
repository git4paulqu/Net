using ProtoBuf;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Net.UDP
{
    [ProtoContract]
    public class UDPPacket
    {
        private static string dataRegexStr = @"""Data"":\[[0-9,]*\]";
        private static Regex dataRegex = new Regex(dataRegexStr, RegexOptions.None);

        [ProtoIgnore]
        public EndPoint Src { get; set; }
        [ProtoIgnore]
        public EndPoint Dst { get; set; }
        [ProtoIgnore]
        public DateTime Sent { get; set; }
        [ProtoIgnore]
        public DateTime Received { get; set; }
        [ProtoIgnore]
        public bool Retransmit { get; set; }

        [ProtoMember(1)]
        public int Seq { get; set; }
        [ProtoMember(2)]
        public int Id { get; set; }
        [ProtoMember(3)]
        public int Qty { get; set; }
        [ProtoMember(4)]
        public UDPPacketType Type { get; set; }
        [ProtoMember(5)]
        public UDPPacketFlags Flags { get; set; }
        [ProtoMember(6)]
        public byte[] Data { get; set; }
        [ProtoMember(7)]
        public int[] ACK { get; set; }

        public static UDPPacket Deserialize(byte[] header, byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data.Skip(header.Length).ToArray()))
            {
                return Serializer.Deserialize<UDPPacket>(ms);
            }
        }

        public byte[] ToByteArray(byte[] header)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize<UDPPacket>(ms, this);

                byte[] serialized = ms.ToArray();
                byte[] result = new byte[header.Length + serialized.Length];
                Buffer.BlockCopy(header, 0, result, 0, header.Length);
                Buffer.BlockCopy(serialized, 0, result, header.Length, serialized.Length);
                return result;
            }
        }

        //public override string ToString()
        //{
        //    string js = _js.Serialize(this);
        //    if (Data != null && Data.Length > 30)
        //        return dataRegex.Replace(js, "\"Data\":" + (Data == null ? 0 : Data.Length) + "b");
        //    else
        //        return js;
        //}
    }
}
