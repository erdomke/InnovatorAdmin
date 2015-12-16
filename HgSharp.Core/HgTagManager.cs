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

using NLog;

using HgSharp.Core.Util;

namespace HgSharp.Core
{
    internal class HgTagManager
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly HgRepository repository;
        private readonly HgFileSystem fileSystem;

        private readonly IDictionary<HgNodeID, HgNodeID> filelogNodeCache = new Dictionary<HgNodeID, HgNodeID>(); 
        private readonly IList<uint> revisionCache = new List<uint>(); 
        private readonly IList<HgNodeID> headNodeCache = new List<HgNodeID>();
        private readonly IDictionary<string, Tuple<HgNodeID, IList<HgNodeID>>> tagCache = new Dictionary<string, Tuple<HgNodeID, IList<HgNodeID>>>(); 

        public HgTagManager(HgRepository repository, HgFileSystem fileSystem)
        {
            this.repository = repository;
            this.fileSystem = fileSystem;
        }

        public ReadOnlyCollection<HgTag> GetTags()
        {
            var tags = GetTagsInternal();
            
            var tip = 
                repository.Changelog.Tip == null ?
                    HgNodeID.Null :
                    repository.Changelog.Tip.Metadata.NodeID;
            
            tags.Add(new HgTag("tip", tip));

            return new ReadOnlyCollection<HgTag>(tags);
        }

        private IList<HgTag> GetTagsInternal()
        {
            if(repository.Changelog.Tip == null) return new List<HgTag>();

            ReadTagsCache();

            var tip = repository.Changelog[repository.Changelog.Revlog.Entries.Last().Revision];
            if(headNodeCache.Count > 0 && headNodeCache[0] == tip.Metadata.NodeID && revisionCache.Count > 0 && revisionCache[0] == tip.Metadata.Revision)
            {
                var tags = 
                    tagCache.
                        Where(kvp => repository.Changelog.Revlog.GetEntry(kvp.Value.Item1) != null).
                        Select(kvp => new HgTag(kvp.Key, kvp.Value.Item1)).
                        ToList();

                return tags;
            } // if

            var heads = repository.GetHeads();
            if(heads.Count == 0)
                return new List<HgTag>();

            var hgFilelog = repository.GetFilelog(new HgPath(".hgtags"));
            if(hgFilelog == null || hgFilelog.Revlog.Entries.Count == 0)
            {
                headNodeCache.Clear();
                headNodeCache.AddRange(heads.Select(h => h.NodeID));

                return new List<HgTag>();
            } // if
                

            var newHeads = new HgRevset(heads.Where(h => !headNodeCache.Contains(h.NodeID)).ToList());
            RefreshTagsInternal(newHeads);

            var visitedFileNodes = new HashSet<HgNodeID>();

            foreach(var head in newHeads.OldestToNewest)
            {
                var fileNode = filelogNodeCache.ContainsKey(head.NodeID) ? filelogNodeCache[head.NodeID] : HgNodeID.Null;
                if(fileNode != HgNodeID.Null && !visitedFileNodes.Contains(fileNode))
                {
                    visitedFileNodes.Add(fileNode);
                    var hgtags = repository.GetFile(new HgManifestFileEntry(new HgPath(".hgtags"), fileNode));
                    if(hgtags == null) continue;

                    using(var streamReader = new StreamReader(new MemoryStream(hgtags.Data), repository.Encoder.Utf8))
                    {
                        IDictionary<string, Tuple<HgNodeID, IList<HgNodeID>>> fileTags = new Dictionary<string, Tuple<HgNodeID, IList<HgNodeID>>>();
                        
                        ReadTags(streamReader.ReadLine, fileTags);
                        UpdateTags(fileTags);
                    } // using
                } // if
            } // foreach

            WriteTagsCache();

            return tagCache.Select(t => new HgTag(t.Key, t.Value.Item1)).ToList();
        }

        private void UpdateTags(IDictionary<string, Tuple<HgNodeID, IList<HgNodeID>>> fileTags)
        {
            foreach(var fileTag in fileTags)
            {
                if(!tagCache.ContainsKey(fileTag.Key))
                {
                    tagCache[fileTag.Key] = fileTag.Value;
                    continue;
                } // if

                var anode = fileTag.Value.Item1;
                var ahist = fileTag.Value.Item2;

                var bnode = tagCache[fileTag.Key].Item1;
                var bhist = tagCache[fileTag.Key].Item2;

                if(anode != bnode && bhist.Contains(anode) && (!ahist.Contains(bnode) || bhist.Count > ahist.Count))
                {
                    anode = bnode;
                } // if

                ahist.AddRange(bhist.Where(b => !ahist.Contains(b)));
                tagCache[fileTag.Key] = Tuple.Create(anode, ahist);
            } // foreach
        }

