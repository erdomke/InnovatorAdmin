using HgSharp.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class HgDiffDirectory : IDiffDirectory
  {
    private HgManifestEntry _manifest;
    private HgRepository _repo;
    private Dictionary<string, HgDiffFile> _files;

    public HgDiffDirectory(HgRepository repo, HgChangeset changeset)
    {
      _repo = repo;
      var manifest = repo.Manifest[changeset.ManifestNodeID];
      _files = manifest.Files
        .Select(f => new HgDiffFile(repo, manifest, f.Path.FullPath.TrimStart('/')))
        .ToDictionary(f => f.Path);

      var changes = new Queue<HgChangeset>();
      changes.Enqueue(changeset);
      HgDiffFile file;
      HgChangeset curr;

      while (changes.Any() && _files.Values.Any(f => f.Revision < 0))
      {
        curr = changes.Dequeue();
        foreach (var path in curr.Files)
        {
          if (_files.TryGetValue(path, out file) && file.Revision < 0)
            file.Revision = (int)curr.Metadata.Revision;
        }

        foreach (var change in curr.Metadata.Parents.Select(r => _repo.Changelog[r.NodeID]))
        {
          changes.Enqueue(change);
        }
      }
    }

    public IEnumerable<IDiffFile> GetFiles()
    {
      return _files.Values;
    }

    private class HgDiffFile : IDiffFile
    {
      private HgRepository _repo;
      private HgManifestEntry _manifest;

      public string Path { get; set; }
      public int Revision { get; set; }
      public IComparable CompareKey { get { return this.Revision; }}

      public HgDiffFile(HgRepository repo, HgManifestEntry manifest, string path)
      {
        this.Path = path;
        this.Revision = -1;
        _repo = repo;
        _manifest = manifest;
      }

      public Stream OpenRead()
      {
        var fileNode = _manifest.GetFile(this.Path);
        if (fileNode == null) return new MemoryStream();
        return new MemoryStream(_repo.GetFile(fileNode).Data);
      }
    }
  }
}
