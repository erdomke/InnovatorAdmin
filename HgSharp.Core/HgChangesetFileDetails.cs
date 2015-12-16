// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.Diagnostics;

namespace HgSharp.Core
{
    [DebuggerDisplay("{Operation,nq} {Path.FullPath,nq} {FilelogNodeID.Short,nq}")]
    public class HgChangesetFileDetails
    {
        public HgPath Path { get; private set; }
        
        public HgChangesetFileOperation Operation { get; private set; }

        public HgRollupFileDiffInfo Diff { get; private set; }

        public HgChangesetFileDetails(HgPath path, HgChangesetFileOperation operation, HgRollupFileDiffInfo diff)
        {
            Path = path;
            Operation = operation;
            Diff = diff;
        }
    }
}