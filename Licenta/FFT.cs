using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Music_Extract_Feature
{
    public class DataPoint
    {
        public List<int> Points { get; set; }
        public long Hash { get; set; }
        public double Time { get; set; }
        public double Duration { get; set; }
        public List<double> HighScores { get; set; }
    }

    public class Fft
    {
        //public Complex[][] Result;                // storage for FFT answer
        public List<DataPoint> Result;

        private static readonly int[] Range = { 40, 80, 120, 180, 300 };

        private static int GetIndex(int freq)
        {
            var i = 0;
            while (i < Range.Length && Range[i] < freq)
                i++;
            return i;
        }

        private const int FUZ_FACTOR = 2;
        private static long Hash(long p1, long p2, long p3, long p4)
        {
            return (p4 - p4 % FUZ_FACTOR) * 100000000 + (p3 - p3 % FUZ_FACTOR)
                   * 100000 + (p2 - p2 % FUZ_FACTOR) * 100
                   + (p1 - p1 % FUZ_FACTOR);
        }

        public static Fft CalculateFft(ISound sound)
        {
            var res = new Fft {Result = new List<DataPoint>()};

            var list = sound.Data;
            var totalSize = list.Count;
            const int chunkSize = 4000;
            var sampledChunkSize = totalSize / chunkSize;
            var result = new Complex[sampledChunkSize][];
            for (var i = 0; i < sampledChunkSize; i++)
            {
                result[i] = new Complex[chunkSize];
                for (var j = 0; j < chunkSize; j++)
                    result[i][j] = new Complex(list[i * chunkSize + j], 0);
                result[i] = CalculateFft(result[i]);
            }

            var timeForChunk = sound.Duration / result.Length;

            for (var i = 0; i < result.Length; i++)
            {
                var points = Enumerable.Repeat(0, Range.Length).ToList();
                var highScores = Enumerable.Repeat(0.0, Range.Length).ToList();

                for (var freq = 30; freq < 300; freq++)
                {
                    var mag = Math.Log(result[i][freq].Magnitude + 1);
                    var index = GetIndex(freq);
                    if (!(mag > highScores[index])) continue;
                    points[index] = freq;
                    highScores[index] = mag;
                }

                res.Result.Add(new DataPoint
                {
                    Hash = Hash(points[0], points[1], points[2], points[3]),
                    Points = points,
                    Time = timeForChunk * i,
                    Duration = timeForChunk,
                    HighScores = highScores
                });
            }

            return res;
        }

        // TODO: ASSURE THAT MP3 IS PARSED CORRECTLY

        // TODO: UI: Login

        private static Complex[] CalculateFft(IReadOnlyList<Complex> data)
        {
            var n = data.Count;

            var even = new Complex[n / 2];
            var odd = new Complex[n / 2];
            for (var i = 0; i < n / 2; i++)
            {
                even[i] = data[2 * i];
                odd[i] = data[2 * i + 1];
            }

            var evenResult = n / 2 > 1 ? CalculateFft(even) : even;
            var oddResult = n / 2 > 1 ? CalculateFft(odd) : odd;

            var finalResult = new Complex[n];
            for (var i = 0; i < n / 2; i++)
            {
                var kth = -2 * i * Math.PI / n;
                var wk = new Complex(Math.Cos(kth), Math.Sin(kth));
                finalResult[i] = evenResult[i] + wk * oddResult[i];
                finalResult[i + n / 2] = evenResult[i] - wk * oddResult[i];
            }
            return finalResult;
        }
    }
}   