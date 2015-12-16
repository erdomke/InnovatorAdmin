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
using System.Runtime.InteropServices;

namespace HgSharp.Core.Util
{
    [DebuggerDisplay("Offset: {Offset}, Length: {Length}, Hash: {GetHashCode()}")]
    public class Segment : IEquatable<Segment>
    {
        private int hashCode;
        private readonly byte[] array;

        public int Offset { get; private set; }

        public int Length { get; private set; }

        public Segment(byte[] array, int offset, int length)
        {
            this.array = array;

            Offset = offset;
            Length = length;

            hashCode = (int)new MurmurHash2().Hash(array, Length, Offset);
        }

        public bool Equals(Segment other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.Length != Length) return false;

            return CompareBuffers(other.array, other.Offset, array, Offset, (uint)Length) == 0;
            /*for(var i = 0; i < Length; ++i)
                if(other.array[i + other.Offset] != array[i + Offset]) return false;

            return true;*/
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Segment)) return false;
            return Equals((Segment) obj);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public static bool operator ==(Segment left, Segment right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Segment left, Segment right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("({0}; {1})", Offset, Length);
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int memcmp(byte* b1, byte* b2, UIntPtr count);

        private static unsafe int GetBufferHash(byte[] buffer, int offset, int count)
        {
            fixed(byte* b = buffer)
            {
                int hash = 0;
                for(var i = 0; i < count; ++i)
                    hash = (hash * 1664525) + b[i + offset] + 1013904223;

                return hash;
            }
        }

        private static unsafe int CompareBuffers(byte[] buffer1, int offset1, byte[] buffer2, int offset2, uint count)
        {
            fixed (byte* b1 = buffer1, b2 = buffer2)
            {
                return memcmp(b1 + offset1, b2 + offset2, new UIntPtr(count));

            }
        }
    }
}