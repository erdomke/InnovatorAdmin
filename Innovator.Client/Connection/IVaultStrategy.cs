using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  public interface IVaultStrategy
  {
    void Initialize(IAsyncConnection conn);
    IPromise<IEnumerable<Vault>> WritePriority(bool async);
    IPromise<IEnumerable<Vault>> ReadPriority(bool async);
  }
}
