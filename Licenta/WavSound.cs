using System.Collections.Generic;

namespace Music_Extract_Feature
{
    public class WavSound : Sound
    {
        public string ChunkId { get; set; }
        public int ChunkSize { get; set; }
        public string Format { get; set; }
        public string SubChunk1Id { get; set; }
        public int SubChunk1Size { get; set; }
        public int AudioFormat { get; set; }
        public int NumChannels { get; set; }
        public int SampleRate { get; set; }
        public int ByteRate { get; set; }
        public int BlockAlign { get; set; }
        public int BitsPerSample { get; set; }
        public string SubChunk2Id { get; set; }
        public int SubChunk2Size { get; set; }
        public List<long> Data { get; set; }
        public SoundType Type { get; } = SoundType.Wav;
    }
}