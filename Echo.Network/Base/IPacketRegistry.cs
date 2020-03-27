using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Base
{
    public interface IPacketRegistry<T> where T : class, IPacket
    {

        Type FindPacketType(int pid);

    }
}
