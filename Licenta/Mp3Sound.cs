using System.Collections.Generic;

namespace Music_Extract_Feature
{
    public class Mp3Sound : Sound
    {
        public SoundType Type { get; } = SoundType.Mp3;
        public List<long> Data { get; set; }
        public MpegVersion MpegVersion { get; set; }
        public LayerType LayerType { get; set; }
        public bool Protection { get; set; }
    }
}