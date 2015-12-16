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
using System.Linq;

using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public class HgManifestTreeBuilder
    {
        public HgManifestTree GetManifestTreeNode(HgRepository hgRepository, HgManifestEntry hgManifestEntry, HgPath hgPath)
        {
            var separators = hgPath.FullPath.Count(c => c == '/');
            
            var paths =
                hgManifestEntry.Files.
                    Select(f => new {
                        file = f,
                        separators = f.Path.FullPath.Count(c => c == '/')
                    }).
                    Where(f => f.file.Path.FullPath.StartsWith(hgPath.FullPath)).
                    ToList();

            var directories =
                paths.
                    Where(d => d.separators > separators).
                    Select(d => d.file.Path.FullPath.SubstringBeforeNth("/", separators)).
                    Distinct().
                    Select(d => HgManifestNode.Directory(new HgPath(d))).
                    ToList();
            
            var files = 
                paths.
                    Where(f => f.separators == separators).
                    Select(f => HgManifestNode.File(f.file.Path, f.file.FilelogNodeID)).
                    ToList();

            var subrepositories =
                hgRepository.GetSubrepositories(hgManifestEntry).
                    Select(s => new {
                        subrepository = s,
                        separators = s.Path.FullPath.Count(c => c == '/')
                    }).
                    Where(s => s.subrepository.Path.FullPath.StartsWith(hgPath.FullPath) && s.separators == separators).
                    Select(s => HgManifestNode.Subrepository(s.subrepository.Path, s.subrepository.NodeID)).
                    ToList();

            return new HgManifestTree(hgPath, directories.Append(files).Append(subrepositories));
        }

       /* public HgManifestTree BuildManifestTree(HgManifestEntry manifestEntry)
        {
            var paths = manifestEntry.Files.Select(f => Tuple.Create(f, f.Path.FullPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))).ToList();

            var childNodes = BuildDirectoryManifestTreeNode(paths, new HgPath("/"), 1);
            var root = new HgManifestTreeNode(new HgPath("/"), "", childNodes);
            
            var tree = new HgManifestTree(root);

            return tree;
        }

        private HgManifestTreeNode BuildFileManifestTreeNode(Tuple<HgManifestFileEntry, string[]> path)
        {
            return new HgManifestTreeNode(path.Item1.Path, path.Item2[path.Item2.Length - 1], path.Item1.FilelogNodeID);
        }

        private IEnumerable<HgManifestTreeNode> BuildDirectoryManifestTreeNode(IList<Tuple<HgManifestFileEntry, string[]>> paths, HgPath path, int depth)
        {
            var files = paths.Where(p => p.Item2.Length == depth);
            var fileNodes = files.Select(BuildFileManifestTreeNode);

            var subdirectories = paths.Where(d => d.Item2.Length > depth).ToList();
            var subdirectoryNames = subdirectories.Select(s => s.Item2[depth - 1]).Distinct();

            var subdirectoryNodes = subdirectoryNames.Select(s => BuildDirectoryManifestTreeNodeRecursive(paths, path, depth, s));

            foreach(var n in subdirectoryNodes)
                yield return n;

            foreach(var n in fileNodes)
                yield return n;
        }

        private HgManifestTreeNode BuildDirectoryManifestTreeNodeRecursive(IList<Tuple<HgManifestFileEntry, string[]>> paths, HgPath path, int depth, string name)
        {
            var subdirectories = paths.Where(p => p.Item2[depth - 1] == name).ToList();
            var children = BuildDirectoryManifestTreeNode(subdirectories, path.Append(name), depth + 1);

            return new HgManifestTreeNode(path.Append(name), name, children);
        }*/
    }
}
