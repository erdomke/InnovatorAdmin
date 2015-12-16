// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.Collections.Generic;

namespace HgSharp.Core
{
    public class HgManifest : HgRevlogBasedStorage
    {
        private readonly HgEncoder hgEncoder;
        private readonly HgManifestReader hgManifestReader;

        public virtual HgManifestEntry this[HgNodeID hgNodeID]
        {
            get
            {
                var key = string.Format("manifest:{0}@{1}", Revlog.IndexPath, hgNodeID.Long);
                return ObjectCache.GetOrAdd(key,
                    () => {
                        var hgRevlogEntryData = GetRevlogEntryData(hgNodeID);
                        return 
                            hgRevlogEntryData == null ?
                                null :
                                hgManifestReader.ReadManifestEntry(hgRevlogEntryData);
                    });
            }
        }

        public virtual HgManifestEntry this[uint revision]
        {
            get
            {
                var key = string.Format("manifest:{0}@{1}", Revlog.IndexPath, revision);
                return ObjectCache.GetOrAdd(key, 
                    () => {
                        var hgRevlogEntryData = GetRevlogEntryData(revision);
                        return 
                            hgRevlogEntryData == null ?
                                null :
                                hgManifestReader.ReadManifestEntry(hgRevlogEntryData);
                    });
            }
        }

        public HgManifest(HgRevlog hgRevlog, HgRevlogReader revlogReader, HgEncoder hgEncoder) :
            base(hgRevlog, revlogReader)
        {
            this.hgEncoder = hgEncoder;
            hgManifestReader = new HgManifestReader(this.hgEncoder);
        }
    }
}