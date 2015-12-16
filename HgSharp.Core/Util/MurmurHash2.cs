using System;

namespace HgSharp.Core.Util
{
    /// <summary>
    /// See http://landman-code.blogspot.ru/2009/02/c-superfasthash-and-murmurhash2.html
    /// </summary>
    internal class MurmurHash2
    {
        public UInt32 Hash(Byte[] data)
        {
            return Hash(data, 0xc58f1a7b, 0, data.Length);
        }
        const UInt32 m = 0x5bd1e995;
        const Int32 r = 24;

        public UInt32 Hash(Byte[] data, int length, int currentIndex)
        {
            return Hash(data, 0xc58f1a7b, length, currentIndex);
        }

        public UInt32 Hash(Byte[] data, UInt32 seed, int length, int currentIndex)
        {
            //Int32 length = data.Length;
            if(length == 0) return 0;
            var h = seed ^ (UInt32)length;

            //Int32 currentIndex = 0;
            unchecked
            {
                while(length >= 4)
                {
                    var k =
                        (UInt32)
                            (data[currentIndex++] | data[currentIndex++] << 8 | data[currentIndex++] << 16 |
                             data[currentIndex++] << 24);
                    k *= m;
                    k ^= k >> r;
                    k *= m;

                    h *= m;
                    h ^= k;
                    length -= 4;
                }
                switch(length)
                {
                    case 3:
                        h ^= (UInt16) (data[currentIndex++] | data[currentIndex++] << 8);
                        h ^= (UInt32) (data[currentIndex] << 16);
                        h *= m;
                        break;
                    case 2:
                        h ^= (UInt16) (data[currentIndex++] | data[currentIndex] << 8);
                        h *= m;
                        break;
                    case 1:
                        h ^= data[currentIndex];
                        h *= m;
                        break;
                    default:
                        break;
                }

                // Do a few final mixes of the hash to ensure the last few
                // bytes are well-incorporated.

                h ^= h >> 13;
                h *= m;
                h ^= h >> 15;

                return h;
            }
        }
    }
}