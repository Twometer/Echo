using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets
{
    public class P01HandshakeReply : IPacket
    {
        public int Id => 1;

        public StatusCode Status { get; set; }

        public enum StatusCode
        {
            Ok,
            Outdated
        }
    }
}
