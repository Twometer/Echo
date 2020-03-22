using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Echo.Network
{
    public class PacketStream : IDisposable
    {
        private Stream baseStream;
        private BinaryWriter binaryWriter;
        private BinaryReader binaryReader;

        public PacketStream(Stream baseStream)
        {
            this.baseStream = baseStream;
            this.binaryWriter = new BinaryWriter(baseStream);
            this.binaryReader = new BinaryReader(baseStream);
        }

        public void WritePacket(IPacket packet)
        {
            var jsonData = JsonConvert.SerializeObject(packet);
            var binaryData = Encoding.UTF8.GetBytes(jsonData);
            binaryWriter.Write(binaryData.Length);
            binaryWriter.Write(packet.Id);
            binaryWriter.Write(binaryData);
        }

        public IPacket ReadPacket()
        {
            var dataLen = binaryReader.ReadInt32();
            var packetId = binaryReader.ReadInt32();
            var packetType = PacketRegistry.FindPacketType(packetId);

            var data = new byte[dataLen];
            binaryReader.Read(data, 0, data.Length);
            var json = Encoding.UTF8.GetString(data);

            return (IPacket) JsonConvert.DeserializeObject(json, packetType);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    baseStream.Dispose();
                    binaryWriter.Dispose();
                    binaryReader.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PacketStream()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
