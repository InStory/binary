using System;
using System.Buffers.Binary;

namespace InStory.binary.other
{
    public class Endianness
    {

        public static bool DontFlipEndianness
        {
            get
            {
                if (!_loadedEndianness)
                { 
                    LoadEndianness();
                }

                return _dontFlipEndianness;
            }
        }

        private static bool _dontFlipEndianness;
        private static bool _loadedEndianness;

        /// <summary>
        /// On Mac OS ReadInt16LittleEndian working like ReadInt16BigEndian
        /// </summary>
        /// <exception cref="Exception"></exception>
        internal static void LoadEndianness()
        {
            var r = BinaryPrimitives.ReadInt16LittleEndian(stackalloc byte[] { 0xFF, 0xFE });
            _dontFlipEndianness = r switch
            {
                -2 => true,
                -257 => false,
                _ => throw new Exception("Unknown value got while calculating endianness: " + r)
            };
            _loadedEndianness = true;
        }

    }
}