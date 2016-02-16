using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public interface IColumnMetadata
  {
    Type DataType { get; }
    string Label { get; }
    string Name { get; }
  }
}
