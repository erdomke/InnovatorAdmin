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
    public class HgRevsetManager
    {
        public HgRevset GetRevset(HgRevspec hgRevspec)
        {
            return null;
        }

        public HgRevset GetRevset(HgRepository hgRepository, uint startRevision, uint endRevision)
        {
            return new HgRevset(hgRepository.Changelog.Revlog.Entries.Where(e => e.Revision >= startRevision && e.Revision <= endRevision));
        }

        public HgRevset GetRevset(HgRepository hgRepository)
        {
            return new HgRevset(hgRepository.Changelog.Revlog.Entries);
        }

        public HgRevset GetRevset(HgRepository hgRepository, HgRevset commonNodeIDs, HgRevset headNodeIDs)
        {
            var commonAncestors = 
                commonNodeIDs == null || commonNodeIDs.Count == 0 || commonNodeIDs[0] == HgNodeID.Null ?
                    new HgRevset() : 
                    GetAncestors(hgRepository, commonNodeIDs);

            var headAncestors = 
                headNodeIDs == null || headNodeIDs.Count == 0 ?
                    GetAncestors(hgRepository, hgRepository.GetHeads()) :
                    GetAncestors(hgRepository, headNodeIDs);

            var revset = headAncestors - commonAncestors;

            return revset;
        }

        public HgRevset GetAncestors(HgRepository hgRepository, HgRevset hgNodeIDs)
        {
            Func<HgNodeID, uint> revisionMapper = id => hgRepository.Changelog[id].Metadata.Revision;

            if(hgNodeIDs == null || hgNodeIDs.Count == 0) 
                hgNodeIDs = hgRepository.GetHeads();

            var revset = new HgRevset();
            var pending = new Queue<HgRevlogEntry>();

            foreach(var hgNodeID in hgNodeIDs)
            {
                //revset.Add(hgNodeID, revisionMapper(hgNodeID));
                pending.Enqueue(hgRepository.Changelog.Revlog[hgNodeID.Revision]);
            } // foreach

            while(pending.Count > 0)
            {
                var current = pending.Dequeue();
                if(!revset.Add(current.NodeID, current.Revision)) 
                    continue;

                var next = hgRepository.Changelog.Revlog[current.FirstParentRevision];
                if(next != null) pending.Enqueue(next);

                if(current.SecondParentRevisionNodeID != HgNodeID.Null)
                {
                    next = hgRepository.Changelog.Revlog[current.SecondParentRevision];
                    if(next != null) pending.Enqueue(next);
                }
            } // while

            return revset;

        }

        public HgRevset GetDescendants(HgRepository hgRepository, HgRevset hgRevset)
        {
            var seen = new HashSet<HgNodeID>(hgRevset.Select(r => r.NodeID));
            return null;
        }

        public HgRevset GetRevset(HgRepository hgRepository, IEnumerable<uint> revisions)
        {
            var revset = new HgRevset(revisions.OrderBy(r => r).Select(r => new HgRevsetEntry(r, hgRepository.Changelog[r].Metadata.NodeID)));
            return revset;
        }

        public ISet<HgNodeID> GetReachableNodes(HgRepository hgRepository, HgNodeID node, HgNodeID stop)
        {
            var reachable = new HashSet<HgNodeID>(new[] { node });
            var visit = new Queue<HgNodeID>(new[] { node });

            var stopn = stop == HgNodeID.Null ? 0 : hgRepository.Changelog.Revlog.GetEntry(stop).Revision;

            while(visit.Count > 0)
            {
                var n = visit.Dequeue();
                
                if(n == stop) continue;
                if(n == HgNodeID.Null) continue;

                var nc = hgRepository.Changelog.Revlog.GetEntry(n);
                foreach(var p in new[] { hgRepository.Changelog.Revlog[nc.FirstParentRevision], hgRepository.Changelog.Revlog[nc.SecondParentRevision] })
                {
                    if(p == null) continue;
                    if(p.Revision < stopn) continue;
                    if(!reachable.Contains(p.NodeID))
                    {
                        reachable.Add(p.NodeID);
                        visit.Enqueue(p.NodeID);
                    }
                }
            }

            return reachable;
        } 
    }
}