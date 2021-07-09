using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using InStory.binary.other;
using InStory.binary.pool;
using Microsoft.IO;

namespace InStory.binary.stream
{
    public class RStream : PooledObject<RStream>, ILoadOnGet
    {
        private static readonly RecyclableMemoryStreamManager Manager = new();

        public MemoryStream Buffer
        {
            get;
            private set;
        }

        public void Load()
        {
            if (Buffer != null)
            {
                throw new ReUseStreamException(); 
            }
            Buffer = Manager.GetStream();
        }

        public void CleanBuffer()
        {
            Buffer?.Dispose();
            Buffer = Manager.GetStream();
        }

        /// <summary>
        /// FOR TESTING PURPOSES ONLY!!!
        /// </summary>
        /// <param name="buf"></param>
        public void SetBuffer(byte[] buf)
        {
            Buffer.Seek(0, SeekOrigin.Begin);
            Buffer.Write(buf);
            Buffer.Seek(0, SeekOrigin.Begin);
        }

        private void ReadExact(Span<byte> buf)
        {
            var n = Buffer.Read(buf);
            if (n < buf.Length)
            {
                throw new NotEnoughBytesException();
            }
        }
        
        public sbyte ReadSignedByte()
        {
            var value = Buffer.ReadByte();
            if (value == -1)
            {
                throw new NotEnoughBytesException();
            }
            
            return (sbyte)value;
        }
        
        public byte ReadUnsignedByte()
        {
            var value = Buffer.ReadByte();
            if (value == -1)
            {
                throw new NotEnoughBytesException();
            }
            
            return (byte)value;
        }

        public byte ReadByte() => ReadUnsignedByte();

        public short ReadSignedShort()
        {
            const int size = sizeof(short);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadInt16BigEndian(t) : BinaryPrimitives.ReadInt16LittleEndian(t);
        }

        public ushort ReadUnsignedShort()
        {
            const int size = sizeof(ushort);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadUInt16BigEndian(t) : BinaryPrimitives.ReadUInt16LittleEndian(t);
        }

        public short ReadSignedShortLittleEndian()
        {
            const int size = sizeof(short);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadInt16LittleEndian(t) : BinaryPrimitives.ReadInt16BigEndian(t);
        }

        public ushort ReadUnsignedShortLittleEndian()
        {
            const int size = sizeof(ushort);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadUInt16LittleEndian(t) : BinaryPrimitives.ReadUInt16BigEndian(t);
        }

        public int ReadSignedInt()
        {
            const int size = sizeof(int);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadInt32BigEndian(t) : BinaryPrimitives.ReadInt32LittleEndian(t);
        }

        public uint ReadUnsignedInt()
        {
            const int size = sizeof(uint);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadUInt32BigEndian(t) : BinaryPrimitives.ReadUInt32LittleEndian(t);
        }

        public int ReadSignedIntLittleEndian()
        {
            const int size = sizeof(int);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadInt32LittleEndian(t) : BinaryPrimitives.ReadInt32BigEndian(t);
        }

        public uint ReadUnsignedIntLittleEndian()
        {
            const int size = sizeof(uint);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadUInt32LittleEndian(t) : BinaryPrimitives.ReadUInt32BigEndian(t);
        }

        public long ReadSignedLong()
        {
            const int size = sizeof(long);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadInt64BigEndian(t) : BinaryPrimitives.ReadInt64LittleEndian(t);
        }

        public ulong ReadUnsignedLong()
        {
            const int size = sizeof(ulong);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadUInt64BigEndian(t) : BinaryPrimitives.ReadUInt64LittleEndian(t);
        }

        public long ReadSignedLongLittleEndian()
        {
            const int size = sizeof(long);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadInt64LittleEndian(t) : BinaryPrimitives.ReadInt64BigEndian(t);
        }

        public ulong ReadUnsignedLongLittleEndian()
        {
            const int size = sizeof(ulong);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadUInt64LittleEndian(t) : BinaryPrimitives.ReadUInt64BigEndian(t);
        }

        public int ReadTriad()
        {
            const int size = 3;

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return t[0] | t[1] << 8 | t[2] << 16; // todo Можно потом напрямую в структуру читать, но тогда надо будет функцию объявить unsafe 
        }

        public int ReadTriadLittleEndian()
        {
            const int size = 3;
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return t[0] << 16 | t[1] << 8 | t[2];
        }

        public uint ReadUnsignedVarInt()
        {
            uint val = 0;
            for (var i = 0; i <= 28; i += 7)
            {
                var b = ReadByte();
                val |= ((uint)b & 0x7F) << i;
                if ((b & 0x80) == 0)
                {
                    return val;
                }
            }
            
            return val;
        }

        public int ReadSignedVarInt()
        {
            return DecodeZigzag32(ReadUnsignedVarInt());
        }

        public ulong ReadUnsignedVarLong()
        {
            ulong val = 0;
            for (var i = 0; i <= 63; i += 7)
            {
                var b = ReadUnsignedByte();
                val |= ((ulong)b & 0x7F) << i;
                if ((b & 0x80) == 0)
                {
                    return val;
                }
            }
            
            return val;
        }

        public long ReadSignedVarLong()
        {
            return DecodeZigzag64(ReadUnsignedVarLong());
        }

        public string ReadString(Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetString(ReadByteArray().Span);
        }

        public ReadOnlyMemory<byte> ReadByteArray()
        {
            var count = ReadUnsignedVarInt();

            return Read((int)count);
        }

        public void ReadByteArrayInto(MemoryStream stream)
        {
            var count = ReadUnsignedVarInt();

            if (stream.Capacity < stream.Position + count)
            {
                stream.Capacity = (int)(stream.Position + count); //todo Переделать, пока не знаю как это адекватно реализовать
            }
            Buffer.Read(stream.GetBuffer(), (int)stream.Position, (int)count);
        }

        public string ReadByteSizedString(Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetString(ReadByteSizedByteArray().Span);
        }

        public ReadOnlyMemory<byte> ReadByteSizedByteArray()
        {
            var count = ReadUnsignedByte();

            return Read(count);
        }

        public ReadOnlyMemory<byte> Read(int size)
        {
            var memory = new Memory<byte>(new byte[size]);
            ReadExact(memory.Span);
            
            return memory;
        }
        
        // Про Zigzag encoding: https://en.wikipedia.org/wiki/Variable-length_quantity
        // https://gist.github.com/mfuerstenau/ba870a29e16536fdbaba
        
        private static int DecodeZigzag32(uint n)
        {
            return (int) (n >> 1) ^ -(int) (n & 1);
        }
        
        private static long DecodeZigzag64(ulong n)
        {
            return (long) (n >> 1) ^ - (long) (n & 1);
        }

        public override void Dispose()
        {
            base.Dispose();
            Buffer?.Dispose();
            Buffer = null;
        }
    }
}