// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.IO;
using System.Text;
using HgSharp.Core.Util.Conversion;
using HgSharp.Core.Util.IO;

namespace HgSharp.Core.Util
{
    public class BigEndianBinaryWriter : EndianBinaryWriter, IBinaryWriter
    {
        public BigEndianBinaryWriter(Stream stream) : 
            base(EndianBitConverter.Big, stream)
        {
        }

        public BigEndianBinaryWriter(Stream stream, Encoding encoding) : 
            base(EndianBitConverter.Big, stream, encoding)
        {
        }
    }
}
