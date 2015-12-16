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

namespace HgSharp.Core
{
    public class HgFnCacheStore : HgStore
    {
        private readonly string fnCacheFilePath;
        private readonly IList<string> fnCache;
        private readonly HgPathEncoder hgPathEncoder;
 
        public HgFnCacheStore(HgPathEncodings encodings, HgEncoder hgEncoder, string storeBasePath, HgFileSystem fileSystem) :
            base(hgEncoder, storeBasePath, fileSystem)
        {
            fnCacheFilePath = Path.Combine(StoreBasePath, "fncache");
            hgPathEncoder = new HgPathEncoder(encodings, hgEncoder);

            if(File.Exists(fnCacheFilePath))
            {
                var fnCacheEntries =
                    File.ReadAllText(fnCacheFilePath, hgEncoder.Local).
                        Split('\n').
                        Select(n => HgPathEncoder.DecodeDirectory(n.Trim()));

                fnCache = new List<string>(fnCacheEntries);
            } // if
            else
                fnCache = new List<string>();
        }

        public override IList<HgDataFile> GetDataFiles()
        {
            var dataFilePaths = fnCache.Append(new[] { "00changelog.i", "00changelog.d", "00manifest.i", "00manifest.d", "fncache" }).ToList();
            return 
                dataFilePaths.
                    Select(f => new {
                        storeRelativePath = Path.Combine(StoreBasePath, hgPathEncoder.EncodePath(f)),
                        path = new HgPath(f),
                    }).
                    Select(f => new {
                        f.storeRelativePath,
                        f.path,
                        resolvedPath = ResolveFilePath(f.storeRelativePath)
                    }).
                    OrderBy(f => f.resolvedPath).
                    Where(f => File.Exists(f.resolvedPath)).
                    Select(f => new {
                        f.storeRelativePath,
                        f.path,
                        length = new FileInfo(f.resolvedPath).Length
                    }).
                    Select(f => new HgDataFile(f.path, f.storeRelativePath, f.length)).
                    ToList();
        }

        public override string ResolveFilePath(string storeRelativePath)
        {
            var encodedStoreRelativePath = hgPathEncoder.EncodePath(storeRelativePath);
            return base.ResolveFilePath(encodedStoreRelativePath);
        }
        
        public override HgFilelog GetFilelog(HgPath hgPath)
        {
            //
            // According to http://mercurial.selenic.com/wiki/fncacheRepoFormat:
            //
            //  The fncache file may contain duplicates or inexistent entries (this can happen when using the strip or rollback commmands).
            //
            // So there's no need to enlist it in a transaction or somesuch
            var filelogIndexPath = "data/" + hgPath.FullPath.TrimStart('/') + ".i";
            var filelogDataPath = "data/" + hgPath.FullPath.TrimStart('/') + ".d";

            var hgRevlog = GetRevlog(hgPathEncoder.EncodePath(filelogIndexPath), hgPathEncoder.EncodePath(filelogDataPath));
            if(hgRevlog == null)
            {
                fnCache.Remove(filelogIndexPath);
                fnCache.Remove(filelogDataPath);

                return null;
            } // if

            if(!fnCache.Contains(filelogIndexPath))
            {
                using(var streamWriter = new StreamWriter(FileSystem.OpenOrCreateWrite(fnCacheFilePath), Encoder.Local))
                {
                    streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                    streamWriter.Write(hgPathEncoder.EncodeDirectory(filelogIndexPath) + '\n');
                } // using

                fnCache.Add(filelogIndexPath);
            } // if

            if(!fnCache.Contains(filelogDataPath) && !hgRevlog.InlineData)
            {
                using(var fileStream = new FileStream(fnCacheFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                using(var streamWriter = new StreamWriter(fileStream, Encoder.Local))
                {
                    streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                    streamWriter.Write(hgPathEncoder.EncodeDirectory(filelogDataPath) + '\n');
                } // using

                fnCache.Add(filelogDataPath);
            } // if

            return new HgFilelog(hgRevlog, hgPath, new HgRevlogReader(hgRevlog, FileSystem));
        }

        public override HgFilelog CreateFilelog(HgPath hgPath)
        {
            var filelogIndexPath = "data/" + hgPath.FullPath.TrimStart('/') + ".i";
            var filelogDataPath = "data/" + hgPath.FullPath.TrimStart('/') + ".d";

            var hgRevlog = CreateRevlog(hgPathEncoder.EncodePath(filelogIndexPath), hgPathEncoder.EncodePath(filelogDataPath));

            if(!fnCache.Contains(filelogIndexPath))
            {
                using(var streamWriter = new StreamWriter(FileSystem.OpenOrCreateWrite(fnCacheFilePath), Encoder.Local))
                {
                    streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                    streamWriter.Write(hgPathEncoder.EncodeDirectory(filelogIndexPath) + '\n');
                } // using

                fnCache.Add(filelogIndexPath);
            } // if

            if(!fnCache.Contains(filelogDataPath) && !hgRevlog.InlineData)
            {
                using(var fileStream = new FileStream(fnCacheFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                using(var streamWriter = new StreamWriter(fileStream, Encoder.Local))
                {
                    streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                    streamWriter.Write(hgPathEncoder.EncodeDirectory(filelogDataPath) + '\n');
                } // using

                fnCache.Add(filelogDataPath);
            } // if

            return new HgFilelog(hgRevlog, hgPath, new HgRevlogReader(hgRevlog, FileSystem));
        }
    }
}