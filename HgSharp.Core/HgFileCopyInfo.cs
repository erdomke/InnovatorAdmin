// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.

using System;
using System.Linq;
using System.Text;
using HgSharp.Core.Util;

namespace HgSharp.Core
{
    /// <summary>
    /// Encapsulates information about file copying operation.
    /// </summary>
    public class HgFileCopyInfo
    {
        public HgPath Path { get; private set; }

        public HgNodeID NodeID { get; private set; }

        public HgFileCopyInfo(HgPath path, HgNodeID nodeID)
        {
            Path = path;
            NodeID = nodeID;
        }

        public static HgFileCopyInfo ExtractFileCopyInfo(ref byte[] data)
        {
            if(data.Length <= 20 || data[0] != 1 || data[1] != 10) return null;

            var metablockLength = Array.IndexOf(data, (byte)1, 1) + 1; // +1 for that \10 that goes after \1 we just found
            var metadataLength = metablockLength - 4;

            var metadata = new byte[metadataLength];
            Buffer.BlockCopy(data, 2, metadata, 0, metadataLength);

            var metadataText = Encoding.UTF8.GetString(metadata);
            var metadataValues =
                metadataText.Split((char)10).
                    Select(s => s.Split(':')).
                    ToDictionary(s => s[0].Trim(), s => s[1].Trim()).
                    DefaultableWith(k => "");

            var copyInfo = new HgFileCopyInfo(new HgPath(metadataValues["copy"]), new HgNodeID(metadataValues["copyrev"]));

            var dt = new byte[data.Length - metablockLength - 1];
            Buffer.BlockCopy(data, metablockLength + 1, dt, 0, dt.Length);

            data = dt;

            return copyInfo;
        }
    }
}