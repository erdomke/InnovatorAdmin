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
using System.Runtime.Caching;

using HgSharp.Core.Caching;

using NLog;

namespace HgSharp.Core
{
    public class HgRevlogBasedStorage
    {
        private static IObjectCache revlogEntryCache = new NullObjectCache();

        protected Logger log;

        private readonly HgRevlogReader revlogReader;

        public static IObjectCache ObjectCache
        {
            get { return revlogEntryCache; }
            set { revlogEntryCache = value; }
        }

        public virtual HgRevlog Revlog { get; private set; }

        public HgRevlogBasedStorage(HgRevlog revlog, HgRevlogReader revlogReader)
        {
            log = LogManager.GetLogger(GetType().FullName);

            Revlog = revlog;
            this.revlogReader = revlogReader;
        }
        
        protected virtual HgRevlogEntryData GetRevlogEntryData(HgNodeID hgNodeID)
        {
            if(hgNodeID == HgNodeID.Null) return null;

            var key = string.Format("revlog:{0}@{1}", Revlog.IndexPath, hgNodeID.Long);
            var hgRevlogEntryData = revlogEntryCache.GetOrAdd(key, () => revlogReader.ReadRevlogEntry(hgNodeID));

            return hgRevlogEntryData;
        }

        protected virtual HgRevlogEntryData GetRevlogEntryData(uint revision)
        {
            var key = string.Format("revlog:{0}@{1}", Revlog.IndexPath, revision);
            var hgRevlogEntryData = revlogEntryCache.GetOrAdd(key, () => revlogReader.ReadRevlogEntry(revision));
            return hgRevlogEntryData;
        }

        protected virtual IEnumerable<HgRevlogEntryData> GetRevlogEntriesData(UInt32 startRevision, UInt32 endRevision)
        {
            var hgRevlogEntryData = revlogReader.ReadRevlogEntries(startRevision, endRevision);
            return hgRevlogEntryData;
        }

        protected virtual IEnumerable<HgRevlogEntryData> GetRevlogEntriesData(IEnumerable<uint> revisions)
        {
            var hgRevlogEntryData = revlogReader.ReadRevlogEntries(revisions);
            return hgRevlogEntryData;
        }
    }
}