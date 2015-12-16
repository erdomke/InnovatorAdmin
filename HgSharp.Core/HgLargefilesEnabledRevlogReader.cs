// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HgSharp.Core
{
    public class HgLargefilesEnabledRevlogReader : IHgRevlogReader
    {
        private readonly string storeBasePath;
        private readonly HgRevlog standinRevlog;
        private readonly IHgRevlogReader standinRevlogReader;

        public HgLargefilesEnabledRevlogReader(string storeBasePath, HgRevlog standinRevlog, IHgRevlogReader standinRevlogReader)
        {
            this.storeBasePath = storeBasePath;
            this.standinRevlog = standinRevlog;
            this.standinRevlogReader = standinRevlogReader;
        }

        public IEnumerable<HgRevlogEntryData> ReadRevlogEntries(HgRevset hgRevset)
        {
            foreach(var hgRevlogEntryData in standinRevlogReader.ReadRevlogEntries(hgRevset))
                yield return GetRevlogEntryData(hgRevlogEntryData);
        }

        public IEnumerable<HgRevlogEntryData> ReadRevlogEntries(IEnumerable<uint> revisions)
        {
            foreach(var hgRevlogEntryData in standinRevlogReader.ReadRevlogEntries(revisions))
                yield return GetRevlogEntryData(hgRevlogEntryData);
        }

        public IEnumerable<HgRevlogEntryData> ReadRevlogEntries(uint startRevision, uint endRevision)
        {
            throw new System.NotImplementedException();
        }

        public HgRevlogEntryData ReadRevlogEntry(HgNodeID nodeID)
        {
            var revlogEntryData = standinRevlogReader.ReadRevlogEntry(nodeID);
            return GetRevlogEntryData(revlogEntryData);
        }

        private HgRevlogEntryData GetRevlogEntryData(HgRevlogEntryData revlogEntryData)
        {
            var data = revlogEntryData.Data;

            HgFileCopyInfo.ExtractFileCopyInfo(ref data);

            //
            // Apparently, entries in standins end with \n, so we explicitly limit to 40 bytes
            HgNodeID nodeID;
            if(HgNodeID.TryParse(Encoding.ASCII.GetString(data, 0, 40), out nodeID))
            {
                var lfPath = Path.Combine(storeBasePath, @"..\largefiles", nodeID.Long);
                if(File.Exists(lfPath))
                {
                    data = File.ReadAllBytes(lfPath);
                    revlogEntryData = new HgRevlogEntryData(revlogEntryData.Entry, data);
                } // if
            } // if

            return revlogEntryData;
        }
    }
}