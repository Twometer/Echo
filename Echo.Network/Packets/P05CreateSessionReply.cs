using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets
{
    public class P05CreateSessionReply : IPacket
    {
        public int Id => 5;

        public bool Authenticated { get; set; }

    }
}
