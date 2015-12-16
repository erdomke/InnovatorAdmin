using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HgSharp.Core
{
  public static class Extensions
  {
    public static IEnumerable<HgRevision> Parents(this HgRepository repo, HgChangeset change)
    {
      var changEnum = new ChangeSetParentEnumerator(repo, change);
      while (changEnum.MoveNext())
      {
        foreach (var curr in changEnum.Current)
        {
          yield return curr;
        }
      }
    }

    public static HgChangeset CommonParent(this HgRepository repo, HgChangeset local, HgChangeset remote)
    {
      var localEnum = new ChangeSetParentEnumerator(repo, local);
      var remoteEnum = new ChangeSetParentEnumerator(repo, remote);
      HgRevision maxCommon;

      while (localEnum.MoveNext() && remoteEnum.MoveNext())
      {
        if (TryGetMaxCommon(localEnum.All, remoteEnum.All, out maxCommon))
          return repo.Changelog[maxCommon.Revision];
      }
      while (localEnum.MoveNext())
      {
        if (TryGetMaxCommon(localEnum.All, remoteEnum.All, out maxCommon))
          return repo.Changelog[maxCommon.Revision];
      }
      while (remoteEnum.MoveNext())
      {
        if (TryGetMaxCommon(localEnum.All, remoteEnum.All, out maxCommon))
          return repo.Changelog[maxCommon.Revision];
      }

      return null;
    }

    private static bool TryGetMaxCommon(IEnumerable<HgRevision> x, IEnumerable<HgRevision> y, out HgRevision max)
    {
      var set = new HashSet<HgRevision>(x);
      set.IntersectWith(y);
      max = set.Any() ? set.OrderByDescending(r => r.Revision).First() : new HgRevision();
      return set.Any();
    }

    private class ChangeSetParentEnumerator : IEnumerator<IEnumerable<HgRevision>>
    {
      private HgRepository _repo;
      private HgChangeset _root;
      private IEnumerable<HgRevision> _current;
      private List<HgRevision> _all = new List<HgRevision>();
      private bool _atEnd;

      public IEnumerable<HgRevision> All
      {
        get { return _all; }
      }
      public IEnumerable<HgRevision> Current
      {
        get { return _atEnd ? Enumerable.Empty<HgRevision>() : _current; }
      }

      public ChangeSetParentEnumerator(HgRepository repo, HgChangeset root)
      {
        _repo = repo;
        _root = root;
        Reset();
      }

      public void Dispose()
      {
        // Do nothing
      }

      object System.Collections.IEnumerator.Current
      {
        get { return this.Current; }
      }

      public bool MoveNext()
      {
        if (_atEnd) return false;

        if (this._current == null)
        {
          _current = _root.Metadata.Parents;
        }
        else
        {
          _current = _current.SelectMany(r => _repo.Changelog[r.NodeID].Metadata.Parents);
        }

        _atEnd = _current == null || !_current.Any();
        if (!_atEnd)
          _all.AddRange(_current);

        return !_atEnd;
      }

      public void Reset()
      {
        _current = null;
        _atEnd = false;
        _all.Clear();
      }
    }
  }
}
