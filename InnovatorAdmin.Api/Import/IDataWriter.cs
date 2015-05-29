using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin
{
  public interface IDataWriter
  {
    void Row();
    void RowEnd();
    void Cell(string columnName, object value);
  }
}
