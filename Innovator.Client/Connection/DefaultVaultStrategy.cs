using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  public class DefaultVaultStrategy : IVaultStrategy
  {
    private IAsyncConnection _conn;
    private IPromise<User> _userInfo;

    public void Initialize(IAsyncConnection conn)
    {
      _conn = conn;
    }

    public IPromise<IEnumerable<Vault>> WritePriority(bool async)
    {
      return GetUserInfo(async)
        .Convert(u => Enumerable.Repeat(u.DefaultVault, 1).Concat(u.ReadPriority));
    }

    public IPromise<IEnumerable<Vault>> ReadPriority(bool async)
    {
      return GetUserInfo(async)
        .Convert(u => u.ReadPriority.Concat(Enumerable.Repeat(u.DefaultVault, 1)));
    }

    private IPromise<User> GetUserInfo(bool async)
    {
      if (_userInfo == null)
      {
        _userInfo = _conn.ItemByQuery(new Command("<Item type=\"User\" action=\"get\" select=\"default_vault\" expand=\"1\"><id>@0</id><Relationships><Item type=\"ReadPriority\" action=\"get\" select=\"priority, related_id\" expand=\"1\" orderBy=\"priority\"/></Relationships></Item>", _conn.UserId), async)
          .FailOver(() => _conn.ItemByQuery(new Command("<Item type=\"User\" action=\"get\" select=\"default_vault\" expand=\"1\"><id>@0</id></Item>", _conn.UserId), async))
          .Convert<IReadOnlyItem, User>(i =>
          {
            var result = new User();
            result.Id = i.Id();
            var vault = i.Property("default_vault").AsItem();
            if (vault.Exists) result.DefaultVault = Vault.GetVault(vault);
            foreach (var rel in i.Relationships("ReadPriority"))
            {
              vault = rel.RelatedId().AsItem();
              if (vault.Exists)
              {
                result.ReadPriority.Add(Vault.GetVault(vault));
              }
            }
            return result;
          })
          .Fail(ex => { ex.Rethrow(); });
      }
      return _userInfo;
    }

    private class User
    {
      private List<Vault> _vaults = new List<Vault>();

      public string Id { get; set; }
      public Vault DefaultVault { get; set; }
      public IList<Vault> ReadPriority { get { return _vaults; } }
    }
  }
}
