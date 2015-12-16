using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace HgSharp.Core
{
    public class HgLargefilesEnabledRevlog : HgRevlog
    {
        private readonly HgRevlog standinRevlog;
        private readonly HgRevlogReader standinRevlogReader;
        private readonly string storeBasePath;

        internal HgRevlog StandinRevlog
        {
            get { return standinRevlog; }
        }

        public HgLargefilesEnabledRevlog(HgRevlog standinRevlog, HgRevlogReader standinRevlogReader, string storeBasePath) :
            base(standinRevlog.IndexPath, standinRevlog.DataPath, standinRevlog.Version, standinRevlog.InlineData, standinRevlog.Entries)
        {
            this.standinRevlog = standinRevlog;
            this.standinRevlogReader = standinRevlogReader;
            this.storeBasePath = storeBasePath;
        }

        internal override void Add(HgRevlogEntry hgRevlogEntry)
        {
            standinRevlog.Add(hgRevlogEntry);
        }

        public override ReadOnlyCollection<HgRevlogEntry> Entries
        {
            get { return standinRevlog.Entries.Select(GetRevlogEntry).ToList().AsReadOnly(); }
        }

        public override IEnumerable<HgRevlogEntry> GetEntries(uint startRevision, uint endRevision)
        {
            return standinRevlog.GetEntries(startRevision, endRevision).Select(GetRevlogEntry);
        }

        public override HgRevlogEntry GetEntry(uint revision)
        {
            return GetRevlogEntry(standinRevlog.GetEntry(revision));
        }

        public override HgRevlogEntry GetEntry(HgNodeID nodeID)
        {
            return GetRevlogEntry(standinRevlog.GetEntry(nodeID));
        }

        public override HgRevlogEntry this[uint revision]
        {
            get { return GetRevlogEntry(standinRevlog[revision]); }
        }

        private HgRevlogEntry GetRevlogEntry(HgRevlogEntry revlogEntry)
        {
            var revlogEntryData = standinRevlogReader.ReadRevlogEntry(revlogEntry.NodeID);
            var data = revlogEntryData.Data;

            HgFileCopyInfo.ExtractFileCopyInfo(ref data);

            //
            // Apparently, entries in standins end with \n, so we explicitly limit to 40 bytes
            HgNodeID nodeID;
            if(HgNodeID.TryParse(Encoding.ASCII.GetString(data, 0, 40), out nodeID))
            {
                var lfPath = Path.Combine(storeBasePath, @"..\largefiles", nodeID.Long);
                var lf = new FileInfo(lfPath);
                if(lf.Exists)
                {
                    revlogEntry = new HgRevlogEntry(revlogEntry.Revision, revlogEntry.NodeID, revlogEntry.Offset,
                        revlogEntry.Flags, revlogEntry.CompressedLength, (uint)lf.Length, revlogEntry.BaseRevision, revlogEntry.LinkRevision,
                        revlogEntry.FirstParentRevision, revlogEntry.FirstParentRevisionNodeID, revlogEntry.SecondParentRevision, revlogEntry.SecondParentRevisionNodeID);
                } // if
            } // if
            
            return revlogEntry;
        }
    }
}