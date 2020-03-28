using Echo.Client.Network;
using Echo.Client.Util;
using Echo.Network.Packets.Udp;
using OpenTK;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Echo.Client.Voice
{
    public class VoiceManager
    {
        private IntPtr mic;
        private IntPtr micBuf;

        private ALPlayback playback;

        private volatile bool working = true;

        private Thread transmitThread;
        private Thread receiveThread;

        public async void BeginStreaming()
        {
            try
            {
                mic = Alc.CaptureOpenDevice(null, 44100, ALFormat.Mono16, 1024);
                Alc.CaptureStart(mic);

                playback = new ALPlayback();
                micBuf = Marshal.AllocHGlobal(22050);


                (transmitThread = new Thread(Transmitter)).Start();
                (receiveThread = new Thread(Receiver)).Start();
            }
            catch
            {
                await MsgBox.Show("Missing audio library", "Please download the audio library OpenAL from https://openal.org/downloads/");
            }
        }

        public void EndStreaming()
        {
            working = false;
            transmitThread?.Join();
            receiveThread?.Join();
        }

        private void Transmitter()
        {
            while (working)
            {
                Alc.GetInteger(mic, AlcGetInteger.CaptureSamples, sizeof(int), out int samples);
                Alc.CaptureSamples(mic, micBuf, samples);

                var data = new byte[1024];

                Marshal.Copy(micBuf, data, 0, data.Length);

                
                playback.Enqueue(data);
                //EchoClient.Instance.VoiceClient.SendVoiceData(data).Wait();
            }
            Alc.CaptureStop(mic);
            Alc.CaptureCloseDevice(mic);

            Marshal.FreeHGlobal(micBuf);
        }

        private void Receiver()
        {
            /*var source = AL.GenSource();
            var sampleSet = AL.GenBuffer();
            AL.Source(source, ALSourcei.Buffer, sampleSet);*/

            while (working)
            {
                var packet = EchoClient.Instance.VoiceClient.PacketStream.ReadPacket().Result;
                if (packet is U02VoiceData voiceData)
                {
                    
                    //var data = voiceData.Data;
                    /*Marshal.Copy(data, 0, speakerBuf, data.Length);
                    AL.BufferData(sampleSet, ALFormat.Mono16, speakerBuf, data.Length, 44100);
                    if (AL.GetSourceState(source) != ALSourceState.Playing)
                        AL.SourcePlay(source);*/

                }
            }

            playback.Destroy();
        }

        /*private async void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            var data = new byte[e.BytesRecorded];
            Buffer.BlockCopy(e.Buffer, 0, data, 0, e.BytesRecorded);
            await EchoClient.Instance.VoiceClient.SendVoiceData(data);
        }*/
    }
}
