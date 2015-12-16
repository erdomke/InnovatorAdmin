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
using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public class HgBundleBuilder : HgRevlogSupport
    {
        private readonly HgFileSystem fileSystem;
        private readonly HgEncoder hgEncoder;
        private readonly bool performIntegrityChecks;

        public HgBundleBuilder(HgFileSystem fileSystem, HgEncoder hgEncoder, bool performIntegrityChecks = false)
        {
            this.fileSystem = fileSystem;
            this.hgEncoder = hgEncoder;
            this.performIntegrityChecks = performIntegrityChecks;
        }

        public HgBundle BuildBundle(HgRepository hgRepository, HgRevset hgRevset)
        {
            log.Debug("bundling changelog");
            var paths = new HashSet<string>();
            var changelog = BuildChangesetBundleGroup(hgRepository, hgRevset, hc => paths.AddRange(hc.Files));

            log.Debug("bundling manifests");
            var manifest = BuildManifestBundleGroup(hgRepository, hgRevset);

            //
            // List of all files that ever were tracked
            log.Debug("bundling filelogs");
            var files = BuildBundleFiles(hgRepository, hgRevset, paths);

            var hgBundle = new HgBundle(changelog, manifest, files);
            return hgBundle;
        }

        private IEnumerable<HgBundleFile> BuildBundleFiles(HgRepository hgRepository, HgRevset hgRevset, HashSet<string> paths)
        {
            var orderedPaths = paths.OrderBy(p => p).ToList();
            foreach(var path in orderedPaths)
            {
                var bundleFile = BuildBundleFile(hgRepository, hgRevset, path);
                if(bundleFile != null)
                    yield return bundleFile;
            } // foreach
        }

        private IEnumerable<HgChunk> BuildManifestBundleGroup(HgRepository hgRepository, HgRevset hgRevset)
        {
            var manifestRevset =
                new HgRevset(hgRepository.Manifest.Revlog.Entries.Where(hre => hgRevset.Contains(hre.LinkRevision)));
            var manifest = BuildBundleGroup(hgRepository, hgRepository.Manifest.Revlog, manifestRevset);
            return manifest;
        }

        private IEnumerable<HgChunk> BuildChangesetBundleGroup(HgRepository hgRepository, HgRevset hgRevset, Action<HgChangeset> callback)
        {
            var hgChangelogReader = new HgChangelogReader(hgEncoder);
            return BuildBundleGroup(hgRepository, hgRepository.Changelog.Revlog, hgRevset, hred => callback(hgChangelogReader.ReadChangeset(hred)));
        }

        private HgBundleFile BuildBundleFile(HgRepository hgRepository, HgRevset hgRevset, string path)
        {
            // TODO: Do not bundle files without chunks
            log.Debug("bundling {0}", path);

            var hgPath = new HgPath(path);
            var hgFilelog = hgRepository.GetFilelog(hgPath);
            if(hgFilelog == null) return null;

            var filelogRevset = new HgRevset(hgFilelog.Revlog.Entries.Where(fre => hgRevset.Contains(fre.LinkRevision)));
            var hgBundleGroup = BuildBundleGroup(hgRepository, hgFilelog.Revlog, filelogRevset);

            var hgBundleFile = new HgBundleFile(hgPath, hgBundleGroup);
            return hgBundleFile;
        }

        private IEnumerable<HgChunk> BuildBundleGroup(HgRepository hgRepository, HgRevlog hgRevlog, HgRevset hgRevset, Action<HgRevlogEntryData> callback = null)
        {
            var hgRevlogReader = new HgRevlogReader(hgRevlog, fileSystem);
            
            //
            // See http://stackoverflow.com/a/10359273/60188. Pure magic
            var revisionChunks =
                hgRevset.
                    Select(hre => hre.Revision).
                    OrderBy(r => r).
                    Select((r, i) => new { r, i }).
                    GroupBy(x => x.r - x.i). 
                    Select(x => x.Select(xx => xx.r)).
                    Select(c => c.ToArray()).
                    ToArray();

            if(revisionChunks.Length == 0) yield break;

            byte[] prev = null;
            uint prevRev = uint.MaxValue;
            var prediff = false;
            var hgRevlogEntry = hgRevlog[revisionChunks[0][0]];
            if(hgRevlogEntry.FirstParentRevisionNodeID != HgNodeID.Null)
            {
                prev = hgRevlogReader.ReadRevlogEntry(hgRevlogEntry.FirstParentRevision).Data;
                prediff = true;
            }
            
            foreach(var revisionChunk in revisionChunks)
            {
                foreach(var revision in revisionChunk)
                {
                    hgRevlogEntry = hgRevlog[revision];
                    var hgChangeset = hgRepository.Changelog.Revlog[hgRevlogEntry.LinkRevision];
                
                    byte[] data = null;

                    if(prev == null || hgRevlogEntry.BaseRevision == hgRevlogEntry.Revision || prediff || (prevRev != UInt32.MaxValue && prevRev + 1 != revision))
                    {
                        var hgRevlogEntryData = hgRevlogReader.ReadRevlogEntry(revision);

                        if(prev == null)
                        {   
                            //
                            // Trivial case
                            var buffer = new byte[hgRevlogEntryData.Data.Length + 12];
                            using(var stream = new MemoryStream(buffer))
                            using(var binaryWriter = new BigEndianBinaryWriter(stream))
                            {
                                binaryWriter.Write((uint)0);
                                binaryWriter.Write((uint)0);
                                binaryWriter.Write((uint)hgRevlogEntryData.Data.Length);
                                binaryWriter.Write(hgRevlogEntryData.Data);
                            } // using

                            data = buffer;
                        } // if
                        else
                        {
                            data = BDiff.Diff(prev, hgRevlogEntryData.Data);
                            if(prediff)
                                prediff = false;
                        } // else

                        prev = hgRevlogEntryData.Data;
                    } // if
                    else
                    {
                        data = hgRevlogReader.ReadRevlogEntryDataRaw(revision);
                        prev = MPatch.Patch(prev, new List<byte[]> { data });
                    } // else

                    if(callback != null) callback(new HgRevlogEntryData(hgRevlogEntry, prev));

                    if(performIntegrityChecks)
                    {
                        var expectedNodeID = GetRevlogEntryDataNodeID(hgRevlogEntry.FirstParentRevisionNodeID, hgRevlogEntry.SecondParentRevisionNodeID, prev);
                        if(expectedNodeID != hgRevlogEntry.NodeID)
                        {
                            // TODO: Exception class
                            throw new ApplicationException("integrity violation for " + hgRevlogEntry.NodeID.Short);
                        } // if
                    } // if

                    var hgChunk = new HgChunk(hgRevlogEntry.NodeID, hgRevlogEntry.FirstParentRevisionNodeID, hgRevlogEntry.SecondParentRevisionNodeID,
                        hgChangeset.NodeID, data);

                    yield return hgChunk;

                    prevRev = revision;
                } // foreach
            } // foreach
        }
    }
}