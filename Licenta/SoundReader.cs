using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Music_Extract_Feature
{
    public class SoundReader
    {
        private static readonly List<string> AcceptedFormats = new List<string> { ".wav", ".mp3" };
        private SoundReader()
        {

        }

        public static Sound ReadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($@"{path}: No such file or Directory");

          
            var fileExtension = Path.GetExtension(path);
            if (!AcceptedFormats.Contains(fileExtension)) // TODO: Check through other way using Linux Style :)
                throw new Exception($@"{path}: Format not accepted"); // TODO: Define new type of exception

            var bytes = File.ReadAllBytes(path);

            switch (fileExtension)
            {
                case ".wav":
                    var sound = new WavSound();

                    sound.ChunkId = Encoding.UTF8.GetString(bytes.Slice(0, 4));
                    sound.ChunkSize = BitConverter.ToInt32(bytes.Slice(4, 4), 0);
                    sound.Format = Encoding.UTF8.GetString(bytes.Slice(8, 4));
                    sound.SubChunk1Id = Encoding.UTF8.GetString(bytes.Slice(12, 4));
                    sound.SubChunk1Size = BitConverter.ToInt32(bytes.Slice(16, 4), 0);
                    sound.AudioFormat = BitConverter.ToInt32(bytes.Slice(20, 2, 4), 0);
                    sound.NumChannels = BitConverter.ToInt32(bytes.Slice(22, 2, 4), 0);
                    sound.SampleRate = BitConverter.ToInt32(bytes.Slice(24, 4), 0);
                    sound.ByteRate = BitConverter.ToInt32(bytes.Slice(28, 4), 0);
                    sound.BlockAlign = BitConverter.ToInt32(bytes.Slice(32, 2, 4), 0);
                    sound.BitsPerSample = BitConverter.ToInt32(bytes.Slice(34, 2, 4), 0);
                    sound.SubChunk2Id = Encoding.UTF8.GetString(bytes.Slice(36, 4));
                    sound.SubChunk2Size = BitConverter.ToInt32(bytes.Slice(40, 2, 4), 0);

                    bytes = bytes.Skip(44).ToArray();
                    sound.Data = new List<long>();
                    for (var i = 0; i < bytes.Length; i += sound.BitsPerSample / 4)
                        sound.Data.Add(BitConverter.ToInt64(bytes.Slice(i, sound.BitsPerSample / 4, 8), 0));
                    return sound;
                case ".mp3":
                    var sound2 = new Mp3Sound();
                    var headerBits = bytes.Slice(0, 4).ToByteArray();

                    var frameSync = headerBits.Take(11).ToList();
                    if (frameSync.Sum() != frameSync.Count)
                        throw new Exception("Invalid MP3 file");

                    var mpegVersion = headerBits.Skip(11).Take(2).ToList();
                    if (mpegVersion[0] == 0)
                        sound2.MpegVersion = mpegVersion[1] == 0 ? MpegVersion.Mpeg25 : MpegVersion.Reserved;
                    else
                        sound2.MpegVersion = mpegVersion[1] == 0 ? MpegVersion.Mpeg2 : MpegVersion.Mpeg1;

                    var layerType = headerBits.Skip(13).Take(2).ToList();
                    if (layerType[0] == 0)
                        sound2.LayerType = layerType[1] == 0 ? LayerType.Reserved : LayerType.Layer3;
                    else
                        sound2.LayerType = layerType[1] == 0 ? LayerType.Layer2 : LayerType.Layer1;

                    var protectionBit = headerBits.Skip(15).Take(1).ToList()[0];
                    sound2.Protection = protectionBit == 0;

                    var bitRateBits = headerBits.Skip(16).Take(4).ToList();


                    // TODO: Use bits instead of bytes


                    return sound2;
                default:
                    return null;
            }
        }
    }
}