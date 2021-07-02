using System;

namespace InStory.binary.stream
{
    public class ReUseStreamException : Exception
    {

        public ReUseStreamException() : base("Trying to reuse not disposed Stream"){}

    }
}