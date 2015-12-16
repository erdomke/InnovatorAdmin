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
using System.Collections.ObjectModel;
using System.IO;

using System.Linq;
using System.Text;

using HgSharp.Core.Util;

using NLog;

namespace HgSharp.Core
{
    public class HgRepository
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly string basePath;
        private readonly HgTagManager tagManager;
        private HgChangelog changelog;
        private HgManifest manifest;
        private readonly HgStore store;
        private readonly HgFileSystem fileSystem;
        private readonly HgFileSystem atomicFileSystem;
        private readonly HgBranchManager branchManager;
        private readonly HgBookmarkManager bookmarkManager;
    
        public HgBundleCommittedEventHandler BundleCommitting;

        public HgBundleCommittedEventHandler BundleCommitted;

        public HgStore Store { get { return store; } }

        public HgRequirements Requirements { get; private set; }

        public HgRc Rc { get; private set; }

        /// <summary>
        /// Gets a full path to <c>.hg</c>.
        /// </summary>
        internal string BasePath
        {
            get { return  basePath;}
        }

        public HgChangelog Changelog
        {
            get { return changelog ?? (changelog = Store.GetChangelog()); }
        }

        public HgManifest Manifest
        {
            get { return manifest ?? (manifest = Store.GetManifest()); }
        }

        public HgChangeset this[string changeset]
        {
            get
            {
                var branch = GetBranches().OrderByDescending(b => b.Metadata.Revision).FirstOrDefault(b => string.Equals(b.Branch.Name, changeset, StringComparison.InvariantCulture));
                if(branch != null)
                    return branch;

                var bookmark = GetBookmark(changeset);
                if(bookmark != null)
                    return bookmark.Changeset;

                var tag = GetTags().SingleOrDefault(t => string.Compare(t.Name, changeset, StringComparison.InvariantCulture) == 0);
                if(tag != null)
                    return Changelog[tag.NodeID];

                uint revision;
                if(uint.TryParse(changeset, out revision))
                    return Changelog[revision];

                HgNodeID hgNodeID;
                if(HgNodeID.TryParse(changeset, out hgNodeID))
                    return Changelog[hgNodeID];

                return null;
            }
        }

        public string FullPath { get; private set; }
        
        public HgEncoder Encoder { get; private set; }

        public HgRepository(string path) :
            this(path, new HgEncoder())
        {
        }

        public HgRepository(string path, HgEncoder hgEncoder)
        {
            Encoder = hgEncoder;

            basePath = path.ToLowerInvariant().EndsWith(".hg") ? 
                path : 
                Path.Combine(path, ".hg");
            var storeBasePath = Path.Combine(basePath, "store");
            FullPath = new Alphaleonis.Win32.Filesystem.DirectoryInfo(basePath).Parent.FullName;

            fileSystem = new HgFileSystem();
            atomicFileSystem = new HgTransactionalFileSystem();

            Requirements = new HgRequirementsReader(fileSystem).ReadRequirements(Path.Combine(basePath, "requires"));

            store = new HgStoreFactory().CreateInstance(storeBasePath, Requirements, fileSystem, Encoder);
            branchManager = new HgBranchManager(this, fileSystem);
            bookmarkManager = new HgBookmarkManager(this, fileSystem);

            //
            // Make sure it looks more or less like a repository.
            if(!Directory.Exists(basePath)) throw new IOException(basePath);

            var hgRcReader = new HgRcReader();
            var hgRc = hgRcReader.ReadHgRc(Path.Combine(basePath, "hgrc"));
            Rc = hgRc;

            tagManager = new HgTagManager(this, fileSystem);
        }

        public HgRevset GetTopologicalHeads()
        {
            var hgRevlogGraph = new HgRevlogGraph();
            hgRevlogGraph.Add(Changelog.Revlog.Entries);

            var headNodes =
                hgRevlogGraph.Nodes.
                    Where(n => n.Children.Count == 0).
                    //
                    // FIXME: Since we know that HgRevlogReader reuses cached revision data in forward-only
                    // manner, we need to order heads by revision ascending to ensure that this
                    // cache will actually get hit.
                    OrderBy(n => n.Revision).
                    Select(n => new HgRevsetEntry(n.Revision, n.NodeID)).
                    ToList();

            /*if(headNodes.Count == 0)
                headNodes.Add(new HgRevsetEntry(uint.MaxValue, HgNodeID.Null));*/

            var heads = new HgRevset(headNodes);
            return heads;
        }

