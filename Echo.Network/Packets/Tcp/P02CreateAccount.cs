using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets.Tcp
{
    public class P02CreateAccount : IPacket
    {
        public int Id => 2;

        public string Nickname { get; set; }

        public byte[] PasswordHash { get; set; }
    }
}
