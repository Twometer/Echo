using CSCore;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using Echo.Client.Network;
using Echo.Network.Packets.Udp;
using System;
using System.Collections.Generic;
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

        public void BeginStreaming()
        {
            waveIn = new WaveIn(new WaveFormat(44100, 16, 1)); // 
            waveIn.Device = WaveInDevice.DefaultDevice;
            waveIn.Latency = 25;
            waveIn.Initialize();
            waveIn.DataAvailable += WaveIn_DataAvailable;
            
            waveIn.Start();

            source = new WriteableBufferingSource(waveIn.WaveFormat); // new WaveFormat(44100, 16, 1)

            soundOut = new WasapiOut();
            soundOut.Latency = 25;
            soundOut.Initialize(source);
            soundOut.Play();

            Receiver();
        }

        private async void Receiver()
        {
            while (true)
            {
                var packet = await EchoClient.Instance.VoiceClient.PacketStream.ReadPacket();
                if (packet is U02VoiceData voice)
                {
                    source.Write(voice.Data, 0, voice.Data.Length);
                    if (soundOut.PlaybackState != PlaybackState.Playing)
                        soundOut.Play();
                }
            }
        }

        private void WaveIn_DataAvailable(object sender, DataAvailableEventArgs e)
        {
            var data = new byte[e.ByteCount];
            Buffer.BlockCopy(e.Data, 0, data, 0, e.ByteCount);
            _ = EchoClient.Instance.VoiceClient.SendVoiceData(data);
        }
    }
}
