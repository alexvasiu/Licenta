using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Music_Extract_Feature
{
    class Program
    {
        public static long GetDSSDuration(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            long size = fi.Length;
            long length = (long)(((size * 1.1869) - ((size / 1054) * 210)) / 1054);
            if (length > 1000)
            {
                length = (long)(length * (0.61 + (length / 100 * 0.0005)));
            }
            else
            {
                length = (long)(length * (0.61 + ((length / 100) * 0.0015)));
            }
            return length;
        }

        static double ZeroCrossingRate(List<sbyte> bytes, long duration)
        {
            var count = 0;
            for (var i = 1; i < bytes.Count; i++)
                count += bytes[i - 1] < 0 && bytes[i] > 0 ||
                         bytes[i - 1] > 0 && bytes[i] < 0 ||
                         bytes[i - 1] != 0 && bytes[i] == 0 ? 1 : 0;
            
            return (count * 1.0) / (duration * 1.0);
        }

        static void Main(string[] args)
        {
             const string file = "music\\bells.wav";
             var watch = Stopwatch.StartNew();

             var sound =  (WavSound) SoundReader.ReadFromFile(file);
             var ft = new Ft(sound);
             watch.Stop();

             var elapsedMs = watch.ElapsedMilliseconds;
             Console.WriteLine($"Time: {elapsedMs} ms");
             Console.WriteLine($"ChunkId: {sound.ChunkId}");
             Console.WriteLine($"Format: {sound.Format}");
             Console.WriteLine($"SubChunk1Id: {sound.SubChunk1Id}");
             Console.WriteLine($"SubChunk2Id: {sound.SubChunk2Id}");

             Console.ReadKey();
        }
    }
}
