// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Runtime.InteropServices;
using System.Text;
using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public struct HgNodeID : IEquatable<HgNodeID>
    {
        public static readonly HgNodeID Null = new HgNodeID(new byte[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        
        private readonly byte[] nodeID;
        private string hexNodeID;
        private int? hashCode;

        public string Long
        {
            get { return ToString(); }
        }

        public string Short
        {
            get { return Long.Substring(0, 12); }
        }

        public string Tiny
        {
            get { return Long.Substring(0, 6); }
        }

        public byte[] NodeID
        {
            get { return nodeID; }
        }

        public HgNodeID(byte[] nodeID) :
            this()
        {
            this.nodeID = nodeID;
        }

        public HgNodeID(byte[] nodeID, int length) :
            this()
        {
            this.nodeID = new byte[Math.Min(20, length)];
            Buffer.BlockCopy(nodeID, 0, this.nodeID, 0, this.nodeID.Length);
        }

        public HgNodeID(string hexNodeID) :
            this()
        {
            this.hexNodeID = hexNodeID;
            nodeID = HexUtil.FromHex(hexNodeID);
        }
            
        public override string ToString()
        {
            return hexNodeID ?? (hexNodeID = GetHexValue());
        }

        public bool Equals(HgNodeID other)
        {
            if(!string.IsNullOrWhiteSpace(hexNodeID) && !string.IsNullOrWhiteSpace(other.hexNodeID))
                return this.hexNodeID == other.hexNodeID;
            
            //
            // Caching this for performance seasons. Be sure not to access via property
            var length = Math.Min(NodeID.Length, other.NodeID.Length);
 
            for(var i = 0; i < length; i++)
                if(!nodeID[i].Equals(other.nodeID[i])) return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(obj.GetType() != typeof(HgNodeID)) return false;
            return Equals((HgNodeID)obj);
        }

        public override int GetHashCode()
        {
            if(!hashCode.HasValue)
            {
                if(NodeID == null) hashCode = 0;
                else
                {
                    //
                    // See http://stackoverflow.com/a/468084/60188
                    unchecked
		            {
			            const int p = 16777619;
			            var hash = (int)2166136261;

			            for(var i = 0; i < nodeID.Length; i++)
				            hash = (hash ^ nodeID[i]) * p;

			            hash += hash << 13;
			            hash ^= hash >> 7;
			            hash += hash << 3;
			            hash ^= hash >> 17;
			            hash += hash << 5;
			            
                        hashCode = hash;
		            } // unchecked
                } // else
            } // if

            return hashCode.Value;
        }

        public static bool operator ==(HgNodeID left, HgNodeID right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HgNodeID left, HgNodeID right)
        {
            return !left.Equals(right);
        }

        private string GetHexValue()
        {
            return HexUtil.ToHex(NodeID);
        }

        public static bool TryParse(string changeset, out HgNodeID hgNodeID)
        {
            try
            {
                var nodeID = HexUtil.FromHex(changeset);
                hgNodeID = new HgNodeID(nodeID);

                return true;
            }
            catch(Exception)
            {
                hgNodeID = Null;
                return false;
            }
        }
    }
}