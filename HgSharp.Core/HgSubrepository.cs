using System;
using System.Linq;
using System.Text;

using Alphaleonis.Win32.Filesystem;

namespace HgSharp.Core
{
    public class HgSubrepository
    {
        public HgPath Path { get; private set; }

        public string RepositoryPath { get; private set; }

        public HgNodeID NodeID { get; private set; }

        public HgSubrepository(HgPath path, string repositoryPath, HgNodeID nodeID)
        {
            Path = path;
            RepositoryPath = repositoryPath;
            NodeID = nodeID;
        }
    }
}
