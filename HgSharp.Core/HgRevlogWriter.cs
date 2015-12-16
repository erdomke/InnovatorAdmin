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
using HgSharp.Core.Util.IO;
using Ionic.Zlib;

namespace HgSharp.Core
{
    public class HgRevlogWriter : HgRevlogSupport
    {
        private const uint SnapshottingFactor = 5;

        private readonly HgRevlog revlog;
        private readonly HgRevlogReader revlogReader;
        private readonly HgFileSystem fileSystem;

        public HgRevlogWriter(HgRevlog revlog, HgFileSystem fileSystem)
        {
            this.revlog = revlog;
            this.fileSystem = fileSystem;

            revlogReader = new HgRevlogReader(revlog, fileSystem);
        }

        private void WriteRevlog()
        {
            var directoryPath = Path.GetDirectoryName(revlog.IndexPath);
            Debug.Assert(directoryPath != null, "directoryPath != null");

            if(File.Exists(revlog.IndexPath)) return;

            if(!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            if(!revlog.InlineData) fileSystem.CreateWrite(revlog.DataPath).Close();

            using(var stream = fileSystem.CreateWrite(revlog.IndexPath))
            {
                using(var binaryWriter = new BigEndianBinaryWriter(stream))
                {
                    var header = NG;

                    if(revlog.InlineData) header |= InlineDataFlag;

                    binaryWriter.Write(header);
                    binaryWriter.Write((Int16)0);
                } // using
            } // using
        }
        
        public void WriteRevlogEntryDataRaw(HgNodeID revlogEntryNodeID, uint linkRevision, HgNodeID firstParentNodeID, HgNodeID secondParentNodeID, byte[] data,
            ref ulong offset, uint baseRevision, uint uncompressedLength, out uint compressedLength, IBinaryWriter indexBinaryWriter, IBinaryWriter dataBinaryWriter)
        {
            if(!File.Exists(revlog.IndexPath))
            {
                //
                // We need to decide whether we want inline data initially
                // No, it's not you who decides.
                // revlog.InlineData = data.Length < MaxInlineFileSize;
                WriteRevlog();
            } // if

            var tip = revlog.Entries.Count > 0 ? revlog.Entries[revlog.Entries.Count - 1] : null;
            if(tip != null && baseRevision < tip.BaseRevision)
            {
                throw new Exception("invalid base revision");
            } // if

            byte[] revlogEntryData = data;
            /*ulong offset = 0;
            uint baseRevision = 0;*/
            
            if(revlog.GetEntry(revlogEntryNodeID) != null)
            {
                compressedLength = 0;
                return;
            } // if

            var revlogEntryDataTuple = GetPotentiallyCompressedData(revlogEntryData, 0, revlogEntryData.Length);

            //var uncompressedLength = (uint)data.Length;
            compressedLength = (uint)(revlogEntryDataTuple.Item2.Length + (revlogEntryDataTuple.Item1 == null ? 0 : 1));
           
            var firstParentRevlogEntry = revlog.GetEntry(firstParentNodeID);
            var secondParentRevlogEntry = revlog.GetEntry(secondParentNodeID);

            var firstParentRevision = firstParentRevlogEntry == null ? uint.MaxValue : firstParentRevlogEntry.Revision;
            var secondParentRevision = secondParentRevlogEntry == null ? uint.MaxValue : secondParentRevlogEntry.Revision;

            var revlogEntry = new HgRevlogEntry((uint)revlog.Entries.Count, revlogEntryNodeID, offset, 0, 
                compressedLength, uncompressedLength, baseRevision, linkRevision, 
                firstParentRevision, firstParentNodeID, secondParentRevision, secondParentNodeID);

            revlog.Add(revlogEntry);

            log.Debug("writing revlog entry {0} for changeset r{1}", revlogEntryNodeID.Short, linkRevision);

            /*using(var fileStream = fileSystem.OpenWrite(revlog.IndexPath))
            {
                fileStream.Seek(fileStream.Length, SeekOrigin.Begin);

                using(var binaryWriter = new BigEndianBinaryWriter(fileStream))
                {*/
                    if(offset >= 0 && revlogEntry.Revision > 0)
                    {
                        var offsetBuffer = new byte[6];
                        Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)offset)), 2, offsetBuffer, 0, 6);
                        
