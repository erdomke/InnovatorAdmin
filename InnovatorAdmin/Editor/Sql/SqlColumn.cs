using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class SqlColumn
  {
    public string Name { get; set; }
    public string Type { get; set; }

    public SqlColumn() { }
    public SqlColumn(string name, string type)
    {
      this.Name = name;
      this.Type = type;
    }
  }
}
