using System;

namespace InStory.binary.packet
{
    public class PacketNotFoundException : Exception
    {

        public PacketNotFoundException(byte packetId) : base("Packet with id " + packetId + " not found"){}

    }
}