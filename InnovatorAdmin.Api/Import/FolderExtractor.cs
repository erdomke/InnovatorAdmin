using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Aras.Tools.InnovatorAdmin
{
  public class FolderExtractor : FileSysExtractor
  {
    private IEnumerable<string> _paths;

    internal FolderExtractor() { _paths = Enumerable.Empty<string>(); }
    public FolderExtractor(IEnumerable<string> paths)
    {
      _paths = paths;
    }

    protected override IEnumerable<string> GetPaths()
    {
      return _paths.Where(p => Directory.Exists(p)).SelectMany(p => Directory.EnumerateFiles(p, "*.*", SearchOption.AllDirectories))
                   .Concat(_paths.Where(p => File.Exists(p)));
    }

    protected override void ReadProperties(System.Xml.XmlReader reader)
    {
      base.ReadProperties(reader);
      reader.MoveToContent();
      var isEmptyElement = reader.IsEmptyElement;
      reader.ReadStartElement("Paths");
      if (!isEmptyElement)
      {
        reader.MoveToContent();
        var paths = new List<string>();
        while (reader.LocalName == "Path")
        {
          paths.Add(reader.ReadElementString("Path"));
          reader.MoveToContent();
        }
        reader.ReadEndElement();
      }
    }
    public override void WriteXml(System.Xml.XmlWriter writer)
    {
      base.WriteXml(writer);
      writer.WriteStartElement("Paths");
      foreach (var path in _paths)
      {
        writer.WriteElementString("Path", path);
      }
      writer.WriteEndElement();
    }
  }
}
