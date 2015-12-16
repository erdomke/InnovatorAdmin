using HgSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class Upgrade
  {
    public static void Merge()
    {
      var changes = GetChanges(@"C:\Users\edomke\Documents\Local_Projects\ArasUpgrade")
        .OrderBy(f => f.Path).ToArray();

      var test = 2;
    }

    public static IEnumerable<FileCompare> GetChanges(string repoPath)
    {
      var repo = new HgRepository(repoPath);
      var heads = repo.GetHeads().Select(h => repo.Changelog[h.NodeID]).ToArray();
      var local = heads.FirstOrDefault(h => h.Branch.Name == "default") ?? heads.First();
      var remote = heads.First(h => h.Metadata.NodeID != local.Metadata.NodeID);
      var baseRev = repo.CommonParent(local, remote);

      var dict = new Dictionary<string, FileCompare>();


      foreach (var file in ChangesetManifestPaths(repo, baseRev))
      {
        dict[file] = new FileCompare() { Path = file, InBase = FileStatus.Unchanged };
      }

      FileCompare buffer;
      var modifiedFiles = new HashSet<string>(local.Files);
      foreach (var file in ChangesetManifestPaths(repo, local))
      {
        if (!dict.TryGetValue(file, out buffer))
        {
          buffer = new FileCompare() { Path = file };
          dict.Add(file, buffer);
        }
        buffer.InLocal = modifiedFiles.Contains(file) ? FileStatus.Modified : FileStatus.Unchanged;
          //repo.GetFileHistory(new HgPath(file))
          //.Any(c => c.Metadata.NodeID == local.Metadata.NodeID)
          //? FileStatus.Modified : FileStatus.Unchanged;
      }

      modifiedFiles = new HashSet<string>(remote.Files);
      foreach (var file in ChangesetManifestPaths(repo, remote))
      {
        if (!dict.TryGetValue(file, out buffer))
        {
          buffer = new FileCompare() { Path = file };
          dict.Add(file, buffer);
        }
        buffer.InRemote = modifiedFiles.Contains(file) ? FileStatus.Modified : FileStatus.Unchanged;
        //buffer.InRemote = repo.GetFileHistory(new HgPath(file))
        //  .Any(c => c.Metadata.NodeID == remote.Metadata.NodeID)
        //  ? FileStatus.Modified : FileStatus.Unchanged;
      }

      return dict.Values;
    }

    private static IEnumerable<string> ChangesetManifestPaths(HgRepository repo, HgChangeset ch)
    {
      return repo.Manifest[ch.ManifestNodeID].Files.Select(f => f.Path.FullPath.TrimStart('/'));
    }
  }
}
