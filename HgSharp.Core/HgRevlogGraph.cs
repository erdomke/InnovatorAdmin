// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HgSharp.Core
{
    public class HgRevlogGraph
    {
        private readonly IDictionary<string, HgRevlogGraphNode> nodes = new Dictionary<string, HgRevlogGraphNode>();

        public HgRevlogGraphNode Root { get; private set; }

        public ICollection<HgRevlogGraphNode> Nodes
        {
            get { return nodes.Values;  }
        }
             
        public HgRevlogGraph()
        {
            Root = new HgRevlogGraphNode(HgNodeID.Null, uint.MaxValue);
        }

        public void Add(IEnumerable<HgRevlogEntry> revlogEntries)
        {
            foreach(var revlogEntry in revlogEntries)
                Add(revlogEntry);
        }

        public void Add(HgRevlogEntry revlogEntry)
        {
            var node = new HgRevlogGraphNode(revlogEntry.NodeID, revlogEntry.Revision);
            
            //
            // We only link back to Root when both parents are Null
            if(revlogEntry.FirstParentRevisionNodeID == HgNodeID.Null && revlogEntry.SecondParentRevisionNodeID == HgNodeID.Null)
            {
                node.FirstParent = Root;
                Root.Children.Add(node);
            } // if
            else
            {
                node.FirstParent = nodes[revlogEntry.FirstParentRevisionNodeID.Long]; // First parent is always there
                node.SecondParenet = 
                    revlogEntry.SecondParentRevisionNodeID == HgNodeID.Null ?
                        null :
                        nodes[revlogEntry.SecondParentRevisionNodeID.Long];
                
                node.FirstParent.Children.Add(node);
                if(node.SecondParenet != null)
                    node.SecondParenet.Children.Add(node);
            } // else

            nodes[node.NodeID.Long] = node;
        }
    }
}
