using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.ParserMapping
{
  public interface IDestinationColumn
  {
    string Name { get; }
    string Label { get; }
  }
}
