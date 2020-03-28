using OpenTK;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Echo.Client.Voice
{
    public class ALPlayback
    {
        private IntPtr speaker;
        private IntPtr speakerBuf;
        private ContextHandle speakerCtx;

        private int source;

        private int packetCount;
        private bool playbackStarted;

        private int currentPacketSize;

        private ConcurrentQueue<byte[]> dataQueue = new ConcurrentQueue<byte[]>();

        public ALPlayback()
        {
            speaker = Alc.OpenDevice(null);
            speakerCtx = Alc.CreateContext(speaker, (int[])null);
            Alc.MakeContextCurrent(speakerCtx);

            source = AL.GenSource();

            speakerBuf = Marshal.AllocHGlobal(22050);
        }

        public void Enqueue(byte[] data)
        {
            dataQueue.Enqueue(data);

            packetCount++;

            if (packetCount > 5)
                new Thread(Player).Start();
        }

        private void Player()
        {
            for (int i = 0; i < 3; i++)
            {
                FillBuffer();
                var buf = AL.GenBuffer();
                AL.BufferData(buf, ALFormat.Mono16, speakerBuf, currentPacketSize, 44100);
                AL.SourceQueueBuffer(source, buf);
            }

            AL.SourcePlay(source);

            var elapsed = 0;
            while (true) {
                AL.GetSource(source, ALGetSourcei.BuffersProcessed, out int p);
                while (p != 0)
                {
                    elapsed++;
                    if (elapsed > 15) break;

                    var buf = AL.SourceUnqueueBuffer(source);
                    FillBuffer();
                    AL.BufferData(buf, ALFormat.Mono16, speakerBuf, currentPacketSize, 44100);
                    AL.SourceQueueBuffer(source, buf);
                    AL.GetSource(source, ALGetSourcei.BuffersProcessed, out p);
                }
            }

            /*Marshal.Copy(data, 0, speakerBuf, data.Length);
           if (packetCount < 32)
           {
               var buf = AL.GenBuffer();
               AL.BufferData(buf, ALFormat.Mono16, speakerBuf, data.Length, 44100);
               AL.SourceQueueBuffer(source, buf);
           }
           else
           {
               if (AL.GetSourceState(source) != ALSourceState.Playing) // AL.GetSourceState(source) != ALSourceState.Playing
               {
                   playbackStarted = true;
                   AL.SourcePlay(source);
               }

               AL.GetSource(source, ALGetSourcei.BuffersProcessed, out int processed);
               if (processed != 0) // Drop late frames for now
               {
                   var buf = AL.SourceUnqueueBuffer(source);
                   AL.BufferData(buf, ALFormat.Mono16, speakerBuf, data.Length, 44100);
                   AL.SourceQueueBuffer(source, buf);
               }
           }

           packetCount++;*/
        }

        private void FillBuffer()
        {
            byte[] result;
            while (!dataQueue.TryDequeue(out result))
            {
                Thread.Sleep(5);
            }
            Marshal.Copy(result, 0, speakerBuf, result.Length);
            currentPacketSize = result.Length;
        }

        public void Destroy()
        {
            AL.DeleteSource(source);
            Marshal.FreeHGlobal(speakerBuf);
        }

    }
}
