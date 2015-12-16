// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Security.Cryptography;

using NLog;

namespace HgSharp.Core
{
    public abstract class HgRevlogSupport
    {
        protected const int InlineDataFlag = (1 << 16);
        public static readonly int NodeIDLength = 32;
        public static readonly int RevlogNgEntryHeaderLength = 64;
        public static readonly int NodeIDEffectiveLength = 20;

        protected Logger log;

        public const int NG = 1;

        public HgRevlogSupport()
        {
            log = LogManager.GetLogger(GetType().FullName);
        }

        protected HgNodeID GetRevlogEntryDataNodeID(HgNodeID firstParentNodeID, HgNodeID secondParentNodeID, byte[] data)
        {
            var parents = new[] {
                Min(firstParentNodeID, secondParentNodeID),
                Max(firstParentNodeID, secondParentNodeID)
            };

            using(var hash = new SHA1Managed())
            {
                hash.Initialize();
                var buffer = new byte[hash.HashSize / 8];
                hash.TransformBlock(parents[0].NodeID, 0, 20, buffer, 0);
                hash.TransformBlock(parents[1].NodeID, 0, 20, buffer, 0);
                hash.TransformFinalBlock(data, 0, data.Length);

                buffer = hash.Hash;

                var node = new HgNodeID(buffer);

                return node;
            } // using
            
        }

        protected HgNodeID GetRevlogEntryDataNodeID(HgRevlogEntryData revlogEntryData)
        {
            return GetRevlogEntryDataNodeID(
                revlogEntryData.Entry.FirstParentRevisionNodeID,
                revlogEntryData.Entry.SecondParentRevisionNodeID,
                revlogEntryData.Data);
        }

        private static HgNodeID Min(HgNodeID l, HgNodeID r)
        {
            var ln = l.NodeID;
            var rn = r.NodeID;
            for(var i = 0; i < Math.Min(ln.Length, rn.Length); ++i)
            {
                if(ln[i] < rn[i]) return l;
                if(ln[i] > rn[i]) return r;
            } // for

            return l;
        }

        private static HgNodeID Max(HgNodeID l, HgNodeID r)
        {
            var ln = l.NodeID;
            var rn = r.NodeID;
            for(var i = 0; i < Math.Min(ln.Length, rn.Length); ++i)
            {
                if(ln[i] > rn[i]) return l;
                if(ln[i] < rn[i]) return r;
            } // for

            return l;
        }
    }
}
