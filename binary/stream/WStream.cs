using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using InStory.binary.other;
using InStory.binary.pool;
using Microsoft.IO;

namespace InStory.binary.stream
{
    public class WStream : PooledObject<WStream>, ILoadOnGet
    {

        private static readonly RecyclableMemoryStreamManager Manager = new();
        public MemoryStream Buffer;

        public void Load()
        {
            if (Buffer != null)
            {
                throw new ReUseStreamException(); 
            }
            Buffer = Manager.GetStream();
        }

        public RStream ToRStream()
        {
            var r = RStream.Get();
            
            r.Buffer.Write(Buffer.GetBuffer());
            r.Buffer.Seek(0, SeekOrigin.Begin);
            return r;
        }

        public RStream ToRStream(string name)
        {
            var r = ToRStream();
            r.Name = name;
            return r;
        }

        public void WriteSignedByte(sbyte value)
        {
            Buffer.WriteByte((byte)value);
        }

        public void WriteUnsignedByte(byte value)
        {
            Buffer.WriteByte(value);
        }

        public void WriteByte(byte value)
        {
            Buffer.WriteByte(value);
        }

        public void WriteSignedShort(short value)
        {
            const int size = sizeof(short);
            Span<byte> t = stackalloc byte[size];

            if (Endianness.DontFlipEndianness)
            {
                BinaryPrimitives.WriteInt16BigEndian(t, value);
            }
            else
            {
                BinaryPrimitives.WriteInt16LittleEndian(t, value);
            }

            Buffer.Write(t);
        }

