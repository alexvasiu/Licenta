using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Music_Extract_Feature;

namespace UnitTesting
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestWav()
        {
            var musicPath = Path.GetFullPath(
                Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\bells.wav"));
            var wavSound = (WavSound) SoundReader.ReadFromFile(musicPath);
            Assert.IsNotNull(wavSound);
            Assert.AreEqual(16, wavSound.BitDepth);
            Assert.AreEqual(2, wavSound.NumChannels);
            Assert.AreEqual((uint) 44100, wavSound.SampleRate);
            Assert.AreEqual(1222414, wavSound.Data.Count);
        }
    }
}
