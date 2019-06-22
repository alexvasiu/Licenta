using System.Collections.Generic;

namespace Music_Extract_Feature
{
    public interface ISound
    {
        SoundType Type { get; }

        ushort NumChannels { get; set; }
        List<double> Data { get; set; }
        List<byte> RawBinary { get; set; }
        double Duration { get; set; }
        ushort BitDepth { get; set; }
        uint SampleRate { get; set; }
        double GetChunkSize();
    }
    public enum SoundType
    {
        Wav,
        Mp3
    }
}