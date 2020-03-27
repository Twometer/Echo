using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Echo.Network.Base
{
    public abstract class UdpPacket : IPacket
    {
        public IPEndPoint Sender { get; set; }

        public abstract int Id { get; }

        public abstract void Read(BinaryReader reader);

        public abstract void Write(BinaryWriter writer);

    }
}
