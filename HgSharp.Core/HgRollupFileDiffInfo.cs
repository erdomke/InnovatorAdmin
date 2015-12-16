using System.Linq;

namespace HgSharp.Core
{
    public class HgRollupFileDiffInfo
    {
        public HgPath File { get; private set; }

        public bool IsBinary { get; private set; } 

        public HgUnifiedDiff Diff { get; private set; }

        public int Additions { get; private set; }

        public int Removals { get; private set; }

        public HgRollupFileDiffInfo(HgPath file, bool isBinary, HgUnifiedDiff diff, int additions, int removals)
        {
            File = file;
            Diff = diff;
            IsBinary = isBinary;
            Additions = additions;
            Removals = removals;
        }
    }
}