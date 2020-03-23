using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network
{
    public class PacketHandler
    {
        private readonly IDictionary<Type, Action<IPacket>> handlerDict = new Dictionary<Type, Action<IPacket>>();

        public void Handle<T>(Action<T> handler) where T : IPacket
        {
            handlerDict.Add(typeof(T), p => { handler((T) p); });
        }

        public void Process(IPacket packet)
        {
            handlerDict[packet.GetType()]?.Invoke(packet);
        }
    }
}
