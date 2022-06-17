using NAudio.MediaFoundation;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonKit
{
    public class AudioSource
    {
        public float[] Samples { get; init; }
        public int SampleCount => Samples.Length;
        public TimeSpan Duration => TimeSpan.FromSeconds((double)SampleCount / AudioPlaybackEngine.Fs);

        public AudioSource(float[] samples)
        {
            Samples = samples;
        }

        public static AudioSource FromFileStream(Stream stream)
        {
            MediaFoundationApi.Startup();
            using var mfReader = new StreamMediaFoundationReader(stream);
            using var reader = new MediaFoundationResampler(mfReader, AudioPlaybackEngine.PlaybackWaveFormat);
            var sampleReader = reader.ToSampleProvider();

            var outArrayLength = 0;
            var sampleChunks = new List<float[]>();
            var buffer = new float[AudioPlaybackEngine.Fs * AudioPlaybackEngine.Channels];
            for (; ; )
            {
                var bytesRead = sampleReader.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    sampleChunks.Add(buffer[..bytesRead]);
                    outArrayLength += bytesRead;
                }
                else
                    break;
            }

            var outArray = new float[outArrayLength];
            var outWriteIndex = 0;
            foreach (var chunk in sampleChunks)
            {
                Array.Copy(chunk, 0, outArray, outWriteIndex, chunk.Length);
                outWriteIndex += chunk.Length;
            }

            return new AudioSource(outArray);
        }

        public static AudioSource FromFile(string filePath)
        {
            using var fileStream = File.OpenRead(filePath);
            return FromFileStream(fileStream);
        }
    }
}
