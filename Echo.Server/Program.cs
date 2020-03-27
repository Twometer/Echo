using Echo.Network;
using Echo.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Echo
{
    public class Program
    {
        private static IDictionary<Guid, Client> clients = new ConcurrentDictionary<Guid, Client>();

        public static void Main(string[] args)
        {
            AsyncMain().Wait();
        }

        private static async Task AsyncMain()
        {
            var listener = new TcpListener(IPAddress.Any, NetConfig.TcpPort);
            listener.Start();
            new Thread(RunUdp).Start();
            Console.WriteLine("Echo server online");

            var ch = Guid.NewGuid();
            Storage.Channels[ch] = new Network.Model.Channel() { ChannelId = ch, Description = "Default voice channel", Name = "voice", Type = Network.Model.Channel.ChannelType.Voice };

            ch = Guid.NewGuid();
            Storage.Channels[ch] = new Network.Model.Channel() { ChannelId = ch, Description = "Default text channel", Name = "default", Type = Network.Model.Channel.ChannelType.Text };

            Storage.ServerName = "Twometer Echo Test Server";

            while (true)
            {
                var tcp = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Incoming connection from " + tcp.Client.RemoteEndPoint);

                var client = new Client(tcp);
                clients[client.Id] = client;

                client.BeginReading();
            }
        }

        private static void RunUdp()
        {
            var udp = new Socket(SocketType.Dgram, ProtocolType.Udp);
            udp.Bind(new IPEndPoint(IPAddress.Any, NetConfig.UdpPort));

            var frame = new byte[8192];
            while (true)
            {
                var remote = new IPEndPoint(IPAddress.Any, 0) as EndPoint;
                var received = udp.ReceiveFrom(frame, ref remote);

                var memStream = new MemoryStream(frame);
                var wr = new BinaryReader(memStream);
                var packetNum = wr.ReadInt32();
                var packetId = wr.ReadInt32();
                var packetLen = wr.ReadInt32();

                if (received > packetLen + 12)
                {
                    Console.WriteLine($"Warn: UDP: length mismatch, received={received}, packetLength={packetLen}");
                }

                var packetContent = wr.ReadBytes(packetLen);
                Console.WriteLine("packet #" + packetNum + " ~" + packetId + " datalen=" + packetLen + ", " + Encoding.UTF8.GetString(packetContent));
            }
        }

        internal static void Broadcast(Client from, IPacket packet)
        {
            foreach (var kvp in clients)
                if (kvp.Key != from.Id)
                    kvp.Value.SendPacket(packet);
        }

        internal static void RemoveClient(Client client)
        {
            clients.Remove(client.Id);
        }
    }
}