        public HgRevset GetHeads()
        {
            var branchmap = GetBranchmap();
            var headNodes = branchmap.SelectMany(b => b.Heads).OrderByDescending(h => h.Revision);

            var heads = new HgRevset(headNodes);
            
            return heads;
        }

        public IList<HgBranchHeads> GetBranchmap()
        {
            return branchManager.GetBranchmap();
        }

        public IList<HgChangeset> GetBranches()
        {
            //
            // Returns all branches
            var branchmap = GetBranchmap();
            var branches = GetChangesets(branchmap.SelectMany(b => b.Heads).OrderByDescending(h => h.Revision).Select(h => h.Revision));

            return branches.ToList();
        }

        public IList<HgSubrepository> GetSubrepositories(HgManifestEntry hgManifestEntry)
        {
            var reader = new HgSubrepositoryReader(this);
            return new List<HgSubrepository>(reader.ReadSubrepositories(hgManifestEntry));
        } 

        public IList<HgChangeset> GetChangesets(HgRevset hgRevset)
        {
            return GetChangesets(hgRevset.Select(hre => hre.Revision)).ToList();
        }

        public IEnumerable<HgChangeset> GetChangesets()
        {
            var revlogReader = new HgRevlogReader(Changelog.Revlog, fileSystem);
            var entries = revlogReader.ReadRevlogEntries(0, UInt32.MaxValue);

            var changesets = new HgChangelogReader(Encoder).ReadChangesets(entries);

            return changesets.ToList();
        }

        public IEnumerable<HgManifestEntry> GetManifestEntries(HgRevset hgRevset)
        {
            var revlogReader = new HgRevlogReader(Manifest.Revlog, fileSystem);
            var entries = revlogReader.ReadRevlogEntries(hgRevset);

            var manifestEntries = new HgManifestReader(Encoder).ReadManifestEntries(entries);

            return manifestEntries.ToList();
        }

        public IEnumerable<HgChangesetDetails> GetChangesetsDetails(HgRevset hgRevset)
        {
            //
            // We need to preload all changesets and manifests
            var changesets =
                GetChangesets(
                    hgRevset.
                        Select(e => Changelog.Revlog[e.Revision]).
                        SelectMany(e => new[] { e.Revision, e.FirstParentRevision, e.SecondParentRevision }).
                        Where(e => e != uint.MaxValue).
                        Distinct()).
                        ToDictionary(c => c.Metadata.NodeID);

            var manifests =
                    GetManifestEntries(
                        new HgRevset(
                            changesets.Values.
                                Select(c => c.ManifestNodeID).
                                Select(e => Manifest.Revlog.GetEntry(e)))).
                        ToDictionary(m => m.Metadata.NodeID);

            foreach(var entry in hgRevset.OldestToNewest)
            {
                var c = changesets[entry.NodeID];
                
                //
                // This is somehow possible for imported repos
                if(c.ManifestNodeID == HgNodeID.Null) continue;

                var m = manifests[c.ManifestNodeID];

                IList<HgChangesetFileDetails> files;

                //
                // For a very first changeset we treat all files as Added
                if(c.Metadata.FirstParentRevisionNodeID == HgNodeID.Null)
                {
                    files =
                        m.Files.
                            Select(mfe => new HgChangesetFileDetails(mfe.Path, HgChangesetFileOperation.Added, null)).
                            ToList();
                } //  if
                else
                {
                    Func<HgManifestFileEntry, string> fileName = (mfe) => mfe.Path.FullPath.TrimStart('/');

                    var cf = new HashSet<string>(c.Files.Select(f => f.TrimStart('/')).ToList());
                    var f_ = new HashSet<string>(m.Files.Select(fileName));

                    var p1 = changesets[c.Metadata.FirstParentRevisionNodeID];
                    var mp1 = manifests[p1.ManifestNodeID];
                    var fp1 = new HashSet<string>(mp1.Files.Select(fileName));

                    //
                    // This is possible for imported repos. See above
                    var parentManifestFiles = mp1 == null ?
                        new List<string>() : 
                        mp1.Files.Select(fileName).ToList();

                    var addedFiles = f_.Except(fp1).Intersect(cf).ToList();
                    var removedFiles = fp1.Except(f_).Intersect(cf).ToList();
                    var modifiedFiles = f_.Intersect(fp1).Intersect(cf).Where(f => m[f].FilelogNodeID != mp1[f].FilelogNodeID).ToList();
                    var removed = removedFiles.Select(Removed);
                    var added = addedFiles.Select(Added);
                    var modified = modifiedFiles.Select(Modified);

                    //
                    // Prepare enough room to avoid reallocations later on
                    files = new List<HgChangesetFileDetails>(f_.Count * 2);

                    files.AddRange(added);
                    files.AddRange(modified);
                    files.AddRange(removed);
                } // else

                var changesetDetails = new HgChangesetDetails(c, files);
                yield return changesetDetails;
            } // foreach
        }

