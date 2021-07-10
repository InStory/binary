using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using InStory.binary.stream;

namespace InStory.binary.packet
{
    public class PacketPool
    {

        private ReadOnlyDictionary<byte, IPacket> _packets = null;
        private Dictionary<byte, IPacket> _tempPackets = new();

        public void Add<T>(T pk) where T : IPacket
        {
            if (_tempPackets == null)
            {
                throw new ReadOnlyPoolException();
            }

            if (!_tempPackets.TryAdd(pk.GetId(), pk))
            {
                throw new PacketAlreadyAddedException(pk.GetId());
            }
        }

        public void Start()
        {
            _packets = new ReadOnlyDictionary<byte, IPacket>(_tempPackets);
            
            _tempPackets = null;
        }

        public IPacket Get(RStream r)
        {
            if (_packets == null)
            {
                throw new PoolIsNotStartedException();
            }
            
            var packetId = r.ReadByte();

            if (!_packets.TryGetValue(packetId, out var pk))
            {
                throw new PacketNotFoundException(packetId);
            }

            return pk._GetInstance();
        }
        
    }
}