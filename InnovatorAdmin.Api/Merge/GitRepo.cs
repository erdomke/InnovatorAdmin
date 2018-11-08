using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class GitRepo : IDisposable
  {
    private Repository _repo;

    public GitRepo(string repoPath)
    {
      _repo = new Repository(repoPath);
    }

    public IMergeOperation GetMerge(string localBranch, string remoteBranch)
    {
      return new GitMergeOperation(_repo, localBranch, remoteBranch);
    }

    public IDiffDirectory GetDirectory(GitDirectorySearch options)
    {
      var commit = default(Commit);
      if (!string.IsNullOrEmpty(options.Sha))
        commit = _repo.Commits.Single(c => c.Sha.StartsWith(options.Sha));
      else if (options.BranchNames.Count == 1)
        commit = _repo.Branches[options.BranchNames.First()].Tip;
      else if (options.BranchNames.Count > 1)
        commit = MostRecentCommonAncestor(options.BranchNames);
      else
        commit = _repo.Head.Tip;

      return new GitDiffDirectory(commit, options.Path);
    }

    /// <remarks>Trying to avoid loading all commits into memory at once.</remarks>
    private Commit MostRecentCommonAncestor(IList<string> branchNames)
    {
      var walkers = branchNames.Select(b => _repo.Commits.QueryBy(new CommitFilter()
      {
        IncludeReachableFrom = _repo.Branches[b]
      }).GetEnumerator()).ToList();

      var branchCounts = new Dictionary<string, int>();
      while (walkers.Count > 0)
      {
        var i = 0;
        while (i < walkers.Count)
        {
          if (walkers[i].MoveNext())
          {
            if (branchCounts.TryGetValue(walkers[i].Current.Sha, out var cnt))
            {
              if ((cnt + 1) == branchNames.Count)
                return walkers[i].Current;
              branchCounts[walkers[i].Current.Sha] = cnt + 1;
            }
            else
            {
              branchCounts[walkers[i].Current.Sha] = 1;
            }
            i++;
          }
          else
          {
            walkers.RemoveAt(i);
          }
        }
      }

      return null;
    }

    public void Dispose()
    {
      _repo.Dispose();
    }
  }
}
