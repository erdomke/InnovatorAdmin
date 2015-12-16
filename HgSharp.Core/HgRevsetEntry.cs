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
    [DebuggerDisplay("{Revision}:{NodeID.Short,nq}")]
    public class HgRevsetEntry : IEquatable<HgRevsetEntry>
    {
        public uint Revision { get; private set; }

        public HgNodeID NodeID { get; private set; }

        public HgRevsetEntry(uint revision, HgNodeID nodeID)
        {
            Revision = revision;
            NodeID = nodeID;
        }

        public bool Equals(HgRevsetEntry other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return other.Revision == Revision && other.NodeID.Equals(NodeID);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != typeof(HgRevsetEntry)) return false;
            return Equals((HgRevsetEntry)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Revision.GetHashCode() * 397) ^ NodeID.GetHashCode();
            }
        }

        public static bool operator ==(HgRevsetEntry left, HgRevsetEntry right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HgRevsetEntry left, HgRevsetEntry right)
        {
            return !Equals(left, right);
        }
    }
}