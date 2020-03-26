using Echo.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Echo.Network
{
    public class PacketRegistry
    {
        private static IPacket[] registeredPackets = {
            new P00Handshake(),
            new P01HandshakeReply(),
            new P02CreateAccount(),
            new P03CreateAccountReply(),
            new P04CreateSession(),
            new P05CreateSessionReply(),
            new P06Sync(),
            new P07ChatMessageOut(),
            new P08ChatMessageOutReply(),
            new P09ChatMessageIn(),
            new P10JoinChannel(),
            new P11JoinChannelReply()
        };

        public static Type FindPacketType(int id)
        {
            return registeredPackets.Where(p => p.Id == id).SingleOrDefault()?.GetType();
        }

    }
}
