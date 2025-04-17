using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace InnovatorAdmin
{
  internal class GitPackage : IPackage
  {
    private Repository _repository;
    private Dictionary<string, IPackageFile> _files = new Dictionary<string, IPackageFile>(StringComparer.OrdinalIgnoreCase);

    public GitPackage(Repository repository, Commit commit, string root = null)
    {
      _repository = repository;
      var files = new List<IPackageFile>();
      root = string.IsNullOrEmpty(root) ? "\\" : "\\" + root.Trim('\\').Replace('/', '\\') + "\\";
      GitMergeOperation.WalkTree(commit.Tree, (path, blob) =>
      {
        if (("\\" + path).Replace('/', '\\').StartsWith(root))
        {
          var relPath = path.Substring(root.Length - 1).TrimStart('/', '\\');
          files.Add(new GitDiffFile(blob) { Path = relPath });
        }
      });
      _files = files.ToDictionary(f => f.Path.Trim());
    }

    public bool TryAccessFile(string path, bool create, out IPackageFile file)
    {
      if (create)
        throw new NotSupportedException();
      return _files.TryGetValue(path.Replace('\\', '/').Trim(), out file); ;
    }

    public IEnumerable<IPackageFile> Files()
    {
      return _files.Values
        .Where(f => !f.Path.EndsWith(".innpkg", StringComparison.OrdinalIgnoreCase));
    }

    public IPackageFile Manifest(bool create)
    {
      return _files.Values.FirstOrDefault(f => f.Path.EndsWith(".innpkg", StringComparison.OrdinalIgnoreCase));
    }

    public void Dispose()
    {
      _repository.Dispose();
    }

    [DebuggerDisplay("{Path,nq}")]
    private class GitDiffFile : IPackageFile
    {
      private Blob _blob;

      public GitDiffFile(Blob blob)
      {
        _blob = blob;
      }
      public string Path { get; set; }

      public Stream Open()
      {
        return _blob.GetContentStream();
      }
    }
  }
}
