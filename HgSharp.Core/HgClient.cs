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
    public class HgClient
    {
        public HgClient(HgRepository repository)
        {
            
        }

        public static HgRepository Clone(string sourceRepositoryPath, string destinationRepositoryPath)
        {
            //
            // Create dummy repo and then clone over existing files
            var sourceHgRepository = new HgRepository(sourceRepositoryPath);
            var destinationHgRepository = HgRepository.Create(destinationRepositoryPath);

            using(sourceHgRepository.AcquireLock())
            {
                foreach(var hgDataFile in sourceHgRepository.Store.GetDataFiles())
                {
                    var destinationPath = System.IO.Path.Combine(destinationRepositoryPath, ".hg\\store", hgDataFile.Path.FullPath.Trim('/'));
                
                    var directoryName = System.IO.Path.GetDirectoryName(destinationPath);
                    if(!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    File.CreateHardlink(hgDataFile.StoreRelativePath, destinationPath);
                } // foreach
            } // using

            return destinationHgRepository;
        }
    }
}