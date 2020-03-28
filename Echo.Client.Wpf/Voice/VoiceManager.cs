using CSCore;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using Echo.Client.Network;
using Echo.Network.Packets.Udp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Client.Wpf.Voice
{
    internal class VoiceManager
    {
        private WaveIn waveIn;
        private WasapiOut soundOut;

        private WriteableBufferingSource source;

        private volatile bool running = true;

        public bool Muted { get; set; }

        public bool Deafened { get; set; }

        public void EndStreaming()
        {
            running = false;
            waveIn?.Stop();
            soundOut?.Stop();
        }

        public void BeginStreaming()
        {
            waveIn = new WaveIn(new WaveFormat(44100, 16, 1)); 
            waveIn.Device = WaveInDevice.DefaultDevice;
            waveIn.Latency = 25;
            waveIn.Initialize();
            waveIn.DataAvailable += WaveIn_DataAvailable;
            
            waveIn.Start();

            source = new WriteableBufferingSource(waveIn.WaveFormat);

            soundOut = new WasapiOut();
            soundOut.Latency = 25;
            soundOut.Initialize(source);
            soundOut.Play();

            Receiver();
        }

        private async void Receiver()
        {
            while (running)
            {
                var packet = await EchoClient.Instance.VoiceClient.PacketStream.ReadPacket();
                if (packet is U02VoiceData voice)
                {
                    if (Deafened)
                        continue;

                    source.Write(voice.Data, 0, voice.Data.Length);
                    if (soundOut.PlaybackState != PlaybackState.Playing)
                        soundOut.Play();
                }
            }
        }

        private int emptySamples = 0;

        private void WaveIn_DataAvailable(object sender, DataAvailableEventArgs e)
        {
            var data = new byte[e.ByteCount];
            Buffer.BlockCopy(e.Data, 0, data, 0, e.ByteCount);

            var loudness = GetLoudness(data);
            if (loudness < 80)
            {
                emptySamples++;
            }
            else
            {
                emptySamples = 0;
            }

            if (Muted || emptySamples > 8)
                return;

            _ = EchoClient.Instance.VoiceClient.SendVoiceData(data);
        }

        private float GetLoudness(byte[] pcm)
        {
            float avg = 0;

            var ms = new BinaryReader(new MemoryStream(pcm));
            for (int i = 0; i < pcm.Length / 2; i++)
                avg += Math.Abs(ms.ReadInt16());

            avg /= pcm.Length / 2;

            return avg;
        }
    }
}
