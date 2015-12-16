// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.

using System;
using System.Linq;

namespace HgSharp.Core
{
    public class HgFile
    {
        public HgPath Path { get; private set; }

        public HgRevlogEntry Entry { get; private set; }

        public byte[] Data { get; private set; }

        public HgFileCopyInfo CopyInfo { get; private set; }

        public bool IsBinary
        {
            get { return Data == null || Data.Length == 0 ? false : Array.IndexOf(Data, (byte)0) > -1; }
        }

        public HgFile(HgPath path, HgRevlogEntry entry, byte[] data, HgFileCopyInfo copyInfo)
        {
            Path = path;
            Entry = entry;
            Data = data;
            CopyInfo = copyInfo;
        }
    }
}