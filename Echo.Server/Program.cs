using Echo.Network;
using Echo.Network.Base;
using Echo.Network.Packets.Udp;
using Echo.Network.Streams;
using Echo.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            RunUdp();
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

        private async static void RunUdp()
        {
            var udp = new UdpClient(new IPEndPoint(IPAddress.Any, NetConfig.UdpPort));
            var stream = new UdpPacketStream(udp);


            while (true)
            {
                try
                {
                    var packet = await stream.ReadPacket() as UdpPacket;
                    if (packet is U00Handshake handshake)
                    {
                        var clientId = Guid.Parse(handshake.Token);
                        if (!clients.ContainsKey(clientId))
                        {
                            Console.WriteLine("Invalid access token: " + clientId);
                        }
                        else
                        {
                            Console.WriteLine("Client " + handshake.Token + "'s UDP endpoint is " + packet.Endpoint);
                            clients[clientId].UdpEndpoint = packet.Endpoint;
                        }
                    }
                    else if (packet is U02VoiceData voice)
                    {
                        var senderClient = clients.Values.First(c => c.UdpEndpoint == packet.Endpoint);
                        var channel = senderClient.VoiceChannel;

                        if (channel == null)
                            throw new Exception("Protocol violation: no such channel for voice data");

                        var clientsInChannel = clients.Values.Where(c => c.VoiceChannel?.ChannelId == channel.ChannelId);
                        foreach(var c in clientsInChannel)
                        {
                            var forwarded = new U02VoiceData() { Data = voice.Data, Endpoint = c.UdpEndpoint };
                            await stream.WritePacket(forwarded);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Udp: Error: " + e.Message);
                }
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
