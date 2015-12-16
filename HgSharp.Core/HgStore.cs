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
using System.Threading;

using NLog;

namespace HgSharp.Core
{
    public class HgStore
    {
        private readonly string storeBasePath;
        private readonly HgFileSystem fileSystem;
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly HgPathEncoder hgPathEncoder;

        internal virtual string StoreBasePath
        {
            get { return storeBasePath; }
        }

        internal virtual HgFileSystem FileSystem
        {
            get { return fileSystem; }
        }

        public virtual HgEncoder Encoder { get; private set; }

        public HgStore(HgEncoder hgEncoder, string storeBasePath, HgFileSystem fileSystem)
        {
            Encoder = hgEncoder;
            
            this.storeBasePath = storeBasePath;
            this.fileSystem = fileSystem;

            hgPathEncoder = new HgPathEncoder(HgPathEncodings.None, hgEncoder);
        }

        public virtual IHgRevlogReader CreateRevlogReader(HgRevlog hgRevlog)
        {
            return new HgRevlogReader(hgRevlog, fileSystem);
        }

        public virtual string ResolveFilePath(string storeRelativePath)
        {
            return Path.Combine(storeBasePath, storeRelativePath);
        }

        public virtual HgChangelog GetChangelog()
        {
            var revlog = GetMetaRevlog("changelog");
            return new HgChangelog(revlog, new HgRevlogReader(revlog, FileSystem), Encoder);
        }

        public virtual HgManifest GetManifest()
        {
            var revlog = GetMetaRevlog("manifest");
            return new HgManifest(revlog, new HgRevlogReader(revlog, FileSystem), Encoder);
        }

        public virtual HgFilelog GetFilelog(HgPath hgPath)
        {
            var filelogIndexName = hgPath.FullPath + ".i";
            var filelogDataName = hgPath.FullPath + ".d";

            var revlog = GetRevlog(filelogIndexName, filelogDataName);
            return new HgFilelog(revlog, hgPath, new HgRevlogReader(revlog, FileSystem));
        }

        public virtual HgFilelog CreateFilelog(HgPath hgPath)
        {
            var filelogIndexName = hgPath.FullPath + ".i";
            var filelogDataName = hgPath.FullPath + ".d";

            var revlog = CreateRevlog(filelogIndexName, filelogDataName);
            return new HgFilelog(revlog, hgPath, new HgRevlogReader(revlog, FileSystem));
        }

        internal virtual void Enlist(HgChangelog hgChangelog, HgTransaction hgTransaction)
        {
            log.Debug("enlisting changelog");

            hgTransaction.AddJournalEntry("00changelog.i", hgChangelog.Revlog.IndexPath);

            if(!hgChangelog.Revlog.InlineData)
                hgTransaction.AddJournalEntry("00changelog.d", hgChangelog.Revlog.DataPath);
        }

        internal virtual void Enlist(HgManifest hgManifest, HgTransaction hgTransaction)
        {
            log.Debug("enlisting manifest");

            hgTransaction.AddJournalEntry("00manifest.i", hgManifest.Revlog.IndexPath);

            if(!hgManifest.Revlog.InlineData)
                hgTransaction.AddJournalEntry("00manifest.d", hgManifest.Revlog.DataPath);
        }

        internal virtual void Enlist(HgFilelog hgFilelog, HgTransaction hgTransaction)
        {
            log.Debug("enlisting filelog '{0}'", hgFilelog.Path.FullPath);

            hgTransaction.AddJournalEntry("data/" + hgFilelog.Path.FullPath.TrimStart('/') + ".i", hgFilelog.Revlog.IndexPath);

            if(!hgFilelog.Revlog.InlineData)
                hgTransaction.AddJournalEntry("data/" + hgFilelog.Path.FullPath.TrimStart('/') + ".d", hgFilelog.Revlog.DataPath);
        }

