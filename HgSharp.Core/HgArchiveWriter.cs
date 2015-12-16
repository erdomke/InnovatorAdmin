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
using System.IO;
using System.Linq;
using System.Text;

using HgSharp.Core.Util;

using ICSharpCode.SharpZipLib.Zip;

namespace HgSharp.Core
{
    public class HgArchiveWriter
    {
        private readonly HgRepository hgRepository;

        public HgArchiveWriter(HgRepository hgRepository)
        {
            this.hgRepository = hgRepository;
        }

        public void WriteArchive(HgNodeID changesetNodeID, Stream stream, HgArchiveFormat format, Func<bool> cancellationCallback = null)
        {
            var hgChangeset = hgRepository.Changelog[changesetNodeID];
            var hgManifestEntry = hgRepository.Manifest[hgChangeset.ManifestNodeID];
            
            using(var outputStream = new ZipOutputStream(stream))
            {
                outputStream.SetLevel(3);

                var hgArchival =
                    string.Format(
                        "repo: {0}\nnode: {1}\nbranch: {2}\n",
                        hgRepository.Changelog[0].Metadata.NodeID.Long,
                        changesetNodeID.Long,
                        hgChangeset.Branch.Name);

                var hgTags = hgRepository.GetTags().Where(t => t.Name != "tip" && t.NodeID == changesetNodeID).ToList();
                if(hgTags.Count > 0)
                {
                    hgArchival +=
                        string.Join(
                            "",
                            hgTags.Select(t => string.Format("tag: {0}\n", t.Name)));
                } // if
                else
                {
                    var latestTag = GetLatestTag(hgRepository, changesetNodeID);

                    hgArchival +=
                        string.Format(
                            "latesttag: {0}\nlatesttagdistance: {1}\n",
                            latestTag == null || string.IsNullOrWhiteSpace(latestTag.Item3) ? "null" : latestTag.Item3,
                            latestTag == null ? 0 : latestTag.Item2);
                } // else
                       
                var now = DateTime.UtcNow;
                var buffer = Encoding.ASCII.GetBytes(hgArchival);

                var zipEntry = new ZipEntry(".hg_archival.txt");
                zipEntry.DateTime = now;
                zipEntry.Size = buffer.Length;

                outputStream.PutNextEntry(zipEntry);
                outputStream.Write(buffer, 0, buffer.Length);

                //
                // Now iterate over each file in the manifest
                foreach(var hgManifestFileEntry in hgManifestEntry.Files)
                {
                    if(cancellationCallback != null && cancellationCallback()) return;

                    var hgFile = hgRepository.GetFile(hgManifestFileEntry);

                    zipEntry = new ZipEntry(ZipEntry.CleanName(hgFile.Path.FullPath));
                    zipEntry.DateTime = now;
                    zipEntry.Size = hgFile.Data.Length;

                    outputStream.PutNextEntry(zipEntry);
                    outputStream.Write(hgFile.Data, 0, hgFile.Data.Length);
                } // foreach

                outputStream.Finish();
                outputStream.Flush();
            } // using
        }

        private static Tuple<DateTimeOffset, int, string> GetLatestTag(HgRepository hgRepository, HgNodeID hgNodeID)
        {
            var revisions = new Stack<uint>();
            var revision = hgRepository.Changelog[hgNodeID].Metadata.Revision;

            var cache = new Dictionary<uint, Tuple<DateTimeOffset, int, string>>();

            revisions.Push(revision);

            while(revisions.Count > 0)
            {
                revision = revisions.Pop();
                if(cache.ContainsKey(revision)) continue;

                var hgChangeset = hgRepository.Changelog[revision];
                var tags = hgRepository.GetTags().Where(t => t.Name != "tip" && t.NodeID == hgChangeset.Metadata.NodeID).ToList();

                if(tags.Count > 0)
                {
                    cache[revision] = Tuple.Create(hgChangeset.CommittedAt, 0, tags[0].Name);
                    continue;
                } // if

                int distance = 0;
                string name = "";
                DateTimeOffset committedAt = DateTimeOffset.MinValue;

                if(hgChangeset.Metadata.Parents.All(p => cache.ContainsKey(p.Revision)))
                {
                    var enumerable = hgChangeset.Metadata.Parents.Select(p => cache[p.Revision]).ToList();
                    if(enumerable.Any())
                    {
                        var max = enumerable.MaxBy(p => p.Item1);

                        committedAt = max.Item1;
                        distance = max.Item2;
                        name = max.Item3;
                    } // if
                }
                else
                {
                    revisions.Push(revision);
                    
                    foreach(var p in hgChangeset.Metadata.Parents)
                        revisions.Push(p.Revision);

                    continue;
                } // catch

                cache[revision] = Tuple.Create(committedAt, distance + 1, name);
            } // while

            revision = hgRepository.Changelog[hgNodeID].Metadata.Revision;
            
            return cache[revision];
        } 
    }
}
