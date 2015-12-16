// HgSharp
//
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
//
// The following code is a derivative work of the code from the Mercurial project,
// which is licensed GPLv2. This code therefore is also licensed under the terms
// of the GNU Public License, verison 2.
using System;
using System.Diagnostics;
using System.IO;

using Alphaleonis.Win32.Filesystem;

using NLog;

namespace HgSharp.Core
{
    public class HgFileSystem
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private static Random random = new Random();
        private static int counter;

        public HgFileSystem()
        {
            counter = random.Next();
        }

        public virtual FileStream OpenRead(string path)
        {
            return new FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
        }

        public virtual FileStream OpenWrite(string path)
        {
            EnsureDirectoryPathExists(path);
            PerformCopyOnWrite(path);

            return OpenWriteInternal(path);
        }

        protected virtual FileStream OpenWriteInternal(string path)
        {
            return new FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Write, System.IO.FileShare.Read);
        }

        public virtual FileStream CreateWrite(string path)
        {
            EnsureDirectoryPathExists(path);

            return CreateWriteInternal(path);
        }

        protected virtual FileStream CreateWriteInternal(string path)
        {
            return new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read);
        }

        public virtual FileStream OpenOrCreateWrite(string path)
        {
            EnsureDirectoryPathExists(path);
            PerformCopyOnWrite(path);

            return OpenOrCreateWriteInternal(path);
        }

        protected virtual FileStream OpenOrCreateWriteInternal(string path)
        {
            return new FileStream(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.Read);
        }

        private void EnsureDirectoryPathExists(string path)
        {
            var directoryPath = System.IO.Path.GetDirectoryName(path);

            Debug.Assert(directoryPath != null, "directoryPath != null");

            if(!System.IO.Directory.Exists(directoryPath))
                System.IO.Directory.CreateDirectory(directoryPath);
        }

        private void PerformCopyOnWrite(string path)
        {
            if(!ShouldCopyOnWrite(path)) return;

            log.Info("copy-on-writing '{0}'", path);

            var tempFilePath = GetTempFilePath(path);
            Alphaleonis.Win32.Filesystem.File.Copy(path, tempFilePath, true);
            Alphaleonis.Win32.Filesystem.File.Delete(path);
            Alphaleonis.Win32.Filesystem.File.Move(tempFilePath, path, MoveOptions.ReplaceExisting | MoveOptions.WriteThrough);
        }

        private bool ShouldCopyOnWrite(string path)
        {
            if(!Alphaleonis.Win32.Filesystem.File.Exists(path)) return false;

            using(var fileStream = OpenRead(path))
            {
                var fileInfo = Alphaleonis.Win32.Filesystem.File.GetFileInfoByHandle(fileStream.SafeFileHandle);
                return fileInfo.NumberOfLinks > 1;
            } // using
        }

        protected string GetTempFilePath(string path)
        {
            var directoryPath = System.IO.Path.GetDirectoryName(path);
            Debug.Assert(directoryPath != null, "directoryPath != null");

            var fileExtension = System.IO.Path.GetExtension(path);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            Debug.Assert(fileName != null, "fileName != null");

            fileName = fileName.TrimEnd('.');

            var tempFileName = string.Format("{0}{1:yyyyMMdd}{2}{3}{4}",
                fileName,
                DateTime.UtcNow,
                random.Next(1024 * 1024),
                counter++,
                fileExtension);

            var tempFilePath = System.IO.Path.Combine(directoryPath, tempFileName);
            new FileStream(tempFilePath, System.IO.FileMode.CreateNew).Dispose();

            return tempFilePath;
        }
    }
}
