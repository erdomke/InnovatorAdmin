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

using HgSharp.Core.Util;
using HgSharp.Core.Util.IO;
using Ionic.BZip2;
using Ionic.Zlib;

namespace HgSharp.Core
{
    public class HgBundleReader
    {
        private readonly HgEncoder hgEncoder;

        public HgBundleReader(HgEncoder hgEncoder)
        {
            this.hgEncoder = hgEncoder;
        }

        public HgBundle ReadBundle(Stream stream)
        {   
            var uncompressedStream = GetUncompressedStream(stream);
            var binaryReader = new BigEndianBinaryReader(new BufferedStream(new NonClosingStreamWrapper(uncompressedStream), 1024 * 1024));
            var changelog = ReadBundleGroup(binaryReader);
            var manifest = ReadBundleGroup(binaryReader);
            var files = ReadBundleFiles(binaryReader); new List<HgBundleFile>();

            return new HgBundle(changelog, manifest, files);
        }

        private IEnumerable<HgBundleFile> ReadBundleFiles(BigEndianBinaryReader binaryReader)
        {
            uint fileNameLength;
            while((fileNameLength = binaryReader.ReadUInt32()) != 0)
            {
                var fileNameBytes = binaryReader.ReadBytes((int)fileNameLength - 4 /* For "file name length" value itself */);
                var fileName = hgEncoder.DecodeAsLocal(fileNameBytes);
                var fileGroup = ReadBundleGroup(binaryReader);

                var file = new HgBundleFile(new HgPath(fileName), fileGroup);
                yield return file;
            } // while
        }

        private Stream GetUncompressedStream(Stream stream)
        {
            var headerBuffer = new byte[6];
            stream.Read(headerBuffer, 0, 6);
            var header = Encoding.ASCII.GetString(headerBuffer);

            switch(header)
            {
                case "HG10UN":
                    return stream;
                case "HG10BZ":
                    //
                    // Seek two bytes back to get correct "BZ" signature
                    stream.Seek(-2, SeekOrigin.Current);
                    return new BZip2InputStream(stream);
                case "HG10GZ":
                    return new ZlibStream(stream, CompressionMode.Decompress);
                default:
                    //
                    // Old-style uncompressed bundle
                    if(stream.CanSeek)
                    {
                        stream.Position = 0;
                        return stream;
                    } // if

                    return new MergeStream(new MemoryStream(headerBuffer), stream);
            } // switch
        }

        private IEnumerable<HgChunk> ReadBundleGroup(IBinaryReader binaryReader)
        {
            const uint nullChunkMaxSize = 4;
            const int chunkHeaderSize = 84;

            uint length;
            while((length = binaryReader.ReadUInt32()) > nullChunkMaxSize)
            {
                var nodeID = new HgNodeID(binaryReader.ReadBytes(20));
                var firstParentNodeID = new HgNodeID(binaryReader.ReadBytes(20));
                var secondParentNodeID = new HgNodeID(binaryReader.ReadBytes(20));
                var changesetNodeID = new HgNodeID(binaryReader.ReadBytes(20));
                
                var dataLength = (int)length - chunkHeaderSize;
                
                var data = new byte[dataLength];
                binaryReader.Read(data, 0, dataLength);

                var chunk = new HgChunk(nodeID, firstParentNodeID, secondParentNodeID, changesetNodeID, data);
                yield return chunk;
            } // while
        }
    }
}