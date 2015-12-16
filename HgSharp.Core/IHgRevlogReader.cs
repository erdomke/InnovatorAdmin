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

namespace HgSharp.Core
{
    public interface IHgRevlogReader
    {
        IEnumerable<HgRevlogEntryData> ReadRevlogEntries(HgRevset hgRevset);
        
        IEnumerable<HgRevlogEntryData> ReadRevlogEntries(IEnumerable<uint> revisions);

        IEnumerable<HgRevlogEntryData> ReadRevlogEntries(UInt32 startRevision, UInt32 endRevision);
        
        HgRevlogEntryData ReadRevlogEntry(HgNodeID nodeID);
    }
}