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
    public class HgTransactionalFileSystem : HgFileSystem
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        protected override FileStream OpenWriteInternal(string path)
        {
            return GetAtomicFileStream(path);
        }

        protected override FileStream CreateWriteInternal(string path)
        {
            return GetAtomicFileStream(path);
        }

        protected override FileStream OpenOrCreateWriteInternal(string path)
        {
            return GetAtomicFileStream(path);        
        }

        private FileStream GetAtomicFileStream(string path)
        {
            var tempFilePath = GetTempFilePath(path);

            log.Debug("initializing for '{0}' with temp path '{1}'", path, tempFilePath);

            if(Alphaleonis.Win32.Filesystem.File.Exists(path))
                Alphaleonis.Win32.Filesystem.File.Copy(path, tempFilePath, true);
            
            return new AtomicFileStream(path, tempFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Write, System.IO.FileShare.Read);
        }
    }
}