        public void WriteUnsignedShort(ushort value)
        {
            const int size = sizeof(ushort);
            Span<byte> t = stackalloc byte[size];

            if (Endianness.DontFlipEndianness)
            {
                BinaryPrimitives.WriteUInt16BigEndian(t, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt16LittleEndian(t, value);
            }
            
            Buffer.Write(t);
        }

        public void WriteSignedShortLittleEndian(short value)
        {
            const int size = sizeof(short);
            Span<byte> t = stackalloc byte[size];

            if (Endianness.DontFlipEndianness)
            {
                BinaryPrimitives.WriteInt16LittleEndian(t, value);
            }
            else
            {
                BinaryPrimitives.WriteInt16BigEndian(t, value);
            }

            Buffer.Write(t);
        }

        public void WriteUnsignedShortLittleEndian(ushort value)
        {
            const int size = sizeof(ushort);
            Span<byte> t = stackalloc byte[size];

            if (Endianness.DontFlipEndianness)
            {
                BinaryPrimitives.WriteUInt16LittleEndian(t, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt16BigEndian(t, value);
            }
            
            Buffer.Write(t);
        }

        public void WriteSignedInt(int value)
        {
            const int size = sizeof(int);
            Span<byte> t = stackalloc byte[size];

            if (Endianness.DontFlipEndianness)
            {
                BinaryPrimitives.WriteInt32BigEndian(t, value);
            }
            else
            {
                BinaryPrimitives.WriteInt32LittleEndian(t, value);
            }
            
            Buffer.Write(t);
        }

        public void WriteUnsignedInt(uint value)
        {
            const int size = sizeof(uint);
            Span<byte> t = stackalloc byte[size];

            if (Endianness.DontFlipEndianness)
            {
                BinaryPrimitives.WriteUInt32BigEndian(t, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt32LittleEndian(t, value);
            }
            
            Buffer.Write(t);
        }

        public void WriteSignedIntLittleEndian(int value)
        {
            const int size = sizeof(int);
            Span<byte> t = stackalloc byte[size];

            if (Endianness.DontFlipEndianness)
            {
                BinaryPrimitives.WriteInt32LittleEndian(t, value);
            }
            else
            {
                BinaryPrimitives.WriteInt32BigEndian(t, value);
            }

            Buffer.Write(t);
        }

        public void WriteUnsignedIntLittleEndian(uint value)
        {
            const int size = sizeof(uint);
            Span<byte> t = stackalloc byte[size];

            if (Endianness.DontFlipEndianness)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(t, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt32BigEndian(t, value);
            }

            Buffer.Write(t);
        }

        public void WriteSignedLong(long value)
        {
            const int size = sizeof(long);
            Span<byte> t = stackalloc byte[size];

            if (Endianness.DontFlipEndianness)
            {
                BinaryPrimitives.WriteInt64BigEndian(t, value);
            }
            else
            {
                BinaryPrimitives.WriteInt64LittleEndian(t, value);
            }

            Buffer.Write(t);
        }

        public void WriteUnsignedLong(ulong value)
        {
            const int size = sizeof(ulong);
            Span<byte> t = stackalloc byte[size];

            if (Endianness.DontFlipEndianness)
            {
                BinaryPrimitives.WriteUInt64BigEndian(t, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt64LittleEndian(t, value);
            }
            
            Buffer.Write(t);
        }
        public void WriteSignedLongLittleEndian(long value)
        {
            const int size = sizeof(long);
            Span<byte> t = stackalloc byte[size];

            if (Endianness.DontFlipEndianness)
            {
                BinaryPrimitives.WriteInt64LittleEndian(t, value);
            }
            else
            {
                BinaryPrimitives.WriteInt64BigEndian(t, value);
            }
            
            Buffer.Write(t);
        }

        public void WriteUnsignedLongLittleEndian(ulong value)
        {
            const int size = sizeof(ulong);
            Span<byte> t = stackalloc byte[size];

            if (Endianness.DontFlipEndianness)
            {
                BinaryPrimitives.WriteUInt64LittleEndian(t, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt64BigEndian(t, value);
            }

            Buffer.Write(t);
        }

        public void WriteTriad(int value)
        {
            WriteUnsignedByte((byte) value);
            WriteUnsignedByte((byte) (value >> 8));
            WriteUnsignedByte((byte) (value >> 16));
        }

        public void WriteTriadLittleEndian(int value)
        {
            
            WriteUnsignedByte((byte) (value >> 16)); // todo можно будет напрямую из структуры читать байты
            WriteUnsignedByte((byte) (value >> 8));
            WriteUnsignedByte((byte) value);
        }

        public void WriteUnsignedVarInt(uint value)
        {
            while ((value & 0xFFFFFF80) != 0) //todo может это можно заменить на >> 7 != 0
            {
                WriteUnsignedByte((byte) ((value & 0x7F) | 0x80));
                value >>= 7;
            }

            WriteUnsignedByte((byte) value);
        }

        public void WriteSignedVarInt(int value)
        {
            WriteUnsignedVarInt(EncodeZigzag32(value));
        }

        public void WriteUnsignedVarLong(ulong value)
        {
            while ((value & 0xFFFFFFFFFFFFFF80) != 0) //todo может это можно заменить на >> 7 != 0
            {
                WriteUnsignedByte((byte) ((value & 0x7F) | 0x80));
                value >>= 7;
            }

            WriteUnsignedByte((byte) value);
        }

        public void WriteSignedVarLong(long value)
        {
            WriteUnsignedVarLong(EncodeZigzag64(value));
        }

        public void WriteString(string value, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            WriteByteArray(encoding.GetBytes(value)); // todo Может есть получше способы?
        }

        public void WriteByteArray(ReadOnlyMemory<byte> value)
        {
            WriteUnsignedVarInt((uint)value.Length);
            Write(value);
        }

        public void WriteByteSizedString(string value, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            WriteByteSizedByteArray(encoding.GetBytes(value)); // todo Может есть получше способы?
        }

        public void WriteByteSizedByteArray(ReadOnlyMemory<byte> value)
        {
            if (value.Length > 0xFF)
            {
                throw new TooBigValueException();
            }
            WriteUnsignedByte((byte)value.Length);
            Write(value);
        }

        public void Write(ReadOnlyMemory<byte> value)
        {
            Buffer.Write(value.Span);
        }
        
        public void Write(ReadOnlySpan<byte> value)
        {
            Buffer.Write(value);
        }

        // Про Zigzag encoding: https://en.wikipedia.org/wiki/Variable-length_quantity
        // https://gist.github.com/mfuerstenau/ba870a29e16536fdbaba
        
        private static uint EncodeZigzag32(int n)
        {
            return (uint) ((n << 1) ^ (n >> 31));
        }

        private static ulong EncodeZigzag64(long n)
        {
            return (ulong) ((n << 1) ^ (n >> 63));
        }

        public override void Dispose()
        {
            base.Dispose();
            Buffer?.Dispose();
            Buffer = null;
        }
    }
    
}