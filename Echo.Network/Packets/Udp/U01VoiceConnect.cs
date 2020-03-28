using Echo.Network.Base;
using Echo.Network.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Echo.Network.Packets.Udp
{
    public class U01VoiceConnect : UdpPacket
    {
        public override int Id => 1;

        public Guid ChannelId { get; set; }

        public override void Read(BinaryReader reader)
        {
            ChannelId = Guid.Parse(reader.ReadString());
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(ChannelId.ToString());
        }
    }
}
