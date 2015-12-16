// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using HgSharp.Core.Util;

namespace HgSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This has all kinds of corner cases regarding Windows and UTF8. See 
    /// http://mercurial.selenic.com/wiki/WindowsUTF8Plan
    /// http://mercurial.selenic.com/wiki/EncodingStrategy
    /// </remarks>
    public class HgManifestReader : HgRevlogReaderBase
    {
        private readonly HgEncoder hgEncoder;

        public HgManifestReader(HgEncoder hgEncoder)
        {
            this.hgEncoder = hgEncoder;
        }

        public IEnumerable<HgManifestEntry> ReadManifestEntries(IEnumerable<HgRevlogEntryData> revlogEntryDatas)
        {
            foreach(var revlogEntryData in revlogEntryDatas)
                yield return ReadManifestEntry(revlogEntryData);
        }

        public HgManifestEntry ReadManifestEntry(HgRevlogEntryData revlogEntryData)
        {
            var data = revlogEntryData.Data;
            var lines = data.Split((byte)'\n');
            
            var files = new List<HgManifestFileEntry>(lines.Length);

            foreach(var line in lines)
            {
                var z = Array.IndexOf(data, (byte)0, line.Offset) - line.Offset;

                var path = hgEncoder.DecodeAsLocal(data, line.Offset, z);
                var nodeID = Encoding.ASCII.GetString(data, line.Offset + z + 1, line.Length - z - 2);
                if(nodeID.Length > 40)
                    nodeID = nodeID.Substring(0, 40);

                files.Add(new HgManifestFileEntry(GetManifestEntryPath(path), new HgNodeID(nodeID)));
            } // foreach

            return new HgManifestEntry(GetRevlogEntryMetadata(revlogEntryData.Entry), files);
        }

        private static HgPath GetManifestEntryPath(string path)
        {
            return new HgPath(path);
        }
    }
}
