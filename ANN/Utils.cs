using System;
using System.Collections.Generic;
using System.IO;

namespace ANN
{
    public static class Utils
    {
        private static readonly Random Rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create);
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
            stream.Close();
        }

        public static T ReadFromBinaryFile<T>(string filePath)
        {
            try
            {
                using Stream stream = File.Open(filePath, FileMode.Open);
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
            catch (IOException)
            {
                return default;
            }
        }
    }
}