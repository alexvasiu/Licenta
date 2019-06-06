using System.Collections.Generic;

namespace Music_Extract_Feature
{
    public class Mp3Sound : Sound
    {
        public SoundType Type { get; } = SoundType.Mp3;
        public List<double> Data { get; set; }
        public double GetChunkSize()
        {
            throw new System.NotImplementedException();
        }

        public MpegVersion MpegVersion { get; set; }
        public LayerType LayerType { get; set; }
        public bool Protection { get; set; }
        public int BitRateIndex { get; set; }
        public int RateFrequencyIndex { get; set; }
        public bool Padded { get; set; }
        public bool PrivateBit { get; set; }
        public ChannelMode ChannelMode { get; set; }
        public bool Copyright { get; set; }
        public bool Original { get; set; }
        public EmphasisType EmphasisType { get; set; }

        public Tag Tag { get; set; } = null;

    }
}