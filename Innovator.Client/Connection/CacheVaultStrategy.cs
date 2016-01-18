using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  internal class CacheVaultStrategy: IVaultStrategy
  {
    private IVaultStrategy _child;
    private IPromise<IEnumerable<Vault>> _writePriority;
    private IPromise<IEnumerable<Vault>> _readPriority;

    public CacheVaultStrategy(IVaultStrategy child)
    {
      _child = child;
    }

    public void Initialize(IAsyncConnection conn)
    {
      _child.Initialize(conn);
    }

    public IPromise<IEnumerable<Vault>> WritePriority(bool async)
    {
      if (_writePriority == null) 
        _writePriority = _child.WritePriority(async);
      if (!async) _writePriority.Wait();
      return _writePriority;
    }

    public IPromise<IEnumerable<Vault>> ReadPriority(bool async)
    {
      if (_readPriority == null)
        _readPriority = _child.ReadPriority(async);
      if (!async) _readPriority.Wait();
      return _readPriority;
    }
  }
}
