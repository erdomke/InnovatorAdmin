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
using System.Text;

using HgSharp.Core.Util;
using HgSharp.Core.Util.IO;
using Ionic.BZip2;
using Ionic.Zlib;

using NLog;

namespace HgSharp.Core
{
    public class HgBundleWriter
    {
        private readonly HgEncoder hgEncoder;
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public HgBundleWriter(HgEncoder hgEncoder)
        {
            this.hgEncoder = hgEncoder;
        }

        public void WriteBundle(HgBundle hgBundle, Stream stream, HgBundleFormat format, HgBundleCompression compression)
        {
            //
            // First things first -- we need to write out bundle header
            var header = 
                format == HgBundleFormat.BundlePre10 ?
                    null :
                    GetCompressionHeader(compression);
            
            if(header != null)
            {
                var headerBuffer = Encoding.ASCII.GetBytes(header);
                stream.Write(headerBuffer, 0, headerBuffer.Length);
            } // if

            using(var compressedStream = GetCompressedStream(stream, compression))
            using(var binaryWriter = new BigEndianBinaryWriter(new BufferedStream(compressedStream, 1024 * 128)))
            {
                log.Debug("writing changesets");
                WriteBundleGroup(hgBundle.Changelog, binaryWriter);
                binaryWriter.Flush();

                log.Debug("writing manifests");
                WriteBundleGroup(hgBundle.Manifest, binaryWriter);
                binaryWriter.Flush();
                //
                // Sometimes HgBundleFile.File has no chunks and Mercurial chokes on that.
                log.Debug("writing files");
                foreach(var file in hgBundle.Files)
                {
                    string filePath = null;
                    foreach(var chunk in file.File)
                    {
                        if(filePath == null)
                        {
                            filePath = file.Path.FullPath.TrimStart('/');
                            binaryWriter.Write((uint)filePath.Length + 4);
                            binaryWriter.Write(hgEncoder.EncodeAsLocal(filePath));

                            log.Debug("writing file '{0}'", filePath);
                        } // if

                        WriteChunk(binaryWriter, chunk);
                    } // foreach

                    if(filePath != null)
                        WriteZeroChunk(binaryWriter);

                    binaryWriter.Flush();
                } // foreach

                binaryWriter.Write((uint)0);
                
                binaryWriter.Flush();
                compressedStream.Flush();
                stream.Flush();
            } // using
        }

        private string GetCompressionHeader(HgBundleCompression compression)
        {
            switch(compression)
            {
                case HgBundleCompression.None:
                    return "HG10UN";
                case HgBundleCompression.BZip:
                    //
                    // Docs say that "HG10BZ" is the correct value, 
                    // but BZip2OutputStream writes that "BZ" all by itself
                    return "HG10";
                case HgBundleCompression.GZip:
                    return "HG10GZ";
                default:
                    throw new ArgumentOutOfRangeException("compression");
            } // switch
        }

        private Stream GetCompressedStream(Stream stream, HgBundleCompression compression)
        {
            switch(compression)
            {
                case HgBundleCompression.None:
                    return new NonClosingStreamWrapper(stream);
                case HgBundleCompression.GZip:
                    return new ZlibStream(stream, CompressionMode.Compress, CompressionLevel.BestCompression, true);
                case HgBundleCompression.BZip:
                    return new BZip2OutputStream(stream, true);
                default:
                    throw new ArgumentOutOfRangeException("compression");
            } // switch
        }

        private static void WriteBundleGroup(IEnumerable<HgChunk> chunks, IBinaryWriter binaryWriter)
        {
            foreach(var chunk in chunks)
            {
                WriteChunk(binaryWriter, chunk);
            } // foreach

            WriteZeroChunk(binaryWriter);
        }

        private static void WriteZeroChunk(IBinaryWriter binaryWriter)
        {
            binaryWriter.Write((uint)0); // Zero chunk
        }

        private static void WriteChunk(IBinaryWriter binaryWriter, HgChunk chunk)
        {
            const int chunkHeaderSize = 84;

            var chunkDataLength = chunk.Data.Length;

            binaryWriter.Write(chunkDataLength + chunkHeaderSize);
            binaryWriter.Write(chunk.NodeID.NodeID);
            binaryWriter.Write(chunk.FirstParentNodeID.NodeID);
            binaryWriter.Write(chunk.SecondParentNodeID.NodeID);
            binaryWriter.Write(chunk.ChangesetNodeID.NodeID);

            binaryWriter.Write(chunk.Data);
        }
    }
}