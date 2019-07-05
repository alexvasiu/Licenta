using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Music_Extract_Feature
{
    public class DataPoint
    {
        public List<int> Points { get; set; }
        public string Hash { get; set; }
        public double Time { get; set; }
        public double Duration { get; set; }
        public List<double> HighScores { get; set; }
    }

    public class Fft
    {
        //public Complex[][] Result;                // storage for FFT answer
        public List<DataPoint> Result;

        // private static readonly int[] Range = { 40, 80, 120, 180, 300 }; UNDO
        private static readonly int[] Range = { 10, 20, 40, 80, 160 };

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

        private static void ConvertStereoToMono(ref ISound sound)
        {
            sound.NumChannels = 1;
            var bytesPerSample = (ushort)(sound.BitDepth / 8);
            if (bytesPerSample == 2)
            {
                sound.Data = sound.Data.Group(2).Select(x => (x[0] + x[1]) / 2.0).ToList();
                //.Group(2).Select(x => (double) x.GetShort()).ToList();
            }
            else
                throw new NotImplementedException();
        }

        private static void LowPassFilter(ref ISound sound)
        {
            var newResult = new double[sound.Data.Count];
            newResult[0] = sound.Data[0];
            for (var n = 1; n < sound.Data.Count; n++)
                newResult[n] = sound.Data[n] + sound.Data[n - 1];
            sound.Data = newResult.ToList();
        }

        private static void DownSampling(ref ISound sound)
        {
            sound.Data = sound.Data.Group(4).Select(x => x.Average()).ToList();
            sound.SampleRate /= 4;
        }

        private static void HammingWindowFunction(ref ISound sound)
        {
            var length = sound.Data.Count;
            for (var i = 0; i < length; i += 1024)
            {
                var m = i + 1024 <= length ? 1024 : length - i;
                for (var n = 0; n < m; n++)
                    sound.Data[n + i] = sound.Data[n + i] * (0.54 - 0.46 * 2 * Math.PI * n / (m - 1));
            }
              
        }

        private static string Hash(List<int> points)
        {
            if (points == null) throw new ArgumentNullException(nameof(points));
            using var md5 = System.Security.Cryptography.MD5.Create();
            return string.Concat(md5.ComputeHash(Encoding.ASCII.GetBytes(string.Join("-", points)))
                .Select(x => x.ToString("x2")));
        }

        private static void Filter(ref List<int> points, ref List<double> highScores, double avg)
        {
            var list = highScores;
            points = points.Where((x, index) => list[index] > avg).ToList();
            highScores = highScores.Where((x, index) => x > avg).ToList();
        }

        public static Fft CalculateFft(ISound sound, bool isFromServer = true, bool keepAll = false)
        {
            var res = new Fft {Result = new List<DataPoint>()};
            if (isFromServer)
            {
                if (sound.NumChannels == 2)
                    ConvertStereoToMono(ref sound);
                LowPassFilter(ref sound);
                DownSampling(ref sound);
            }

            HammingWindowFunction(ref sound);

            var list = sound.Data;
            var chunkSize = 1024;
            var sampledChunkSize = list.Count / chunkSize;
            var result = new Complex[sampledChunkSize][];

            for (var i = 0; i < sampledChunkSize; i++)
            {
                result[i] = new Complex[chunkSize];
                for (var j = 0; j < chunkSize; j++)
                    result[i][j] = new Complex(list[i * chunkSize + j], 0);
                result[i] = CalculateFft(result[i]);
            }

            var timeForChunk = sound.Duration / result.Length;

            var mean = 0.0;

            var pointsGlobal = Enumerable.Repeat(0, Range.Length + 1).ToList();
            var highScoresGlobal = Enumerable.Repeat(0.0, Range.Length + 1).ToList();

            for (var i = 0; i < result.Length; i++)
            {
                var points = Enumerable.Repeat(0, Range.Length + 1).ToList();
                var highScores = Enumerable.Repeat(0.0, Range.Length + 1).ToList();

                for (var freq = 0; freq < 512; freq++)
                {
                    var mag = Math.Log(result[i][freq].Magnitude + 1);
                    var index = GetIndex(freq);

                    if (mag >= highScoresGlobal[index])
                    {
                        highScoresGlobal[index] = mag;
                        pointsGlobal[index] = freq;
                    }

                    if (!(mag >= highScores[index])) continue;
                    points[index] = freq;
                    highScores[index] = mag;
                }

                // var filteredPoints = Filter(points, highScores);

                mean += highScores.Sum();

                res.Result.Add(new DataPoint
                {
                    Hash = Hash(points),
                    Points = points,
                    Time = timeForChunk * i,
                    Duration = timeForChunk,
                    HighScores = !keepAll ? highScores : points.Select(x => (double) x).ToList()
                });
            }

            if (!keepAll)
            {
                var avg = highScoresGlobal.Average();

                for (var i = 0; i < res.Result.Count; i++)
                {
                    var dataPoint = res.Result[i];
                    var aPoints = dataPoint.Points;
                    var aHighScores = dataPoint.HighScores;
                    Filter(ref aPoints, ref aHighScores, avg);
                    dataPoint.HighScores = dataPoint.Points.Select(x => (double) x).ToList();
                    dataPoint.Points = aPoints;
                    dataPoint.Hash = Hash(aPoints);
                    res.Result[i] = dataPoint;
                }
            }

            return res;
        }

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