        private void RefreshTagsInternal(HgRevset headNodes)
        {
            foreach(var hgHead in headNodes)
            {
                var changeset = repository.Changelog[hgHead.Revision];
                var manifestNodeID = changeset.ManifestNodeID;
                var hgManifest = repository.Manifest[manifestNodeID];
                var hgManifestFileEntry = hgManifest.GetFile(new HgPath(".hgtags"));
                if(hgManifestFileEntry == null) continue;

                /*var hgTagsFile = repository.GetFile(hgManifestFileEntry);

                var headTags =
                    Encoding.UTF8.GetString(hgTagsFile.Data).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).
                        Select(s => new { nodeID = new HgNodeID(s.Substring(0, 40)), name = s.Substring(41) }).
                        Select(t => new HgTag(t.name, repository.Changelog[t.nodeID])).
                        ToList();*/

                filelogNodeCache[hgHead.NodeID] = hgManifestFileEntry.FilelogNodeID;

                //tags.AddRange(headTags);
            } // foreach

            //tags = tags.Distinct().OrderByDescending(t => t.Changeset.Metadata.Revision).ToList();

            //log.Debug("retrieved {0} tags", tags.Count);

            //cachedTipNodeID = tip.Metadata.NodeID;
            //cachedTipRevision = tip.Metadata.Revision;

            //return null;
        }

        private void WriteTagsCache()
        {
            var tagsCachePath = Path.Combine(repository.BasePath, Path.Combine("cache", "tags"));
            using(var tagsCacheStream = fileSystem.CreateWrite(tagsCachePath))
            using(var streamWriter = new StreamWriter(tagsCacheStream))
            {
                foreach(var head in repository.GetHeads().NewestToOldest)
                {
                    var changeset = repository.Changelog[head.NodeID];
                    var fileNode = filelogNodeCache.ContainsKey(changeset.Metadata.NodeID) ?
                        filelogNodeCache[changeset.Metadata.NodeID] :
                        HgNodeID.Null;

                    if(fileNode != HgNodeID.Null)
                        streamWriter.Write("{0} {1} {2}\n", changeset.Metadata.Revision, head.NodeID.Long, fileNode.Long);
                    else
                        streamWriter.Write("{0} {1}\n", changeset.Metadata.Revision, head.NodeID.Long);
                } // foreach

                streamWriter.Write("\n");

                foreach(var tag in tagCache)
                {
                    streamWriter.Write("{0} {1}\n", tag.Value.Item1.Long, tag.Key);
                } // foreach
            } // using
        }

        private void ReadTagsCache()
        {
            var tagsCachePath = Path.Combine(repository.BasePath, Path.Combine("cache", "tags"));
            if(!Alphaleonis.Win32.Filesystem.File.Exists(tagsCachePath)) return;

            using(var tagsCacheStream = fileSystem.OpenRead(tagsCachePath))
            using(var streamReader = new StreamReader(tagsCacheStream))
            {
                var line = "";
                while(!string.IsNullOrWhiteSpace(line = streamReader.ReadLine())) // Apparently, ReadLine strips \n characters
                {
                    var parts = line.Split(' ');
                    
                    revisionCache.Add(uint.Parse(parts[0]));
                    
                    var headNode = new HgNodeID(parts[1]);
                    headNodeCache.Add(headNode);

                    if(parts.Length == 3)
                    {
                        var fileNode = new HgNodeID(parts[2]);
                        filelogNodeCache[headNode] = fileNode;
                    } // if
                } // while

                ReadTags(streamReader.ReadLine, tagCache);
            } // using
        }

        private void ReadTags(Func<string> lineReader , IDictionary<string, Tuple<HgNodeID, IList<HgNodeID>>> tags)
        {
            string line = "";
            while(!string.IsNullOrWhiteSpace(line = lineReader()))
            {
                var changesetNodeID = new HgNodeID(line.SubstringBefore(" "));
                var name = line.SubstringAfter(" ").Trim();

                IList<HgNodeID> history = new List<HgNodeID>();
                if(tags.ContainsKey(name))
                {
                    var tag = tags[name];

                    history = tag.Item2;
                    history.Add(tag.Item1);
                } // if

                tags[name] = Tuple.Create(changesetNodeID, history);
            } // while
        }
    }
}