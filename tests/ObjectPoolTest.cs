using InStory.binary.packet;
using InStory.binary.stream;
using NUnit.Framework;

namespace tests
{
    
    public class ObjectPoolTest
    {
        [Test]
        public void NoReAllocOnGet()
        {
            //todo rewrite: there can be more than 1 pooled object
            
            Assert.AreEqual(0, WStream.AllObjects - WStream.PooledObjects);
            
            var n = WStream.AllObjects - WStream.PooledObjects;

            var s = WStream.Get("Write NoReAlloc first");
            
            Assert.AreEqual(n+1, WStream.AllObjects);
            Assert.AreEqual(n, WStream.PooledObjects);
            
            s.Dispose();
            
            Assert.AreEqual(n+1, WStream.AllObjects);
            Assert.AreEqual(n+1, WStream.PooledObjects);

            var t = WStream.Get("Write NoReAlloc second");
            
            Assert.AreEqual(n+1, WStream.AllObjects);
            Assert.AreEqual(n, WStream.PooledObjects);

            Assert.IsTrue(ReferenceEquals(s, t));

            t.Dispose();
        }
    }
}