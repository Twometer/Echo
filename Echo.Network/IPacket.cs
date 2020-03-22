using Newtonsoft.Json;
using System;

namespace Echo.Network
{
    public interface IPacket
    {

        [JsonIgnore]
        int Id { get; }

    }
}
