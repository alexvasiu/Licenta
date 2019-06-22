using System;
using System.Diagnostics;

namespace Music_Extract_Feature
{
    internal class Program
    {
        private static void Main()
        {
             // const string file = "music\\Afrojack - Unstoppable (Official Video).mp3";
             const string file = @"C:\Users\Ale\Licenta-master\Licenta-master\PopulateDatabase\music\genres\blues\blues.00002.wav";
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