        public HgChangesetDetails GetChangesetDetails(HgNodeID hgNodeID, int context = 3)
        {
            var c = Changelog[hgNodeID];
            if(c == null) return null;

            var m = Manifest[c.ManifestNodeID];
            if(m == null) return new HgChangesetDetails(c, new List<HgChangesetFileDetails>());

            IList<HgChangesetFileDetails> files;

            var diffGenerator = new HgDiffGenerator();
            
            //
            // For a very first changeset we treat all files as Added
            if(c.Metadata.FirstParentRevisionNodeID == HgNodeID.Null)
            {
                files =
                    m.Files.
                        Select(f => new {
                            f = f.Path,
                            file = GetFile(m[f.Path.FullPath]),
                        }).
                        Select(f => new HgChangesetFileDetails(f.f, HgChangesetFileOperation.Added, 
                            f.file.IsBinary ? 
                                null :
                                GetHgRollupFileDiffInfo(f.f, f.file.IsBinary, diffGenerator.UnifiedDiff("", Encoder.DecodeAsUtf8(f.file.Data), context, false, true, true)))).
                        ToList();
            } //  if
            else
            {
                Func<HgManifestFileEntry, string> fileName = mfe => mfe.Path.FullPath.TrimStart('/');

                HgManifestEntry mp1 = null;
                ISet<string> cf = null, f_ = null, fp1 = null;

                cf = new HashSet<string>(c.Files.Select(f => f.TrimStart('/')).ToList());
                f_ = new HashSet<string>(m.Files.Select(fileName));

                var p1 = Changelog[c.Metadata.FirstParentRevisionNodeID];
                mp1 = Manifest[p1.ManifestNodeID];
                fp1 = new HashSet<string>(mp1.Files.Select(fileName));

                var addedFiles = f_.Except(fp1).Intersect(cf).ToList();
                var removedFiles = fp1.Except(f_).Intersect(cf).ToList();
                var modifiedFiles = f_.Intersect(fp1).Intersect(cf).Where(f => m[f].FilelogNodeID != mp1[f].FilelogNodeID || 
                    !GetFile(m[f]).Data.SequenceEqual(GetFile(mp1[f]).Data)).ToList();

                Func<HgFile, HgFile, HgUnifiedDiff> diff = (f /* from */, t /* to */) => {
                    if(f != null && f.IsBinary || t != null && t.IsBinary) return null;

                    var a = f == null ? "" : Encoder.DecodeAsUtf8(f.Data);
                    var b = t == null ? "" : Encoder.DecodeAsUtf8(t.Data);

                    return diffGenerator.UnifiedDiff(a, b, context, false, true, true);
                };

                var removed = 
                    removedFiles.
                        Select(f => new {
                            f,
                            file = GetFile(mp1[f]),
                        }).
                        Where(f => f.file != null).
                        Select(f => Removed(f.f, 
                            GetHgRollupFileDiffInfo(
                                new HgPath(f.f),
                                f.file.IsBinary,
                                diff(f.file, null)))).
                        Where(f => f.Diff != null && (f.Diff.Additions > 0 || f.Diff.Removals > 0));

                var added = 
                    addedFiles.
                        Select(f => new {
                            f,
                            file = GetFile(m[f]),
                        }).
                        Where(f => f.file != null).
                        Select(f => Added(f.f, 
                            GetHgRollupFileDiffInfo(
                                new HgPath(f.f),
                                f.file.IsBinary,
                                diff(null, f.file)))).
                        Where(f => f.Diff != null && (f.Diff.Additions > 0 || f.Diff.Removals > 0));

                var modified = 
                    modifiedFiles.
                        Select(f => new {
                            f,
                            file = GetFile(m[f]),
                        }).
                        Where(f => f.file != null).
                        Select(f => Modified(f.f, 
                            GetHgRollupFileDiffInfo(
                                new HgPath(f.f),
                                f.file.IsBinary,
                                diff(GetFile(mp1[f.f]), f.file)))).
                        Where(f => f.Diff == null || f.Diff.Additions > 0 || f.Diff.Removals > 0);

                //
                // Prepare enough room to avoid reallocations later on
                files = new List<HgChangesetFileDetails>(f_.Count * 2);

                files.AddRange(added);
                files.AddRange(modified);
                files.AddRange(removed);
            } // else

            var changesetDetails = new HgChangesetDetails(c, files);
            return changesetDetails;
        }

