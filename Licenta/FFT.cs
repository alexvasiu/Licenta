using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Music_Extract_Feature
{
    public class Ft
    {
        private readonly Complex[] _x;                // storage for sample data
        public Complex[] Result;                // storage for FFT answer
        
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

        private void Fill(IReadOnlyList<long> list)
        {
            for (var i = 0; i < list.Count; i++)
                Result[i] = _x[i] = Signal(list[i]);
        }

        public Ft(Sound sound)
        {
            var list = sound.Data;
            _x = list.Select(x => new Complex(Signal(x), 0)).ToArray();
            Result = _x;
            Fill(list);
            Fft2(ref Result, 0, list.Count);
        }
    }
}   