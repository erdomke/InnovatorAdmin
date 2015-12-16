// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.IO;

namespace HgSharp.Core.Util
{
    public interface IBinaryWriter
    {
        void Write(bool value);
        void Write(byte value);
        void Write(sbyte value);
        void Write(byte[] buffer);
        void Write(byte[] buffer, int index, int count);
        void Write(char ch);
        void Write(char[] chars);
        void Write(double value);
        void Write(decimal value);
        void Write(short value);
        void Write(ushort value);
        void Write(int value);
        void Write(uint value);
        void Write(long value);
        void Write(ulong value);
        void Write(float value);
        void Write(string value);
        Stream BaseStream { get; }
    }
}