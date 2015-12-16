using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HgSharp.Core
{
    public class HgCompareInfo
    {
        public HgChangeset Base { get; private set; }

        public HgChangeset Head { get; private set; }

        public ReadOnlyCollection<HgChangeset> Changesets { get; private set; }

        public ReadOnlyCollection<HgRollupFileDiffInfo> Files { get; private set; } 

        public HgCompareInfo(HgChangeset @base, HgChangeset head, IList<HgChangeset> changesets, IList<HgRollupFileDiffInfo> files)
        {
            Base = @base;
            Head = head;
            Changesets = new ReadOnlyCollection<HgChangeset>(changesets);
            Files = new ReadOnlyCollection<HgRollupFileDiffInfo>(files);
        }
    }
}