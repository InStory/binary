using System;
using System.IO;
using InStory.binary.stream;
using Microsoft.IO;
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
            
            r.SetBuffer(new byte[]{ 0xFF });
            Assert.AreEqual(0xFF, r.ReadByte());
            
            r.SetBuffer(new byte[]{ 0xFF });
            Assert.AreEqual(-1, r.ReadSignedByte());
        }

        [Test]
        public void Short()
        {
            using var r = RStream.Get("Read Short");

            r.SetBuffer(new byte[]{ 0xFE, 0xFF }); 
            Assert.AreEqual(-2, r.ReadSignedShort()); // big endian
            
            r.SetBuffer(new byte[]{ 0xFF, 0xFE });
            Assert.AreEqual(-2, r.ReadSignedShortLittleEndian()); // little endian
            
            r.SetBuffer(new byte[]{ 0xAA, 0x7F});
            Assert.AreEqual(0x7FAA, r.ReadUnsignedShort()); // big endian
            
            r.SetBuffer(new byte[]{ 0x7F, 0xAA });
            Assert.AreEqual(0x7FAA, r.ReadUnsignedShortLittleEndian()); // little endian
        }

        [Test]
        public void Triad()
        {
            using var r = RStream.Get("Read Triad");
            
            r.SetBuffer(new byte[]{ 0xAA, 0xBB, 0x77 });
            Assert.AreEqual(0x77BBAA, r.ReadTriad()); // big endian
            
            r.SetBuffer(new byte[]{ 0x77, 0xAA, 0xBB });
            Assert.AreEqual(0x77AABB, r.ReadTriadLittleEndian()); // little endian
        }

        [Test]
        public void Int()
        {
            using var r = RStream.Get("Read Int");
            
            r.SetBuffer(new byte[]{ 0xFE, 0xFF, 0xFF, 0xFF });
            Assert.AreEqual(-2, r.ReadSignedInt()); // big endian
            
            r.SetBuffer(new byte[]{ 0xFF, 0xFF, 0xFF, 0xFE });
            Assert.AreEqual(-2, r.ReadSignedIntLittleEndian()); // little endian
            
            r.SetBuffer(new byte[]{ 0xAA, 0xBB, 0xCC, 0xDD });
            Assert.AreEqual(0xDDCCBBAA, r.ReadUnsignedInt()); // big endian
            
            r.SetBuffer(new byte[]{ 0xFF, 0xEE, 0xDD, 0xCC });
            Assert.AreEqual(0xFFEEDDCC, r.ReadUnsignedIntLittleEndian()); // little endian
        }

        [Test]
        public void Long()
        {
            using var r = RStream.Get("Read long");
            
            r.SetBuffer(new byte[]{ 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
            Assert.AreEqual(-2, r.ReadSignedLong());
            
            r.SetBuffer(new byte[]{ 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE });
            Assert.AreEqual(-2, r.ReadSignedLongLittleEndian());
            
            r.SetBuffer(new byte[]{ 0x00, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x11, 0xAA });
            Assert.AreEqual(0xAA11FFEEDDCCBB00, r.ReadUnsignedLong());
            
            r.SetBuffer(new byte[]{ 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x11, 0x22 });
            Assert.AreEqual(0xAABBCCDDEEFF1122, r.ReadUnsignedLongLittleEndian());
        }

        [Test]
        public void String()
        {
            using var r = RStream.Get("Read string");
            
            r.SetBuffer(new byte[]{ 0x0A, 0x50, 0x72, 0x69, 0x73, 0x6d, 0x61, 0x2e, 0x4e, 0x45, 0x54 });
            Assert.AreEqual("Prisma.NET", r.ReadString());
            
            r.SetBuffer(new byte[]{ 0x0A, 0x50, 0x72, 0x69, 0x73, 0x6d, 0x61, 0x2e, 0x4e, 0x45, 0x54 });
            Assert.AreEqual("Prisma.NET", r.ReadByteSizedString());
        }

        [Test]
        public void ByteArray()
        {
            using var r = RStream.Get("Read string");
            
            r.SetBuffer(new byte[]{ 0x05, 0x11, 0x22, 0x33, 0x44, 0x55 });
            Assert.AreEqual(new byte[]{ 0x11, 0x22, 0x33, 0x44, 0x55 }, r.ReadByteArray().ToArray());
            
            r.SetBuffer(new byte[]{ 0x05, 0x11, 0x22, 0x33, 0x44, 0x55 });
            Assert.AreEqual(new byte[]{ 0x11, 0x22, 0x33, 0x44, 0x55 }, r.ReadByteSizedByteArray().ToArray());
        }
        
        [Test]
        public void VarInt()
        {
            using var s = RStream.Get("Read varint");
            
            s.SetBuffer(new byte[] { 0xEC, 0x12, 0x3E, 0xC4, 0x56 });
            Assert.AreEqual(2412, s.ReadUnsignedVarInt());

            s.SetBuffer(new byte[]{ 0xBC, 0xD1, 0x23, 0xEF, 0xA0 });
            Assert.AreEqual(583868, s.ReadUnsignedVarInt());

            s.SetBuffer(new byte[]{ 0xEC, 0x12, 0x3E, 0xC4, 0x56 });
            Assert.AreEqual(1206, s.ReadSignedVarInt());

            s.SetBuffer(new byte[]{ 0xBC, 0xD1, 0x23, 0xEF, 0xA0 });
            Assert.AreEqual(291934, s.ReadSignedVarInt());
        }

        [Test]
        public void VarLong()
        {
            using var s = RStream.Get("Read varlong");

            s.SetBuffer(new byte[]{ 0xFF, 0x2E, 0xC4, 0x56, 0xEC, 0x78, 0x9E, 0xC0, 0x12, 0xEC });
            Assert.AreEqual(6015, s.ReadUnsignedVarLong());

            s.SetBuffer(new byte[]{ 0xEE, 0x1C, 0xD3, 0x4B, 0xCD, 0x56, 0xBC, 0xD7, 0x8B, 0xCD });
            Assert.AreEqual(3694, s.ReadUnsignedVarLong());

            s.SetBuffer(new byte[]{ 0xFF, 0x2E, 0xC4, 0x56, 0xEC, 0x78, 0x9E, 0xC0, 0x12, 0xEC });
            Assert.AreEqual(-3008, s.ReadSignedVarLong());

            s.SetBuffer(new byte[]{ 0xEE, 0x1C, 0xD3, 0x4B, 0xCD, 0x56, 0xBC, 0xD7, 0x8B, 0xCD });
            Assert.AreEqual(1847, s.ReadSignedVarLong());
        }
        
        

        [Test]
        public void NoAllocOnRead()
        {
            var manager = new RecyclableMemoryStreamManager();

            using (var stream = manager.GetStream())
            {
                var ar = new byte[100];
                for (byte i = 0; i < 100; ++i)
                {
                    ar[i] = i;
                }

                using var s = WStream.Get("Write NoAllocOnRead");
                s.WriteByteArray(ar);

                using var r = s.ToRStream("Read after Write NoAllocOnRead");

                stream.SetLength(ar.Length);
                r.ReadByteArrayInto(stream);

                stream.Seek(0, SeekOrigin.Begin);

                for (byte i = 0; i < 100; ++i)
                {
                    var b = stream.ReadByte();
                    if (b != i)
                    {
                        Assert.Fail("Wrong byte. Need " + i + ", got " + b);
                    }
                }

                Assert.Pass();
            }
            
            using (var stream = manager.GetStream())
            {
                var ar = new byte[100];
                for (byte i = 0; i < 100; ++i)
                {
                    ar[i] = i;
                }

                using var s = WStream.Get("Write NoAllocOnRead");
                s.WriteByteArray(ar);

                using var r = s.ToRStream("Read after Write NoAllocOnRead");

                stream.SetLength(ar.Length);
                r.ReadByteSizedByteArrayInto(stream);

                stream.Seek(0, SeekOrigin.Begin);

                for (byte i = 0; i < 100; ++i)
                {
                    var b = stream.ReadByte();
                    if (b != i)
                    {
                        Assert.Fail("Wrong byte. Need " + i + ", got " + b);
                    }
                }

                Assert.Pass();
            }
        }
    }
}