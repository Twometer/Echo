using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Packets.Tcp
{
    public class P03CreateAccountReply : IPacket
    {
        public int Id => 3;

        public string EchoTag { get; set; }

        public StatusCode Status { get; set; }

        public enum StatusCode
        {
            Ok,
            InvalidName,
            NameTaken
        }
    }
}
