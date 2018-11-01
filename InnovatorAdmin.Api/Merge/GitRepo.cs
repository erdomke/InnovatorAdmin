using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class GitRepo
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

    public IDiffDirectory GetDirectory(string id = null, string path = null)
    {
      var commit = string.IsNullOrEmpty(id)
        ? _repo.Head.Tip
        : _repo.Commits.Single(c => c.Sha.StartsWith(id));
      return new GitDiffDirectory(commit, path);
    }
  }
}
