using Echo.Network.Base;
using Echo.Network.Packets.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Echo.Network.Packets.Registry
{
    public class UdpPacketRegistry : IPacketRegistry<UdpPacket>
    {
        private static UdpPacket[] registeredPackets = {
            new U00Handshake()
        };

        public Type FindPacketType(int id)
        {
            return registeredPackets.Where(p => p.Id == id).SingleOrDefault()?.GetType();
        }
    }
}
