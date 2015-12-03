using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using Innovator.Server;

namespace Innovator.Client
{
  public interface IAsyncScript
  {
    IPromise<string> Execute(IAsyncConnection conn);
  }
}
