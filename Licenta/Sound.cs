using System.Collections.Generic;

namespace Music_Extract_Feature
{
    public interface Sound
    {
        SoundType Type { get; }
        List<long> Data { get; set; }
    }

    public enum SoundType
    {
        Wav,
        Mp3
    }
}