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
        private WriteableBufferingSource outSource;

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
            var device = WaveInDevice.DefaultDevice;
            var waveFormat = device.SupportedFormats.OrderBy(d => Math.Abs(d.SampleRate - 44100)).First(); // Select wave format that is closest to the 44100 Hz target sampling range

            waveIn = new WaveIn(waveFormat);
            waveIn.Device = device;
            waveIn.Latency = 25;
            waveIn.Initialize();
            waveIn.Start();

            var inSource = new SoundInSource(waveIn);
            inSource.ChangeSampleRate(44100)
                .ToSampleSource()
                .ToWaveSource(16);
            inSource.DataAvailable += WaveIn_DataAvailable;

            outSource = new WriteableBufferingSource(waveIn.WaveFormat);

            soundOut = new WasapiOut();
            soundOut.Latency = 25;
            soundOut.Initialize(outSource);
            soundOut.Play();

            Receiver();
        }

        private async void Receiver()
        {
            while (running)
            {
                var packet = await EchoClient.Instance.VoiceClient.PacketStream.ReadPacket();
                if (packet is U02VoiceData voice && EchoClient.Instance.VoiceClient.ValidatePacket(voice))
                {
                    if (Deafened)
                        continue;

                    outSource.Write(voice.Data, 0, voice.Data.Length);
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
                avg += Abs(ms.ReadInt16());

            avg /= pcm.Length / 2;

            return avg;
        }

        private float Abs(short s)
        {
            if (s == short.MinValue)
                s++;
            return Math.Abs(s);
        }

    }
}
