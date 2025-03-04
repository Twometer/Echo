﻿using Echo.Network;
using Echo.Network.Base;
using Echo.Network.Model;
using Echo.Network.Packets.Udp;
using Echo.Network.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Client.Network
{
    public class VoiceClient
    {
        private string url;
        private string token;

        private UdpClient udpClient;
        public IPacketStream PacketStream { get; private set; }

        public VoiceClient(string url, string token)
        {
            this.url = url;
            this.token = token;
        }

        public async Task<bool> Connect()
        {
            udpClient = new UdpClient();
            udpClient.Connect(Constants.Host, NetConfig.UdpPort);
            PacketStream = new UdpPacketStream(udpClient);

            await PacketStream.WritePacket(new U00Handshake() { Token = token });
            return true;
        }

        public async Task SetChannel(Channel channel)
        {
            await PacketStream.WritePacket(new U01VoiceConnect() { ChannelId = channel.ChannelId });
        }

        public async Task SendVoiceData(byte[] data)
        {
            await PacketStream.WritePacket(new U02VoiceData() { Data = data });
        }

    }
}
