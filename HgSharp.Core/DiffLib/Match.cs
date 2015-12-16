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

namespace HgSharp.Core.DiffLib
{
    [DebuggerDisplay("{SourceIndex} {DestinationIndex} {Length}")]
    public class Match : IEquatable<Match>
    {
        public int SourceIndex { get; private set; }

        public int DestinationIndex { get; private set; }

        public int Length { get; private set; }

        public Match(int sourceIndex, int destinationIndex, int length)
        {
            SourceIndex = sourceIndex;
            DestinationIndex = destinationIndex;
            Length = length;
        }

        public bool Equals(Match other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.SourceIndex == SourceIndex && other.DestinationIndex == DestinationIndex && other.Length == Length;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Match)) return false;
            return Equals((Match) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = SourceIndex;
                result = (result*397) ^ DestinationIndex;
                result = (result*397) ^ Length;
                return result;
            }
        }

        public static bool operator ==(Match left, Match right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Match left, Match right)
        {
            return !Equals(left, right);
        }
    }
}