using System;

namespace InStory.binary.packet
{
    public class PacketAlreadyAddedException : Exception
    {
        
        public PacketAlreadyAddedException(byte packetId): base("Packet with id " + packetId + " already added to the pool"){}
        public PacketAlreadyAddedException(Type packetType): base("Packet with type " + packetType + " already added to the pool"){}
        
    }
}