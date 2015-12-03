using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InnovatorAdmin
{
  public abstract class FileSysExtractor : IDataExtractor, IXmlSerializable
  {
    private bool _atEnd;
    private IEnumerator<string> _enum;
    private int _numProcessed = 0;
    private HashSet<string> _oldPaths = new HashSet<string>();
    private List<string> _newPaths = new List<string>();
    private int _totalCount = -1;

    public bool AtEnd
    {
      get { return _atEnd; }
    }
    public int NumProcessed
    {
      get { return _numProcessed; }
    }

    public int GetTotalCount()
    {
      if (_totalCount >= 0) return _totalCount;
      return GetPaths().Count();
    }

    public void Reset()
    {
      if (_enum != null) _enum.Dispose();
      _enum = GetPaths().GetEnumerator();
      _atEnd = false;
      _numProcessed = 0;
      _newPaths.Clear();
    }

    public void Write(int count, params IDataWriter[] writers)
    {
      if (_atEnd) return;
      if (_enum == null) Reset();
      
      int i = 0;
      var paths = new List<string>(count);
      while (i < count && AdvanceEnum())
      {
        paths.Add(_enum.Current);
        i++;
      }

      for (i = 0; i < paths.Count; i++)
      {
        foreach (var writer in writers)
        {
          writer.Row();
          writer.Cell("Path", paths[i]);
          writer.RowEnd();
        }
      }
    }

    private bool AdvanceEnum()
    {
      _atEnd = !_enum.MoveNext();

      while (!_atEnd && _oldPaths.Contains(_enum.Current))
      {
        _atEnd = !_enum.MoveNext();
      }

      if (_atEnd)
      {
        return false;
      }
      else
      {
        _newPaths.Add(_enum.Current);
        _numProcessed++;
        return true;
      }
    }
    
    protected abstract IEnumerable<string> GetPaths();

    public System.Xml.Schema.XmlSchema GetSchema()
    {
      return null;
    }

    public void ReadXml(System.Xml.XmlReader reader)
    {
      reader.MoveToContent();
      var isEmptyElement = reader.IsEmptyElement;
      reader.ReadStartElement();
      if (!isEmptyElement) // (1)
      {
        ReadProperties(reader);
        reader.ReadEndElement();
      }
    }

    protected virtual void ReadProperties(System.Xml.XmlReader reader)
    {
      reader.MoveToContent();
      var isEmptyElement = reader.IsEmptyElement;
      reader.ReadStartElement("ProcessedPaths");
      if (!isEmptyElement)
      {
        reader.MoveToContent();
        while (reader.LocalName == "ProcessedPath")
        {
          _oldPaths.Add(reader.ReadElementString("ProcessedPath"));
          reader.MoveToContent();
        }
        reader.ReadEndElement();
      }
    }

    public virtual void WriteXml(System.Xml.XmlWriter writer)
    {
      writer.WriteStartElement("ProcessedPaths");
      foreach (var path in _newPaths.Concat(_oldPaths).ToList())
      {
        writer.WriteElementString("ProcessedPath", path);
      }
      writer.WriteEndElement();
    }
  }
}
