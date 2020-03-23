using Echo.Network;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Echo.Server
{
    internal class Client
    {
        public Guid Id { get; } = Guid.NewGuid();

        private TcpClient tcpClient;
        private PacketStream packetStream;
        private PacketHandler packetHandler;

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.packetStream = new PacketStream(tcpClient.GetStream());
            this.packetHandler = new PacketHandler();
            RegisterHandlers();
        }

        public async void BeginReading()
        {
            while (true)
            {
                var packet = await packetStream.ReadPacket();
                Console.WriteLine("Received packet " + packet.GetType().Name);
                if (packet != null)
                    packetHandler.Process(packet);
            }
        }

        private void RegisterHandlers()
        {

        }
    }
}