        private static HgChangesetFileDetails Modified(string f)
        {
            return Modified(f, null);
        }

        private static HgChangesetFileDetails Added(string f)
        {
            return Added(f, null);
        }

        private static HgChangesetFileDetails Removed(string f)
        {
            return Removed(f, null);
        }

        private static HgChangesetFileDetails Modified(string f, HgRollupFileDiffInfo diff)
        {
            return new HgChangesetFileDetails(new HgPath(f), HgChangesetFileOperation.Modified, diff);
        }

        private static HgChangesetFileDetails Added(string f, HgRollupFileDiffInfo diff)
        {
            return new HgChangesetFileDetails(new HgPath(f), HgChangesetFileOperation.Added, diff);
        }

        private static HgChangesetFileDetails Removed(string f, HgRollupFileDiffInfo diff)
        {
            return new HgChangesetFileDetails(new HgPath(f), HgChangesetFileOperation.Removed, diff);
        }

        public HgFile GetFile(HgManifestFileEntry manifestFileEntry)
        {
            if(manifestFileEntry == null) return null;

            var filelogNodeID = manifestFileEntry.FilelogNodeID;
            return 
                HgRevlogBasedStorage.ObjectCache.
                    GetOrAdd(
                        string.Format("file:{0}@{1}", manifestFileEntry.Path.FullPath, filelogNodeID.Long),
                        () => GetFileImpl(manifestFileEntry, filelogNodeID));
        }

        private HgFile GetFileImpl(HgManifestFileEntry manifestFileEntry, HgNodeID filelogNodeID)
        {
            var filelog = GetFilelog(manifestFileEntry.Path);
            if(filelog == null) return null;

            var hgRevlogReader = Store.CreateRevlogReader(filelog.Revlog);
            var hgRevlogData = hgRevlogReader.ReadRevlogEntry(filelogNodeID);
            if(hgRevlogData == null) return null;

            //
            // Handle copyinfo and possibly other inband metadata. This part of Mercurial I don't like
            var data = hgRevlogData.Data;
            var copyInfo = HgFileCopyInfo.ExtractFileCopyInfo(ref data);

            log.Debug("retrieved file {0}@{1}", manifestFileEntry.Path.FullPath, filelogNodeID.Short);

            return new HgFile(manifestFileEntry.Path, hgRevlogData.Entry, data, copyInfo);
        }

        public HgFilelog GetFilelog(HgPath hgPath)
        {
            return Store.GetFilelog(hgPath);
        }

