using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Network.Base
{
    public interface IPacketStream
    {

        Task<IPacket> ReadPacket();

        Task WritePacket(IPacket packet);

    }
}
