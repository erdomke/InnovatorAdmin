using System.Linq;
using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public class HgLargefilesEnabledManifest : HgManifest
    {
        private readonly HgManifest underlyingManifest;

        public HgLargefilesEnabledManifest(HgManifest underlyingManifest) :
            base(null, null, null)
        {
            this.underlyingManifest = underlyingManifest;
        }

        public override HgRevlog Revlog
        {
            get { return underlyingManifest.Revlog; }
        }

        public override HgManifestEntry this[HgNodeID hgNodeID]
        {
            get
            {
                var largefilesEnabledManifestEntry = underlyingManifest[hgNodeID];
                var manifestEntry = new HgManifestEntry(
                    largefilesEnabledManifestEntry.Metadata, 
                    largefilesEnabledManifestEntry.Files.Select(f => new HgManifestFileEntry(new HgPath(f.Path.FullPath.SubstringAfter(".hglf/")), f.FilelogNodeID)).ToList());

                return manifestEntry;
            }
        }

        public override HgManifestEntry this[uint revision]
        {
            get
            {
                var largefilesEnabledManifestEntry = underlyingManifest[revision];
                var manifestEntry = new HgManifestEntry(
                    largefilesEnabledManifestEntry.Metadata, 
                    largefilesEnabledManifestEntry.Files.Select(f => new HgManifestFileEntry(new HgPath(f.Path.FullPath.SubstringAfter(".hglf/")), f.FilelogNodeID)).ToList());

                return manifestEntry;
            }
        }
    }
}