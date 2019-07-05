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
            /*var files = new List<string>();
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
            }*/

            var ann = new RNA(null, AnnMode.Training, ActivationFunctions.Tahn);
            var net = Utils.ReadFromBinaryFile<List<List<Neuron>>>("net");

            //var net = ann.NetInit(10, 9);
            //ann.Training(ref net, 10, 0.001, 1000);

            var sound2 = SoundReader.ReadFromFile(
                @"C:\Users\Ale\Licenta-master\Licenta-master\PopulateDatabase\music\genres\disco\disco.00001.wav");

            var result2 = Fft.CalculateFft(sound2, keepAll: true);
        

            //var realOutputs = ann.TestData.Select(x => x.Item2).ToList();
            ann.TestData = result2.Result.Select(x => (x.HighScores, "disco")).ToList();
            ann.TestData.Shuffle();

            var computedOutputs = ann.Evaluate(ref net, 10);

            var genres = ann.GetGeneres();
            foreach (var result in computedOutputs)
                genres[ann.GetValueFromLabelVector(result)] += 1;

            var s = genres.ToDictionary(x => x.Key, x => x.Value * 100.0 / computedOutputs.Count);
            //Console.Write(ann.ComputePerformance(computedOutputs, realOutputs));


            Utils.WriteToBinaryFile("net", net);

            Console.ReadKey();
        }
    }
}
