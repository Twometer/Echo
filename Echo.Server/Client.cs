using Echo.Network;
using Echo.Network.Packets;
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

        private const int Version = 1;

        private bool authenticated;

        private Random random = new Random();

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.packetStream = new PacketStream(tcpClient.GetStream());
            this.packetHandler = new PacketHandler();
            RegisterHandlers();
        }

        public async void BeginReading()
        {
            try
            {
                while (true)
                {
                    var packet = await packetStream.ReadPacket();
                    Console.WriteLine("Received packet " + packet.GetType().Name);
                    if (packet != null)
                        packetHandler.Process(packet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                tcpClient.Close();
                Program.RemoveClient(this);
            }
        }

        private void RegisterHandlers()
        {
            packetHandler.Handle<P00Handshake>(p =>
            {
                var status = p.Version == Version ? P01HandshakeReply.StatusCode.Ok : P01HandshakeReply.StatusCode.Outdated;
                _ = packetStream.WritePacket(new P01HandshakeReply() { Status = status });
            });
            packetHandler.Handle<P02CreateAccount>(p =>
            {
                if (this.authenticated)
                    throw new Exception("Protocol error [02]: Already logged in");
                if (string.IsNullOrWhiteSpace(p.Nickname) || p.Nickname.Length <= 2)
                {
                    _ = packetStream.WritePacket(new P03CreateAccountReply() { Status = P03CreateAccountReply.StatusCode.InvalidName });
                    return;
                }

                var tag = NewTag(p.Nickname);
                Storage.Accounts[tag] = new Account() { Tag = tag, PasswordHash = p.PasswordHash };
                _ = packetStream.WritePacket(new P03CreateAccountReply() { Status = P03CreateAccountReply.StatusCode.Ok, EchoTag = tag });
            });
            packetHandler.Handle<P04CreateSession>(p =>
            {
                if (this.authenticated)
                    throw new Exception("Protocol error [04]: Already logged in");

                var authenticated = Storage.Accounts.ContainsKey(p.EchoTag) && SequenceEqual(Storage.Accounts[p.EchoTag].PasswordHash, p.KeyHash);
                this.authenticated = authenticated;
                _ = packetStream.WritePacket(new P05CreateSessionReply() { Authenticated = authenticated });
            });
            packetHandler.Handle<P09ChatMessageIn>(p =>
            {

            });
            packetHandler.Handle<P10JoinChannel>(p =>
            {

            });
        }

        private string NewTag(string name)
        {
            string tag;
            do
            {
                tag = $"{name}#{RandomId()}";
            } while (Storage.Accounts.ContainsKey(tag));
            return tag;
        }

        private string RandomId()
        {
            var id = random.Next(1, 10000);
            return id.ToString("D4");
        }

        private bool SequenceEqual(byte[] a, byte[] b)
        {
            for (var i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }
    }
}
