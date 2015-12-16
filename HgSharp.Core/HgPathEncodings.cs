using System;

namespace HgSharp.Core
{
    [Flags]
    public enum HgPathEncodings
    {
        None = 0,

        FnCache = 1,

        DotEncode = 2
    }
}