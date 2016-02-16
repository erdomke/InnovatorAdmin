using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.ParserMapping
{
  public interface IDataSetParser
  {
    bool TryGetDataSet(System.IO.Stream stream, out System.Data.DataSet ds);
  }
}
