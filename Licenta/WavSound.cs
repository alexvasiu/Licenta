using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Music_Extract_Feature
{
    public class WavSound : ISound
    {
        public string ChunkId { get; set; }
        public uint ChunkSize { get; set; }
        public string Format { get; set; }
        public string SubChunk1Id { get; set; }
        public uint SubChunk1Size { get; set; }
        public ushort AudioFormat { get; set; }
        public ushort NumChannels { get; set; }
        public uint SampleRate { get; set; }
        public uint ByteRate { get; set; }
        public ushort BlockAlign { get; set; }
        public ushort BitDepth { get; set; }
        public ushort ExtraChunkSize { get; set; }
        public byte[] ExtraData { get; set; }
        public string SubChunk2Id { get; set; }
        public uint SubChunk2Size { get; set; }
        public List<double> Data { get; set; }
        public List<byte> RawBinary { get; set; }
        public double Duration { get; set; }

        public double GetChunkSize()
        {
            throw new System.NotImplementedException();
        }

        public SoundType Type { get; } = SoundType.Wav;

        public static WavSound GetWavSound(byte[] bytes)
        {
            var sound = new WavSound {RawBinary = bytes.ToList()};

            var currentIndex = 0;
            sound.ChunkId = Encoding.UTF8.GetString(bytes.Slice(currentIndex, 4));
            currentIndex += 4;
            bool bigEndian;
            switch (sound.ChunkId)
            {
                case SoundConstants.RIFF:
                    bigEndian = false;
                    break;
                case SoundConstants.RIFX:
                    bigEndian = true;
                    break;
                default:
                    throw new Exception($"Unknown format: {sound.ChunkId}");
            }
            sound.ChunkSize = bytes.Slice(currentIndex, 4).GetUInt(bigEndian) + 8;
            currentIndex += 4;
            sound.Format = Encoding.UTF8.GetString(bytes.Slice(currentIndex, 4));
            currentIndex += 4;
            if (sound.Format != "WAVE")
                throw new Exception("This is not a WAV File");

            var fmtChunkReceived = false;

            while (currentIndex < bytes.Length)
            {
                var chunkId = Encoding.UTF8.GetString(bytes.Slice(currentIndex, 4));
                currentIndex += 4;
                switch (chunkId)
                {
                    case SoundConstants.FMT_ID:
                        fmtChunkReceived = true;
                        sound.SubChunk1Id = chunkId;
                        sound.SubChunk1Size = bytes.Slice(currentIndex, 4).GetUInt(bigEndian);
                        currentIndex += 4;
                        var bytesRead = 0;
                        if (sound.SubChunk1Size < 16)
                            throw new Exception("Wrong WAV file format");

                        sound.AudioFormat = bytes.Slice(currentIndex, 2, 4).GetUShort(bigEndian);
                        currentIndex += 2;
                        sound.NumChannels = bytes.Slice(currentIndex, 2, 4).GetUShort(bigEndian);
                        currentIndex += 2;
                        sound.SampleRate = bytes.Slice(currentIndex, 4).GetUInt(bigEndian);
                        currentIndex += 4;
                        sound.ByteRate = bytes.Slice(currentIndex, 4).GetUInt(bigEndian);
                        currentIndex += 4;
                        sound.BlockAlign = bytes.Slice(currentIndex, 2, 4).GetUShort(bigEndian);
                        currentIndex += 2;
                        sound.BitDepth = bytes.Slice(currentIndex, 2, 4).GetUShort(bigEndian);
                        currentIndex += 2;
                        bytesRead += 16;

                        if (sound.AudioFormat == SoundConstants.WAVE_FORMAT_EXTENSIBLE && sound.SubChunk1Size >= 18)
                        {
                            sound.ExtraChunkSize = bytes.Slice(currentIndex, 2, 4).GetUShort(bigEndian);
                            bytesRead += 2;
                            currentIndex += 2;
                            if (sound.ExtraChunkSize >= 22)
                            {
                                sound.ExtraData = bytes.Slice(currentIndex, 22, 4);
                                currentIndex += 22;
                                bytesRead += 22;
                                var rawGuid = sound.ExtraData.Slice(6, 16, 4);
                                var tail = bigEndian
                                    ? new byte[] {0x0, 0x0, 0x0, 0x10, 0x80, 0x0, 0x0, 0xAA, 0x00, 0x38, 0x9B, 0x71}
                                    : new byte[] {0x0, 0x0, 0x10, 0x0, 0x80, 0x0, 0x0, 0xAA, 0x0, 0x38, 0x9B, 0x71};
                                if (rawGuid.EndsWith(tail))
                                    sound.AudioFormat = rawGuid.Skip(rawGuid.Length - 4).ToArray().GetUShort(bigEndian);
                            }
                            else
                                throw new Exception("Wave error");
                        }
                        if (!SoundConstants.KNOWN_WAVE_FORMATS.Contains(sound.AudioFormat))
                            throw new Exception("Unknown wave format");

                        if (sound.SubChunk1Size > bytesRead)
                            currentIndex += (int) (sound.SubChunk1Size - bytesRead);

                        if (!new List<ushort> {8, 16, 32, 64, 96, 128}.Contains(sound.BitDepth))
                            throw new Exception($"Unsupported bit depth: {sound.BitDepth}-Bit");
                        break;
                    case SoundConstants.FACT_ID:
                        if (currentIndex + 4 < bytes.Length)
                            currentIndex += 4 + bytes.Slice(currentIndex, 4, 4).GetUShort(bigEndian);
                        break;
                    case SoundConstants.DATA_ID:
                        if (!fmtChunkReceived)
                            throw new Exception("No ftm chunk before data");
                        sound.SubChunk2Id = chunkId;
                        sound.SubChunk2Size = bytes.Slice(currentIndex, 4, 4).GetUInt(bigEndian);
                        currentIndex += 4;

                        var bytesPerSample = (ushort) (sound.BitDepth / 8);
                        var type = sound.BitDepth == 8 ?
                            "u1" :
                            (bigEndian ? ">" : "<") +
                                    (sound.AudioFormat == SoundConstants.WAVE_FORMAT_PCM ? "i" : "f") +
                                    bytesPerSample;
                        sound.Data = bytes.Skip(currentIndex).Take((int) sound.SubChunk2Size).ToArray().FromBuffer(type);
                        currentIndex += (int) sound.SubChunk2Size;
                       // if (sound.NumChannels > 1)
                        break;
                    case SoundConstants.LIST_ID:
                        if (currentIndex + 4 < bytes.Length)
                            currentIndex += 4 + bytes.Slice(currentIndex, 4, 4).GetUShort(bigEndian);
                        break;
                    case var _ when new List<string> {SoundConstants.JUNK_ID, SoundConstants.FAKE_ID}.Contains(sound.SubChunk1Id):
                        if (currentIndex + 4 < bytes.Length)
                            currentIndex += 4 + bytes.Slice(currentIndex, 4, 4).GetUShort(bigEndian);
                        break;
                    default:
                        // warning
                        if (currentIndex + 4 < bytes.Length)
                            currentIndex += 4 + bytes.Slice(currentIndex, 4, 4).GetUShort(bigEndian);
                        break;
                }
            }

            sound.Duration = sound.ChunkSize * 1.0 / (sound.SampleRate * sound.NumChannels * sound.BitDepth / 8.0);
            return sound;
        }
    }
}