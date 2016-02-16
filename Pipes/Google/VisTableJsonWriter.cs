using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Google
{
  public class VisTableJsonWriter : Data.Table.ITableWriter
  {
    private Json.IJsonWriter _writer;

    public VisTableJsonWriter() : this(new VisJsonTextWriter(new System.IO.StringWriter())) { }
    public VisTableJsonWriter(Json.IJsonWriter writer)
    {
      _writer = writer;
      _writer.Object();
    }

    public void Flush()
    {
      _writer.ArrayEnd().ObjectEnd();
      _writer.Flush();
    }

    public Data.Table.ITableWriter Cell(string name, object value)
    {
      if (value == null || value == DBNull.Value)
      {
        _writer.Object().ObjectEnd();
      }
      else
      {
        _writer.Object().Prop("v", value).ObjectEnd();
      }
      return this;
    }

    public Data.Table.ITableWriter Column(Data.Table.IColumnMetadata column)
    {
      _writer.Object().Prop("id", column.Name).Prop("label", column.Label);
      if (column.DataType == typeof(int) || column.DataType == typeof(long) || column.DataType == typeof(double) ||
          column.DataType == typeof(decimal) || column.DataType == typeof(float) || column.DataType == typeof(byte) ||
          column.DataType == typeof(short) || column.DataType == typeof(sbyte) || column.DataType == typeof(ushort) ||
          column.DataType == typeof(uint) || column.DataType == typeof(ulong) || typeof(Enum).IsAssignableFrom(column.DataType))
      {
        _writer.Prop("type", "number");
      }
      else if (column.DataType == typeof(bool))
      {
        _writer.Prop("type", "boolean");
      }
      else if (column.DataType == typeof(DateTime))
      {
        _writer.Prop("type", "datetime");
      }
      else
      {
        _writer.Prop("type", "string");
      }
      _writer.ObjectEnd();
      return this;
    }

    public Data.Table.ITableWriter Head()
    {
      _writer.Prop("cols").Array();
      return this;
    }

    public Data.Table.ITableWriter HeadEnd()
    {
      _writer.ArrayEnd().Prop("rows").Array();
      return this;
    }

    public Data.Table.ITableWriter Row()
    {
      _writer.Object().Prop("c").Array();
      return this;
    }

    public Data.Table.ITableWriter RowEnd()
    {
      _writer.ArrayEnd().ObjectEnd();
      return this;
    }

    public override string ToString()
    {
      return _writer.ToString();
    }
  }
}
