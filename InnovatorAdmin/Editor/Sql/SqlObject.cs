using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin.Editor
{
  public class SqlObject
  {
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Schema { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Definition { get; set; }
    public bool IsTableValued { get; set; }
    public IPromise<IEnumerable<SqlColumn>> Columns { get; set; }

    public static SqlObject Create(string schema, string name, string type, int id, int parentId, bool tableValued)
    {
      return new SqlObject()
      {
        Schema = schema,
        Name = name,
        Type = type,
        Id = id,
        ParentId = parentId,
        IsTableValued = tableValued
      };
    }
  }
}
