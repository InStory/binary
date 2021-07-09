using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
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
        public void WriteBuffer(byte[] buf)
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

            return (short)(t[0] | (t[1] << 8));
        }

        public ushort ReadUnsignedShort()
        {
            const int size = sizeof(ushort);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return (ushort)(t[0] | t[1] << 8);
        }

        public short ReadSignedShortLittleEndian()
        {
            const int size = sizeof(short);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return (short)(t[0] << 8 | t[1]);
        }

        public ushort ReadUnsignedShortLittleEndian()
        {
            const int size = sizeof(ushort);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return (ushort)(t[0] << 8 | t[1]);
        }

        public int ReadSignedInt()
        {
            const int size = sizeof(int);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return t[0] | t[1] << 8 | t[2] << 16 | t[3] << 24;
        }

        public uint ReadUnsignedInt()
        {
            const int size = sizeof(uint);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return (uint)(t[0] | t[1] << 8 | t[2] << 16 | t[3] << 24);
        }

        public int ReadSignedIntLittleEndian()
        {
            const int size = sizeof(int);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return t[0] << 24 | t[1] << 16 | t[2] << 8 | t[3];
        }

        public uint ReadUnsignedIntLittleEndian()
        {
            const int size = sizeof(uint);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return (uint)(t[0] << 24 | t[1] << 16 | t[2] << 8 | t[3]);
        }

        public long ReadSignedLong()
        {
            const int size = sizeof(long);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return (uint)(t[0] | t[1] << 8 | t[2] << 16 | t[3] << 24) | (long)t[4] << 32 | (long)t[5] << 40 | (long)t[6] << 48 | (long)t[7] << 56;
        }

        public ulong ReadUnsignedLong()
        {
            const int size = sizeof(ulong);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);
            
            return (uint)(t[0] | t[1] << 8 | t[2] << 16 | t[3] << 24) | (ulong)t[4] << 32 | (ulong)t[5] << 40 | (ulong)t[6] << 48 | (ulong)t[7] << 56;
        }

        public long ReadSignedLongLittleEndian()
        {
            const int size = sizeof(long);

            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return (long)t[0] << 56 | (long)t[1] << 48 | (long)t[2] << 40 | (long)t[3] << 32 | ((uint)(t[4] << 24) | (uint)(t[5] << 16) | (uint)(t[6] << 8) | t[7]);
        }

        public ulong ReadUnsignedLongLittleEndian()
        {
            const int size = sizeof(ulong);
            
            Span<byte> t = stackalloc byte[size];
            ReadExact(t);

            return (ulong)t[0] << 56 | (ulong)t[1] << 48 | (ulong)t[2] << 40 | (ulong)t[3] << 32 | ((uint)(t[4] << 24) | (uint)(t[5] << 16) | (uint)(t[6] << 8) | t[7]);
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