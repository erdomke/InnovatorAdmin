// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.IO;

using NLog;

namespace HgSharp.Core
{
    public class AtomicFileStream : FileStream
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly string path;
        private readonly string tempPath;
        private bool committed;

        public AtomicFileStream(string path, string tempPath, FileMode mode, FileAccess access, FileShare share) : 
            base(tempPath, mode, access, share)
        {
            this.path = path;
            this.tempPath = tempPath;
        }

        public override void Close()
        {
            base.Close();
            
            if(!committed)
            {
                log.Info("atomically committing '{0}' to '{1}'", tempPath, path);

                if(File.Exists(path))
                    File.Replace(tempPath, path, null);
                else
                    File.Move(tempPath, path);

                committed = true;
            } // if
        }
    }
}