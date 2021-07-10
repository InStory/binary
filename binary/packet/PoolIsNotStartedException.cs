using System;

namespace InStory.binary.packet
{
    public class PoolIsNotStartedException : Exception
    {
        
        public PoolIsNotStartedException(): base("This pool is not started yet"){}
        
    }
}