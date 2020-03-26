using Echo.Network;
using Echo.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Client.Network
{
    public class EchoClient
    {
        public static EchoClient Instance { get; } = new EchoClient();

        public const int Version = 1;

        private TcpClient tcpClient;

        private PacketStream packetStream;

        private IList<(Type type, TaskCompletionSource<IPacket> tcs)> waitingResponses = new List<(Type type, TaskCompletionSource<IPacket> tcs)>();

        public async Task Connect(string endpoint)
        {
            if (tcpClient?.Connected == true)
                return;

            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(endpoint, NetConfig.Port);
            packetStream = new PacketStream(tcpClient.GetStream());
            BeginReading();
            await SendPacket(new P00Handshake() { Version = Version });
        }

        public async Task<T> SendRequest<T>(IPacket packet) where T : IPacket
        {
            await SendPacket(packet);
            return await AwaitPacket<T>();
        }

        public async Task SendPacket(IPacket packet)
        {
            await packetStream.WritePacket(packet);
        }

        public async void BeginReading()
        {
            while (tcpClient.Connected)
            {
                var packet = await packetStream.ReadPacket();
                foreach (var response in waitingResponses.Reverse())
                {
                    if (packet.GetType() == response.type)
                    {
                        response.tcs.SetResult(packet);
                        waitingResponses.Remove(response);
                    }
                }
            }
        }

        private async Task<T> AwaitPacket<T>() where T : IPacket
        {
            var tcs = new TaskCompletionSource<IPacket>();
            waitingResponses.Add((typeof(T), tcs));
            return (T)(await tcs.Task);
        }

    }
}
