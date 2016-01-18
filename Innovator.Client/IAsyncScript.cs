using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public interface IAsyncScript
  {
    IPromise<string> Execute(IAsyncConnection conn);
  }
}
