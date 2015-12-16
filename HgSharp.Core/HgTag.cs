// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Diagnostics;

namespace HgSharp.Core
{
    [DebuggerDisplay("{Name,nq} {Changeset}")]
    public class HgTag : IEquatable<HgTag>
    {
        public string Name { get; private set; }

        public HgNodeID NodeID { get; private set; }

        public HgTag(string name, HgNodeID nodeID)
        {
            Name = name;
            NodeID = nodeID;
        }

        public bool Equals(HgTag other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.NodeID, NodeID);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != typeof(HgTag)) return false;
            return Equals((HgTag)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ NodeID.GetHashCode();
            }
        }

        public static bool operator ==(HgTag left, HgTag right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HgTag left, HgTag right)
        {
            return !Equals(left, right);
        }
    }
}