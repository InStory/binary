using System.Runtime.CompilerServices;
using InStory.binary.pool;
using InStory.binary.stream;

namespace InStory.binary.packet
{
    public abstract class Packet<T> : PooledObject<T>, IPacket where T : NamedObject, IPacket, new()
    {
        public abstract byte GetId();

        protected abstract void Decode(RStream r);
        protected abstract void Encode(WStream w);

        public IPacket _GetInstance()
        {
            return Get();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DecodeTo(RStream r)
        {
            Decode(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EncodeTo(WStream w)
        {
            w.WriteByte(GetId());
            Encode(w);
        }

    }
    
}