        public HgNodeID Commit(HgCommit hgCommit)
        {
            var hgJournal = new HgJournal(Store);
            var hgTransaction = new HgTransaction(Store, hgJournal);

            var tip = Changelog.Tip;

            try
            {
                var manifestFileEntries = new List<HgManifestFileEntry>();
                foreach(var file in hgCommit.Files)
                {
                    
                    var filelog = Store.GetFilelog(file.Path) ?? Store.CreateFilelog(file.Path);
                    hgTransaction.Enlist(filelog);

                    var fileNodeID = Commit(file.FirstParentNodeID, file.SecondParentNodeID, file.Content, filelog.Revlog);
                    manifestFileEntries.Add(new HgManifestFileEntry(file.Path, fileNodeID));
                } // foreach

                //
                // We need to alter existing manifest here
                var manifestEntry = new HgManifestEntry(null, manifestFileEntries);
                if(hgCommit.FirstParentNodeID != HgNodeID.Null)
                {
                    var firstParentChangeset = Changelog[hgCommit.FirstParentNodeID];
                    var firstParentManifest = Manifest[firstParentChangeset.ManifestNodeID];

                    var previousManifestFileEntries = firstParentManifest.Files.ToList();

                    while(manifestFileEntries.Count > 0)
                    {
                        var mfe = manifestFileEntries[0];
                        manifestFileEntries.Remove(mfe);

                        var i = previousManifestFileEntries.FindIndex(e => e.Path == mfe.Path);
                        if(i == -1)
                            previousManifestFileEntries.Add(mfe);
                        else
                            previousManifestFileEntries[i] = mfe;
                    } // while

                    manifestEntry = new HgManifestEntry(null, previousManifestFileEntries);
                } // if

                var manifestEntryData = new HgManifestWriter(Encoder).WriteManifestEntry(manifestEntry);

                var manifestFirstParentNodeID = Manifest.Revlog.Entries.Any() ? 
                    Manifest.Revlog.Entries[Manifest.Revlog.Entries.Count - 1].NodeID :
                    HgNodeID.Null;

                hgTransaction.Enlist(Manifest);
                var manifestNodeID = Commit(manifestFirstParentNodeID, HgNodeID.Null, manifestEntryData, Manifest.Revlog);

                var hgChangeset = new HgChangeset(null, manifestNodeID, hgCommit.CommittedBy, hgCommit.CommittedAt, hgCommit.Branch, HgNodeID.Null, hgCommit.Files.Select(f => f.Path.FullPath.TrimStart('/')), hgCommit.Comment);
                var changesetEntryData = new HgChangelogWriter().WriteChangeset(hgChangeset);

                hgTransaction.Enlist(Changelog);
                var changesetNodeID = Commit(hgCommit.FirstParentNodeID, hgCommit.SecondParentNodeID, changesetEntryData, Changelog.Revlog);

                hgTransaction.Commit();

                changelog = null;

                branchManager.RefreshBranchmap(tip == null ? 0 : tip.Metadata.Revision, Changelog.Tip.Metadata);
                
                return changesetNodeID;
            } // try
            catch(Exception)
            {
                hgTransaction.Rollback();
                throw;
            } // catch
        }

        public HgCommitStats Commit(HgBundle hgBundle)
        {
            log.Info("committing bundle");

            using(AcquireLock())
            {
                var hgJournal = new HgJournal(Store);
                var hgTransaction = new HgTransaction(Store, hgJournal);

                var tip = Changelog.Tip;

                try
                {
                    //
                    // Adding changesets
                    log.Info("committing changesets");
                    hgTransaction.Enlist(Changelog);
                    
                    int changesets;
                    var revisions = CommitChangelog(tip, hgBundle.Changelog, Changelog.Revlog, out changesets);

                    //
                    // Adding manifests
                    log.Info("committing manifests");
                    hgTransaction.Enlist(Manifest);

                    var manifests = 0;
                    Commit(revisions, hgBundle.Manifest, Manifest.Revlog, ref manifests);

                    //
                    // Adding file changes
                    log.Info("committing files");
                    var changedFiles = 0;
                    var changes = 0;

                    foreach(var file in hgBundle.Files)
                    {
                        changedFiles++;

                        log.Debug("committing '{0}'", file.Path.FullPath);

                        var filelog = Store.GetFilelog(file.Path) ?? Store.CreateFilelog(file.Path);
                        hgTransaction.Enlist(filelog);

                        var hgBundleGroup = file.File;

                        Commit(revisions, hgBundleGroup, filelog.Revlog, ref changes);
                    } // foreach

                    if(BundleCommitting != null)
                    {
                        var args = new HgBundleCommittedEventArgs(revisions.Keys.Select(k => Changelog[k]));
                        BundleCommitting(args);
                    } // if
                    
                    hgTransaction.Commit();

                    //
                    // We need to force changelog re-read since after CommitChangelog() the changelog is
                    // being read off a temp file.
                    changelog = null;

                    branchManager.RefreshBranchmap(tip == null ? 0 : tip.Metadata.Revision, Changelog.Tip.Metadata);

                    if(BundleCommitted != null)
                    {
                        var args = new HgBundleCommittedEventArgs(revisions.Keys.Select(k => Changelog[k]));
                        BundleCommitted(args);
                    } // if

                    return new HgCommitStats(changesets, manifests, changes, changedFiles);
                } // try
                catch(Exception e)
                {
                    log.ErrorException("could not commit bundle", e);

                    hgTransaction.Rollback();

                    //
                    // Reset changelog and manifest so that they'll be reread
                    changelog = null;
                    manifest = null;

                    throw;
                } // catch
            } // using
        }

