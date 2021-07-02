using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using Microsoft.IO;

namespace InStory.binary.stream
{
    public class WStream : IDisposable
    {

        private static readonly RecyclableMemoryStreamManager Manager = new();
        private MemoryStream _buffer;
        
        // Чтобы можно было получить только из пула
        internal WStream(){}

        public void Load()
        {
            if (_buffer != null)
            {
                throw new ReUseStreamException(); 
            }
            _buffer = Manager.GetStream();
        }

        public void WriteSignedByte(sbyte value)
        {
            _buffer.WriteByte((byte)value);
        }

        public void WriteUnsignedByte(byte value)
        {
            _buffer.WriteByte(value);
        }

        public void WriteSignedShort(short value)
        {
            // Я бы это не дублировал, но https://stackoverflow.com/questions/53155438/why-readonlyspan-may-not-be-used-as-a-type-argument-for-generic-delegates-and-ge
            const int size = sizeof(short);
            Span<byte> temp = stackalloc byte[size];
            
            BinaryPrimitives.WriteInt16BigEndian(temp, value);
            
            _buffer.Write(temp);
        }

        public void WriteUnsignedShort(ushort value)
        {
            const int size = sizeof(ushort);
            Span<byte> temp = stackalloc byte[size];
            
            BinaryPrimitives.WriteUInt16BigEndian(temp, value);
            
            _buffer.Write(temp);
        }

        public void WriteSignedShortLittleEndian(short value)
        {
            const int size = sizeof(short);
            Span<byte> temp = stackalloc byte[size];
            
            BinaryPrimitives.WriteInt16LittleEndian(temp, value);
            
            _buffer.Write(temp);
        }

        public void WriteUnsignedShortLittleEndian(ushort value)
        {
            const int size = sizeof(ushort);
            Span<byte> temp = stackalloc byte[size];
            
            BinaryPrimitives.WriteUInt16LittleEndian(temp, value);
            
            _buffer.Write(temp);
        }

        public void WriteSignedInt(int value)
        {
            const int size = sizeof(int);
            Span<byte> temp = stackalloc byte[size];
            
            BinaryPrimitives.WriteInt32BigEndian(temp, value);
            
            _buffer.Write(temp);
        }

        public void WriteUnsignedInt(uint value)
        {
            const int size = sizeof(uint);
            Span<byte> temp = stackalloc byte[size];
            
            BinaryPrimitives.WriteUInt32BigEndian(temp, value);
            
            _buffer.Write(temp);
        }

        public void WriteSignedIntLittleEndian(int value)
        {
            const int size = sizeof(int);
            Span<byte> temp = stackalloc byte[size];
            
            BinaryPrimitives.WriteInt32LittleEndian(temp, value);
            
            _buffer.Write(temp);
        }

        public void WriteUnsignedIntLittleEndian(uint value)
        {
            const int size = sizeof(uint);
            Span<byte> temp = stackalloc byte[size];
            
            BinaryPrimitives.WriteUInt32LittleEndian(temp, value);
            
            _buffer.Write(temp);
        }

        public void WriteSignedLong(long value)
        {
            const int size = sizeof(long);
            Span<byte> temp = stackalloc byte[size];
            
            BinaryPrimitives.WriteInt64BigEndian(temp, value);
            
            _buffer.Write(temp);
        }

        public void WriteUnsignedLong(ulong value)
        {
            const int size = sizeof(ulong);
            Span<byte> temp = stackalloc byte[size];
            
            BinaryPrimitives.WriteUInt64BigEndian(temp, value);
            
            _buffer.Write(temp);
        }
        public void WriteSignedLongLittleEndian(long value)
        {
            const int size = sizeof(long);
            Span<byte> temp = stackalloc byte[size];
            
            BinaryPrimitives.WriteInt64LittleEndian(temp, value);
            
            _buffer.Write(temp);
        }

        public void WriteUnsignedLongLittleEndian(ulong value)
        {
            const int size = sizeof(ulong);
            Span<byte> temp = stackalloc byte[size];
            
            BinaryPrimitives.WriteUInt64LittleEndian(temp, value);
            
            _buffer.Write(temp);
        }

        public void WriteTriad(int value)
        {
            WriteUnsignedByte((byte) value);
            WriteUnsignedByte((byte) (value >> 8));
            WriteUnsignedByte((byte) (value >> 16));
        }

        public void WriteTriadLittleEndian(int value)
        {
            
            WriteUnsignedByte((byte) (value >> 16));
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

        public void WriteString(string value)
        {
             WriteByteArray(Encoding.ASCII.GetBytes(value)); // todo Может есть получше способы?
        }

        public void WriteByteArray(ReadOnlyMemory<byte> value)
        {
            WriteUnsignedVarInt((uint)value.Length);
            Write(value);
        }

        public void WriteByteSizedString(string value)
        {
            WriteByteSizedByteArray(Encoding.ASCII.GetBytes(value)); // todo Может есть получше способы?
        }

        public void WriteByteSizedByteArray(ReadOnlyMemory<byte> value)
        {
            if (value.Length > 0xFF)
            {
                throw new Exception("Too big value (" + value.Length + ")"); // todo заменить кастомным эксепшеном 
            }
            WriteUnsignedByte((byte)value.Length);
            Write(value);
        }

        public void Write(ReadOnlyMemory<byte> value)
        {
            _buffer.Write(value.Span);
        }
        
        public void Write(ReadOnlySpan<byte> value)
        {
            _buffer.Write(value);
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

        public void Dispose()
        {
            _buffer?.Dispose();
            _buffer = null;
        }
    }
    
}