using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets
{
    public class P11JoinChannelReply : IPacket
    {
        public int Id => 11;

        public StatusCode Status { get; set; }

        public string VoiceUrl { get; set; }


        public enum StatusCode
        {
            Ok,
            Unauthorized,
            InvalidChannel
        }
    }
}
