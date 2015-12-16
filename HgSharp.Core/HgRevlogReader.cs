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
using System.IO;

using System.Linq;
using System.Net;

using HgSharp.Core.Util;

using Ionic.Zlib;

namespace HgSharp.Core
{
    public class HgRevlogReader : HgRevlogSupport, IHgRevlogReader
    {
        private readonly HgRevlog revlog;
        private readonly HgFileSystem fileSystem;

        public HgRevlogReader(HgRevlog revlog, HgFileSystem fileSystem)
        {
            this.revlog = revlog;
            this.fileSystem = fileSystem;
        }

        public virtual HgRevlogEntryData ReadRevlogEntry(HgNodeID nodeID)
        {
            var n = new HgNodeID(nodeID.NodeID);
            var matchingEntries = revlog.Entries.Where(e => e.NodeID.Equals(n));
                
            var revlogEntry = matchingEntries.FirstOrDefault();
            if(revlogEntry == null) return null;

            return ReadRevlogEntry(revlogEntry.Revision);
        }

        public IEnumerable<HgRevlogEntryData> ReadRevlogEntries(HgRevset hgRevset)
        {
            if(hgRevset.Empty) yield break;

            var revisions = hgRevset.Select(hre => hre.Revision);
            foreach(var revlogEntry in ReadRevlogEntries(revisions))
                yield return revlogEntry;
        }

        public virtual IEnumerable<HgRevlogEntryData> ReadRevlogEntries(IEnumerable<uint> revisions)
        {
            //
            // See http://stackoverflow.com/a/10359273/60188. Pure magic
            var revisionChunks = 
                revisions.
                    OrderBy(r => r).
                    Select((r, i) => new { r, i }).
                    GroupBy(x => x.r - x.i). //this line will group consecutive nums in the seq
                    Select(x => x.Select(xx => xx.r)).
                    Select(c => c.ToArray()).
                    ToArray();

            HgRevlogEntryData pivotRevlogEntryData = null;

            foreach(var revisionChunk in revisionChunks)
            {
                var startRevision = revisionChunk[0];
                var endRevision = revisionChunk[revisionChunk.Length - 1];

                foreach(var revlogEntry in ReadRevlogEntries(startRevision, endRevision, pivotRevlogEntryData))
                    yield return (pivotRevlogEntryData = revlogEntry);
            } // foreach
        }

        public virtual IEnumerable<HgRevlogEntryData> ReadRevlogEntries(UInt32 startRevision, UInt32 endRevision)
        {
            return ReadRevlogEntries(startRevision, endRevision, null);
        }

        private IEnumerable<HgRevlogEntryData> ReadRevlogEntries(UInt32 startRevision, UInt32 endRevision, HgRevlogEntryData pivotRevlogEntryData)
        {
            byte[] buffer;
            bool usedPivot;
            var revlogEntries = ReadRevlogEntriesBuffer(startRevision, endRevision, out buffer, out usedPivot, pivotRevlogEntryData);

            //
            // Do not pass pivotRevlogEntryData if it was not used in the first place
            var effectivePivotRevlogEntryData = pivotRevlogEntryData;
            if(!usedPivot)// pivotRevlogEntryData != null && pivotRevlogEntryData.Entry.Revision < revlogEntries[0].Revision)
                effectivePivotRevlogEntryData = null;

            return ReadRevlogEntries(startRevision, endRevision, revlogEntries, buffer, effectivePivotRevlogEntryData);
        }

