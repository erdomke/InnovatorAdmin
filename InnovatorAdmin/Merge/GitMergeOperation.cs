using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class GitMergeOperation : IMergeOperation
  {
    private readonly Repository _repo;
    private readonly IEnumerable<FileCompare> _compares;
    private readonly Commit _localCommit;
    private readonly Commit _remoteCommit;
    private readonly Commit _baseCommit;
    private readonly string _path;

    public GitMergeOperation(Repository repo, string localBranch, string remoteBranch)
    {
      _repo = repo;
      var local = _repo.Branches[localBranch];
      _localCommit = local.Commits.First();
      var shas = new HashSet<string>(local.Commits.Select(c => c.Sha));
      var remote = _repo.Branches[remoteBranch];
      _remoteCommit = remote.Commits.First();
      _baseCommit = remote.Commits.First(c => shas.Contains(c.Sha));

      var list = new Dictionary<string, FileShas>();
      WalkTree(_baseCommit.Tree, (path, blob) => list.Add(path, new FileShas() { Path = path, BaseSha = blob.Sha }));
      WalkTree(_localCommit.Tree, (path, blob) =>
      {
        FileShas info;
        if (list.TryGetValue(path, out info))
          info.LocalSha = blob.Sha;
        else
          list.Add(path, new FileShas() { Path = path, LocalSha = blob.Sha });
      });
      WalkTree(_remoteCommit.Tree, (path, blob) =>
      {
        FileShas info;
        if (list.TryGetValue(path, out info))
          info.RemoteSha = blob.Sha;
        else
          list.Add(path, new FileShas() { Path = path, RemoteSha = blob.Sha });
      });

      _compares = list.Values.Select(s => new FileCompare()
      {
        Path = s.Path,
        InBase = string.IsNullOrEmpty(s.BaseSha) ? FileStatus.DoesntExist : FileStatus.Unchanged,
        InLocal = string.IsNullOrEmpty(s.LocalSha) ? FileStatus.DoesntExist : (s.LocalSha == s.BaseSha ? FileStatus.Unchanged : FileStatus.Modified),
        InRemote = string.IsNullOrEmpty(s.RemoteSha) ? FileStatus.DoesntExist : (s.RemoteSha == s.BaseSha ? FileStatus.Unchanged : FileStatus.Modified)
      }).ToArray();

      var i = _repo.Info.Path.LastIndexOf(".git");
      _path = _repo.Info.Path.Substring(0, i - 1);
    }

    internal static void WalkTree(Tree parent, Action<string, Blob> visitor)
    {
      foreach (var node in parent)
      {
        switch (node.TargetType)
        {
          case TreeEntryTargetType.Tree:
            WalkTree((Tree)node.Target, visitor);
            break;
          case TreeEntryTargetType.Blob:
            visitor(node.Path, (Blob)node.Target);
            break;
          default:
            Debug.Print("{0}: {1}", node.Path, node.TargetType);
            break;
        }
      }
    }

    private class FileShas
    {
      public string Path { get; set; }
      public string BaseSha { get; set; }
      public string LocalSha { get; set; }
      public string RemoteSha { get; set; }
    }

    public IEnumerable<FileCompare> GetChanges()
    {
      return _compares;
    }

    public Stream GetLocal(string relPath)
    {
      return GetFileBytes(_localCommit, relPath);
    }

    public MergeFilePaths GetPaths(string relPath)
    {
      var fileName = Path.GetFileNameWithoutExtension(relPath)
        + "_" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
        + Path.GetExtension(relPath);
      var tempFolder = Path.GetTempPath();

      var result = new MergeFilePaths()
      {
        Base = Path.Combine(tempFolder, "Base_" + fileName),
        Local = Path.Combine(tempFolder, "Local_" + fileName),
        Remote = Path.Combine(tempFolder, "Remote_" + fileName),
        Merged = Path.Combine(_path, relPath)
      };

      using (var write = new FileStream(result.Base, FileMode.Create, FileAccess.Write))
      {
        GetFileBytes(_baseCommit, relPath).CopyTo(write);
      }

      GetFileBytes(_baseCommit, relPath).WriteTo(result.Base);
      GetFileBytes(_localCommit, relPath).WriteTo(result.Local);
      GetFileBytes(_remoteCommit, relPath).WriteTo(result.Remote);

      return result;
    }

    public Stream GetRemote(string relPath)
    {
      return GetFileBytes(_remoteCommit, relPath);
    }

    public string MergePath(string relPath)
    {
      return Path.Combine(_path, relPath);
    }

    private Stream GetFileBytes(Commit commit, string path)
    {
      var entry = commit.Tree[path];
      if (entry == null || entry.TargetType != TreeEntryTargetType.Blob)
        return new MemoryStream(new byte[] { });
      var blob = (Blob)entry.Target;
      return blob.GetContentStream();
    }
  }
}
