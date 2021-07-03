using InStory.binary.stream;
using NUnit.Framework;

namespace tests
{
    public class Tests
    {
        
        [SetUp]
        public void Setup()
        {
            using (var lol = RStream.Get())
            {
                var b = lol.ReadString();
            }

            using var a = RStream.Get();
            var c = a.ReadString();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}