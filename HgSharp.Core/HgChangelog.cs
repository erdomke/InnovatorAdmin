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
using System.Linq;

namespace HgSharp.Core
{
    public class HgChangelog : HgRevlogBasedStorage
    {
        private readonly HgEncoder hgEncoder;
        private readonly HgChangelogReader hgChangelogReader;

        public HgChangeset this[HgNodeID hgNodeID]
        {
            get
            {
                if(hgNodeID == HgNodeID.Null)
                    return HgChangeset.Null;

                var key = string.Format("changelog:{0}@{1}", Revlog.IndexPath, hgNodeID.Long);
                return ObjectCache.GetOrAdd(key,
                    () => {
                        var hgRevlogEntryData = GetRevlogEntryData(hgNodeID);
                        return
                            hgRevlogEntryData == null
                                ? null
                                : hgChangelogReader.ReadChangeset(hgRevlogEntryData);
                    });
            }
        }

        public HgChangeset this[uint revision]
        {
            get
            {
                var key = string.Format("changelog:{0}@{1}", Revlog.IndexPath, revision);
                return ObjectCache.GetOrAdd(key,
                    () => {
                        var hgRevlogEntryData = GetRevlogEntryData(revision);
                        return 
                            hgRevlogEntryData == null ?
                                null :
                                hgChangelogReader.ReadChangeset(hgRevlogEntryData);
                    });
            }
        }

        public HgChangeset Tip
        {
            get
            {
                return Revlog.Entries.Count == 0 ? 
                    null : 
                    this[Revlog.Entries.Last().Revision];
            }
        }

        public HgChangelog(HgRevlog revlog, HgRevlogReader revlogReader, HgEncoder hgEncoder) :
            base(revlog, revlogReader)
        {
            this.hgEncoder = hgEncoder;
            hgChangelogReader = new HgChangelogReader(this.hgEncoder);
        }

        public IEnumerable<HgChangeset> ReadChangesets(UInt32 startRevision, UInt32 endRevision)
        {
            return GetRevlogEntriesData(startRevision, endRevision).Select(re => hgChangelogReader.ReadChangeset(re));
        }

        public IEnumerable<HgChangeset> ReadChangesets(IEnumerable<uint> revisions)
        {
            return GetRevlogEntriesData(revisions).Select(re => hgChangelogReader.ReadChangeset(re));
        }
    }
}