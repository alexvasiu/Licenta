using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Music_Extract_Feature
{
    internal class Program
    {
        private static void Main()
        {
             // const string file = "music\\Afrojack - Unstoppable (Official Video).mp3";
             const string file = "music\\bells.wav";
             var watch = Stopwatch.StartNew();

             var sound = SoundReader.ReadFromFile(file);
             var ft = Fft.CalculateFft(sound);
             watch.Stop();

             var elapsedMs = watch.ElapsedMilliseconds;
             Console.WriteLine($"Time: {elapsedMs} ms");
             Console.ReadKey(); 
        }
    }
}
