using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class DatabaseList
  {
    public string Id { get; }
    public string Name { get; }
    public string Label { get; }
    public string Description { get; }
    public IEnumerable<ListValue> Values { get; }

    public DatabaseList(IReadOnlyItem list)
    {
      Id = list.Id();
      Name = list.Property("name").Value;
      Label = list.Property("label").Value;
      Description = list.Property("description").Value;
      Values = list.Relationships()
        .Select(i => new ListValue()
        {
          Label = i.Property("label").Value,
          Value = i.Property("value").Value,
          Filter = i.Property("filter").Value
        }).ToList();
    }
  }
}
