using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Pipes.Data.ParserMapping
{
  public class ParserMapping : IXmlSerializable
  {
    private List<ColumnMapping> _columns = new List<ColumnMapping>();

    public IDataSetParser Parser { get; set; }
    public FileDataDirection DataDirection { get; set; }
    public IList<ColumnMapping> Columns { get { return _columns; } }
    public string Table { get; set; }

    public DataTable ParseData(Stream stream)
    {
      if (this.Parser == null) throw new ArgumentNullException("Parser");
      DataSet dSet;
      if (this.Parser.TryGetDataSet(stream, out dSet))
      {
        if (dSet.Tables.Contains(this.Table))
        {
          var table = dSet.Tables[this.Table];
          var result = new DataTable(this.Table);
          DataColumn newCol;
          foreach (var col in _columns)
          {
            newCol = result.Columns.Add(col.Destination.Name);
            newCol.Caption = col.Destination.Label;
          }

          var sequenceColumns = (from c in _columns
                                 where c.Repeat == CellRepeat.UseSequentialValues
                                 select c).ToDictionary(c => c, c => new ColumnStats());
          DataRow newRow;
          ColumnStats stats;
          string value;

          int sequenceValues = 1; // Always generate at least one row of the repeated values 
                                  // if no sequence columns
          int i = 0;
          while (sequenceValues > 0)
          {
            newRow = result.NewRow();
            sequenceValues = 0;

            foreach (var col in _columns)
            {
              if (!string.IsNullOrEmpty(col.ConstantValue))
              {
                if (col.Destination != null) newRow[col.Destination.Name] = col.ConstantValue;
              }
              else if (col.Repeat == CellRepeat.RepeatForAllRows)
              {
                if (col.Destination != null) newRow[col.Destination.Name] = table.Rows[col.Address.Row][col.Address.Column].ToString();
              }
              else
              {
                stats = sequenceColumns[col];
                if (!stats.IsStopped)
                {
                  value = null;

                  if (this.DataDirection == FileDataDirection.Down)
                  {
                    stats.IsStopped = (col.Address.Row + i) >= table.Rows.Count;
                    if (!stats.IsStopped) value = table.Rows[col.Address.Row + i][col.Address.Column].ToString();
                  }
                  else
                  {
                    if (stats.ColumnIndex < 0) stats.ColumnIndex = table.Columns.IndexOf(col.Address.Column);
                    if (stats.ColumnIndex < 0) throw new InvalidOperationException("Cannot find the column " + col.Address.Column);
                    stats.IsStopped = (stats.ColumnIndex + i) >= table.Columns.Count;
                    if (!stats.IsStopped) value = table.Rows[col.Address.Row][(stats.ColumnIndex + i)].ToString();
                  }

                  stats.IsStopped = stats.IsStopped || (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(col.StopValue) || value == col.StopValue);
                  if (!stats.IsStopped && col.Destination != null)
                  {
                    newRow[col.Destination.Name] = value;
                    sequenceValues++;
                  }
                }
              }
            }

            if ((i == 0 && sequenceColumns.Count < 1) || sequenceValues > 0)  result.Rows.Add(newRow);
            i++;
          }

          return result;
        }
        else
        {
          throw new FormatException("Data set does not contain table with name " + this.Table);
        }
      }
      else
      {
        throw new FormatException("Unable to process stream with the parser " + this.Parser.GetType().ToString());
      }
    }

    public string Serialize()
    {
      var serializer = new XmlSerializer(typeof(ParserMapping));
      using (var writer = new StringWriter())
      {
        serializer.Serialize(writer, this);
        writer.Flush();
        return writer.ToString();
      } 
    }

    System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
    {
      return null;
    }

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == typeof(ParserMapping).Name)
      {
        XmlSerializer deserializer;
        string value;

        while (reader.Read() && reader.MoveToContent() == XmlNodeType.Element)
        {
          switch (reader.LocalName)
          {
            case "Columns":
              if (!reader.IsEmptyElement)
              {
                while (reader.Read() && reader.MoveToContent() == XmlNodeType.Element)
                {
                  deserializer = new XmlSerializer(typeof(ColumnMapping));
                  this._columns.Add((ColumnMapping)deserializer.Deserialize(reader));
                }
              }
              break;
            case "DataDirection":
              if (!reader.IsEmptyElement) 
              {
                value = reader.ReadString();
                this.DataDirection = (FileDataDirection)Enum.Parse(typeof(FileDataDirection), value);
              }
              break;
            case "Parser":
              Type type;
              if (!reader.IsEmptyElement)
              {
                if (ParserMapping.TryGetType(reader["type"], reader["aqn"], out type))
                {
                  reader.Read();  // Consume start element
                  reader.MoveToContent();
                  deserializer = new XmlSerializer(type);
                  this.Parser = (IDataSetParser)deserializer.Deserialize(reader);
                }
                else
                {
                  throw new InvalidOperationException("Cannot interpret type name " + reader["type"]);
                }
              }
              break;
            case "Table":
              if (!reader.IsEmptyElement)
              {
                this.Table = reader.ReadString();
              }
              break;
            default:
              throw new InvalidOperationException("Element " + reader.Name + " is unexpected in " + typeof(ParserMapping).Name);
          }
        }
      }
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
      XmlSerializer serializer;

      if (_columns.Any())
      {
        writer.WriteStartElement("Columns");
        serializer = new XmlSerializer(typeof(ColumnMapping));
        foreach (var col in _columns)
        {
          serializer.Serialize(writer, col);
        }
        writer.WriteEndElement();
      }
      writer.WriteElementString("DataDirection", this.DataDirection.ToString());
      if (this.Parser != null)
      {
        writer.WriteStartElement("Parser");
        writer.WriteAttributeString("type", this.Parser.GetType().FullName);
        writer.WriteAttributeString("aqn", this.Parser.GetType().AssemblyQualifiedName);
        serializer = new XmlSerializer(this.Parser.GetType());
        serializer.Serialize(writer, this.Parser);
        writer.WriteEndElement();
      }
      writer.WriteElementString("Table", this.Table);
    }

    public static ParserMapping Deserialize(string data)
    {
      var deserializer = new XmlSerializer(typeof(ParserMapping));
      using (var reader = new StringReader(data))
      {
        var result = (ParserMapping)deserializer.Deserialize(reader);
        reader.Close();
        return result;
      }
    }
    
    internal static bool TryGetType(string typeName, string assemblyQualifiedName, out Type type)
    {
      var result = Type.GetType(typeName);
      if (result == null) result = Type.GetType(assemblyQualifiedName);
      if (result == null)
      {
        foreach (var assy in AppDomain.CurrentDomain.GetAssemblies())
        {
          result = assy.GetType(typeName, false, false);
          if (result != null)
          {
            type = result;
            return true;
          }
        }
      }
      type = result;
      return (result != null);
    } 

    private class ColumnStats
    {
      public bool IsStopped { get; set; }
      public int ColumnIndex { get; set; }

      public ColumnStats()
      {
        this.IsStopped = false;
        this.ColumnIndex = -1;
      }
    }
  }
}
