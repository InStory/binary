using InStory.binary.packet;
using InStory.binary.stream;
using NUnit.Framework;

namespace tests
{
    #region Some packet classes
    internal class APacket : Packet<APacket>
    {
        public override byte GetId()
        {
            return 0x0A;
        }

        protected override void Decode(RStream r){}

        protected override void Encode(WStream w){}
    }

    internal class BPacket : Packet<BPacket>
    {
        public int Value;
        
        public override byte GetId()
        {
            return 0x0B;
        }

        protected override void Decode(RStream r)
        {
            Value = r.ReadSignedIntLittleEndian();
        }

        protected override void Encode(WStream w)
        {
            w.WriteSignedIntLittleEndian(Value);
        }
    }
    #endregion
    
    public class PacketPoolTest
    {
        public PacketPool StartedPool = new();
        public PacketPool NotStartedPool = new();
        
        [OneTimeSetUp]
        public void Setup()
        {
            StartedPool.Add(new APacket());
            StartedPool.Start();
        }

        [Test]
        public void AddPackets()
        {
            Assert.Catch<ReadOnlyPoolException>(() =>
            {
                StartedPool.Add(new APacket()); // should throw exception because pool is closed
            });
            
            NotStartedPool.Add(new APacket());

            Assert.Catch<PacketAlreadyAddedException>(() =>
            {
                NotStartedPool.Add(new APacket());
            });
        }

        [Test]
        public void GetPackets()
        {
            Assert.Catch<PoolIsNotStartedException>(() =>
            {
                using var r = RStream.Get();
                r.SetBuffer(new byte[]{ 0x99 });
                
                NotStartedPool.Get(r);
            });

            Assert.Catch<PacketNotFoundException>(() =>
            {
                using var r = RStream.Get();
                r.SetBuffer(new byte[] { 0x99 });

                StartedPool.Get(r);
            });
            
            using var r = RStream.Get();
            r.SetBuffer(new byte[] { 0x0A });
            
            Assert.AreEqual(StartedPool.Get(r).GetId(), 0x0A);
        }
        
    }
}