using System.Collections.Generic;

namespace Music_Extract_Feature
{
    public interface Sound
    {
        SoundType Type { get; }
        List<double> Data { get; set; }
        double GetChunkSize();
    }

    public enum SoundType
    {
        Wav,
        Mp3
    }
}