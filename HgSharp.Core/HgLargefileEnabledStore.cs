using System;
using System.Collections.Generic;

namespace HgSharp.Core
{
    public class HgLargefileEnabledStore : HgStore
    {
        private readonly HgStore underlyingStore;

        public HgLargefileEnabledStore(HgStore underlyingStore) :
            base(underlyingStore.Encoder, underlyingStore.StoreBasePath, underlyingStore.FileSystem)
        {
            this.underlyingStore = underlyingStore;
        }

        public override IHgRevlogReader CreateRevlogReader(HgRevlog hgRevlog)
        {
            var hgRevlogReader = base.CreateRevlogReader(hgRevlog);
            if(!(hgRevlog is HgLargefilesEnabledRevlog)) return hgRevlogReader;

            //
            // We need to do a dirty little trick here. Since HgLargefilesEnabledRevlog essentially
            // "lies" about UncompressedLength, we need to "unwrap" it back
            var standinRevlog = ((HgLargefilesEnabledRevlog)hgRevlog).StandinRevlog;

            return new HgLargefilesEnabledRevlogReader(StoreBasePath, standinRevlog, base.CreateRevlogReader(standinRevlog));

        }

        public override HgLock AcquireLock(TimeSpan timeout)
        {
            return underlyingStore.AcquireLock(timeout);
        }

        public override HgFilelog CreateFilelog(HgPath hgPath)
        {
            return underlyingStore.CreateFilelog(hgPath);
        }

        public override HgEncoder Encoder
        {
            get { return underlyingStore.Encoder; }
        }

        internal override void Enlist(HgChangelog hgChangelog, HgTransaction hgTransaction)
        {
            underlyingStore.Enlist(hgChangelog, hgTransaction);
        }

        internal override void Enlist(HgManifest hgManifest, HgTransaction hgTransaction)
        {
            underlyingStore.Enlist(hgManifest, hgTransaction);
        }

        internal override void Enlist(HgFilelog hgFilelog, HgTransaction hgTransaction)
        {
            underlyingStore.Enlist(hgFilelog, hgTransaction);
        }

        internal override HgFileSystem FileSystem
        {
            get { return underlyingStore.FileSystem; }
        }

        public override HgChangelog GetChangelog()
        {
            return underlyingStore.GetChangelog();
        }

        public override IList<HgDataFile> GetDataFiles()
        {
            return underlyingStore.GetDataFiles();
        }

        public override HgFilelog GetFilelog(HgPath hgPath)
        {
            var hglfPath = HgPath.Combine(".hglf", hgPath);
            var standinFilelog = underlyingStore.GetFilelog(hglfPath);

            if(standinFilelog == null) return underlyingStore.GetFilelog(hgPath);

            var standinRevlogReader = new HgRevlogReader(standinFilelog.Revlog, FileSystem);
            var largefileRevlog = new HgLargefilesEnabledRevlog(standinFilelog.Revlog, standinRevlogReader, StoreBasePath);

            standinFilelog = new HgLargefilesEnabledFilelog(largefileRevlog, hglfPath, standinRevlogReader);

            return standinFilelog;
        }

        public override HgManifest GetManifest()
        {
            return new HgLargefilesEnabledManifest(underlyingStore.GetManifest());
        }

        public override string ResolveFilePath(string storeRelativePath)
        {
            return underlyingStore.ResolveFilePath(storeRelativePath);
        }

        internal override string StoreBasePath
        {
            get { return underlyingStore.StoreBasePath; }
        }
    }
}