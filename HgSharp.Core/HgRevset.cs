// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HgSharp.Core
{
    [DebuggerDisplay("{Count}")]
    public class HgRevset : IEnumerable<HgRevsetEntry>
    {
        private readonly IDictionary<HgNodeID, uint> revset;
        private readonly IDictionary<uint, HgNodeID> nodeset;

        public HgNodeID? this[uint revision]
        {
            get { return nodeset.ContainsKey(revision) ? (HgNodeID?)nodeset[revision] : null; }
        }

        public int Count
        {
            get { return revset.Count; }
        }

        public IEnumerable<HgRevsetEntry> OldestToNewest
        {
            get { return revset.Select(r => new HgRevsetEntry(r.Value, r.Key)).Distinct().OrderBy(r => r.Revision); }
        }

        public IEnumerable<HgRevsetEntry> NewestToOldest
        {
            get { return revset.Select(r => new HgRevsetEntry(r.Value, r.Key)).Distinct().OrderByDescending(r => r.Revision); }
        }

        public bool Empty 
        { 
            get { return revset.Count == 0 && nodeset.Count == 0; }    
        }

        public HgRevset()
        {
            revset = new Dictionary<HgNodeID, uint>();
            nodeset = new Dictionary<uint, HgNodeID>();
        }

        internal HgRevset(IDictionary<HgNodeID, uint> revset, IDictionary<uint, HgNodeID> nodeset)
        {
            this.revset = revset;
            this.nodeset = nodeset;
        }

        public HgRevset(HgRevlogEntryMetadata hgRevlogEntryMetadata) :
            this()
        {
            Add(hgRevlogEntryMetadata.NodeID, hgRevlogEntryMetadata.Revision);
        }

        public HgRevset(IEnumerable<HgRevlogEntry> hgRevlogEntries) :
            this()
        {
            foreach(var hgRevlogEntry in hgRevlogEntries)
                Add(hgRevlogEntry.NodeID, hgRevlogEntry.Revision);
        }

        public HgRevset(IEnumerable<HgRevsetEntry> hgRevsetEntries) :
            this()
        {
            foreach(var hgRevsetEntry in hgRevsetEntries)
                Add(hgRevsetEntry.NodeID, hgRevsetEntry.Revision);
        }

        public HgRevset(IEnumerable<HgRevlogEntryMetadata> hgRevlogEntries) :
            this()
        {
            foreach(var hgRevlogEntry in hgRevlogEntries)
                Add(hgRevlogEntry.NodeID, hgRevlogEntry.Revision);
        }

        public bool Contains(uint revision)
        {
            return nodeset.ContainsKey(revision);
        }

        
        public bool Contains(HgNodeID hgNodeID)
        {
            return revset.ContainsKey(hgNodeID);
        }

        public bool Add(HgNodeID nodeID, uint revision)
        {
            if(nodeID == HgNodeID.Null) throw new ArgumentOutOfRangeException("nodeID");
            if(revision == uint.MaxValue) throw new ArgumentOutOfRangeException("revision");

            if(revset.ContainsKey(nodeID) && nodeset.ContainsKey(revision)) return false;

            revset[nodeID] = revision;
            nodeset[revision] = nodeID;

            return true;
        }

        public IEnumerator<HgRevsetEntry> GetEnumerator()
        {
            return revset.Select(r => new HgRevsetEntry(r.Value, r.Key)).Distinct().OrderBy(r => r.Revision).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public HgRevset IntersectedWith(HgRevset other)
        {
            var union = this.Intersect(other);
            return new HgRevset(union);
        }

        public static HgRevset operator -(HgRevset l, HgRevset r)
        {
            var entries = l.Except(r);
            return new HgRevset(entries);
        }

        public bool Remove(HgNodeID nodeID)
        {
            if(!revset.ContainsKey(nodeID)) return false;
            
            var revision = revset[nodeID];

            nodeset.Remove(revision);
            revset.Remove(nodeID);

            return true;
        }
    }
}