using System;
using System.Buffers.Binary;

namespace InStory.binary.stream
{
    public class RStream
    {
        private ReadOnlyMemory<byte> _buffer;
        private int _offset;
        
        // Чтобы можно было получить только из пула
        internal RStream(){}

        public sbyte ReadSignedByte()
        {
            return (sbyte)_buffer.Span[_offset++];
        }
        
        public byte ReadUnsignedByte()
        {
            return _buffer.Span[_offset++];
        }

        public short ReadSignedShort()
        {
            const int size = sizeof(short);
            
            var r = BinaryPrimitives.ReadInt16BigEndian(_buffer.Span.Slice(_offset, size));
            _offset += size;
            return r;
        }

        public ushort ReadUnsignedShort()
        {
            const int size = sizeof(ushort);

            var r = BinaryPrimitives.ReadUInt16BigEndian(_buffer.Span.Slice(_offset, size));
            _offset += size;
            return r;
        }

        public short ReadSignedShortLittleEndian()
        {
            const int size = sizeof(short);
            
            var r = BinaryPrimitives.ReadInt16LittleEndian(_buffer.Span.Slice(_offset, size));
            _offset += size;
            return r;
        }

        public ushort ReadUnsignedShortLittleEndian()
        {
            const int size = sizeof(ushort);

            var r = BinaryPrimitives.ReadUInt16LittleEndian(_buffer.Span.Slice(_offset, size));
            _offset += size;
            return r;
        }

        public int ReadSignedInt()
        {
            const int size = sizeof(int);

            var r = BinaryPrimitives.ReadInt32BigEndian(_buffer.Span.Slice(_offset, size));
            _offset += size;
            return r;
        }

        public uint ReadUnsignedInt()
        {
            const int size = sizeof(uint);

            var r = BinaryPrimitives.ReadUInt32BigEndian(_buffer.Span.Slice(_offset, size));
            _offset += size;
            return r;
        }

        public int ReadSignedIntLittleEndian()
        {
            const int size = sizeof(int);

            var r = BinaryPrimitives.ReadInt32LittleEndian(_buffer.Span.Slice(_offset, size));
            _offset += size;
            return r;
        }

        public uint ReadUnsignedIntLittleEndian()
        {
            const int size = sizeof(uint);

            var r = BinaryPrimitives.ReadUInt32LittleEndian(_buffer.Span.Slice(_offset, size));
            _offset += size;
            return r;
        }

        public long ReadSignedLong()
        {
            const int size = sizeof(long);

            var r = BinaryPrimitives.ReadInt64BigEndian(_buffer.Span.Slice(_offset, size));
            _offset += size;
            return r;
        }

        public ulong ReadUSignedLong()
        {
            const int size = sizeof(ulong);

            var r = BinaryPrimitives.ReadUInt64BigEndian(_buffer.Span.Slice(_offset, size));
            _offset += size;
            return r;
        }

        public long ReadSignedLongLittleEndian()
        {
            const int size = sizeof(long);

            var r = BinaryPrimitives.ReadInt64LittleEndian(_buffer.Span.Slice(_offset, size));
            _offset += size;
            return r;
        }

        public ulong ReadUSignedLongLittleEndian()
        {
            const int size = sizeof(ulong);

            var r = BinaryPrimitives.ReadUInt64LittleEndian(_buffer.Span.Slice(_offset, size));
            _offset += size;
            return r;
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
            var r = _buffer.Slice(_offset, size);
            _offset += size;
            return r;
        }

        public void Load(ReadOnlyMemory<byte> buffer)
        {
            _buffer = buffer;
            _offset = 0;
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
    }
}