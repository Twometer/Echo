using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets
{
    public class P02CreateAccount : IPacket
    {
        public int Id => 2;

        public string AccountName { get; set; }

        public byte[] PasswordHash { get; set; }
    }
}
