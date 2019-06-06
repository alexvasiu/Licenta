// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
namespace Music_Extract_Feature
{
    public static class SoundConstants
    {
        public const string RIFF = "RIFF";
        public const string RIFX = "RIFX";
        public const string FMT_ID = "fmt ";
        public const string FACT_ID = "fact";
        public const string DATA_ID = "data";
        public const string LIST_ID = "LIST";
        public const string JUNK_ID = "JUNK";
        public const string FAKE_ID = "Fake";
        public const ushort WAVE_FORMAT_EXTENSIBLE = 0xfffe;
        public const ushort WAVE_FORMAT_PCM = 0x0001;
        public const ushort WAVE_FORMAT_IEEE_FLOAT = 0x0003;
        public static readonly ushort[] KNOWN_WAVE_FORMATS = {WAVE_FORMAT_PCM, WAVE_FORMAT_IEEE_FLOAT};
    }
}