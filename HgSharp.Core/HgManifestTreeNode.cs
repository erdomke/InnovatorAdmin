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
using System.Diagnostics;

using System.Linq;

namespace HgSharp.Core
{
    [DebuggerDisplay("{Type,nq} {Name,nq} {NodeID,nq}")]
    public class HgManifestNode
    {
        public HgManifestTreeNodeType Type { get; private set; }
        
        public string Name { get; private set; }

        public HgPath Path { get; private set; }

        public HgNodeID? NodeID { get; private set; }

        private HgManifestNode(HgManifestTreeNodeType type, string name, HgPath path, HgNodeID? nodeID)
        {
            Type = type;
            Name = name;
            Path = path;
            NodeID = nodeID;
        }

        public static HgManifestNode File(HgPath path, HgNodeID fileNodeID)
        {
            return new HgManifestNode(HgManifestTreeNodeType.File, path.Segments.Last(), path, fileNodeID);
        }

        public static HgManifestNode Directory(HgPath path)
        {
            return new HgManifestNode(HgManifestTreeNodeType.Directory, path.Segments.Last(), path, null);
        }

        public static HgManifestNode Subrepository(HgPath path, HgNodeID changesetNodeID)
        {
            return new HgManifestNode(HgManifestTreeNodeType.Subrepository, path.Segments.Last(), path, changesetNodeID);
        }
    }
    [DebuggerDisplay("{Type,nq} {Path.FullPath,nq}")]
    public class HgManifestTreeNode
    {
        private readonly ReadOnlyCollection<HgManifestTreeNode> children;

        public HgNodeID FileNodeID { get; private set; }

        public HgPath Path { get; private set; }

        public string Name { get; private set; }

        public HgManifestTreeNode Parent { get; private set; }

        public HgManifestTreeNodeType Type { get; private set; }

        public ReadOnlyCollection<HgManifestTreeNode> Children
        {
            get { return children; }
        }

        public HgManifestTreeNode(HgPath hgPath, string name, IEnumerable<HgManifestTreeNode> children) : 
            this(hgPath, name, HgManifestTreeNodeType.Directory)
        {
            this.children = new ReadOnlyCollection<HgManifestTreeNode>(new List<HgManifestTreeNode>(children));
        }

        public HgManifestTreeNode(HgPath hgPath, string name, HgNodeID fileNodeID) :
            this(hgPath, name, HgManifestTreeNodeType.File)
        {
            FileNodeID = fileNodeID;
        }

        public HgManifestTreeNode(HgPath hgPath, HgNodeID fileNodeID) :
            this(hgPath, "", HgManifestTreeNodeType.Subrepository)
        {
            FileNodeID = fileNodeID;
        }

        private HgManifestTreeNode(HgPath hgPath, string name, HgManifestTreeNodeType type)
        {
            Path = hgPath;
            Name = name;
            Type = type;
        }

        /*public HgManifestTreeNode GetPath(string treePath)
        {
            if(treePath == null) throw new ArgumentNullException("treePath");

            var segments = treePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            HgManifestTreeNode currentNode = this;

            for(var i = 0; i < segments.Length; ++i)
            {
                var segment = segments[i];
                currentNode = currentNode.Children.SingleOrDefault(c => c.Name == segment);

                if(currentNode == null) return null;
            } // foreach

            return currentNode;
        }*/
    }
}
