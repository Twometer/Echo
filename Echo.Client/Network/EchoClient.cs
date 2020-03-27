using Echo.Client.Network.Voice;
using Echo.Network;
using Echo.Network.Model;
using Echo.Network.Packets;
using Echo.Network.Util;
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

        public ServerInfo ServerInfo { get; private set; }

        public UserInfo UserInfo { get; private set; }

        public VoiceClient VoiceClient { get; set; }

        public const int Version = 1;

        public event EventHandler ServerInfoChanged;

        private TcpClient tcpClient;

        private PacketStream packetStream;

        private IList<(Type type, TaskCompletionSource<IPacket> tcs)> waitingResponses = new List<(Type type, TaskCompletionSource<IPacket> tcs)>();

        public async Task Connect(string endpoint)
        {
            if (tcpClient?.Connected == true)
                return;

            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(endpoint, NetConfig.TcpPort);
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
                if (packet == null)
                    break;

                foreach (var response in waitingResponses.Reverse())
                {
                    if (packet.GetType() == response.type)
                    {
                        response.tcs.SetResult(packet);
                        waitingResponses.Remove(response);
                    }
                }

                HandlePacket(packet);
            }
            Console.WriteLine("Lost connection");
        }

        public async Task<bool> Login(string tag, string password)
        {
            var key = Hash.Sha256(password);
            P05CreateSessionReply reply = await SendRequest<P05CreateSessionReply>(new P04CreateSession() { EchoTag = tag, KeyHash = key });
            if (reply.Authenticated)
                UserInfo = new UserInfo() { Tag = tag, Key = key };
            return reply.Authenticated;
        }

        private async Task<T> AwaitPacket<T>() where T : IPacket
        {
            var tcs = new TaskCompletionSource<IPacket>();
            waitingResponses.Add((typeof(T), tcs));
            return (T)(await tcs.Task);
        }

        private void HandlePacket(IPacket packet)
        {
            if (packet is P06Sync sync)
            {
                ServerInfo = sync.ServerInfo;
                ServerInfoChanged?.Invoke(this, new EventArgs());
            }
        }

    }
}
