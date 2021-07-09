using System;
using InStory.binary.stream;
using NUnit.Framework;

namespace tests
{
    public class RStreamTest
    {
        [SetUp]
        public void Setup(){}

        [Test]
        public void Byte()
        {
            using var r = RStream.Get("Read Byte");
            
            r.WriteBuffer(new byte[]{ 0xFF });
            Assert.AreEqual(0xFF, r.ReadByte());
            
            r.WriteBuffer(new byte[]{ 0xFF });
            Assert.AreEqual(-1, r.ReadSignedByte());
        }

        [Test]
        public void Short()
        {
            using var r = RStream.Get("Read Short");

            r.WriteBuffer(new byte[]{ 0xFE, 0xFF }); 
            Assert.AreEqual(-2, r.ReadSignedShort()); // big endian
            
            r.WriteBuffer(new byte[]{ 0xFF, 0xFE });
            Assert.AreEqual(-2, r.ReadSignedShortLittleEndian()); // little endian
            
            r.WriteBuffer(new byte[]{ 0xAA, 0x7F});
            Assert.AreEqual(0x7FAA, r.ReadUnsignedShort()); // big endian
            
            r.WriteBuffer(new byte[]{ 0x7F, 0xAA });
            Assert.AreEqual(0x7FAA, r.ReadUnsignedShortLittleEndian()); // little endian
        }

        [Test]
        public void Triad()
        {
            using var r = RStream.Get("Read Triad");
            
            r.WriteBuffer(new byte[]{ 0xAA, 0xBB, 0x77 });
            Assert.AreEqual(0x77BBAA, r.ReadTriad()); // big endian
            
            r.WriteBuffer(new byte[]{ 0x77, 0xAA, 0xBB });
            Assert.AreEqual(0x77AABB, r.ReadTriadLittleEndian()); // little endian
        }

        [Test]
        public void Int()
        {
            using var r = RStream.Get("Read Int");
            
            r.WriteBuffer(new byte[]{ 0xFE, 0xFF, 0xFF, 0xFF });
            Assert.AreEqual(-2, r.ReadSignedInt()); // big endian
            
            r.WriteBuffer(new byte[]{ 0xFF, 0xFF, 0xFF, 0xFE });
            Assert.AreEqual(-2, r.ReadSignedIntLittleEndian()); // little endian
            
            r.WriteBuffer(new byte[]{ 0xAA, 0xBB, 0xCC, 0xDD });
            Assert.AreEqual(0xDDCCBBAA, r.ReadUnsignedInt()); // big endian
            
            r.WriteBuffer(new byte[]{ 0xFF, 0xEE, 0xDD, 0xCC });
            Assert.AreEqual(0xFFEEDDCC, r.ReadUnsignedIntLittleEndian()); // little endian
        }

        [Test]
        public void Long()
        {
            using var r = RStream.Get("Read long");
            
            r.WriteBuffer(new byte[]{ 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
            Assert.AreEqual(-2, r.ReadSignedLong());
            
            r.WriteBuffer(new byte[]{ 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE });
            Assert.AreEqual(-2, r.ReadSignedLongLittleEndian());
            
            r.WriteBuffer(new byte[]{ 0x00, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x11, 0xAA });
            Assert.AreEqual(0xAA11FFEEDDCCBB00, r.ReadUnsignedLong());
            
            r.WriteBuffer(new byte[]{ 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x11, 0x22 });
            Assert.AreEqual(0xAABBCCDDEEFF1122, r.ReadUnsignedLongLittleEndian());
        }

        [Test]
        public void String()
        {
            using var r = RStream.Get("Read string");
            
            r.WriteBuffer(new byte[]{ 0x0A, 0x50, 0x72, 0x69, 0x73, 0x6d, 0x61, 0x2e, 0x4e, 0x45, 0x54 });
            Assert.AreEqual("Prisma.NET", r.ReadString());
            
            r.WriteBuffer(new byte[]{ 0x0A, 0x50, 0x72, 0x69, 0x73, 0x6d, 0x61, 0x2e, 0x4e, 0x45, 0x54 });
            Assert.AreEqual("Prisma.NET", r.ReadByteSizedString());
        }

        [Test]
        public void ByteArray()
        {
            using var r = RStream.Get("Read string");
            
            r.WriteBuffer(new byte[]{ 0x05, 0x11, 0x22, 0x33, 0x44, 0x55 });
            Assert.AreEqual(new byte[]{ 0x11, 0x22, 0x33, 0x44, 0x55 }, r.ReadByteArray().ToArray());
            
            r.WriteBuffer(new byte[]{ 0x05, 0x11, 0x22, 0x33, 0x44, 0x55 });
            Assert.AreEqual(new byte[]{ 0x11, 0x22, 0x33, 0x44, 0x55 }, r.ReadByteSizedByteArray().ToArray());
        }
        
        [Test]
        public void VarInt()
        {
            using var s = RStream.Get("Read varint");
            
            s.WriteBuffer(new byte[] { 0xEC, 0x12, 0x3E, 0xC4, 0x56 });
            Assert.AreEqual(2412, s.ReadUnsignedVarInt());

            s.WriteBuffer(new byte[]{ 0xBC, 0xD1, 0x23, 0xEF, 0xA0 });
            Assert.AreEqual(583868, s.ReadUnsignedVarInt());

            s.WriteBuffer(new byte[]{ 0xEC, 0x12, 0x3E, 0xC4, 0x56 });
            Assert.AreEqual(1206, s.ReadSignedVarInt());

            s.WriteBuffer(new byte[]{ 0xBC, 0xD1, 0x23, 0xEF, 0xA0 });
            Assert.AreEqual(291934, s.ReadSignedVarInt());
        }

        [Test]
        public void VarLong()
        {
            using var s = RStream.Get("Read varlong");

            s.WriteBuffer(new byte[]{ 0xFF, 0x2E, 0xC4, 0x56, 0xEC, 0x78, 0x9E, 0xC0, 0x12, 0xEC });
            Assert.AreEqual(6015, s.ReadUnsignedVarLong());

            s.WriteBuffer(new byte[]{ 0xEE, 0x1C, 0xD3, 0x4B, 0xCD, 0x56, 0xBC, 0xD7, 0x8B, 0xCD });
            Assert.AreEqual(3694, s.ReadUnsignedVarLong());

            s.WriteBuffer(new byte[]{ 0xFF, 0x2E, 0xC4, 0x56, 0xEC, 0x78, 0x9E, 0xC0, 0x12, 0xEC });
            Assert.AreEqual(-3008, s.ReadSignedVarLong());

            s.WriteBuffer(new byte[]{ 0xEE, 0x1C, 0xD3, 0x4B, 0xCD, 0x56, 0xBC, 0xD7, 0x8B, 0xCD });
            Assert.AreEqual(1847, s.ReadSignedVarLong());
        }
    }
}