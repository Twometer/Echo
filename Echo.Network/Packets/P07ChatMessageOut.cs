using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets
{
    public class P07ChatMessageOut : IPacket
    {
        public int Id => 7;

        public Guid ChannelId { get; set; }

        public int Nonce { get; set; }

        public string Content { get; set; }

    }
}
