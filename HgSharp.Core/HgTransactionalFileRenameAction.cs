// HgSharp
//
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
//
// The following code is a derivative work of the code from the Mercurial project,
// which is licensed GPLv2. This code therefore is also licensed under the terms
// of the GNU Public License, verison 2.
using Alphaleonis.Win32.Filesystem;

namespace HgSharp.Core
{
    internal class HgTransactionalFileRenameAction : IHgTransactionalAction
    {
        private readonly string originalFilePath;
        private readonly string targetFilePath;

        public HgTransactionalFileRenameAction(string originalFilePath, string targetFilePath)
        {
            this.originalFilePath = originalFilePath;
            this.targetFilePath = targetFilePath;
        }

        public void Commit()
        {
          File.Move(originalFilePath, targetFilePath, MoveOptions.ReplaceExisting | MoveOptions.WriteThrough);
        }

        public void Rollback()
        {
        }
    }
}
