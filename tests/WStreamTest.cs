using System;
using System.IO;
using System.Text;
using InStory.binary.stream;
using Microsoft.IO;
using Microsoft.VisualBasic;
using NUnit.Framework;

namespace tests
{
    public class WStreamTest
    {
        [SetUp]
        public void Setup(){}

        [Test]
        public void Byte()
        {
            using var s = WStream.Get("Write Byte");
            
            s.WriteByte(0xFF);
            s.WriteSignedByte(0x7F);
            s.WriteSignedByte(-0x01);

            using var r = s.ToRStream("Read after Write Byte");

            Assert.AreEqual(0xFF, r.ReadByte());
            Assert.AreEqual(0x7F, r.ReadSignedByte());
            Assert.AreEqual(-1, r.ReadSignedByte());
        }

        [Test]
        public void Short()
        {
            using var s = WStream.Get("Write Short");
            
            s.WriteSignedShort(0x7FAA);
            s.WriteSignedShort(-1);
            s.WriteSignedShortLittleEndian(0x1122);
            s.WriteSignedShortLittleEndian(-1);
            
            s.WriteUnsignedShort(0xAABB);
            s.WriteUnsignedShortLittleEndian(0xCCDD);

            using var r = s.ToRStream("Read after Write Short");

            Assert.AreEqual(0x7FAA, r.ReadSignedShort());
            Assert.AreEqual(-1, r.ReadSignedShort());
            Assert.AreEqual(0x1122, r.ReadSignedShortLittleEndian());
            Assert.AreEqual(-1, r.ReadSignedShortLittleEndian());

            Assert.AreEqual(0xAABB, r.ReadUnsignedShort());
            Assert.AreEqual(0xCCDD, r.ReadUnsignedShortLittleEndian());
        }

        [Test]
        public void Triad()
        {
            using var s = WStream.Get("Write Triad");
            
            s.WriteTriad(0xAABBCC);
            s.WriteTriadLittleEndian(0xCCDDEE);

            using var r = s.ToRStream("Read after Write Triad");
            
            Assert.AreEqual(0xAABBCC, r.ReadTriad());
            Assert.AreEqual(0xCCDDEE, r.ReadTriadLittleEndian());
        }

        [Test]
        public void Int()
        {
            using var s = WStream.Get("Write Int");
            
            s.WriteSignedInt(0x7FAABBCC);
            s.WriteSignedInt(-1);
            s.WriteSignedIntLittleEndian(0x11223344);
            s.WriteSignedIntLittleEndian(-1);
            
            s.WriteUnsignedInt(0xAABBCCDD);
            s.WriteUnsignedIntLittleEndian(0xCCDDEEFF);

            using var r = s.ToRStream("Read after Write Int");

            Assert.AreEqual(0x7FAABBCC, r.ReadSignedInt());
            Assert.AreEqual(-1, r.ReadSignedInt());
            Assert.AreEqual(0x11223344, r.ReadSignedIntLittleEndian());
            Assert.AreEqual(-1, r.ReadSignedIntLittleEndian());

            Assert.AreEqual(0xAABBCCDD, r.ReadUnsignedInt());
            Assert.AreEqual(0xCCDDEEFF, r.ReadUnsignedIntLittleEndian());
        }

        [Test]
        public void Long()
        {
            using var s = WStream.Get("Write Long");
            
            s.WriteSignedLong(0x7FAABBCCDDEEFF11);
            s.WriteSignedLong(-1);
            s.WriteSignedLongLittleEndian(0x1122334455667788);
            s.WriteSignedLongLittleEndian(-1);
            
            s.WriteUnsignedLong(0xAABBCCDDEEFF11);
            s.WriteUnsignedLongLittleEndian(0xCCDDEEFFAABBCCDD);

            using var r = s.ToRStream("Read after Write Long");

            Assert.AreEqual(0x7FAABBCCDDEEFF11, r.ReadSignedLong());
            Assert.AreEqual(-1, r.ReadSignedLong());
            Assert.AreEqual(0x1122334455667788, r.ReadSignedLongLittleEndian());
            Assert.AreEqual(-1, r.ReadSignedLongLittleEndian());

            Assert.AreEqual(0xAABBCCDDEEFF11, r.ReadUnsignedLong());
            Assert.AreEqual(0xCCDDEEFFAABBCCDD, r.ReadUnsignedLongLittleEndian());
        }

        [Test]
        public void String()
        {
            var s = WStream.Get("Write String");
            
            s.WriteString("Prisma.NET");
            
            s.WriteByteSizedString("NolikTop");

            Assert.Catch<TooBigValueException>(() =>
            {
                s.WriteByteSizedString(new string('n', 0xFF + 1));
            });

            using var r = s.ToRStream("Read after Write String");
            
            Assert.AreEqual("Prisma.NET", r.ReadString());
            Assert.AreEqual("NolikTop", r.ReadByteSizedString());
        }

        [Test]
        public void ByteArray()
        {
            using var s = WStream.Get("Write ByteArray");
            
            s.WriteByteArray(new byte[]{ 0xAA, 0xBB, 0xCC, 0xDD });
            s.WriteByteSizedByteArray(new byte[]{ 0x11, 0x22, 0x33, 0x44 });

            Assert.Catch<TooBigValueException>(() =>
            {
                var data = new string('n', 0xFF + 1);
                s.WriteByteSizedByteArray(Encoding.ASCII.GetBytes(data));
            });

            using var r = s.ToRStream("Read after Write ByteArray");
            
            Assert.AreEqual(new byte[]{ 0xAA, 0xBB, 0xCC, 0xDD }, r.ReadByteArray().ToArray());
            Assert.AreEqual(new byte[]{ 0x11, 0x22, 0x33, 0x44 }, r.ReadByteSizedByteArray().ToArray());
        }

        [Test]
        public void VarInt()
        {
            using var s = WStream.Get("Write VarInt");
            
            s.WriteUnsignedVarInt(237356812);
            s.WriteSignedVarInt(0x7A3ECA71);
            
            s.WriteSignedVarInt(-1);
            s.WriteSignedVarInt(-0x77BBCCDD);

            using var r = s.ToRStream("Read after Write VarInt");
            
            Assert.AreEqual(237356812, r.ReadUnsignedVarInt());
            Assert.AreEqual(0x7A3ECA71, r.ReadSignedVarInt());
            
            Assert.AreEqual(-1, r.ReadSignedVarInt());
            Assert.AreEqual(-0x77BBCCDD,r.ReadSignedVarInt());
        }

        [Test]
        public void VarLong()
        {
            using var s = WStream.Get("VarLong");
            
            s.WriteUnsignedVarLong(0x1234567812345678L);
            s.WriteSignedVarLong(0x7A3ECA710BECECECL);
            
            s.WriteSignedVarLong(-1);
            s.WriteSignedVarLong(-0x7766554433221100);

            using var r = s.ToRStream("Read after Write VarLong");

            Assert.AreEqual(0x1234567812345678L, r.ReadUnsignedVarLong());
            Assert.AreEqual(0x7A3ECA710BECECECL, r.ReadSignedVarLong());

            Assert.AreEqual(-1, r.ReadSignedVarLong());
            Assert.AreEqual(-0x7766554433221100, r.ReadSignedVarLong());
        }

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