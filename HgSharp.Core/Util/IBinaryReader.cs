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
    public interface IBinaryReader
    {
        int Read();
        bool ReadBoolean();
        byte ReadByte();
        sbyte ReadSByte();
        short ReadInt16();
        ushort ReadUInt16();
        int ReadInt32();
        uint ReadUInt32();
        long ReadInt64();
        ulong ReadUInt64();
        float ReadSingle();
        double ReadDouble();
        decimal ReadDecimal();
        string ReadString();
        int Read(char[] buffer, int index, int count);
        int Read(byte[] buffer, int index, int count);
        byte[] ReadBytes(int count);
        Stream BaseStream { get; }
    }
}