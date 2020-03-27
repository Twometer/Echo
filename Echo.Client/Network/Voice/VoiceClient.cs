using Echo.Network;
using Echo.Network.Base;
using Echo.Network.Packets.Udp;
using Echo.Network.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Client.Network.Voice
{
    public class VoiceClient
    {
        private string url;
        private string token;

        private UdpClient udpClient;
        private IPacketStream packetStream;

        public VoiceClient(string url, string token)
        {
            this.url = url;
            this.token = token;
        }

        public async Task<bool> Connect()
        {
            udpClient = new UdpClient();
            udpClient.Connect(Constants.Host, NetConfig.UdpPort);
            packetStream = new UdpPacketStream(udpClient);

            await packetStream.WritePacket(new U00Handshake() { Token = token });
            return true;
        }

    }
}