        private List<HgRevlogEntry> ReadRevlogEntriesBuffer(uint startRevision, uint endRevision, out byte[] buffer, out bool usedPivot, HgRevlogEntryData pivotRevlogEntryData = null)
        {
            var effectiveStartRevision = GetEffectiveStartRevision(startRevision);

            usedPivot = false;
            if(pivotRevlogEntryData != null && effectiveStartRevision <= pivotRevlogEntryData.Entry.Revision)
            {
                effectiveStartRevision = pivotRevlogEntryData.Entry.Revision + 1;
                usedPivot = true;
            } // if

            var revlogEntries = revlog.GetEntries(effectiveStartRevision, endRevision).ToList();

            var offsetIncrement = revlog.InlineData ? RevlogNgEntryHeaderLength * (revlogEntries[0].Revision + 1) : 0;
            var lengthIncrement = revlog.InlineData ? RevlogNgEntryHeaderLength * revlogEntries.Count : 0;

            var offset = (int)revlogEntries[0].Offset + offsetIncrement; // Where to start reading
            var length = revlogEntries.Sum(r => r.CompressedLength) + lengthIncrement;

            var revlogPath = revlog.InlineData ? revlog.IndexPath : revlog.DataPath;
            buffer = new byte[length];

            using(var fileStream = fileSystem.OpenRead(revlogPath))
            {
                fileStream.Seek(offset, SeekOrigin.Begin);
                fileStream.Read(buffer, 0, (int)length);
            } // using

            return revlogEntries;
        }

        private uint GetEffectiveStartRevision(uint startRevision)
        {
            var startRevlogEntry = revlog.GetEntry(startRevision);
            
            if(startRevlogEntry.BaseRevision != startRevlogEntry.Revision)
                startRevlogEntry = revlog.GetEntry(startRevlogEntry.BaseRevision);
            
            return startRevlogEntry.Revision;
        }

        private IEnumerable<HgRevlogEntryData> ReadRevlogEntries(uint startRevision, uint endRevision, List<HgRevlogEntry> revlogEntries, byte[] buffer, HgRevlogEntryData pivotRevlogEntryData = null)
        {
            var revlogEntryData = pivotRevlogEntryData;

            var dataOffset = 0;
            var effectiveStartRevision = revlogEntries[0].Revision;

            if(startRevision > 0 && ((pivotRevlogEntryData == null && revlogEntries[0].BaseRevision < startRevision - 1) || (pivotRevlogEntryData != null && pivotRevlogEntryData.Entry.Revision < startRevision - 1)))
            {
                var baseRevlogEntryData = 
                    pivotRevlogEntryData ?? ReadRevlogEntryData(revlogEntries[0], buffer, ref dataOffset);
                var patchingRevlogEntries = 
                    revlogEntries.
                        Skip(pivotRevlogEntryData == null ? 1 : 0).
                        Where(re => re.Revision <= startRevision).
                        Select(re => ReadRevlogEntryData(re, buffer, ref dataOffset)).
                        ToList();

                if(patchingRevlogEntries.Count == 0)
                {
                    revlogEntryData = baseRevlogEntryData;
                } // if
                else
                {
                    if(patchingRevlogEntries.Any(re => re.Entry.BaseRevision == re.Entry.Revision))
                    {
                        throw new Exception();
                    }

                    var patches =
                        patchingRevlogEntries.
                            Select(re => re.Data).
                            ToArray();

                    revlogEntryData = 
                        new HgRevlogEntryData(
                            patchingRevlogEntries.Last().Entry,
                            MPatch.Patch(baseRevlogEntryData.Data, patches));

                    yield return revlogEntryData;

                    effectiveStartRevision = revlogEntryData.Entry.Revision + 1;
                } // else
            } // if

            var unpatchedRevlogEntries = revlogEntries.Where(re => re.Revision >= effectiveStartRevision).ToList();
            foreach(var revlogEntry in unpatchedRevlogEntries)
            {
                if(revlogEntryData != null && revlogEntryData.Entry.Revision == revlogEntry.Revision)
                {
                    yield return revlogEntryData;
                } // if
                else if(revlogEntry.BaseRevision == revlogEntry.Revision)
                {
                    revlogEntryData = ReadRevlogEntryData(revlogEntry, buffer, ref dataOffset);
                } // else if
                else
                {
                    Debug.Assert(revlogEntryData != null, "revlogEntryData != null");

                    var patchRevlogEntryData = ReadRevlogEntryData(revlogEntry, buffer, ref dataOffset);
                    var patchedRevlogEntryData = MPatch.Patch(revlogEntryData.Data, new List<byte[]> { patchRevlogEntryData.Data });

                    revlogEntryData = new HgRevlogEntryData(revlogEntry, patchedRevlogEntryData);
                } // else

                //ValidateIntegrity(revlogEntryData);

                if(revlogEntryData.Entry.Revision >= startRevision && revlogEntryData.Entry.Revision <= endRevision)
                    yield return revlogEntryData;
            } // while
        }

