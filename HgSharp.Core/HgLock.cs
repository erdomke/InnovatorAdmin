// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.IO;

namespace HgSharp.Core
{
    public class HgLock : IDisposable
    {
        private string lockFilePath;
        private readonly bool owning;

        public HgLock(string lockFilePath, bool owning)
        {
            this.lockFilePath = lockFilePath;
            this.owning = owning;
        }

        public void Release()
        {
            if(lockFilePath == null || !owning) return;

            try
            {
                File.Delete(lockFilePath);
                lockFilePath = null;
            } // try
            catch(IOException)
            {
            } // catch
        }

        public void Dispose()
        {
            Release();
        }
    }
}