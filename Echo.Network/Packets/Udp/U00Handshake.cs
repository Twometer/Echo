using Echo.Network.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Echo.Network.Packets.Udp
{
    public class U00Handshake : UdpPacket
    {
        public override int Id => 0;

        public string Token { get; set; }

        public override void Read(BinaryReader reader)
        {
            Token = reader.ReadString();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Token);
        }

    }
}
