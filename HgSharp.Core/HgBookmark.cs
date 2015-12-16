using System;
using System.Diagnostics;

namespace HgSharp.Core
{
    [DebuggerDisplay("{Name,nq} {Changeset}")]
    public class HgBookmark : IEquatable<HgBookmark>
    {
        public string Name { get; private set; }

        public HgChangeset Changeset { get; private set; }

        public HgBookmark(string name, HgChangeset changeset)
        {
            Name = name;
            Changeset = changeset;
        }

        public bool Equals(HgBookmark other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.Changeset.Metadata.NodeID, Changeset.Metadata.NodeID);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != typeof(HgBookmark)) return false;
            return Equals((HgBookmark)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Changeset != null ? Changeset.Metadata.NodeID.GetHashCode() : 0);
            }
        }

        public static bool operator ==(HgBookmark left, HgBookmark right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HgBookmark left, HgBookmark right)
        {
            return !Equals(left, right);
        }
    }
}