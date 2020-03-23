using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets
{
    public class P08ChatMessageOutReply : IPacket
    {
        public int Id => 8;

        public int Nonce { get; set; }

        public Guid MessageId { get; set; }

    }
}
