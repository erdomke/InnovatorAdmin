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
using System.Collections.ObjectModel;

using System.Linq;

using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public class HgManifestEntry
    {
        private IDictionary<HgPath, HgManifestFileEntry> filesCache = new Dictionary<HgPath, HgManifestFileEntry>(); 

        public HgRevlogEntryMetadata Metadata { get; private set; }

        public ReadOnlyCollection<HgManifestFileEntry> Files { get; private set; }

        public HgManifestFileEntry this[string path]
        {
            get { return GetFile(new HgPath(path)); }
        }

        public HgManifestEntry(HgRevlogEntryMetadata metadata, IList<HgManifestFileEntry> files)
        {
            Metadata = metadata;
            Files = new ReadOnlyCollection<HgManifestFileEntry>(new List<HgManifestFileEntry>(files));

            filesCache.AddRange(files.Select(f => new KeyValuePair<HgPath, HgManifestFileEntry>(f.Path, f)));
        }

        public HgManifestFileEntry GetFile(HgPath path)
        {
            var file = 
                !string.IsNullOrWhiteSpace(path.FileName) && filesCache.ContainsKey(path) ? 
                    filesCache[path] :
                    null;
            
            return file;
        }
    }
}