        private HgNodeID Commit(HgNodeID firstParentNodeID, HgNodeID secondParentNodeID, byte[] content, HgRevlog hgRevlog)
        {
            var data = new byte[]{};

            if(firstParentNodeID != HgNodeID.Null)
            {
                var hgRevlogReader = new HgRevlogReader(hgRevlog, fileSystem);
                data = hgRevlogReader.ReadRevlogEntry(firstParentNodeID).Data;
            } // if
                        
            var linkRevision = Changelog == null ? 0 : Changelog.Revlog.Entries.Count;

            var hgNodeID = new HgRevlogWriter(hgRevlog, fileSystem).WriteRevlogEntryData((uint)linkRevision, firstParentNodeID, secondParentNodeID, content);
            return hgNodeID;
        }

        private void Commit(IDictionary<HgNodeID, uint> revisions, IEnumerable<HgChunk> chunks, HgRevlog hgRevlog, ref int changes)
        {
            var hgRevlogWriter = new HgRevlogWriter(hgRevlog, fileSystem);
            changes += 
                hgRevlogWriter.WriteRevlogEntries(chunks, 
                    n => {
                        var linkRevision = revisions.ContainsKey(n) ? revisions[n] : (uint?)null;
                        if(!linkRevision.HasValue)
                            throw new Exception("missing changeset in bundle");

                        return linkRevision.Value;
                    });
        }

        private IDictionary<HgNodeID, uint> CommitChangelog(HgChangeset tip, IEnumerable<HgChunk> chunks, HgRevlog hgRevlog, out int changesets)
        {
            var revisions = new Dictionary<HgNodeID, uint>();
            var revision = tip == null ? 0 : 
                tip.Metadata.Revision + 1;
            
            changesets = 0;

            var hgRevlogWriter = new HgRevlogWriter(hgRevlog, atomicFileSystem);
            changesets = 
                hgRevlogWriter.WriteRevlogEntries(chunks, 
                    n => {
                        var linkRevision = revision++;
                        revisions[n] = linkRevision;

                        return linkRevision;
                    }, 
                    e => {
                        revisions[e.NodeID] = e.Revision;
                    });

            return revisions;
        }


