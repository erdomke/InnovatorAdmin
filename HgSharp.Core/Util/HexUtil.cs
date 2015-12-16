namespace HgSharp.Core.Util
{
    /// <summary>
    /// An unholy mess of code from http://stackoverflow.com/a/24343727/60188 and someplace else
    /// </summary>
    internal static class HexUtil
    {
        private static readonly uint[] Lookup32 = CreateLookup32();

        public static byte[] FromHex(string hex)
        {
            var l = hex.Length >> 1;

            var ret = new byte[l];
            unchecked
            {
                for(var i = 0; i < l; ++i)
                    ret[i] = (byte)((ParseNybble(hex[i << 1]) << 4) + (ParseNybble(hex[(i << 1) + 1])));
            }

            return ret;
        }

        public static string ToHex(byte[] bytes)
        {
            var lookup32 = Lookup32;
            var result = new char[bytes.Length * 2];
            
            unchecked
            {
                for(var i = 0; i < bytes.Length; ++i)
                {
                    var val = lookup32[bytes[i]];
                
                    result[2 * i] = (char)val;
                    result[2 * i + 1] = (char)(val >> 16);
                }
            }
            
            return new string(result);
        }

        private static int ParseNybble(char c)
        {
            return c - (c < 58 ? 48 : (c < 97 ? 55 : 87));
        }

        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for(var i = 0; i < 256; i++)
            {
                var s = i.ToString("X2").ToLowerInvariant();
                result[i] = s[0] + ((uint) s[1] << 16);
            }

            return result;
        }
    }
}