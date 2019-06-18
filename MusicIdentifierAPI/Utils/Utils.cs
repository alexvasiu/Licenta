using System.IO;

namespace MusicIdentifierAPI.Utils
{
    public class Utils
    {
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
                return (T) binaryFormatter.Deserialize(stream);
            }
            catch (IOException)
            {
                return default;
            }
        }
    }
}