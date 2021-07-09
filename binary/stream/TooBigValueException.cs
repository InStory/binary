using System;

namespace InStory.binary.stream
{
    public class TooBigValueException : Exception
    {
        public TooBigValueException() : base("Too big value"){}
    }
}