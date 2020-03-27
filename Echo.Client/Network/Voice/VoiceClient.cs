using Echo.Network;
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

        public VoiceClient(string url, string token)
        {
            this.url = url;
            this.token = token;
        }

        public async Task<bool> Connect()
        {
            var udpClient = new UdpClient();
            udpClient.Connect(Constants.Host, NetConfig.UdpPort);

            var memStream = new MemoryStream();
            var wr = new BinaryWriter(memStream);
            wr.Write(0);
            wr.Write(4);

            var payload = Encoding.UTF8.GetBytes("ThisIsATestMessageFromTheUdpStreamLetsSeeHowWellThisWorks");

            wr.Write(payload.Length);
            wr.Write(payload);

            var data = memStream.ToArray();
            await udpClient.SendAsync(data, data.Length);

            return true;
        }

    }
}
