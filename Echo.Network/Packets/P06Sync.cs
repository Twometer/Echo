using Echo.Network.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets
{
    public class P06Sync : IPacket
    {
        public int Id => 6;

        public ServerInfo ServerInfo { get; set; }
    }
}
