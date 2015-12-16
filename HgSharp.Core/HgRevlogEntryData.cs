// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HgSharp.Core
{
    [DebuggerDisplay("rev: {Entry.Revision}, offset: {Entry.Offset}, length: {Entry.CompressedLength}, base: {Entry.BaseRevision}, linkrev: {Entry.LinkRevision}, nodeid: {Entry.NodeID.Short,nq}")]
    public class HgRevlogEntryData
    {
        public HgRevlogEntry Entry { get; private set; }

        public byte[] Data { get; private set; }

        public HgRevlogEntryData(HgRevlogEntry entry, byte[] data)
        {
            Entry = entry;
            Data = data;
        }
    }
}
