namespace HgSharp.Core
{
    public class HgCommitStats
    {
        public int Changesets { get; private set; }

        public int Manifests { get; private set; }

        public int Changes { get; private set; }

        public int ChangedFiles { get; private set; }

        public HgCommitStats(int changesets, int manifests, int changes, int changedFiles)
        {
            Changesets = changesets;
            Manifests = manifests;
            Changes = changes;
            ChangedFiles = changedFiles;
        }
    }
}