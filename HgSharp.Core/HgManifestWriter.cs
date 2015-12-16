// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.

using System.IO;
using System.Linq;

namespace HgSharp.Core
{
    public class HgManifestWriter
    {
        private readonly HgEncoder hgEncoder;

        public HgManifestWriter(HgEncoder hgEncoder)
        {
            this.hgEncoder = hgEncoder;
        }

        public byte[] WriteManifestEntry(HgManifestEntry hgManifestEntry)
        {
            using(var memoryStream = new MemoryStream(1024 * 1024))
            {
                foreach(var hgManifestFileEntry in hgManifestEntry.Files.OrderBy(mfe => mfe.Path.FullPath))
                {
                    var buffer = hgEncoder.EncodeAsLocal(hgManifestFileEntry.Path.FullPath.TrimStart('/'));
                    memoryStream.Write(buffer, 0, buffer.Length);
                    memoryStream.WriteByte(0);

                    buffer = hgEncoder.EncodeAsLocal(hgManifestFileEntry.FilelogNodeID.Long);
                    memoryStream.Write(buffer, 0, buffer.Length);
                    memoryStream.WriteByte((byte)'\n');
                } // foreach

                return memoryStream.ToArray();
            } // using
        }
    }
}