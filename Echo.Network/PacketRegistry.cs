using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Echo.Network
{
    public class PacketRegistry
    {
        private static IPacket[] registeredPackets = { };

        public static Type FindPacketType(int id)
        {
            return registeredPackets.Where(p => p.Id == id).Single().GetType();
        }

    }
}
