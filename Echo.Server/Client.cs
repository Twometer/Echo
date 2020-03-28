﻿using Echo.Network;
using Echo.Network.Base;
using Echo.Network.Model;
using Echo.Network.Packets;
using Echo.Network.Packets.Tcp;
using Echo.Network.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Echo.Server
{
    internal class Client
    {
        public Guid Id { get; } = Guid.NewGuid();

        public IPEndPoint UdpEndpoint { get; set; }

        public Account Account { get; private set; }

        public Channel VoiceChannel { get; private set; }

        private TcpClient tcpClient;
        private IPacketStream packetStream;
        private PacketHandler packetHandler;

        private const int Version = 1;

        private bool authenticated;

        private Random random = new Random();

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.packetStream = new TcpPacketStream(tcpClient.GetStream());
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
                if (string.IsNullOrWhiteSpace(p.Nickname) || p.Nickname.Length <= 2 || p.Nickname.Contains("#"))
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

                bool CheckAuthenticated()
                {
                    if (!Storage.Accounts.ContainsKey(p.EchoTag))
                        return false;
                    var account = Storage.Accounts[p.EchoTag];
                    if (SequenceEqual(account.PasswordHash, p.KeyHash))
                    {
                        Account = account;
                        return true;
                    }
                    return false;
                }

                authenticated = CheckAuthenticated();

                _ = packetStream.WritePacket(new P05CreateSessionReply() { Authenticated = authenticated });

                if (!authenticated)
                    return;

                var users = Storage.Accounts.Values.Select(acc => new User() { Id = acc.Id, EchoTag = acc.Tag, State = User.OnlineState.Online });
                var info = new ServerInfo() { ServerName = Storage.ServerName, Channels = Storage.Channels.Values, Users = users };
                _ = packetStream.WritePacket(new P06Sync() { ServerInfo = info });
            });
            packetHandler.Handle<P07ChatMessageOut>(p =>
            {
                if (!Storage.Channels.ContainsKey(p.ChannelId))
                    throw new Exception("Protocol error [07]: Channel does not exist");

                var message = new Message() { SendDate = DateTime.Now, ChannelId = p.ChannelId, Content = p.Content, MessageId = Guid.NewGuid(), SenderId = Account.Id };
                _ = packetStream.WritePacket(new P08ChatMessageOutReply() { Nonce = p.Nonce, MessageId = message.MessageId });
                Program.Broadcast(this, new P09ChatMessageIn() { Message = message });
            });
            packetHandler.Handle<P10JoinChannel>(p =>
            {
                if (!Storage.Channels.ContainsKey(p.ChannelId))
                    throw new Exception("Protocol error [10]: Channel does not exist");
                var channel = Storage.Channels[p.ChannelId];
                if (channel.Type != Channel.ChannelType.Voice)
                {
                    _ = packetStream.WritePacket(new P11JoinChannelReply() { Status = P11JoinChannelReply.StatusCode.InvalidChannel });
                    return;
                }
                var reply = new P11JoinChannelReply() { Status = P11JoinChannelReply.StatusCode.Ok, VoiceToken = Id.ToString(), VoiceUrl = $"udp://localhost:{NetConfig.UdpPort}/{channel.Name}" };
                _ = packetStream.WritePacket(reply);

                VoiceChannel = channel;
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

        public void SendPacket(IPacket packet)
        {
            _ = packetStream.WritePacket(packet);
        }
    }
}
