// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.

using NLog;

namespace HgSharp.Core
{
    public class HgStoreFactory
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public HgStore CreateInstance(string storeBasePath, HgRequirements hgRequirements, HgFileSystem hgFileSystem, HgEncoder hgEncoder)
        {
            var hgStore = CreateInstanceInternal(storeBasePath, hgRequirements, hgFileSystem, hgEncoder);
            
            if(hgRequirements.Requires(HgRequirements.Largefiles))
                hgStore = new HgLargefileEnabledStore(hgStore);

            return hgStore;
        }

        private HgStore CreateInstanceInternal(string storeBasePath, HgRequirements hgRequirements, HgFileSystem hgFileSystem, HgEncoder hgEncoder)
        {
            log.Trace("creating HgStore with requirements '{0}' for repository at '{1}'", string.Join("', '", hgRequirements.Requirements), storeBasePath);

            if(hgRequirements.Requires(HgRequirements.Store))
            {
                if(hgRequirements.Requires(HgRequirements.FnCache))
                {
                    log.Trace("creating HgFnCacheStore");

                    var encodings = HgPathEncodings.FnCache;
                    if(hgRequirements.Requires(HgRequirements.DotEncode))
                        encodings |= HgPathEncodings.DotEncode;

                    return new HgFnCacheStore(encodings, hgEncoder, storeBasePath, hgFileSystem);
                } // if

                log.Trace("creating HgEncodedStore");
                return new HgEncodedStore(hgEncoder, storeBasePath, hgFileSystem);
            }
            
            log.Trace("creating HgBasicStore");
            return new HgBasicStore(hgEncoder, storeBasePath, hgFileSystem);
        }
    }
}