using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class SqlTableInfo
  {
    public SqlName Name { get; set; }
    public string Alias { get; set; }
    public IEnumerable<string> Columns { get; set; }
    public IEnumerable<SqlTableInfo> AdditionalColumns { get; set; }
  }
}
