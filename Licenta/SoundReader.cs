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
        private SoundReader() { }

        public static ISound ReadFromData(byte[] data, SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.Wav:
                    return WavSound.GetWavSound(data);
                case SoundType.Mp3:
                    return null;
                default:
                    return null;
            }
        }

        public static ISound ReadFromFile(string path)
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
                    return WavSound.GetWavSound(bytes);
                case ".mp3":
                    var sound2 = new Mp3Sound();

                    // TODO: ID3v2

                    //ID3v1
                    if (Encoding.UTF8.GetString(bytes.Slice(0, 3)) != "ID3")
                    {
                        var headerBits = bytes.Slice(0, 4).ToBitsArray();

                        /*var frameSync = headerBits.Take(11).ToList();
                        if (frameSync.Sum() != frameSync.Count)
                            throw new Exception("Invalid MP3 file");*/

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

                        switch (bitRateBits)
                        {
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 0, 0, 0, 0 }):
                                sound2.BitRateIndex = 0;
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 0, 0, 0, 1 }):
                                if ((sound2.MpegVersion == MpegVersion.Mpeg2 || sound2.MpegVersion == MpegVersion.Mpeg25) &&
                                    (sound2.LayerType == LayerType.Layer2 || sound2.LayerType == LayerType.Layer3))
                                    sound2.BitRateIndex = 8;
                                else
                                    sound2.BitRateIndex = 32;
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 0, 0, 1, 0 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 64 : 48;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 48 : 16;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 40 : 16;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 0, 0, 1, 1 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 96 : 56;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 56 : 24;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 48 : 24;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 0, 1, 0, 0 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 128 : 64;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 64 : 32;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 56 : 32;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 0, 1, 0, 1 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 160 : 80;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 80 : 40;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 64 : 40;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 0, 1, 1, 0 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 192 : 96;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 96 : 48;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 80 : 48;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 0, 1, 1, 1 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 224 : 112;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 112 : 56;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 96 : 56;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 1, 0, 0, 0 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 256 : 128;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 128 : 64;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 112 : 64;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 1, 0, 0, 1 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 288 : 144;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 160 : 80;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 128 : 80;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 1, 0, 1, 0 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 320 : 160;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 192 : 96;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 160 : 96;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 1, 0, 1, 1 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 352 : 176;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 224 : 112;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 192 : 112;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 1, 1, 0, 0 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 384 : 192;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 256 : 128;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 224 : 128;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 1, 1, 0, 1 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 416 : 224;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 320 : 144;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 256 : 144;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case var _ when bitRateBits.IsEqualTo(new List<int> { 1, 1, 1, 0 }):
                                switch (sound2.LayerType)
                                {
                                    case LayerType.Layer1:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 448 : 256;
                                        break;
                                    case LayerType.Layer2:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 384 : 160;
                                        break;
                                    case LayerType.Layer3:
                                        sound2.BitRateIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 320 : 160;
                                        break;
                                    case LayerType.Reserved:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            default:
                                sound2.BitRateIndex = -1;
                                break;
                        }

                        var frequencyIndexBits = headerBits.Skip(20).Take(2).ToList();

                        switch (frequencyIndexBits)
                        {
                            case var _ when frequencyIndexBits.IsEqualTo(new List<int> { 0, 0 }):
                                sound2.RateFrequencyIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 44100 :
                                    sound2.MpegVersion == MpegVersion.Mpeg2 ? 22050 : 11025;
                                break;
                            case var _ when frequencyIndexBits.IsEqualTo(new List<int> { 0, 1 }):
                                sound2.RateFrequencyIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 48000 :
                                    sound2.MpegVersion == MpegVersion.Mpeg2 ? 24000 : 12000;
                                break;
                            case var _ when frequencyIndexBits.IsEqualTo(new List<int> { 1, 0 }):
                                sound2.RateFrequencyIndex = sound2.MpegVersion == MpegVersion.Mpeg1 ? 32000 :
                                    sound2.MpegVersion == MpegVersion.Mpeg2 ? 16000 : 8000;
                                break;
                            case var _ when frequencyIndexBits.IsEqualTo(new List<int> { 1, 1 }):
                                sound2.RateFrequencyIndex = -1;
                                break;
                        }

                        var paddingBit = headerBits.Skip(22).Take(1).ToList();
                        sound2.Padded = paddingBit[0] == 1;

                        var privateBit = headerBits.Skip(23).Take(1).ToList();
                        sound2.PrivateBit = privateBit[0] == 1;

                        var channelModeBits = headerBits.Skip(24).Take(2).ToList();
                        if (channelModeBits[0] == 0)
                            sound2.ChannelMode = channelModeBits[1] == 0 ? ChannelMode.Stereo : ChannelMode.JointStereo;
                        else
                            sound2.ChannelMode = channelModeBits[1] == 0 ? ChannelMode.DualChannel : ChannelMode.SingleChannel;

                        var modeExtenstionBits = headerBits.Skip(26).Take(2).ToList();

                        var copyrightBits = headerBits.Skip(28).Take(1).ToList();
                        sound2.Copyright = copyrightBits[0] == 1;

                        var originalBits = headerBits.Skip(29).Take(1).ToList();
                        sound2.Original = originalBits[0] == 1;

                        var emphasisBits = headerBits.Skip(30).ToList();
                        if (emphasisBits[0] == 0)
                            sound2.EmphasisType = emphasisBits[1] == 0 ? EmphasisType.None : EmphasisType._50_15ms;
                        else
                            sound2.EmphasisType = emphasisBits[1] == 0 ? EmphasisType.Reserved : EmphasisType.Ccitj17;
                    }
                    else //ID3v2
                    {
                        
                        var majorVersion = BitConverter.ToInt32(bytes.Slice(3, 1, 4), 0);
                        var minorVersion = BitConverter.ToInt32(bytes.Slice(4, 1, 4), 0);

                        var flagsBits = bytes[5].ToBitsArray();

                        var unsynchronisationFlag = flagsBits[0] == 1;
                        var extendedHeaderFlag = flagsBits[1] == 1;
                        var experimentalIndicatorFlag = flagsBits[2] == 1;
                        var footerPresentFlag = flagsBits[3] == 1;

                        if (!flagsBits.Skip(4).ToList().IsEqualTo(new List<int> {0, 0, 0, 0}))
                            throw new Exception("MP3 Error");

                        var size = bytes[9] + bytes[8] << 7 + bytes[6] << 14 + bytes[5] << 21;

                        var currentByte = 10;

                        if (extendedHeaderFlag)
                        {
                            var extendedHeaderSize = bytes[currentByte + 3] + bytes[currentByte + 2] <<
                                                     7 + bytes[currentByte + 1] << 14 + bytes[currentByte] << 21;
                            currentByte += extendedHeaderSize; //5; // skip flags size
                            // TODO: parse extended header - optional
                            //  https://mutagen-specs.readthedocs.io/en/latest/id3/id3v2.4.0-structure.html
                        }

                        // TODO: Frames

                        

                        // TODO: Parse padding - optional
                        // TODO: Parse footer - optional

                    }

                    // TODO: Data from mp3

                    var tagBytes = bytes.Reverse().Take(128).Reverse().ToArray();

                    var tagString = Encoding.UTF8.GetString(tagBytes.Slice(0, 3));

                    if (tagString.ToUpper() != "TAG") return sound2;

                    var tag = new Tag
                    {
                        Title = Encoding.UTF8.GetString(tagBytes.Slice(3, 30)).Trim('\0'),
                        Artist = Encoding.UTF8.GetString(tagBytes.Slice(33, 30)).Trim('\0'),
                        Album = Encoding.UTF8.GetString(tagBytes.Slice(63, 30)).Trim('\0'),
                        Year = Encoding.UTF8.GetString(tagBytes.Slice(93, 4)).Trim('\0'),
                        CommentV1 = Encoding.UTF8.GetString(tagBytes.Slice(97, 30)).Trim('\0'),
                        CommentV2 = Encoding.UTF8.GetString(tagBytes.Slice(97, 28)).Trim('\0'),
                        AlbumTrack = Encoding.UTF8.GetString(tagBytes.Slice(125, 2)).Trim('\0'),
                        Genre = BitConverter.ToInt32(tagBytes.Slice(127, 1, 4), 0)
                    };

                    sound2.Tag = tag;

                    return sound2;
                default:
                    return null;
            }
        }
    }
}