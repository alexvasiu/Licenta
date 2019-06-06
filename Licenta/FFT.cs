using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Music_Extract_Feature
{
    public class Ft
    {
        private readonly Complex[] _x;                // storage for sample data
        public Complex[][] Result;                // storage for FFT answer
        
        private static void Separate(ref Complex[] a, int m, int n)
        {
            var b = new Complex[(n - m) / 2];
            
            for (var i = 0; i < (n - m) / 2; i++)    // copy all odd elements to b
                b[i] = a[m + i * 2 + 1];
            for (var i = 0; i < (n - m) / 2; i++)    // copy all even elements to lower-half of a
                a[m + i] = a[m + i * 2];
            for (var i = 0; i < (n - m) / 2; i++)    // copy all odd (from b) to upper-half of a[]
                a[m + i + (n - m) / 2] = b[i];
        }

        private static void Fft2(ref Complex[] x, int m, int n)
        {
            if (n - m < 2) return;
            Separate(ref x, m, n);     
            Fft2(ref x, m, m + (n - m) / 2);
            Fft2(ref x, m + (n - m) / 2, n);
            for (var k = 0; k < (n - m) / 2; k++)
            {
                var e = x[m + k];
                var o = x[m + k + (n - m) / 2];                                                     
                var w = Complex.Exp(new Complex(0, -2 * Math.PI * k / (n - m)));
                x[m + k] = e + w * o;
                x[m + k + (n - m) / 2] = e - w * o;
            }
        }

        private static double Signal(long t)
        {
            return new double[] { 2, 5, 11, 17, 29 }.Sum(x => 2 * Math.PI * x * t); // know freq sum
        }

        /*private void Fill(IReadOnlyList<long> list)
        {
            for (var i = 0; i < list.Count; i++)
                Result[i] = _x[i] = Signal(list[i]);
        }*/

        private readonly int[] _range = { 40, 80, 120, 180, 300 };

        private int GetIndex(int freq)
        {
            var i = 0;
            while (i < _range.Length && _range[i] < freq)
                i++;
            return i;
        }

        private const int FuzFactor = 2;

        private static long Hash(long p1, long p2, long p3, long p4)
        {
            return (p4 - p4 % FuzFactor) * 100000000 + (p3 - p3 % FuzFactor)
                   * 100000 + (p2 - p2 % FuzFactor) * 100
                   + (p1 - p1 % FuzFactor);
        }

        public Ft(Sound sound)
        {
            
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
                result[i] = OtherFft(result[i]);
            }

            var highscores = new double[result.Length, _range.Length];
            var points = new int[result.Length, _range.Length];
            var dict = new Dictionary<long, int>();
            for (var i = 0; i < result.Length; i++)
            {
                for (var freq = 40; freq < 300; freq++)
                {
                    // Get the magnitude:
                    var mag = Math.Log(result[i][freq].Magnitude + 1);

                    // Find out which range we are in:
                    var index = GetIndex(freq);

                    // Save the highest magnitude and corresponding frequency:
                    if (mag > highscores[i, index])
                    {
                        points[i, index] = freq;
                        highscores[i, index] = mag;
                    }
                }

                // form hash tag
                var h = Hash(points[i, 0], points[i, 1], points[i, 2], points[i, 3]);
                if (h == 0) continue;   
                if (dict.ContainsKey(h))
                    dict[h]++;
                else
                    dict[h] = 1;
            }

            foreach (var (key, value) in dict)
            {
                Console.WriteLine($"{key} - {value}");
            }

            Result = result;
        }

        // TODO: ASSURE THAT MP3 IS PARSED CORRECTLY

        // TODO: UI: Login

        private static Complex[] OtherFft(IReadOnlyList<Complex> data)
        {
            var n = data.Count;
            /*if (n < 2)
                return data.ToArray();*/

            var even = new Complex[n / 2];
            var odd = new Complex[n / 2];
            for (var i = 0; i < n / 2; i++)
            {
                even[i] = data[2 * i];
                odd[i] = data[2 * i + 1];
            }

            var evenResult = n / 2 > 1 ? OtherFft(even) : even;
            var oddResult = n / 2 > 1 ? OtherFft(odd) : odd;

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