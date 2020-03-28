using Echo.Network.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Echo.Network.Packets.Udp
{
    public class U02VoiceData : UdpPacket
    {
        public override int Id => throw new NotImplementedException();

        public byte[] Data { get; set; }

        public override void Read(BinaryReader reader)
        {
            var len = reader.ReadInt32();
            Data = reader.ReadBytes(len);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Data.Length);
            writer.Write(Data);
        }
    }
}
