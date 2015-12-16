// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HgSharp.Core
{
    public class HgAnnotationManager
    {
        private readonly HgRepository repository;

        public HgAnnotationManager(HgRepository repository)
        {
            this.repository = repository;
        }

        public HgAnnotation GetAnnotation(HgPath path, HgNodeID? startFilelogNodeID = null)
        {
            var fileHistory = repository.GetFileHistory(path, startFilelogNodeID).OrderBy(c => c.Metadata.Revision).ToList();

            var prev = "";
            var diffGenerator = new HgDiffGenerator();

            var annotationLines = new LinkedList<HgAnnotationLine>();

            var manifestEntries = 
                repository.
                    GetManifestEntries(new HgRevset(fileHistory.Select(f => repository.Manifest.Revlog.GetEntry(f.ManifestNodeID)))).
                    ToDictionary(me => me.Metadata.NodeID, me => me);

            foreach(var changeset in fileHistory)
            {
                var manifest = manifestEntries[changeset.ManifestNodeID];
                var current =
                    Encoding.UTF8.GetString(
                        repository.GetFile(manifest.GetFile(path)).Data);

                var diff = diffGenerator.Diff(prev, current);

                LinkedListNode<HgAnnotationLine> annotationLine = null; // before first
                for(var i = 0; i < diff.Lines.Count; ++i)
                {
                    if(diff.Lines[i].Unchanged)
                    {
                        annotationLine = annotationLine == null ? 
                            annotationLines.First : 
                            annotationLine.Next;
                        
                        continue;
                    } // if

                    if(diff.Lines[i].Removed)
                    {
                        if(annotationLine == null)
                        {
                            annotationLines.RemoveFirst();
                        } // if
                        else
                        {
                            if(annotationLine.Next != null)
                                annotationLines.Remove(annotationLine.Next);
                        } // else
                    }
                    else
                    {
                        if(annotationLine == null)
                        {
                            annotationLines.AddFirst(new HgAnnotationLine(changeset, diff.Lines[i].Content));
                            annotationLine = annotationLines.First;
                        }
                        else
                        {
                            annotationLines.AddAfter(annotationLine, new HgAnnotationLine(changeset, diff.Lines[i].Content));
                            annotationLine = annotationLine.Next;
                        }
                    }
                } // for


                prev = current;
            } // foreach


           /* var lines = prev.Split('\n');
            Debug.Assert(
                lines.Length == annotationLines.Count,
                string.Format("Annotation line count mismatch: {0} in annotation vs {1} in file", annotationLines.Count, lines.Length));*/

            return new HgAnnotation(path, annotationLines);
        }
    }
}