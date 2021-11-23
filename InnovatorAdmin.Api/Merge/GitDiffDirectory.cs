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

    public GitDiffDirectory(Commit commit, string root = null)
    {
      var files = new List<IDiffFile>();
      root = string.IsNullOrEmpty(root) ? "\\" : "\\" + root.Trim('\\').Replace('/', '\\') + "\\";
      GitMergeOperation.WalkTree(commit.Tree, (path, blob) =>
      {
        if (("\\" + path).Replace('/', '\\').StartsWith(root))
        {
          var relPath = path.Substring(root.Length - 1).TrimStart('/', '\\');
          files.Add(new GitDiffFile(blob) { Path = relPath });
        }
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
