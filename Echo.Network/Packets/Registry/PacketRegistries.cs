using Echo.Network.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets.Registry
{
    public class PacketRegistries
    {
        public static IPacketRegistry<IPacket> Tcp { get; set; } = new TcpPacketRegistry();

        public static IPacketRegistry<UdpPacket> Udp { get; set; } = new UdpPacketRegistry();
    }
}