                        indexBinaryWriter.Write(offsetBuffer);
                    } // if
                        
                    indexBinaryWriter.Write(revlogEntry.Flags);
                    indexBinaryWriter.Write(revlogEntry.CompressedLength);
                    indexBinaryWriter.Write(revlogEntry.UncompressedLength);
                    indexBinaryWriter.Write(revlogEntry.BaseRevision);
                    indexBinaryWriter.Write(revlogEntry.LinkRevision);
                    indexBinaryWriter.Write(revlogEntry.FirstParentRevision);
                    indexBinaryWriter.Write(revlogEntry.SecondParentRevision);

                    //
                    // Writing NodeID (20 bytes) and 12 more bytes (see HgRevlogReader)
                    indexBinaryWriter.Write(revlogEntry.NodeID.NodeID);
                    indexBinaryWriter.Write(new byte[12]);

                    if(revlog.InlineData)
                    {
                        if(revlogEntryDataTuple.Item1 != null)
                            indexBinaryWriter.Write(revlogEntryDataTuple.Item1.Value);
                        indexBinaryWriter.Write(revlogEntryDataTuple.Item2);
                    } // if
               /* } // using
            } // using

            if(!revlog.InlineData && uncompressedLength > 0)
            {
                using(var fileStream = fileSystem.OpenOrCreateWrite(revlog.DataPath))
                {
                    fileStream.Seek(fileStream.Length, SeekOrigin.Begin);

                    using(var binaryWriter = new BigEndianBinaryWriter(fileStream))*/
                    else
                    {
                        if(revlogEntryDataTuple.Item1 != null)
                            dataBinaryWriter.Write(revlogEntryDataTuple.Item1.Value);
                        dataBinaryWriter.Write(revlogEntryDataTuple.Item2);
                    /*} // using
                } // using*/
            } // if

            offset += compressedLength;
        }

        public HgNodeID WriteRevlogEntryData(uint linkRevision, HgNodeID firstParentNodeID, HgNodeID secondParentNodeID, byte[] data)
        {
            //
            // Get previous revision and diff against it
            byte[] revlogEntryData = null;
            ulong offset = 0;
            uint baseRevision = 0;
            
            if(revlog.Entries.Count > 0)
            {
                var lastRevlogEntry = revlog.Entries[revlog.Entries.Count - 1];
                
                baseRevision = lastRevlogEntry.BaseRevision;
                offset = lastRevlogEntry.Offset + lastRevlogEntry.CompressedLength;

                //
                // If the size of stored entries is comparable to the size of @data, store snapshot as well
                if(revlog.Entries.Count > 1)
                {
                    var accumulatedDataLength =
                        revlog.Entries.
                            Where(e => e.Revision > baseRevision && e.Revision <= revlog.Entries.Count).
                            Sum(e => e.CompressedLength);

                    log.Trace("accumulated {0}", accumulatedDataLength);

                    if(accumulatedDataLength > data.Length * SnapshottingFactor)
                    {   
                        baseRevision = (uint)revlog.Entries.Count;
                        revlogEntryData = data;

                        log.Trace("storing snapshot at r{0} - accumulated data length exceeds data length with factor of {1}", baseRevision, SnapshottingFactor);
                    } // if
                } // if

                if(revlogEntryData == null)
                {
                    var lastRevlogEntryData = revlogReader.ReadRevlogEntry(lastRevlogEntry.Revision);
                    var diff = BDiff.Diff(lastRevlogEntryData.Data, data);

                    //
                    // If a diff we got is actually bigger than data itself, store snapshot. Do not forget to set @baseRevision appropriately
                    if(diff.Length > data.Length)
                    {
                        baseRevision = (uint)revlog.Entries.Count;
                        revlogEntryData = data;

                        log.Trace("storing snapshot at r{0} - diff length exceeds data length", baseRevision);
                    } // if
                    else
                    {
                        revlogEntryData = diff;
                    } // else
                } // if
            } // if
            else
                revlogEntryData = data;

            var revlogEntryNodeID = GetRevlogEntryDataNodeID(firstParentNodeID, secondParentNodeID, data);
            if(revlog.GetEntry(revlogEntryNodeID) != null) return revlogEntryNodeID;
            
            if(!File.Exists(revlog.IndexPath))
            {
                //
                // We need to decide whether we want inline data initially
                // No, it's not you who decides.
                // revlog.InlineData = data.Length < MaxInlineFileSize;
                WriteRevlog();
            } // if

            using(var indexFileStream = fileSystem.OpenWrite(revlog.IndexPath))
            using(var dataFileStream = revlog.InlineData ? new NonClosingStreamWrapper(indexFileStream) : (Stream)fileSystem.OpenOrCreateWrite(revlog.DataPath))
            {
                indexFileStream.Seek(indexFileStream.Length, SeekOrigin.Begin);
                dataFileStream.Seek(dataFileStream.Length, SeekOrigin.Begin);

                using(var indexBinaryWriter = new BigEndianBinaryWriter(indexFileStream))
                using(var dataBinaryWriter = new BigEndianBinaryWriter(dataFileStream))
                {
                    uint compressedLength;
                    WriteRevlogEntryDataRaw(revlogEntryNodeID, linkRevision, firstParentNodeID, secondParentNodeID, 
                        revlogEntryData, ref offset, baseRevision, (uint)data.Length, out compressedLength,
                        indexBinaryWriter, dataBinaryWriter);
                } // using
            } // using

            return revlogEntryNodeID;
        }

        public int WriteRevlogEntries(IEnumerable<HgChunk> chunks, Func<HgNodeID, uint> linkRevisionProviderCallback, 
            Action<HgRevlogEntry> duplicateEntryHandlerCallback = null)
        {
            HgNodeID? prevNodeID = null;
            ulong offset = 0;
            long accumulatedCompressedLength = 0;
            var entries = 0;

            WriteRevlog();

            if(revlog.Entries.Count > 0)
            {
                var tip = revlog.Entries[revlog.Entries.Count - 1];
                offset = (uint)(tip.Offset + tip.CompressedLength);
            } // if

            using(var indexFileStream = fileSystem.OpenWrite(revlog.IndexPath))
            using(var dataFileStream = revlog.InlineData ? new NonClosingStreamWrapper(indexFileStream) : (Stream)fileSystem.OpenOrCreateWrite(revlog.DataPath))
            {
                indexFileStream.Seek(indexFileStream.Length, SeekOrigin.Begin);
                dataFileStream.Seek(dataFileStream.Length, SeekOrigin.Begin);

                using(var indexBinaryWriter = new BigEndianBinaryWriter(indexFileStream))
                using(var dataBinaryWriter = new BigEndianBinaryWriter(dataFileStream))
                {
                    var fullSnapshot = new byte[] { };
                    var requiresRediffing = false;

                    foreach(var chunk in chunks)
                    {
                        byte[] data = null;
                        entries++;

                        if(prevNodeID == null)
                        {
                            if(chunk.FirstParentNodeID == HgNodeID.Null)
                            {
                                fullSnapshot = MPatch.Patch(fullSnapshot, new List<byte[]> { chunk.Data });
                            } // if
                            else
                            {
                                var revlogEntry = revlogReader.ReadRevlogEntry(chunk.FirstParentNodeID);
                                fullSnapshot = MPatch.Patch(revlogEntry.Data, new List<byte[]> { chunk.Data });                                   
                            } // else
                        } // if 
                        else
                        {
                            fullSnapshot = MPatch.Patch(fullSnapshot, new List<byte[]> { chunk.Data });
                        } // else

                        var uncompressedLength = (uint)fullSnapshot.Length;

                        uint baseRevision = 0;
                        if(prevNodeID == null || requiresRediffing)
                        {
                            if(revlog.Entries.Count > 1)
                            {
                                var revlogEntry = revlog.Entries[revlog.Entries.Count - 1];
                                accumulatedCompressedLength =
                                    revlog.Entries.
                                        Where(e => e.Revision > revlogEntry.BaseRevision).
                                        Sum(e => e.CompressedLength);

                                if(accumulatedCompressedLength > fullSnapshot.Length * SnapshottingFactor)
                                {
                                    baseRevision = (uint)revlog.Entries.Count;
                                    data = fullSnapshot;
                                    accumulatedCompressedLength = 0;
                                } // if
                                else
                                {
                                    var revlogEntryData = revlogReader.ReadRevlogEntry((uint)revlog.Entries.Count - 1);
                                    data = BDiff.Diff(revlogEntryData.Data, fullSnapshot);
                                    baseRevision = revlogEntryData.Entry.BaseRevision;
                                } // else
                            }
                            else
                            {
                                baseRevision = (uint)revlog.Entries.Count;
                                data = fullSnapshot;
                                accumulatedCompressedLength = 0;
                            } // else

                            requiresRediffing = false;    
                        } // if
                        else
                        {   
                            var revlogEntry = revlog.Entries[revlog.Entries.Count - 1];
                            baseRevision = revlogEntry.BaseRevision;

                            if(accumulatedCompressedLength > fullSnapshot.Length * SnapshottingFactor)
                            {
                                baseRevision = (uint)revlog.Entries.Count;
                                accumulatedCompressedLength = 0;
                                data = fullSnapshot;
                            } // if
                            else
                            {
                                data = chunk.Data;
                            } // else
                        } // else
 
                        prevNodeID = chunk.NodeID;

                        var hgRevlogEntry = revlog.GetEntry(chunk.NodeID);
                        if(hgRevlogEntry == null)
                        {
                            var linkRevision = linkRevisionProviderCallback(chunk.ChangesetNodeID);
                            uint compressedLength;

                            WriteRevlogEntryDataRaw(chunk.NodeID, linkRevision, chunk.FirstParentNodeID, chunk.SecondParentNodeID, 
                                data, ref offset, baseRevision, uncompressedLength, out compressedLength,
                                indexBinaryWriter, dataBinaryWriter);

                            accumulatedCompressedLength += compressedLength;
                        } // if
                        else
                        {
                            if(duplicateEntryHandlerCallback != null)
                                duplicateEntryHandlerCallback(hgRevlogEntry);

                            requiresRediffing = true;
                        } // else
                    }
                } // using
            } // using

            return entries;
        }

        private Tuple<byte?, byte[]> GetPotentiallyCompressedData(byte[] uncompressedData, int offset, int count)
        {
            if(uncompressedData.Length == 0)
                return Tuple.Create((byte?)null, uncompressedData);

            if(uncompressedData.Length < 100)
                return GetUncompressedData(uncompressedData);
            
            var compressedData = Compress(uncompressedData, offset, count);
            if(compressedData.Length > uncompressedData.Length)
                return GetUncompressedData(uncompressedData);

            return Tuple.Create((byte?)null, compressedData);
        }

        private static Tuple<byte?, byte[]> GetUncompressedData(byte[] uncompressedData)
        {
            var marker =
                uncompressedData[0] == 0
                    ? (byte?)null
                    : (byte)'u';

            return Tuple.Create(marker, uncompressedData);
        }

        private byte[] Compress(byte[] revlogEntryData, int offset, int count)
        {
            using(var memoryStream = new MemoryStream())
            {
                using(var zlibStream = new ZlibStream(memoryStream, CompressionMode.Compress))
                {
                    zlibStream.Write(revlogEntryData, offset, count);
                    zlibStream.Flush();
                } // using

                return memoryStream.ToArray();
            } // using
        }
    }
}
