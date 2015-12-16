namespace HgSharp.Core
{
    public class HgDivergenceInfo
    {
        public HgRevset Ahead { get; private set; }

        public HgChangeset LatestHeadChangeset { get; private set; }

        public HgRevset Behind { get; private set; }

        public HgChangeset LatestBaseChangeset { get; private set; }

        public HgDivergenceInfo(HgRevset ahead, HgChangeset latestHeadChangeset, HgRevset behind, HgChangeset latestBaseChangeset)
        {
            Ahead = ahead;
            LatestHeadChangeset = latestHeadChangeset;
            Behind = behind;
            LatestBaseChangeset = latestBaseChangeset;
        }
    }
}