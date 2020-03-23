using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets
{
    public class P10JoinChannel : IPacket
    {
        public int Id => 10;

        public Guid ChannelId { get; set; }
    }
}
