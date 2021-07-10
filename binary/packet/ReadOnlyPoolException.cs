using System;

namespace InStory.binary.packet
{
    public class ReadOnlyPoolException : Exception
    {
        
        public ReadOnlyPoolException() : base("This pool now is readonly"){}
        
    }
}