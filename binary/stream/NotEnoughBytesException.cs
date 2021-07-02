using System;

namespace InStory.binary.stream
{
    public class NotEnoughBytesException : Exception
    {
        public NotEnoughBytesException(int need, int got) : base("Not enough bytes. Need " + need + ", got " + got){}
        public NotEnoughBytesException() : base("Not enough bytes"){}
        
    }
}