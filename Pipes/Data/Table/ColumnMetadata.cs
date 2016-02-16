using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public class ColumnMetadata : IColumnMetadata
  {
    public Type DataType { get; set; }
    public string Label { get; set; }
    public string Name { get; set; }
  }
}