        /// <summary>
        /// Creates a Mercurial repository at <paramref name="path" /> with given <paramref name="requirements"/>.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="requirements"></param>
        /// <returns></returns>
        public static HgRepository Create(string path, params string[] requirements)
        {
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var hgPath = Path.Combine(path, ".hg");
            var hgStorePath = Path.Combine(hgPath, "store");
            Directory.CreateDirectory(hgStorePath);

            var effectiveRequirements = new HashSet<string> { "revlogv1", "fncache", "store", "dotencode" };

            if(requirements != null)
                effectiveRequirements.UnionWith(requirements);

            File.WriteAllBytes(Path.Combine(hgPath, "requires"), Encoding.ASCII.GetBytes(string.Join("\n", effectiveRequirements)));

            //
            // Write dummy changelog at .hg/00changelog.i
            using(var stream = new FileStream(Path.Combine(hgPath, "00changelog.i"), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using(var binaryWriter = new BinaryWriter(stream))
                {
                    binaryWriter.Write(new byte[] { 0, 0, 0, 2 });
                    binaryWriter.Write(Encoding.ASCII.GetBytes(" dummy changelog to prevent using the old repo layout"));
                } // using
            } // using

            return new HgRepository(path);
        }

        public ReadOnlyCollection<HgTag> GetTags()
        {
            return tagManager.GetTags();
                        //Where(t => t.Changeset != null). // TODO: Warning or something
        }

        public HgLock AcquireLock()
        {
            var timeout = TimeSpan.FromSeconds(30);
            log.Info("acquiring lock at {0} with timeout {1}", Store == null ? "null" : Store.GetType().Name, timeout);

            try
            {
                
                var hgLock = Store.AcquireLock(timeout);
                return hgLock;
            } // try
            catch(Exception e)
            {
                log.Info(e.ToString);
                return null;
            } // catch
        }

        public IList<HgChangeset> GetFileHistory(HgPath path, HgNodeID? startFilelogNodeID = null)
        {
            var filelog = GetFilelog(path);
            if(filelog == null) return null;

            var changesetNodeIDs = GetFileHistory(filelog, startFilelogNodeID ?? filelog.Revlog.Entries.Last().NodeID);
            
            var revsetManager = new HgRevsetManager();
            var revset = revsetManager.GetRevset(this, changesetNodeIDs);
            
            return GetChangesets(revset).OrderByDescending(c => c.Metadata.Revision).ToList();
        }

        private IEnumerable<uint> GetFileHistory(HgFilelog filelog, HgNodeID startNodeID)
        {
            var entries = 
                filelog.Revlog.Entries.
                    OrderByDescending(e => e.Revision).
                    SkipWhile(e => e.NodeID != startNodeID).
                    ToList();

            foreach(var entry in entries)
            {
                
                yield return entry.LinkRevision;

                var file = GetFile(new HgManifestFileEntry(filelog.Path, entry.NodeID));
                if(file.CopyInfo != null)
                {
                    var sourceFilelog = GetFilelog(file.CopyInfo.Path);
                    if(sourceFilelog == null) yield break;

                    foreach(var changeset in GetFileHistory(sourceFilelog, file.CopyInfo.NodeID))
                        yield return changeset;

                    yield break;
                }   
            
                if(entry.FirstParentRevisionNodeID == HgNodeID.Null)
                    yield break;

            } // foreach
        }

        public ReadOnlyCollection<HgBookmark> GetBookmarks()
        {
            return bookmarkManager.GetBookmarks();
        }

        public HgDivergenceInfo GetDivergence(HgNodeID baseNodeID, HgNodeID headNodeID)
        {
            var hgRevsetManager = new HgRevsetManager();
            var @base = (baseNodeID == HgNodeID.Null ? null : Changelog[baseNodeID]);
            var head = (headNodeID == HgNodeID.Null ? null : Changelog[headNodeID]);

            var baseAncestors = @base == null ?
                new HgRevset() : 
                hgRevsetManager.GetAncestors(this, new HgRevset(@base.Metadata));
            var headAncestors = head == null ?
                new HgRevset() :
                hgRevsetManager.GetAncestors(this, new HgRevset(head.Metadata));

            var ahead = headAncestors - baseAncestors;
            var behind = baseAncestors - headAncestors;

            //
            // If ahead has only one commit and that one is closing branch without affecting any files, ignore it altogether
            if(ahead.Count == 1)
            {
                var aheadChangeset = Changelog[ahead.Single().NodeID];
                if(aheadChangeset.Branch.Closed && aheadChangeset.Files.Count == 0)
                    ahead = new HgRevset();
            } // if

            return new HgDivergenceInfo(ahead, head, behind, @base);
        }

        public HgCompareInfo PerformComparison(HgNodeID baseNodeID, HgNodeID headNodeID, bool includeFileDiffs = false)
        {
            var hgRevsetManager = new HgRevsetManager();
            var @base = Changelog[baseNodeID];
            var head = Changelog[headNodeID];

            var baseAncestors = hgRevsetManager.GetAncestors(this, new HgRevset(@base.Metadata));
            var headAncestors = hgRevsetManager.GetAncestors(this, new HgRevset(head.Metadata));

            var changesets = GetChangesets(headAncestors - baseAncestors);

            if(changesets.Count == 0)
                return new HgCompareInfo(@base, head, new List<HgChangeset>(), new List<HgRollupFileDiffInfo>());

            var files = changesets.SelectMany(c => c.Files).Distinct().OrderBy(f => f).ToList();

            var startManifest = Manifest[@base.ManifestNodeID];
            var endManifest = Manifest[head.ManifestNodeID];

            //
            // Only pick files that were indeed changed between two bounding changesets
            
            var diffs = new List<HgRollupFileDiffInfo>();
            if(includeFileDiffs)
            {
                var changedFiles = files.Where(f => startManifest[f] == null && endManifest[f] != null || startManifest[f] != null && endManifest[f] == null || (startManifest[f] != null && endManifest[f] != null && startManifest[f].FilelogNodeID != endManifest[f].FilelogNodeID)).ToList();
                var diffGenerator = new HgDiffGenerator();
                diffs = changedFiles.
                    Select(f => new {
                        file = f,
                        start = startManifest[f],
                        end = endManifest[f]
                    }).
                    Select(f => new {
                        f.file,
                        start = f.start == null ? null : GetFile(f.start),
                        end = f.end == null ? null : GetFile(f.end)
                    }).
                    Select(f => new {
                        f.file,
                        isBinary = f.start != null && f.start.IsBinary || f.end != null && f.end.IsBinary,
                        f.start,
                        f.end
                    }).
                    Select(f => new {
                        f.file,
                        f.isBinary,
                        start = f.start == null || f.isBinary ? "" : Encoder.DecodeAsUtf8(f.start.Data),
                        end = f.end == null || f.isBinary ? "" : Encoder.DecodeAsUtf8(f.end.Data)
                    }).
                    Select(f => new { 
                        f.file,
                        f.isBinary,
                        diff = f.isBinary ? null : diffGenerator.UnifiedDiff(f.start, f.end, 3)
                    }).
                    //
                    // Do not include files that were not in fact changed between revision
                    Where(f => f.diff == null || f.diff.Hunks.Sum(h => h.Lines.Count(l => l.Added || l.Removed )) > 0).
                    Select(f => GetHgRollupFileDiffInfo(new HgPath(f.file), f.isBinary, f.diff)).
                    ToList();
            }

            return new HgCompareInfo(@base, head, changesets, diffs);
        }

        private HgRollupFileDiffInfo GetHgRollupFileDiffInfo(HgPath hgPath, bool isBinary, HgUnifiedDiff hgUnifiedDiff)
        {
            var additions = hgUnifiedDiff == null ? 0 : hgUnifiedDiff.Hunks.SelectMany(h => h.Lines).Count(l => l.Added);
            var removals = hgUnifiedDiff == null ? 0 : hgUnifiedDiff.Hunks.SelectMany(h => h.Lines).Count(l => l.Removed);

            return new HgRollupFileDiffInfo(hgPath, isBinary, hgUnifiedDiff, additions, removals);
        }

        public HgBundle GetBundle(HgRevset common, HgRevset heads)
        {
            var hgRevsetManager = new HgRevsetManager();
            var hgRevset = hgRevsetManager.GetRevset(this, common, heads);

            var hgBundleBuilder = new HgBundleBuilder(new HgFileSystem(), Encoder);
            var hgBundle = hgBundleBuilder.BuildBundle(this, hgRevset);

            return hgBundle;
        }

        public HgBookmark GetBookmark(string name)
        {
            return GetBookmarks().SingleOrDefault(b => string.Compare(b.Name, name, StringComparison.InvariantCulture) == 0);
        }

        public void DeleteBookmark(string bookmarkName)
        {
            bookmarkManager.DeleteBookmark(bookmarkName);
        }

        public void UpdateBookmark(string bookmarkName, HgNodeID changesetNodeID)
        {
            bookmarkManager.UpdateBookmark(bookmarkName, changesetNodeID);
        }

        public IEnumerable<HgChangeset> GetChangesets(IEnumerable<uint> revisions)
        {
            return Changelog.ReadChangesets(revisions);
        }
    }
}