        protected virtual HgRevlog GetMetaRevlog(string name)
        {
            var metaRevlogIndexPath = Path.Combine(storeBasePath, string.Format("00{0}.i", name));
            var metaRevlogDataPath = Path.Combine(storeBasePath, string.Format("00{0}.d", name));

            return GetRevlog(metaRevlogIndexPath, metaRevlogDataPath) ?? CreateRevlog(metaRevlogIndexPath, metaRevlogDataPath);
        }

        protected virtual HgRevlog GetRevlog(string revlogIndexPath, string revlogDataPath)
        {
            var i = Path.Combine(storeBasePath, revlogIndexPath);
            var d = Path.Combine(storeBasePath, revlogDataPath);

            if(!File.Exists(i)) return null;

            var revlog = HgRevlogReader.ReadRevlog(i, d, FileSystem);
            return revlog;
        }

        protected virtual HgRevlog CreateRevlog(string revlogIndexPath, string revlogDataPath)
        {
            log.Trace("creating revlog at {0} and {1}", revlogIndexPath, revlogDataPath);

            var i = Path.Combine(storeBasePath, revlogIndexPath);
            var d = Path.Combine(storeBasePath, revlogDataPath);

            var revlog = new HgRevlog(i, d, HgRevlogReader.NG, false, new HgRevlogEntry[] { });
            return revlog;
        }

        public virtual IList<HgDataFile> GetDataFiles()
        {
            var dataDirectory = new DirectoryInfo(storeBasePath);

            var dataFiles =
                dataDirectory.
                    GetFiles("*.i", SearchOption.AllDirectories).
                    Union(dataDirectory.GetFiles("*.d", SearchOption.AllDirectories)).
                    Select(f => new {
                        fullName = f.FullName.Substring(storeBasePath.Length + 1).Replace('\\', '/'), 
                        legth = f.Length
                    }).
                    OrderBy(f => f.fullName).
                    Select(f => 
                        new HgDataFile(
                            new HgPath(f.fullName), 
                            hgPathEncoder.EncodePath(f.fullName), /* Files under /data are by definition encoded with a reversible encoding */ 
                            f.legth)).
                    ToList();

            return dataFiles;
        }

        public virtual HgLock AcquireLock(TimeSpan timeout)
        {
            log.Info("acquiring lock at '{0}'", storeBasePath);

            var lockFilePath = Path.Combine(storeBasePath, "lock");
            var timeoutExpiresAt = DateTime.UtcNow.Add(timeout);
            
            //
            // In ASP.NET we'll be running under same process, so this locking
            // is unreliable. We also need to include ManagedThreadId, but
            // due to ASP.NET Thread Agility this can be problematic too.
            // TODO: Fix this
            var lockContent = string.Format("{0}:{1}@{2}", 
                Environment.MachineName, 
                Thread.CurrentThread.ManagedThreadId, 
                Process.GetCurrentProcess().Id);

            while(DateTime.UtcNow < timeoutExpiresAt)
            {
                try
                {
                    //
                    // First, try to create a file. If that succeeds, this means we've successfully
                    // acquired a lock
                    using(var lockStream = new StreamWriter(fileSystem.CreateWrite(lockFilePath)))
                        lockStream.Write(lockContent);

                    return new HgLock(lockFilePath, true);
                } // try
                catch(IOException e)
                {
                    log.Info("couldn't acquire lock: {0}", e.Message);

                    try
                    {
                        //
                        // Lock has already been acquired by someone else. If that's us,
                        // just return a new instance of a non-owning HgLock. Otherwise, wait.
                        using(var lockStream = new StreamReader(fileSystem.OpenRead(lockFilePath)))
                        {
                            if(lockStream.ReadLine() == lockContent)
                                return new HgLock(lockFilePath, false);
                        } // using
                    } // try
                    catch(IOException ex)
                    {
                        log.Info("couldn't acquire lock: {0}", ex.Message);
                    } // catch
                } // catch

                Thread.Sleep(TimeSpan.FromMilliseconds(250));
            } // while

            throw new Exception("lock timeout");
        }
    }
}
