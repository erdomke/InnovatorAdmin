using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace InnovatorAdmin.Editor
{
  public interface ISqlMetadataProvider
  {
    IEnumerable<string> GetSchemaNames();
    IEnumerable<string> GetTableNames();
    IPromise<IEnumerable<ListValue>> GetColumnNames(string tableName);
  }
}
