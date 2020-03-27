using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets.Tcp
{
    public class P00Handshake : IPacket
    {
        public int Id => 0;

        public int Version { get; set; }

    }
}
