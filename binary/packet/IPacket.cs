using InStory.binary.stream;

namespace InStory.binary.packet
{
    public interface IPacket
    {
        public byte GetId();

        public void DecodeTo(RStream r);
        public void EncodeTo(WStream w);

        public IPacket _GetInstance();
    }

}