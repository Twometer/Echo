using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets.Tcp
{
    public class P04CreateSession : IPacket
    {
        public int Id => 4;

        public string EchoTag { get; set; }

        public byte[] KeyHash { get; set; }
    }
}
