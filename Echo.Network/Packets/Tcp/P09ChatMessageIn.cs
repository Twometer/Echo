using Echo.Network.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets.Tcp
{
    public class P09ChatMessageIn : IPacket
    {
        public int Id => 9;

        public Message Message { get; set; }
    }
}
