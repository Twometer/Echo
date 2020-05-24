using Echo.Network.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Echo.Network.Packets.Udp
{
    public class U02VoiceData : UdpPacket
    {
        public override int Id => 2;

        public uint PacketNumber { get; set; }

        public byte[] Data { get; set; }

        public U02VoiceData()
        {
        }

        public U02VoiceData(uint packetNumber)
        {
            PacketNumber = packetNumber;
        }

        public override void Read(BinaryReader reader)
        {
            PacketNumber = reader.ReadUInt32();
            var len = reader.ReadInt32();
            Data = reader.ReadBytes(len);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(PacketNumber);
            writer.Write(Data.Length);
            writer.Write(Data);
        }
    }
}
