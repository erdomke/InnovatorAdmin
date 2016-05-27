using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Specifies that an <see cref="System.Net.ICredentials"/> instance can be extracted from the credentials object
  /// </summary>
  public interface INetCredentials : ICredentials
  {
    /// <summary>
    /// An <see cref="System.Net.ICredentials"/> instance with the same user name and password
    /// </summary>
    System.Net.ICredentials Credentials { get; }
  }
}
