using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

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
                    newArray[i - start] = default;
            }
            return newArray;
        }

        public static byte SubByte(this byte x, int start = 0, int length = 8)
        {
            return (byte) x.ToBitsArray().Skip(start).Take(length).Reverse().Select((a, index) => a * Math.Pow(2, index)).Sum();
        }

        public static List<int> ToBitsArray(this byte x)
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

        public static List<int> ToBitsArray(this byte[] x)
        {
            var res = new List<int>();
            return x.Aggregate(res, (current, v) => current.Concat(v.ToBitsArray()).ToList());
        }

        public static List<int> ToBitsArray<T>(this List<byte> x)
        {
            return ToBitsArray(x.ToArray());
        }
        
        public static double Log2(double n) => Math.Log(n) / Math.Log(2);

        public static bool IsEqualTo<T>(this List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            return !list1.Where((t, i) => !t.Equals(list2[i])).Any();
        }

        public static uint GetUInt(this byte[] bytes, bool bigEndian = false)
        {
            return bigEndian
                ? (uint) ((bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3])
                : (uint) ((bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0]);
        }

        public static ushort GetUShort(this byte[] bytes, bool bigEndian = false)
        {
            return bigEndian
                ? (ushort)((bytes[0] << 8) | bytes[1])
                : (ushort)((bytes[1] << 8) | bytes[0]);
        }

        public static short GetShort(this byte[] bytes, bool bigEndian = false)
        {
            if (bytes.Length == 1)
                return bigEndian ? (short) (bytes[0] << 8) : bytes[0];
            return bigEndian
                ? (short)((bytes[0] << 8) | bytes[1])
                : (short)((bytes[1] << 8) | bytes[0]);
        }

        public static int GetInt(this byte[] bytes, bool bigEndian = false)
        {
            return bigEndian
                ? (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]
                : (bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0];
        }

        public static long GetLong(this byte[] bytes, bool bigEndian = false)
        {
            return bigEndian
                ? (bytes[0] << 56) | (bytes[1] << 48) | (bytes[2] << 40) | (bytes[3] << 32) | (bytes[4] << 24) | (bytes[5] << 16) | (bytes[6] << 8) | bytes[7]
                : (bytes[7] << 56) | (bytes[6] << 48) | (bytes[5] << 40) | (bytes[4] << 32) | (bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0];
        }

        public static float GetFloat(this byte[] bytes, bool bigEndian = false)
        {
            return bigEndian
                ? (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]
                : (bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0];
        }

        public static bool EndsWith(this byte[] first, byte[] second)
        {
            int i = first.Length - 1, j = second.Length - 1;

            while (j >= 0)
            {
                if (i < 0)
                    return false;
                if (second[j--] != first[i--])
                    return false;
            }
            return true;
        }

        public static List<T[]> Group<T>(this T[] bytes, int groupSize)
        {
            if ((groupSize & (groupSize - 1)) != 0)
                throw new Exception("Invalid group size");
            /*if (bytes.Length % groupSize != 0)
                throw new Exception("Invalid length");*/
            var result = new List<T[]>();
            for (var i = 0; i < bytes.Length; i += groupSize)
            {
                var cur = new List<T>();
                for (var j = 0; i + j < bytes.Length && j < groupSize; j++)
                    cur.Add(bytes[i + j]);
                result.Add(cur.ToArray());
            }
            return result;
        }

        public static List<T[]> Group<T>(this List<T> bytes, int groupSize)
        {
            if ((groupSize & (groupSize - 1)) != 0)
                throw new Exception("Invalid group size");
            /*if (bytes.Count % groupSize != 0)
                throw new Exception("Invalid length");*/
            var result = new List<T[]>();
            for (var i = 0; i < bytes.Count; i += groupSize)
            {
                var cur = new List<T>();
                for (var j = 0; i + j < bytes.Count && j < groupSize; j++)
                    cur.Add(bytes[i + j]);
                result.Add(cur.ToArray());
            }
            return result;
        }

        public static List<double> FromBuffer(this byte[] bytes, string type)
        {
            if (type == "u1")
                return bytes.Select(x => (double) x).ToList();
            var bigEndian = type[0] == '>';
            var size = Convert.ToInt32(type.Substring(2));
            var tip = type[1].ToString();
            switch (tip, size)
            {
                case ("i", 1):
                    return bytes.Select(x => (double) x).ToList();
                case ("i", 2):
                    return bytes.Group(2).Select(x => (double) x.GetShort(bigEndian)).ToList();
                case ("i", 4):
                    return bytes.Group(4).Select(x => (double) x.GetInt(bigEndian)).ToList();
                case ("i", 8):
                    return bytes.Group(8).Select(x => (double) x.GetLong(bigEndian)).ToList();
                case ("f", 2):
                case ("f", 4):
                case ("f", 8):
                default:
                    throw new Exception("Invalid type");
            }
        }

        public static byte[] GetBytes(this IFormFile file)
        {
            return new BinaryReader(file.OpenReadStream()).ReadBytes((int)file.OpenReadStream().Length);
        }
    }
}