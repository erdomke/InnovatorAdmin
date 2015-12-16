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
    [DebuggerDisplay("{Path.FullPath,nq} {FilelogNodeID.Short,nq}")]
    public class HgManifestFileEntry : IEquatable<HgManifestFileEntry>
    {
        public HgPath Path { get; private set; }

        public HgNodeID FilelogNodeID { get; private set; }

        public HgManifestFileEntry(HgPath path, HgNodeID fileNodeID)
        {
            Path = path;
            FilelogNodeID = fileNodeID;
        }

        public bool Equals(HgManifestFileEntry other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return Equals(other.Path, Path) && other.FilelogNodeID.Equals(FilelogNodeID);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != typeof(HgManifestFileEntry)) return false;
            return Equals((HgManifestFileEntry)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Path != null ? Path.GetHashCode() : 0) * 397) ^ FilelogNodeID.GetHashCode();
            }
        }

        public static bool operator ==(HgManifestFileEntry left, HgManifestFileEntry right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HgManifestFileEntry left, HgManifestFileEntry right)
        {
            return !Equals(left, right);
        }
    }
}