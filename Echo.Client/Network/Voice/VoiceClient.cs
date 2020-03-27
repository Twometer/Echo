using System;
using System.Collections.Generic;
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
            return true;
        }

    }
}
