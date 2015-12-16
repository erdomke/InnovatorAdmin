using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin.Editor
{
  public class SqlObject
  {
    public string Schema { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Definition { get; set; }
    public bool IsTableValued { get; set; }
    public IPromise<IEnumerable<SqlColumn>> Columns { get; set; }
  }
}
