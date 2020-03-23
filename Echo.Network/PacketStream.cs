using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Network
{
    public class PacketStream : IDisposable
    {
        private readonly Stream baseStream;

        public PacketStream(Stream baseStream)
        {
            this.baseStream = baseStream;
        }

        public async Task WritePacket(IPacket packet)
        {
            var jsonData = JsonConvert.SerializeObject(packet);
            var binaryData = Encoding.UTF8.GetBytes(jsonData);
            await WriteInt(binaryData.Length);
            await WriteInt(packet.Id);
            await WriteBytes(binaryData);
        }

        public async Task<IPacket> ReadPacket()
        {
            var dataLen = await ReadInt();
            var packetId = await ReadInt();
            var packetType = PacketRegistry.FindPacketType(packetId);

            var json = Encoding.UTF8.GetString(await ReadBytes(dataLen));

            return (IPacket) JsonConvert.DeserializeObject(json, packetType);
        }

        private Task WriteBytes(byte[] data)
        {
            return baseStream.WriteAsync(data, 0, data.Length);
        }

        private async Task<byte[]> ReadBytes(int len)
        {
            var data = new byte[len];
            await baseStream.ReadAsync(data, 0, len);
            return data;
        }

        private Task WriteInt(int i)
        {
            var buf = BitConverter.GetBytes(i);
            return baseStream.WriteAsync(buf, 0, buf.Length);
        }

        private async Task<int> ReadInt()
        {
            var buf = new byte[4];
            await baseStream.ReadAsync(buf, 0, buf.Length);
            return BitConverter.ToInt32(buf, 0);
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
