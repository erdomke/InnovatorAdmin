using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public interface INetCredentials : ICredentials
  {
    System.Net.ICredentials Credentials { get; }
  }
}
