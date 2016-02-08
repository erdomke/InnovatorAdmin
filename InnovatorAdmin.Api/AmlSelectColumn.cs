using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class AmlSelectColumn
  {
    private IList<AmlSelectColumn> _children = new List<AmlSelectColumn>();

    public string Name { get; set; }
    public IEnumerable<AmlSelectColumn> Children { get { return _children; } }

    public void Add(AmlSelectColumn column)
    {
      var existing = _children.FirstOrDefault(c => string.Equals(c.Name, column.Name));
      if (existing != null)
      {
        foreach (var child in column.Children)
        {
          existing.Add(child);
        }
      }
      _children.Add(column);
    }

    private void Write(StringBuilder builder, bool first)
    {
      if (!first)
        builder.Append(",");
      builder.Append(this.Name);
      if (_children.Any())
      {
        builder.Append("(");
        var atStart = true;
        foreach (var child in _children)
        {
          child.Write(builder, atStart);
          atStart = false;
        }
        builder.Append(")");
      }
    }

    public override string ToString()
    {
      var builder = new StringBuilder();
      Write(builder, true);
      return builder.ToString();
    }
  }
}
