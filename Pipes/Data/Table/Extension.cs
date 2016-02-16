using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes.Conversion;

namespace Pipes.Data.Table
{
  public static class Extension
  {
    public static void Cell(this ITableWriter writer, IFieldValue value)
    {
      writer.Cell(value.Name, value.Value);
    }

    public static ITable SelectColumns(this ITable table, Func<IColumnMetadata, bool> criteria)
    {
      var queryable = table as IQueryableTable;
      if (queryable == null) queryable = new QueryableTable(table);
      queryable.AddColumnFilter(criteria);
      return queryable;
    }

    public static ITable Where(this ITable table, Func<IDataRecord, bool> criteria)
    {
      var queryable = table as IQueryableTable;
      if (queryable == null) queryable = new QueryableTable(table);
      queryable.AddRowFilter(criteria);
      return queryable;
    }

    public static void WriteTo(this IEnumerable<IDataRecord> data, IEnumerable<IColumnMetadata> cols, ITableWriter writer)
    {
      var colList = cols.ToList();
      
      writer.Head();
      foreach (var col in colList)
      {
        writer.Column(col);
      }
      writer.HeadEnd();
      foreach (var row in data)
      {
        writer.Row();
        foreach (var col in colList)
        {
          if (row.Status(col.Name) == FieldStatus.FilledIn)
          {
            writer.Cell(col.Name, BaseConverters.Convert(col.DataType, row.Item(col.Name)));
          }
          else
          {
            writer.Cell(col.Name, null);
          }
        }
        writer.RowEnd();
      }
      writer.Flush();
    }

    public static void WriteTo(this ITable table, ITableWriter writer)
    {
      table.WriteTo(table.Columns, writer);
    }
  }
}
