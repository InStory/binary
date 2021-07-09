using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
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

            t[0] = (byte)value;
            t[1] = (byte)(value >> 8);

            Buffer.Write(t);
        }

        public void WriteUnsignedShort(ushort value)
        {
            const int size = sizeof(ushort);
            Span<byte> t = stackalloc byte[size];

            t[0] = (byte)value;
            t[1] = (byte)(value >> 8);
            
            Buffer.Write(t);
        }

        public void WriteSignedShortLittleEndian(short value)
        {
            const int size = sizeof(short);
            Span<byte> t = stackalloc byte[size];
            
            t[0] = (byte)(value >> 8);
            t[1] = (byte)value;

            Buffer.Write(t);
        }

        public void WriteUnsignedShortLittleEndian(ushort value)
        {
            const int size = sizeof(ushort);
            Span<byte> t = stackalloc byte[size];

            t[0] = (byte)(value >> 8);
            t[1] = (byte)value;
            
            Buffer.Write(t);
        }

        public void WriteSignedInt(int value)
        {
            const int size = sizeof(int);
            Span<byte> t = stackalloc byte[size];

            t[0] = (byte)value;
            t[1] = (byte)(value >> 8);
            t[2] = (byte)(value >> 16);
            t[3] = (byte)(value >> 24);
            
            Buffer.Write(t);
        }

        public void WriteUnsignedInt(uint value)
        {
            const int size = sizeof(uint);
            Span<byte> t = stackalloc byte[size];

            t[0] = (byte)value;
            t[1] = (byte)(value >> 8);
            t[2] = (byte)(value >> 16);
            t[3] = (byte)(value >> 24);

            Buffer.Write(t);
        }

        public void WriteSignedIntLittleEndian(int value)
        {
            const int size = sizeof(int);
            Span<byte> t = stackalloc byte[size];

            t[0] = (byte)(value >> 24);
            t[1] = (byte)(value >> 16);
            t[2] = (byte)(value >> 8);
            t[3] = (byte)value;

            Buffer.Write(t);
        }

        public void WriteUnsignedIntLittleEndian(uint value)
        {
            const int size = sizeof(uint);
            Span<byte> t = stackalloc byte[size];

            t[0] = (byte)(value >> 24);
            t[1] = (byte)(value >> 16);
            t[2] = (byte)(value >> 8);
            t[3] = (byte)value;

            Buffer.Write(t);
        }

        public void WriteSignedLong(long value)
        {
            const int size = sizeof(long);
            Span<byte> t = stackalloc byte[size];

            t[0] = (byte)value;
            t[1] = (byte)(value >> 8);
            t[2] = (byte)(value >> 16);
            t[3] = (byte)(value >> 24);
            t[4] = (byte)(value >> 32);
            t[5] = (byte)(value >> 40);
            t[6] = (byte)(value >> 48);
            t[7] = (byte)(value >> 56);

            Buffer.Write(t);
        }

        public void WriteUnsignedLong(ulong value)
        {
            const int size = sizeof(ulong);
            Span<byte> t = stackalloc byte[size];
            
            t[0] = (byte)value;
            t[1] = (byte)(value >> 8);
            t[2] = (byte)(value >> 16);
            t[3] = (byte)(value >> 24);
            t[4] = (byte)(value >> 32);
            t[5] = (byte)(value >> 40);
            t[6] = (byte)(value >> 48);
            t[7] = (byte)(value >> 56);
            
            Buffer.Write(t);
        }
        public void WriteSignedLongLittleEndian(long value)
        {
            const int size = sizeof(long);
            Span<byte> t = stackalloc byte[size];

            t[0] = (byte)(value >> 56);
            t[1] = (byte)(value >> 48);
            t[2] = (byte)(value >> 40);
            t[3] = (byte)(value >> 32);
            t[4] = (byte)(value >> 24);
            t[5] = (byte)(value >> 16);
            t[6] = (byte)(value >> 8);
            t[7] = (byte)value;
            
            Buffer.Write(t);
        }

        public void WriteUnsignedLongLittleEndian(ulong value)
        {
            const int size = sizeof(ulong);
            Span<byte> t = stackalloc byte[size];

            t[0] = (byte)(value >> 56);
            t[1] = (byte)(value >> 48);
            t[2] = (byte)(value >> 40);
            t[3] = (byte)(value >> 32);
            t[4] = (byte)(value >> 24);
            t[5] = (byte)(value >> 16);
            t[6] = (byte)(value >> 8);
            t[7] = (byte)value;

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