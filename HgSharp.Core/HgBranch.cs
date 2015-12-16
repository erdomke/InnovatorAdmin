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
    [DebuggerDisplay("{Name}")]
    public class HgBranch : IEquatable<HgBranch>
    {
        public static readonly HgBranch Default = new HgBranch("default", false);

        public string Name { get; private set; }

        public bool Closed { get; private set; }

        public HgBranch(string name, bool closed)
        {
            Name = name;
            Closed = closed;
        }

        public bool Equals(HgBranch other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && other.Closed.Equals(Closed);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != typeof(HgBranch)) return false;
            return Equals((HgBranch)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Closed.GetHashCode();
            }
        }

        public static bool operator ==(HgBranch left, HgBranch right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HgBranch left, HgBranch right)
        {
            return !Equals(left, right);
        }
    }
}