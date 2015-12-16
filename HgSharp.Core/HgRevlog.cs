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

namespace HgSharp.Core
{
    public class HgRevlog
    {
        private readonly List<HgRevlogEntry> entries;
        private readonly ReadOnlyCollection<HgRevlogEntry> readOnlyEntries;
        private readonly IDictionary<HgNodeID, uint> nodeCache;

        internal string IndexPath { get; private set; }

        internal string DataPath { get; private set; }

        internal int Version { get; private set; }

        internal bool InlineData { get; set; }

        public virtual ReadOnlyCollection<HgRevlogEntry> Entries
        {
            get { return readOnlyEntries; }
        } 

        public virtual HgRevlogEntry this[uint revision]
        {
            get { return GetEntry(revision); }
        }

        /*public HgRevlogEntry this[HgNodeID hgNodeID]
        {
            get { return GetEntry(hgNodeID); }
        }*/

        public HgRevlog(string indexPath, string dataPath, int version, bool inlineData, IEnumerable<HgRevlogEntry> entries)
        {
            IndexPath = indexPath;
            DataPath = dataPath;
            Version = version;
            InlineData = inlineData;

            this.entries = new List<HgRevlogEntry>(entries);
            readOnlyEntries = new ReadOnlyCollection<HgRevlogEntry>(this.entries);

            nodeCache = this.entries.ToDictionary(e => e.NodeID, e => e.Revision);
            //revisionCache = this.entries.ToDictionary(e => e.Revision, e => e);
        }

        internal virtual void Add(HgRevlogEntry hgRevlogEntry)
        {
            entries.Add(hgRevlogEntry);
            nodeCache[hgRevlogEntry.NodeID] = hgRevlogEntry.Revision;

            //nodeCache[hgRevlogEntry.NodeID] = hgRevlogEntry;
            //revisionCache[hgRevlogEntry.Revision] = hgRevlogEntry;
        }

        public virtual HgRevlogEntry GetEntry(UInt32 revision)
        {
            return revision > entries.Count - 1 ?
                null:
                entries[(int)revision];
        }

        public virtual HgRevlogEntry GetEntry(HgNodeID nodeID)
        {
            uint revision = 0;
            if(nodeCache.TryGetValue(nodeID, out revision))
                return GetEntry(revision);

            if (nodeID.NodeID.Length == 20) return null;

            //
            // Nope, no match. Might be a shortened SHA, so we'll have to look through the whole revlog
            var node = entries.FirstOrDefault(e => e.NodeID.Equals(nodeID));
            return node;
        }

        /// <summary>
        /// Returns a range of <see cref="HgRevlogEntry"/> objects with <see cref="HgRevlogEntry.Revision"/> ranging
        /// from <paramref name="startRevision"/> to <paramref name="endRevision"/>, inclusive.
        /// </summary>
        /// <param name="startRevision"></param>
        /// <param name="endRevision"></param>
        /// <returns></returns>
        public virtual IEnumerable<HgRevlogEntry> GetEntries(UInt32 startRevision, UInt32 endRevision)
        {
            if(startRevision > endRevision) 
                throw new ArgumentException();

            var revisions = Entries.Where(e => e.Revision >= startRevision && e.Revision <= endRevision);
            return revisions;
        } 
    }
}
