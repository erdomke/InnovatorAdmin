using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public interface IServerCache
  {
    object this[string key] { get; set; }
    T Get<T>(string key);
  }
}
