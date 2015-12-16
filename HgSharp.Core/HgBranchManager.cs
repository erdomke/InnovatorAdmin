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
using System.IO;

using HgSharp.Core.Util;

using System.Linq;

namespace HgSharp.Core
{
    public class HgBranchManager
    {
        private readonly HgRepository repository;
        private readonly HgFileSystem fileSystem;

        private HgNodeID cachedTipNodeID = HgNodeID.Null;
        private uint cachedTipRevision = uint.MaxValue;
        private readonly IDictionary<string, ISet<Tuple<HgNodeID, uint>>> branchmap; 

        public HgBranchManager(HgRepository repository, HgFileSystem fileSystem)
        {
            this.repository = repository;
            this.fileSystem = fileSystem;

            branchmap = new Dictionary<string, ISet<Tuple<HgNodeID, uint>>>().DefaultableWith(k => new SortedSet<Tuple<HgNodeID, uint>>(new HgBranchManagerComparer()));
        }

        public IList<HgBranchHeads> GetBranchmap()
        {
            return InternalGetBranchmap();
        }

        private IList<HgBranchHeads> InternalGetBranchmap()
        {
            if(repository.Changelog.Tip == null) return new List<HgBranchHeads>();

            ReadBranchmapCache();

            var tip = repository.Changelog.Tip.Metadata;
            
            if(cachedTipNodeID != tip.NodeID || cachedTipRevision != tip.Revision)
            {
                RefreshBranchmapInternal(0, tip);
                WriteBranchmapCache();
            } // if

            return
                branchmap.Select(
                    b =>
                    new HgBranchHeads(b.Key, new HgRevset(b.Value.Select(c => new HgRevsetEntry(c.Item2, c.Item1))))).ToList();
        }

        public void RefreshBranchmap(uint startRevision, HgRevlogEntryMetadata endRevision)
        {
            ReadBranchmapCache();
            RefreshBranchmapInternal(startRevision, endRevision);
            WriteBranchmapCache();
        }

        private void WriteBranchmapCache()
        {
            var branchheadsPath = Path.Combine(repository.BasePath, Path.Combine("cache", "branchheads"));
            using(var branchheadsStream = fileSystem.CreateWrite(branchheadsPath))
            using(var streamWriter = new StreamWriter(branchheadsStream))
            {
                streamWriter.Write("{0} {1}\n", cachedTipNodeID.Long, cachedTipRevision);

                foreach(var branch in branchmap.SelectMany(GetFlattenedBranchmap).OrderByDescending(b => b.Item2))
                {
                    var name = branch.Item1;
                    var node = branch.Item3.Long;

                    Debug.Assert(node.Length == 40);

                    streamWriter.Write("{0} {1}\n", node, name);
                } // foreach
            } // using
        }

        private IEnumerable<Tuple<string, uint, HgNodeID>> GetFlattenedBranchmap(KeyValuePair<string, ISet<Tuple<HgNodeID, uint>>> branch)
        {
            return branch.Value.Select(b => Tuple.Create(branch.Key, b.Item2, b.Item1));
        }

        private void ReadBranchmapCache()
        {
            var branchheadsPath = Path.Combine(repository.BasePath, Path.Combine("cache", "branchheads"));
            if(!Alphaleonis.Win32.Filesystem.File.Exists(branchheadsPath)) return;

            using(var branchheadsStream = fileSystem.OpenRead(branchheadsPath))
            using(var streamReader = new StreamReader(branchheadsStream))
            {
                var lines = streamReader.ReadToEnd().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if(lines.Length == 0) return;

                var node = lines[0].SubstringBefore(" ");
                var revision = lines[0].SubstringAfter(" ");

                Debug.Assert(node.Length == 40);

                cachedTipNodeID = new HgNodeID(node);
                cachedTipRevision = uint.Parse(revision);

                branchmap.Clear();
                var branches = 
                    lines.
                        Skip(1).
                        GroupBy(l => l.SubstringAfter(" "), l => new HgNodeID(l.SubstringBefore(" ")));

                foreach(var branch in branches)
                {
                    var entries = 
                        branch.
                            Where(v => repository.Changelog.Revlog.GetEntry(v) != null).
                            Select(v => Tuple.Create(v, repository.Changelog.Revlog.GetEntry(v).Revision));

                    branchmap[branch.Key].AddRange(entries);
                } // foreach
            } // using
        }

        private void RefreshBranchmapInternal(uint startRevision, HgRevlogEntryMetadata tip)
        {
            var hgRevsetManager = new HgRevsetManager();
            var revset = hgRevsetManager.GetRevset(repository, startRevision, tip.Revision);
            var changesets = 
                repository.GetChangesets(revset).GroupBy(k => k.Branch.Name, v => v).ToDictionary(k => k.Key, v => new Queue<HgRevlogEntryMetadata>(v.Select(c => c.Metadata).OrderByDescending(c => c.Revision)));
            
            foreach(var changeset in changesets)
            {
                var bheads = branchmap[changeset.Key];
                bheads.AddRange(changeset.Value.Select(v => Tuple.Create(v.NodeID, v.Revision)));

                if(bheads.Count <= 1) continue;

                while(changeset.Value.Count > 0)
                {
                    var l = changeset.Value.Dequeue();
                    var latest = Tuple.Create(l.NodeID, l.Revision);
                    if(!bheads.Contains(latest)) continue;

                    var mcset = bheads.First();
                    var minbnode = mcset;
                    var reachable = hgRevsetManager.GetReachableNodes(repository, latest.Item1, minbnode.Item1);
                    reachable.Remove(latest.Item1);

                    if(reachable.Count > 0)
                    {
                        bheads = new HashSet<Tuple<HgNodeID, uint>>(bheads.Where(b => !reachable.Contains(b.Item1)));
                    }
                } // while

                branchmap[changeset.Key] = bheads;
            } // for

            cachedTipNodeID = tip.NodeID;
            cachedTipRevision = tip.Revision;
        }

        
    }

    public class HgBranchManagerComparer : IComparer<Tuple<HgNodeID, uint>>
    {
        public int Compare(Tuple<HgNodeID, uint> x, Tuple<HgNodeID, uint> y)
        {
            return x.Item2.CompareTo(y.Item2);
        }
    }
}
