using System;
using System.Collections.Generic;
using System.Linq;
using Music_Extract_Feature;

namespace ANN
{
    internal class Program
    {
        private static List<string> Genres = new List<string> { "blues", "classical", "country", "disco", "hiphop", "jazz", "metal", "pop", "reggae", "rock" };
        private static void Main(string[] args)
        {
            var files = new List<string>();
            var data = new List<(List<double>, string)>();
            var percent = 0.0;
            Console.WriteLine($"Percent: {percent * 100}%");
            foreach (var genre in Genres)
            {
                for (var i = 0; i < 5; i++)
                {
                    var file = $@"C:\Users\Ale\Licenta-master\Licenta-master\PopulateDatabase\music\genres\{genre}\{genre}.0000{i}.wav";
                    var sound = SoundReader.ReadFromFile(file);
                    var fftResult = Fft.CalculateFft(sound, keepAll: true);
                    foreach (var res in fftResult.Result)
                        data.Add((res.Points.Select(x => (double)x).ToList(), genre));
                    percent += 1.0 / 50;
                    Console.Clear();
                    Console.WriteLine($"Percent: {percent * 100}%");
                }
            }

            var ann = new RNA(data, AnnMode.Testing, ActivationFunctions.Tahn);
            var net = ann.NetInit(10, 9);
            ann.Training(ref net, 10, 0.001, 100);
            var realOutputs = ann.TestData.Select(x => x.Item2).ToList();
            var computedOutputs = ann.Evaluate(ref net, 10);

            Console.Write(ann.ComputePerformance(computedOutputs, realOutputs));

            Console.ReadKey();
        }
    }
}
