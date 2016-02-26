using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InnovatorAdmin
{
  class GitDiffDirectory : IDiffDirectory
  {
    private IEnumerable<IDiffFile> _files;

    public GitDiffDirectory(Commit commit)
    {
      var files = new List<IDiffFile>();
      GitMergeOperation.WalkTree(commit.Tree, (path, blob) =>
      {
        files.Add(new GitDiffFile(blob) { Path = path });
      });
      _files = files;
    }

    public IEnumerable<IDiffFile> GetFiles()
    {
      return _files;
    }

    private class GitDiffFile : IDiffFile
    {
      private Blob _blob;

      public GitDiffFile(Blob blob)
      {
        _blob = blob;
      }

      public IComparable CompareKey { get { return _blob.Sha; } }
      public string Path { get; set; }

      public Stream OpenRead()
      {
        return _blob.GetContentStream();
      }
    }
  }
}
