using System;
using System.Collections.Generic;
using System.Linq;

namespace Music_Extract_Feature
{
    public static class Utils
    {
        public static T[] Slice<T>(this T[] array, int start, int length, int fill = -1)
        {
            var newArray = new T[fill == -1 ? length : fill];
            for (var i = start; i < array.Length && i < length + start && (fill == -1 || i < fill + start); i++)
                newArray[i - start] = array[i];
            if (fill == -1) return newArray;
            {
                for (var i = start + length; i < array.Length && i < start + fill; i++)
                    newArray[i - start] = default(T);
            }
            return newArray;
        }

        public static byte SubByte(this byte x, int start = 0, int length = 8)
        {
            return (byte) x.ToByteArray().Skip(start).Take(length).Reverse().Select((a, index) => a * Math.Pow(2, index)).Sum();
        }

        public static List<int> ToByteArray(this byte x)
        {
            var bits = new List<bool>();
            while (x != 0)
            {
                bits.Add(x % 2 == 1);
                x /= 2;
            }

            while (bits.Count < 8)
                bits.Add(false);

            bits.Reverse();

            return bits.Select(a => a ? 1: 0).ToList();
        }

        public static List<int> ToByteArray(this byte[] x)
        {
            var res = new List<int>();
            return x.Aggregate(res, (current, v) => current.Concat(v.ToByteArray()).ToList());
        }

        public static List<int> ToByteArray<T>(this List<byte> x)
        {
            return ToByteArray(x.ToArray());
        }
        
        public static double Log2(double n) => Math.Log(n) / Math.Log(2);
    }
}