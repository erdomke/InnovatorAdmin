using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes.Xml;

namespace Pipes.Aras
{
  public interface IError : IItem
  {
    string Code { get; }
    IXmlElement Data { get; }
    string Detail { get; }
    string Message { get; }
  }
}
