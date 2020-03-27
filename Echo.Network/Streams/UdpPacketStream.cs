using Echo.Network.Base;
using Echo.Network.Packets.Registry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Network.Streams
{
    public class UdpPacketStream : IPacketStream
    {
        private UdpClient client;

        public UdpPacketStream(UdpClient client)
        {
            this.client = client;
        }

        public async Task<IPacket> ReadPacket()
        {
            var message = await client.ReceiveAsync();
            var reader = new BinaryReader(new MemoryStream(message.Buffer));
            var pid = reader.ReadInt32();

            var type = PacketRegistries.Udp.FindPacketType(pid);
            if (type == null)
                throw new Exception("Unknown packet " + pid);

            if (!(Activator.CreateInstance(type) is UdpPacket packet))
                throw new Exception("Failed to deserialize UDP packet #" + pid);

            packet.Read(reader);
            packet.Sender = message.RemoteEndPoint;
            return packet;
        }

        public async Task WritePacket(IPacket packet)
        {
            if (packet is UdpPacket udpPacket)
            {
                MemoryStream stream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(packet.Id);
                udpPacket.Write(writer);
                var message = stream.ToArray();
                await client.SendAsync(message, message.Length);
            }
            else
            {
                throw new Exception("UdpPacketStream cannot serialize TCP packets");
            }
        }


    }
}
