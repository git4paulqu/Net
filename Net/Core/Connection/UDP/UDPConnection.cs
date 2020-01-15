using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Net.UDP
{
    public class RUDPConnection : NetSocket
    {
        public bool IsServer { get; set; }
        public bool IsClient { get { return !IsServer; } }
        public UDPConnectionState State { get; set; }
        public int RecvFrequencyMs { get; set; }
        public int PacketIdLimit { get; set; }
        public int SequenceLimit { get; set; }
        public int ClientStartSequence { get; set; }
        public int ServerStartSequence { get; set; }
        public int MTU { get; set; }

        public delegate void dlgEventVoid();
        public delegate void dlgEventConnection(EndPoint ep);
        public delegate void dlgEventUserData(UDPPacket p);
        public event dlgEventConnection OnClientConnect;
        public event dlgEventConnection OnClientDisconnect;
        public event dlgEventConnection OnConnected;
        public event dlgEventUserData OnPacketReceived;

        private Dictionary<string, IPEndPoint> _clients { get; set; }
        private Dictionary<string, UDPConnectionData> _sequences { get; set; }

        private bool _isAlive = false;
        private int _maxMTU { get { return (int)(MTU * 0.80); } }
        private object _debugMutex = new object();
        private byte[] _packetHeader = { 0xDE, 0xAD, 0xBE, 0xEF };
        private byte[] _internalHeader = { 0xFA, 0xCE, 0xFE, 0xED };

        private IPEndPoint remoterPoint = new IPEndPoint(IPAddress.Any, 0);
        public IPEndPoint localEndPoint;

        private Thread _thRecv;

        public RUDPConnection()
        {
            IsServer = false;
            MTU = 500;
            RecvFrequencyMs = 10;
            PacketIdLimit = int.MaxValue / 2;
            SequenceLimit = int.MaxValue / 2;
            ClientStartSequence = 100;
            ServerStartSequence = 200;
            State = UDPConnectionState.CLOSED;
        }

        private void Debug(object obj, params object[] args)
        {
            lock (_debugMutex)
            {
                Console.ForegroundColor = IsServer ? ConsoleColor.Cyan : ConsoleColor.Green;
                NetDebug.Log(obj.ToString(), args);
                Console.ResetColor();
            }
        }

        public void Connect(string address, int port)
        {
            Initialize();
            IsServer = false;
            State = UDPConnectionState.OPENING;

            remoterPoint = NetUtility.GetIPEndPoint(address, port);
            if (null == remoterPoint)
            {
                NetDebug.Log("[TCPClient] remote can not be null.");
                return;
            }

            socket = new Socket(remoterPoint.AddressFamily,
                                SocketType.Dgram,
                                ProtocolType.Udp);
            socket.Connect(remoterPoint);

            ReceiveFromAsync(remoterPoint);
            //socket.Blocking = setting.blocking;
            //socket.NoDelay = setting.noDelay;
          
            Send(remoterPoint, UDPPacketType.SYN);
        }

        public void Listen(string address, int port)
        {
            Initialize();
            IsServer = true;
            localEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            socket.Bind(localEndPoint);
            ReceiveFromAsync(new IPEndPoint(IPAddress.Any, port));
            State = UDPConnectionState.LISTEN;
        }

        public virtual void Initialize(bool startThreads = true)
        {
            _isAlive = true;
            _sequences = new Dictionary<string, UDPConnectionData>();
            _clients = new Dictionary<string, IPEndPoint>();
            Initialize(NetDefine.DEFAUT_IONUM);
            InitThreads(startThreads);
        }

        public void InitThreads(bool start)
        {
            _thRecv = new Thread(() =>
            {
                while (_isAlive)
                {
                    ProcessSendQueue();
                    ProcessRecvQueue();
                    Thread.Sleep(10);
                }
            });
            if (start)
                _thRecv.Start();
        }

        public void Disconnect()
        {
            State = UDPConnectionState.CLOSING;
            _isAlive = false;
            socket.Shutdown(SocketShutdown.Both);
            if (IsServer)
                socket.Close();
            if (_thRecv != null)
                while (_thRecv.IsAlive)
                    Thread.Sleep(10);
            State = UDPConnectionState.CLOSED;
        }

        protected override bool DecodeReceive(byte[] buffer, int offset, int count, EndPoint ep, out int error)
        {
            error = 0;
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            if (count > _packetHeader.Length && data.Take(_packetHeader.Length).SequenceEqual(_packetHeader))
            {
                UDPPacket p = UDPPacket.Deserialize(_packetHeader, data);
                p.Src = IsServer ? ep : remoterPoint;
                p.Received = DateTime.Now;
                InitSequence(p.Src);
                UDPConnectionData sq = _sequences[p.Src.ToString()];
                SendToAsync(p.Src, new UDPInternalPacket.ACKPacket() { header = _internalHeader, sequence = p.Seq }.Serialize());
                Debug("ACK SEND -> {0}: {1}", p.Src, p.Seq);
                lock (sq.ReceivedPackets)
                    sq.ReceivedPackets.Add(p);
            }
            else if (count > _internalHeader.Length && data.Take(_internalHeader.Length).SequenceEqual(_internalHeader))
            {
                EndPoint src = IsServer ? ep : remoterPoint;
                InitSequence(src);
                UDPConnectionData sq = _sequences[src.ToString()];
                UDPInternalPacket.ACKPacket ack = UDPInternalPacket.ACKPacket.Deserialize(data);
                Debug("ACK RECV <- {0}: {1}", src, ack.sequence);
                lock (sq.Pending)
                    sq.Pending.RemoveAll(x => x.Seq == ack.sequence);
            }
            else
                Console.WriteLine("[{0}] RAW RECV: [{1}]", GetType().ToString(), Encoding.ASCII.GetString(data, 0, count));

            return true;
        }

        public void Send(string data)
        {
            Send(remoterPoint, UDPPacketType.DAT, UDPPacketFlags.NUL, Encoding.ASCII.GetBytes(data));
        }

        public void Send(EndPoint destination, UDPPacketType type = UDPPacketType.DAT, UDPPacketFlags flags = UDPPacketFlags.NUL, byte[] data = null, int[] intData = null)
        {
            InitSequence(destination);
            UDPConnectionData sq = _sequences[destination.ToString()];
            if ((data != null && data.Length < _maxMTU) || data == null)
            {
                SendPacket(new UDPPacket()
                {
                    Dst = destination,
                    Id = sq.PacketId,
                    Type = type,
                    Flags = flags,
                    Data = data
                });
                sq.PacketId++;
            }
            else if (data != null && data.Length >= _maxMTU)
            {
                int i = 0;
                List<UDPPacket> PacketsToSend = new List<UDPPacket>();
                while (i < data.Length)
                {
                    int min = i;
                    int max = _maxMTU;
                    if ((min + max) > data.Length)
                        max = data.Length - min;
                    byte[] buf = data.Skip(i).Take(max).ToArray();
                    PacketsToSend.Add(new UDPPacket()
                    {
                        Dst = destination,
                        Id = sq.PacketId,
                        Type = type,
                        Flags = flags,
                        Data = buf
                    });
                    i += _maxMTU;
                }
                foreach (UDPPacket p in PacketsToSend)
                {
                    p.Qty = PacketsToSend.Count;
                    SendPacket(p);
                }
                sq.PacketId++;
            }
            else
                throw new Exception("This should not happen");
            if (sq.PacketId > PacketIdLimit)
                sq.PacketId = 0;
        }

        // ###############################################################################################################################
        // ###############################################################################################################################
        // ###############################################################################################################################

        private bool InitSequence(UDPPacket p)
        {
            return InitSequence(p.Src == null ? p.Dst : p.Src);
        }

        private bool InitSequence(EndPoint ep)
        {
            bool rv = false;
            lock (_sequences)
            {
                if (!_sequences.ContainsKey(ep.ToString()))
                {
                    _sequences[ep.ToString()] = new UDPConnectionData()
                    {
                        EndPoint = ep,
                        Local = IsServer ? ServerStartSequence : ClientStartSequence,
                        Remote = IsServer ? ClientStartSequence : ServerStartSequence
                    };
                    while (!_sequences.ContainsKey(ep.ToString()))
                        Thread.Sleep(10);
                    Debug("NEW SEQUENCE: {0}", _sequences[ep.ToString()]);
                    rv = true;
                }
            }
            return rv;
        }

        // ###############################################################################################################################
        // ###############################################################################################################################
        // ###############################################################################################################################

        private void RetransmitPacket(UDPPacket p)
        {
            p.Retransmit = true;
            SendPacket(p);
        }

        private void SendPacket(UDPPacket p)
        {
            DateTime dtNow = DateTime.Now;

            InitSequence(p.Dst);
            UDPConnectionData sq = _sequences[p.Dst.ToString()];

            if (!p.Retransmit)
            {
                p.Seq = sq.Local;
                sq.Local++;
                p.Sent = dtNow;
                lock (sq.Pending)
                    foreach (UDPPacket unconfirmed in sq.Pending.Where(x => (dtNow - p.Sent).Seconds >= 1))
                        RetransmitPacket(unconfirmed);
                Debug("SEND -> {0}: {1}", p.Dst, p);
            }
            else
                Debug("RETRANSMIT -> {0}: {1}", p.Dst, p);

            lock (sq.Pending)
            {
                sq.Pending.RemoveAll(x => x.Dst.ToString() == p.Dst.ToString() && x.Seq == p.Seq);
                sq.Pending.Add(p);
            }

            SendToAsync(p.Dst, p.ToByteArray(_packetHeader));
        }

        public void ProcessRecvQueue()
        {
            foreach (var cdata in _sequences)
            {
                UDPConnectionData sq = cdata.Value;

                List<UDPPacket> PacketsToRecv = new List<UDPPacket>();
                lock (sq.ReceivedPackets)
                    PacketsToRecv.AddRange(sq.ReceivedPackets.OrderBy(x => x.Seq));
                PacketsToRecv = PacketsToRecv.GroupBy(x => x.Seq).Select(g => g.First()).ToList();

                for (int i = 0; i < PacketsToRecv.Count; i++)
                {
                    UDPPacket p = PacketsToRecv[i];

                    lock (sq.ReceivedPackets)
                        sq.ReceivedPackets.Remove(p);

                    if (p.Seq < sq.Remote)
                        continue;

                    if (p.Seq > sq.Remote)
                    {
                        sq.ReceivedPackets.Add(p);
                        break;
                    }

                    Debug("RECV <- {0}: {1}", p.Src, p);

                    if (p.Qty == 0)
                    {
                        sq.Remote++;

                        if (p.Type == UDPPacketType.SYN)
                        {
                            if (IsServer)
                            {
                                Send(p.Src, UDPPacketType.SYN, UDPPacketFlags.ACK);
                                OnClientConnect?.Invoke(p.Src);
                            }
                            else if (p.Flags == UDPPacketFlags.ACK)
                            {
                                State = UDPConnectionState.OPEN;
                                OnConnected?.Invoke(p.Src);
                            }
                            continue;
                        }

                        OnPacketReceived?.Invoke(p);
                    }
                    else
                    {
                        // Multipacket!
                        List<UDPPacket> multiPackets = PacketsToRecv.Skip(i).Take(p.Qty).ToList();
                        if (multiPackets.Count == p.Qty)
                        {
                            Debug("MULTIPACKET {0}", p.Id);

                            byte[] buf;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (BinaryWriter bw = new BinaryWriter(ms))
                                    foreach (UDPPacket mp in multiPackets)
                                    {
                                        bw.Write(mp.Data);
                                        Debug("RECV MP <- {0}: {1}", p.Src, mp);
                                    }
                                buf = ms.ToArray();
                            }
                            Debug("MULTIPACKET ID {0} DATA: {1}", p.Id, Encoding.ASCII.GetString(buf));

                            OnPacketReceived?.Invoke(new UDPPacket()
                            {
                                ACK = p.ACK,
                                Retransmit = p.Retransmit,
                                Sent = p.Sent,
                                Data = buf,
                                Dst = p.Dst,
                                Flags = p.Flags,
                                Id = p.Id,
                                Qty = p.Qty,
                                Received = p.Received,
                                Seq = p.Seq,
                                Src = p.Src,
                                Type = p.Type
                            });

                            sq.Remote += p.Qty;
                            i += p.Qty;
                        }
                        else
                        {
                            if (multiPackets.Count < p.Qty)
                            {
                                sq.ReceivedPackets.Add(p);
                                break;
                            }
                            else
                            {
                                Debug("P.QTY > MULTIPACKETS.COUNT ({0} > {1})", p.Qty, multiPackets.Count);
                                throw new Exception();
                            }
                        }
                    }
                }
            }
        }

        public void ProcessSendQueue()
        {
            foreach (var connectionPair in _sequences)
            {
                UDPConnectionData connectionData = connectionPair.Value;
                foreach (var item in connectionData.Pending)
                {
                    if (null != item.Data)
                    {
                        //NetDebug.Log(Encoding.ASCII.GetString(item.Data));
                    }
                }
            }
        }

        public override string ToString()
        {
            string type = IsServer ? "SERVER" : "CLIENT";
            return GetType() + "-" + type;
        }
    }
}