        private HgRevlogEntryData ReadRevlogEntryData(HgRevlogEntry revlogEntry, byte[] buffer, ref int dataOffset)
        {
            //
            // Somehow this is possible
            if(revlogEntry.CompressedLength == 0)
            {
                if(revlog.InlineData) dataOffset += RevlogNgEntryHeaderLength;
                return new HgRevlogEntryData(revlogEntry, new byte[]{});
            } // if

            var marker = buffer[dataOffset];
            byte[] revlogEntryData;
                
            switch(marker)
            {
                case (byte)'x':
                    revlogEntryData = Decompress(buffer, dataOffset, (int)revlogEntry.CompressedLength);
                    
                    dataOffset += (int)revlogEntry.CompressedLength;
                    break;
                case 0:
                    revlogEntryData = new byte[revlogEntry.CompressedLength];
                    Buffer.BlockCopy(buffer, dataOffset, revlogEntryData, 0, (int)revlogEntry.CompressedLength);
                    
                    dataOffset += (int)revlogEntry.CompressedLength;
                    break;
                case (byte)'u':
                    revlogEntryData = new byte[revlogEntry.UncompressedLength];
                    Buffer.BlockCopy(buffer, dataOffset + 1, revlogEntryData, 0, (int)revlogEntry.UncompressedLength);
                    
                    dataOffset += (int)revlogEntry.UncompressedLength + 1;
                    break;
                default:
                    throw new ApplicationException();
            } // switch

            //
            // We already read past first revlogng header
            if(revlog.InlineData) dataOffset += RevlogNgEntryHeaderLength;

            return new HgRevlogEntryData(revlogEntry, revlogEntryData);
        }

        private byte[] Decompress(byte[] buffer, int offset, int count)
        {
            using(var memoryStream = new MemoryStream())
            {
                using(var zlibStream = new ZlibStream(memoryStream, CompressionMode.Decompress))
                {
                    zlibStream.Write(buffer, offset, count);
                    zlibStream.Flush();
                } // using

                return memoryStream.ToArray();
            } // using
        }

        public virtual HgRevlogEntryData ReadRevlogEntry(UInt32 revision)
        {
            return ReadRevlogEntries(revision, revision).FirstOrDefault();
        }

        private void ValidateIntegrity(HgRevlogEntryData revlogEntryData)
        {
            var node = GetRevlogEntryDataNodeID(revlogEntryData);

            if(node != revlogEntryData.Entry.NodeID)
            {
                // TODO : Specialized exception class
                throw new ApplicationException(string.Format("integrity violation at r{0}: {1} vs {2}", 
                    revlogEntryData.Entry.Revision, revlogEntryData.Entry.NodeID.Long, node.Long));
            } // if
        }

        public static HgRevlog ReadRevlog(string indexFilePath, string dataFilePath, HgFileSystem fileSystem)
        {
            if(!File.Exists(indexFilePath)) return null;

            using(var fileStream = new BufferedStream(fileSystem.OpenRead(indexFilePath), 2 * 1024 * 1024))
            using(var binaryReader = new BigEndianBinaryReader(fileStream))
            {
                //
                // Read version and two more bytes (6 bytes total)
                var header = binaryReader.ReadInt32();
                binaryReader.ReadInt16();

                var inlineData = (header & InlineDataFlag) != 0;
                var revlogFormat = header & 0xFFFF;
                
                if(revlogFormat != NG) throw new ApplicationException("Revlog format MUST be NG");

                var length = fileStream.Length - RevlogNgEntryHeaderLength;

                UInt32 revision = 0;
                var offset = 0L;
                
                var revlogEntries = new List<HgRevlogEntry>(1000);

                while(offset <= length) 
                {
                    var entry = ReadRevlogEntry(binaryReader, revision++, revlogEntries);
                    revlogEntries.Add(entry);

                    if(inlineData)
                    {
                        offset += entry.CompressedLength;
                        binaryReader.BaseStream.Seek(entry.CompressedLength, SeekOrigin.Current);
                    } // if

                    offset += RevlogNgEntryHeaderLength;
                } // while

                var revlog = new HgRevlog(indexFilePath, dataFilePath, header, inlineData, revlogEntries);
                return revlog;
            } // using
        }

