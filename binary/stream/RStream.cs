using System;
using System.Buffers.Binary;
using System.IO;
using Microsoft.IO;

namespace InStory.binary.stream
{
    public class RStream : IDisposable
    {
        private static readonly RecyclableMemoryStreamManager Manager = new();

        public MemoryStream Buffer
        {
            get;
            private set;
        }
        
        // Чтобы можно было получить только из пула
        internal RStream(){}

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

        public short ReadSignedShort()
        {
            const int size = sizeof(short);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return BinaryPrimitives.ReadInt16BigEndian(t);
        }

        public ushort ReadUnsignedShort()
        {
            const int size = sizeof(ushort);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return BinaryPrimitives.ReadUInt16BigEndian(t);
        }

        public short ReadSignedShortLittleEndian()
        {
            const int size = sizeof(short);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return BinaryPrimitives.ReadInt16LittleEndian(t);
        }

        public ushort ReadUnsignedShortLittleEndian()
        {
            const int size = sizeof(ushort);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return BinaryPrimitives.ReadUInt16LittleEndian(t);
        }

        public int ReadSignedInt()
        {
            const int size = sizeof(int);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return BinaryPrimitives.ReadInt32BigEndian(t);
        }

        public uint ReadUnsignedInt()
        {
            const int size = sizeof(uint);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return BinaryPrimitives.ReadUInt32BigEndian(t);
        }

        public int ReadSignedIntLittleEndian()
        {
            const int size = sizeof(int);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return BinaryPrimitives.ReadInt32LittleEndian(t);
        }

        public uint ReadUnsignedIntLittleEndian()
        {
            const int size = sizeof(uint);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return BinaryPrimitives.ReadUInt32LittleEndian(t);
        }

        public long ReadSignedLong()
        {
            const int size = sizeof(long);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return BinaryPrimitives.ReadInt64BigEndian(t);
        }

        public ulong ReadUSignedLong()
        {
            const int size = sizeof(ulong);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return BinaryPrimitives.ReadUInt64BigEndian(t);
        }

        public long ReadSignedLongLittleEndian()
        {
            const int size = sizeof(long);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return BinaryPrimitives.ReadInt64LittleEndian(t);
        }

        public ulong ReadUSignedLongLittleEndian()
        {
            const int size = sizeof(ulong);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return BinaryPrimitives.ReadUInt64LittleEndian(t);
        }

        public int ReadTriad()
        {
            const int size = 3;
            
            var data = Read(size).Span;
            return data[0] | data[1] << 8 | data[2] << 16;
        }

        public int ReadTriadLittleEndian()
        {
            const int size = 3;
            
            var data = Read(size).Span;
            return data[0] << 16 | data[1] << 8 | data[2];
        }

        public uint ReadUnsignedVarInt()
        {
            uint val = 0;
            for (var i = 0; i <= 28; i += 7)
            {
                var b = ReadUnsignedByte();
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

        public string ReadString()
        {
            return ReadByteArray().ToString();
        }

        public ReadOnlyMemory<byte> ReadByteArray()
        {
            var count = ReadUnsignedVarInt();

            return Read((int)count);
        }
        
        public string ReadByteSizedString()
        {
            return ReadByteSizedByteArray().ToString();
        }

        public ReadOnlyMemory<byte> ReadByteSizedByteArray()
        {
            var count = ReadUnsignedByte();

            return Read(count);
        }

        public ReadOnlyMemory<byte> Read(int size)
        {
            var memory = new Memory<byte>();
            ReadExact(memory.Span);
            
            return memory;
        }

        public void Load()
        {
            Buffer = Manager.GetStream();
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

        public void Dispose()
        {
            Buffer?.Dispose();
            Buffer = null;
        }
    }
}