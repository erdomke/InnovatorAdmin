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

namespace Aras.Tools.InnovatorAdmin
{
  public abstract class FileSysExtractor : IDataExtractor, IXmlSerializable
  {
    private bool _atEnd;
    private IEnumerator<string> _enum;
    private int _numProcessed = 0;
    private HashSet<string> _oldPaths = new HashSet<string>();
    private List<string> _newPaths = new List<string>();
    private int _totalCount = -1;

    [DisplayName("Include Checksum"), Description("Include information about the file checksum in the import"), DefaultValue(false)]
    public bool IncludeChecksum { get; set; }
    [DisplayName("Include Size"), Description("Include information about the file size in the import"), DefaultValue(false)]
    public bool IncludeSize { get; set; }

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
      string[] checksums = null;

      if (this.IncludeChecksum)
      {
        checksums = new string[paths.Count];
        Parallel.For(0, checksums.Length, j =>
        {
          checksums[j] = Utils.GetFileChecksum(paths[j]);
        });
      }

      long size = 0;
      for (i = 0; i < paths.Count; i++)
      {
        if (this.IncludeSize) size = new FileInfo(paths[i]).Length;

        foreach (var writer in writers)
        {
          writer.Row();
          writer.Cell("Path", paths[i]);
          if (this.IncludeChecksum) writer.Cell("Checksum", checksums[i]);
          if (this.IncludeSize) writer.Cell("Size", size);
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
      this.IncludeChecksum = (reader.ReadElementString("IncludeChecksum") == "1");
      this.IncludeSize = (reader.ReadElementString("IncludeSize") == "1");
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
      writer.WriteElementString("IncludeChecksum", IncludeChecksum ? "1" : "0");
      writer.WriteElementString("IncludeSize", IncludeSize ? "1" : "0");
      writer.WriteStartElement("ProcessedPaths");
      foreach (var path in _newPaths.Concat(_oldPaths))
      {
        writer.WriteElementString("ProcessedPath", path);
      }
      writer.WriteEndElement();
    }
  }
}
