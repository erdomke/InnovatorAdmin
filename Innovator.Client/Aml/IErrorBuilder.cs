using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public interface IErrorBuilder
  {
    IErrorBuilder ErrorContext(IReadOnlyItem item);
    IErrorBuilder ErrorMsg(string message);
    IErrorBuilder ErrorMsg(string message, params string[] properties);
    IErrorBuilder ErrorMsg(string message, IEnumerable<string> properties);
  }
}
