using System.Collections.Generic;

namespace Music_Extract_Feature
{
    public interface ISound
    {
        SoundType Type { get; }
        List<double> Data { get; set; }
        List<byte> RawBinary { get; set; }
        double Duration { get; set; }
        double GetChunkSize();
    }
    public enum SoundType
    {
        Wav,
        Mp3
    }
}