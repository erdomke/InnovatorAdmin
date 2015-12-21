using HgSharp.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class HgMergeOperation : IMergeOperation
  {
    private HgRepository _repo;
    private IEnumerable<FileCompare> _compares;
    private HgChangeset _local;
    private HgChangeset _remote;
    private HgChangeset _baseRev;

    public HgMergeOperation(string repoPath, int local = -1, int remote = -1)
    {
      _repo = new HgRepository(repoPath);

      var heads = _repo.GetHeads().Select(h => _repo.Changelog[h.NodeID]).ToArray();
      _local = local < 0
        ? heads.FirstOrDefault(h => h.Branch.Name == "default") ?? heads.First()
        : _repo.Changelog[(uint)local];
      _remote = remote < 0
        ? heads.First(h => h.Metadata.NodeID != _local.Metadata.NodeID)
        : _repo.Changelog[(uint)remote];
      _baseRev = _repo.CommonParent(_local, _remote);

      var dict = new Dictionary<string, FileCompare>();

      foreach (var file in ChangesetManifestPaths(_repo, _baseRev))
      {
        dict[file] = new FileCompare() { Path = file, InBase = FileStatus.Unchanged };
      }

      FileCompare buffer;
      var modifiedFiles = new HashSet<string>(_local.Files);
      foreach (var file in ChangesetManifestPaths(_repo, _local))
      {
        if (!dict.TryGetValue(file, out buffer))
        {
          buffer = new FileCompare() { Path = file };
          dict.Add(file, buffer);
        }
        buffer.InLocal = modifiedFiles.Contains(file) ? FileStatus.Modified : FileStatus.Unchanged;
      }

      modifiedFiles = new HashSet<string>(_remote.Files);
      foreach (var file in ChangesetManifestPaths(_repo, _remote))
      {
        if (!dict.TryGetValue(file, out buffer))
        {
          buffer = new FileCompare() { Path = file };
          dict.Add(file, buffer);
        }
        buffer.InRemote = modifiedFiles.Contains(file) ? FileStatus.Modified : FileStatus.Unchanged;
      }

      _compares = dict.Values.ToArray();
    }

    public IEnumerable<FileCompare> GetChanges()
    {
      return _compares;
    }

    private static IEnumerable<string> ChangesetManifestPaths(HgRepository repo, HgChangeset ch)
    {
      return repo.Manifest[ch.ManifestNodeID].Files.Select(f => f.Path.FullPath.TrimStart('/'));
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
        Merged = Path.Combine(_repo.FullPath, relPath)
      };

      File.WriteAllBytes(result.Base, GetFileBytes(_baseRev, relPath));
      File.WriteAllBytes(result.Local, GetFileBytes(_local, relPath));
      File.WriteAllBytes(result.Remote, GetFileBytes(_remote, relPath));

      return result;
    }

    public Stream GetLocal(string relPath)
    {
      return new MemoryStream(GetFileBytes(_local, relPath));
    }

    public Stream GetRemote(string relPath)
    {
      return new MemoryStream(GetFileBytes(_remote, relPath));
    }

    private byte[] GetFileBytes(HgChangeset change, string relPath)
    {
      var fileNode = _repo.Manifest[change.ManifestNodeID].GetFile(relPath);
      if (fileNode == null) return new byte[] { };
      return _repo.GetFile(fileNode).Data;
    }

    public string MergePath(string relPath)
    {
      return Path.Combine(_repo.FullPath, relPath);
    }

    public IDiffDirectory GetDirectory(int rev)
    {
      var heads = _repo.GetHeads().Select(h => _repo.Changelog[h.NodeID]).ToArray();
      var ch = rev < 0
        ? heads.FirstOrDefault(h => h.Branch.Name == "default") ?? heads.First()
        : _repo.Changelog[(uint)rev];
      return new HgDiffDirectory(_repo, ch);
    }

    public static void Test()
    {
      var repo = new HgMergeOperation(@"C:\Users\edomke\Documents\Local_Projects\ArasUpgrade");
      var baseDir = repo.GetDirectory(2);
      var compareDir = repo.GetDirectory(-1);
      baseDir.WriteAmlMergeScripts(compareDir, @"C:\Users\edomke\Documents\Local_Projects\ArasUpgradeMerge\");
      //var diffs = baseDir.GetDiffs(compareDir);
      var thing = 2;
    }


    //public IEnumerable<FileDiff> GetDiffMergeToLocal(int localRev, int mergeRev)
    //{
    //  var local = _repo.GetChangesets().Single(c => c.Metadata.Revision == localRev);
    //  var merge = _repo.GetChangesets().Single(c => c.Metadata.Revision == mergeRev);

    //  var localPaths = ChangesetManifestPaths(_repo, local).OrderBy(i => i).ToArray();
    //  var mergePaths = ChangesetManifestPaths(_repo, merge).OrderBy(i => i).ToArray();

    //  var results = new List<FileDiff>();
    //  localPaths.MergeSorted(mergePaths, i => i, (i, l, m) =>
    //  {

    //  });
    //  return results;
    //}
  }
}
