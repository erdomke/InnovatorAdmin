using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  public class DbAdapter : IDbDataAdapter
  {
    public DbAdapter(string query, DbConnection conn)
    {
      this.SelectCommand = conn.CreateCommand();
      this.SelectCommand.CommandText = query;
    }

    public int Fill(DataSet dataSet)
    {
      var table = dataSet.Tables.Add("Table");
      return Fill(table);
    }

    public int Fill(DataTable table)
    {
      var reader = this.SelectCommand.ExecuteReader();
      if (table.Columns.Count < 1)
      {
        for (var i = 0; i < reader.FieldCount; i++)
        {
          table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
        }
      }

      int count = 0;
      DataRow row;
      object[] values = new object[reader.FieldCount];
      table.BeginLoadData();
      try
      {
        while (reader.Read())
        {
          row = table.NewRow();
          try
          {
            row.BeginEdit();
            reader.GetValues(values);
            row.ItemArray = values;
            table.Rows.Add(row);
          }
          finally
          {
            row.EndEdit();
            row.AcceptChanges();
          }
        }
      }
      finally
      {
        table.EndLoadData();
      }
      return count;
    }

    #region Implementation
    DataTable[] IDataAdapter.FillSchema(DataSet dataSet, SchemaType schemaType)
    {
      throw new NotSupportedException();
    }

    IDataParameter[] IDataAdapter.GetFillParameters()
    {
      throw new NotSupportedException();
    }

    MissingMappingAction IDataAdapter.MissingMappingAction { get; set; }

    MissingSchemaAction IDataAdapter.MissingSchemaAction { get; set; }

    ITableMappingCollection IDataAdapter.TableMappings
    {
      get { throw new NotSupportedException(); }
    }

    int IDataAdapter.Update(DataSet dataSet)
    {
      throw new NotSupportedException();
    }
    
    public IDbCommand DeleteCommand { get; set; }

    public IDbCommand InsertCommand { get; set; }

    public IDbCommand SelectCommand { get; set; }

    public IDbCommand UpdateCommand { get; set; }

    #endregion
  }
}
