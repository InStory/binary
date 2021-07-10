using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Load()
        {
            if (Buffer != null)
            {
                throw new ReUseStreamException(); 
            }
            Buffer = Manager.GetStream();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadExact(Span<byte> buf)
        {
            var n = Buffer.Read(buf);
            if (n < buf.Length)
            {
                throw new NotEnoughBytesException();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSignedByte()
        {
            var value = Buffer.ReadByte();
            if (value == -1)
            {
                throw new NotEnoughBytesException();
            }
            
            return (sbyte)value;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadUnsignedByte()
        {
            var value = Buffer.ReadByte();
            if (value == -1)
            {
                throw new NotEnoughBytesException();
            }
            
            return (byte)value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte() => ReadUnsignedByte();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadSignedShort()
        {
            const int size = sizeof(short);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadInt16BigEndian(t) : BinaryPrimitives.ReadInt16LittleEndian(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUnsignedShort()
        {
            const int size = sizeof(ushort);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadUInt16BigEndian(t) : BinaryPrimitives.ReadUInt16LittleEndian(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadSignedShortLittleEndian()
        {
            const int size = sizeof(short);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadInt16LittleEndian(t) : BinaryPrimitives.ReadInt16BigEndian(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUnsignedShortLittleEndian()
        {
            const int size = sizeof(ushort);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadUInt16LittleEndian(t) : BinaryPrimitives.ReadUInt16BigEndian(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadSignedInt()
        {
            const int size = sizeof(int);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadInt32BigEndian(t) : BinaryPrimitives.ReadInt32LittleEndian(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUnsignedInt()
        {
            const int size = sizeof(uint);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadUInt32BigEndian(t) : BinaryPrimitives.ReadUInt32LittleEndian(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadSignedIntLittleEndian()
        {
            const int size = sizeof(int);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadInt32LittleEndian(t) : BinaryPrimitives.ReadInt32BigEndian(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUnsignedIntLittleEndian()
        {
            const int size = sizeof(uint);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadUInt32LittleEndian(t) : BinaryPrimitives.ReadUInt32BigEndian(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadSignedLong()
        {
            const int size = sizeof(long);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadInt64BigEndian(t) : BinaryPrimitives.ReadInt64LittleEndian(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUnsignedLong()
        {
            const int size = sizeof(ulong);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadUInt64BigEndian(t) : BinaryPrimitives.ReadUInt64LittleEndian(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadSignedLongLittleEndian()
        {
            const int size = sizeof(long);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadInt64LittleEndian(t) : BinaryPrimitives.ReadInt64BigEndian(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUnsignedLongLittleEndian()
        {
            const int size = sizeof(ulong);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return Endianness.DontFlipEndianness ? BinaryPrimitives.ReadUInt64LittleEndian(t) : BinaryPrimitives.ReadUInt64BigEndian(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadTriad()
        {
            const int size = 3;

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return t[0] | t[1] << 8 | t[2] << 16; // todo Можно потом напрямую в структуру читать через Marshal
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadTriadLittleEndian()
        {
            const int size = 3;
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return t[0] << 16 | t[1] << 8 | t[2];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadSignedVarInt()
        {
            return DecodeZigzag32(ReadUnsignedVarInt());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadSignedVarLong()
        {
            return DecodeZigzag64(ReadUnsignedVarLong());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString(Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetString(ReadByteArray().Span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyMemory<byte> ReadByteArray()
        {
            var count = ReadUnsignedVarInt();

            return Read((int)count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadByteArrayInto(MemoryStream stream)
        {
            var count = ReadUnsignedVarInt();

            if (stream.Capacity < stream.Position + count)
            {
                stream.Capacity = (int)(stream.Position + count); //todo Переделать, пока не знаю как это адекватно реализовать
            }
            Buffer.Read(stream.GetBuffer(), (int)stream.Position, (int)count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadByteSizedString(Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetString(ReadByteSizedByteArray().Span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyMemory<byte> ReadByteSizedByteArray()
        {
            var count = ReadUnsignedByte();

            return Read(count);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadByteSizedByteArrayInto(MemoryStream stream)
        {
            var count = ReadUnsignedByte();

            if (stream.Capacity < stream.Position + count)
            {
                stream.Capacity = (int)(stream.Position + count); 
            }
            Buffer.Read(stream.GetBuffer(), (int)stream.Position, (int)count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyMemory<byte> Read(int size)
        {
            var memory = new Memory<byte>(new byte[size]);
            ReadExact(memory.Span);
            
            return memory;
        }
        
        // About Zigzag encoding: https://en.wikipedia.org/wiki/Variable-length_quantity
        // https://gist.github.com/mfuerstenau/ba870a29e16536fdbaba
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int DecodeZigzag32(uint n)
        {
            return (int) (n >> 1) ^ - (int) (n & 1);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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