        private static HgRevlogEntry ReadRevlogEntry(IBinaryReader binaryReader, uint revision, IList<HgRevlogEntry> revlogEntries)
        {
            //
            // From http://mercurial.selenic.com/wiki/RevlogNG:
            //
            //  As the offset of the first data chunk is always zero, the first 4 bytes (part of the offset) are used to indicate revlog 
            //      version number and flags. All values are in big endian format.
            ulong offset = 0; 
            if(revision > 0)
            {
                var offsetBuffer = new byte[8];
                binaryReader.Read(offsetBuffer, 2, 6);
                
                offset = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt64(offsetBuffer, 0));
            } // if
            
            var flags = binaryReader.ReadUInt16();
            var compressedLength = binaryReader.ReadUInt32();
            var uncompressedLength = binaryReader.ReadUInt32();
            var baseRevision = binaryReader.ReadUInt32();
            var linkRevision = binaryReader.ReadUInt32();
            var firstParentRevision = binaryReader.ReadUInt32();
            var secondParentRevision = binaryReader.ReadUInt32(); 

            //
            // See http://selenic.com/pipermail/mercurial/2012-April/042501.html. We have to read 32 bytes, but
            // only 20 of them are used
            var nodeIDBytes = new byte[NodeIDLength];
            binaryReader.Read(nodeIDBytes, 0, NodeIDLength);
            
            var nodeID = new HgNodeID(nodeIDBytes, NodeIDEffectiveLength);
            var firstParentRevlogEntry = firstParentRevision != uint.MaxValue ? revlogEntries[(int)firstParentRevision] : null;
            var firstParentRevisionNodeID = firstParentRevlogEntry == null ? HgNodeID.Null : firstParentRevlogEntry.NodeID;

            var secondParentRevlogEntry = secondParentRevision != uint.MaxValue ? revlogEntries[(int)secondParentRevision] : null;
            var secondParentRevisionNodeID = secondParentRevlogEntry == null ? HgNodeID.Null : secondParentRevlogEntry.NodeID;

            return new HgRevlogEntry(revision, nodeID, offset, flags, compressedLength, uncompressedLength, baseRevision, linkRevision, 
                firstParentRevision, firstParentRevisionNodeID, secondParentRevision, secondParentRevisionNodeID);
        }

        internal class HgRevisionReverseComparer : IComparer<uint>
        {
            public int Compare(uint x, uint y)
            {
                return y.CompareTo(x);
            }
        }

        public virtual byte[] ReadRevlogEntryDataRaw(uint revision)
        {
            var revlogEntry = revlog[revision];
            var offsetIncrement = revlog.InlineData ? RevlogNgEntryHeaderLength * (revision + 1) : 0;

            var offset = (int)revlogEntry.Offset + offsetIncrement; // Where to start reading
            var length = revlogEntry.CompressedLength;

            var revlogPath = revlog.InlineData ? revlog.IndexPath : revlog.DataPath;
            var buffer = new byte[length];

            using(var fileStream = fileSystem.OpenRead(revlogPath))
            {
                fileStream.Seek(offset, SeekOrigin.Begin);
                fileStream.Read(buffer, 0, (int)length);
            } // using

            int dataOffset = 0;
            return ReadRevlogEntryData(revlogEntry, buffer, ref dataOffset).Data;
        }
    }
}