using Echo.Network.Base;
using Echo.Network.Packets;
using Echo.Network.Packets.Tcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Echo.Network.Packets.Registry
{
    public class TcpPacketRegistry : IPacketRegistry<IPacket>
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

        public Type FindPacketType(int id)
        {
            return registeredPackets.Where(p => p.Id == id).SingleOrDefault()?.GetType();
        }

    }
}
