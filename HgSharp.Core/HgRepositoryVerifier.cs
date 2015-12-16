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
using System.Linq;
using System.Text;

using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public class HgRepositoryVerifier
    {
        private readonly HgRepository repository;
        private readonly HgFileSystem fileSystem;

        public HgRepositoryVerifier(HgRepository repository)
        {
            this.repository = repository;
        }

        public void Verify()
        {
            var manifestLinkrevs = new Dictionary<HgNodeID, IList<UInt32>>().DefaultableWith(k => new List<uint>());
            var fileLinkrevs = new Dictionary<string, IList<UInt32>>().DefaultableWith(k => new List<uint>());
            var fileNodeIDs = new Dictionary<string, IDictionary<HgNodeID, UInt32>>().DefaultableWith(k => new Dictionary<HgNodeID, UInt32>());

            VerifyChangelog(manifestLinkrevs, fileLinkrevs);
            VerifyManifest(manifestLinkrevs, fileNodeIDs);
            VerifyCrosscheck(manifestLinkrevs, fileLinkrevs, fileNodeIDs);

            VerifyFiles(fileLinkrevs, fileNodeIDs, repository.Store.GetDataFiles());
            //VerifyStore();
        }

        private void VerifyChangelog(IDictionary<HgNodeID, IList<UInt32>> manifestLinkrevs, IDictionary<string, IList<uint>> fileLinkrevs)
        {
            VerifyRevlog(repository.Changelog.Revlog);
            var hgChangelogReader = new HgChangelogReader(repository.Encoder);

            foreach(var hgChangelogEntry in repository.Changelog.Revlog.Entries)
            {
                //VerifyRevlogEntry(hgChangelogEntry);

                var hgEntry = new HgRevlogReader(repository.Changelog.Revlog, fileSystem).ReadRevlogEntry(hgChangelogEntry.Revision);
                var hgChangeset = hgChangelogReader.ReadChangeset(hgEntry);

                manifestLinkrevs[hgChangeset.ManifestNodeID].Add(hgChangelogEntry.Revision);

                foreach(var file in hgChangeset.Files)
                    fileLinkrevs[file].Add(hgChangelogEntry.Revision);
            } // foreach
        }

        private void VerifyManifest(IDictionary<HgNodeID, IList<uint>> manifestLinkrevs, IDictionary<string, IDictionary<HgNodeID, UInt32>> fileNodeIDs)
        {
            var hgManifestReader = new HgManifestReader(repository.Encoder);

            foreach(var hgManifestEntry in repository.Manifest.Revlog.Entries)
            {
                var hgEntry = new HgRevlogReader(repository.Manifest.Revlog, fileSystem).ReadRevlogEntry(hgManifestEntry.Revision);
                var hgManifest = hgManifestReader.ReadManifestEntry(hgEntry);

                if(!manifestLinkrevs[hgManifestEntry.NodeID].Any())
                    throw new Exception(hgManifestEntry.NodeID.Short + " not in changesets");

                manifestLinkrevs.Remove(hgManifestEntry.NodeID);

                foreach(var hgManifestFileEntry in hgManifest.Files)
                {
                    fileNodeIDs[hgManifestFileEntry.Path.FullPath][hgManifestFileEntry.FilelogNodeID] = hgManifestEntry.LinkRevision;
                } // forach
            }
        }

        private void VerifyCrosscheck(IDictionary<HgNodeID, IList<uint>> manifestLinkrevs, IDictionary<string, IList<uint>> fileLinkrevs, IDictionary<string, IDictionary<HgNodeID, uint>> fileNodeIDs)
        {
            if(manifestLinkrevs.Any())
            {
                throw new Exception("changeset refers to unknown manifest " + manifestLinkrevs.Keys.First().Short);
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies <paramref name="hgRevlog"/> for inconsistencies with the main format.
        /// </summary>
        /// <param name="hgRevlog"></param>
        private void VerifyRevlog(HgRevlog hgRevlog)
        {
            //throw new NotImplementedException();
        }

        private void VerifyFiles(IDictionary<string, IList<uint>> fileLinkrevs, IDictionary<string, IDictionary<HgNodeID, uint>> fileNodeIDs, IList<HgDataFile> dataFiles)
        {
            var files = fileNodeIDs.Keys.Union(fileLinkrevs.Keys).Distinct().OrderBy(f => f).ToList();

            foreach(var file in files)
            {
                var linkrev = fileLinkrevs[file][0];
                //var fileLog = repository.GetFilelog()
            } // foreach
        }
    }
}
