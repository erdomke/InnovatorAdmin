using System;

namespace HgSharp.Core
{
    public class HgRepositoryContext
    {
        private readonly HgStore store;

        private readonly Lazy<HgChangelog> changelog; 
        public HgChangelog Changelog
        {
            get { return changelog.Value; }
        }

        public HgRepositoryContext()
        {
            changelog = new Lazy<HgChangelog>(() => store.GetChangelog());
        }
    }
}
