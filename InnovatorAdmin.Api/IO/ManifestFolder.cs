using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public class ManifestFolder
  {
    private string _path;
    private string _baseFolderPath;
    private XmlWriterSettings _settings;

    public ManifestFolder(string path)
    {
      _path = path;

      _settings = new XmlWriterSettings();
      _settings.OmitXmlDeclaration = true;
      _settings.Indent = true;
      _settings.IndentChars = "  ";
    }

    public XmlDocument Read(out string title)
    {
      var result = new XmlDocument();
      var pkgDoc = new XmlDocument();
      pkgDoc.Load(_path);
      string folderPath;
      result.AppendChild(result.CreateElement("AML"));
      XmlElement child;

      var scripts = new List<InstallItem>();
      title = null;
      foreach (var pkg in pkgDoc.DocumentElement.Elements("package"))
      {
        title = pkg.Attribute("name", "");
        folderPath = pkg.Attribute("path");
        if (folderPath == ".\\") folderPath = title.Replace('.', '\\');
        foreach (var file in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(_path), folderPath), "*.xml", SearchOption.AllDirectories))
        {
          child = result.CreateElement("AML");
          child.InnerXml = System.IO.File.ReadAllText(file);
          foreach (var item in child.Element("AML").Elements("Item"))
          {
            result.AppendChild(item);
          }
        }
      }

      return result;
    }
    public void Write(InstallScript script)
    {
      using (var xml = XmlTextWriter.Create(_path, _settings))
      {
        xml.WriteStartElement("imports");
        xml.WriteStartElement("package");
        xml.WriteAttributeString("name", script.Title);

        if (script.Title.StartsWith("com.aras.innovator"))
        {
          if (script.Title.StartsWith("com.aras.innovator.solution."))
          {
            _baseFolderPath = script.Title.Substring(28).Replace('.', '\\') + "\\Import";
            xml.WriteAttributeString("path", _baseFolderPath);
          }
          else
          {
            _baseFolderPath = script.Title.Replace('.', '\\');
            xml.WriteAttributeString("path", ".\\");
          }
        }
        else
        {
          _baseFolderPath = script.Title + "\\Import";
          xml.WriteAttributeString("path", _baseFolderPath);
        }

        xml.WriteEndElement();
        xml.WriteEndElement();
      }

      _baseFolderPath = Path.Combine(Path.GetDirectoryName(_path), _baseFolderPath);

      XmlWriter writer;
      InstallItem first;
      foreach (var group in script.GroupLines(i => i.Type != InstallType.DependencyCheck))
      {
        first = group.First();
        writer = GetWriter(first.Reference.Type + "\\" + (first.Reference.KeyedName ?? first.Reference.Unique) + ".xml");
        writer.WriteStartElement("AML");
        foreach (var line in group)
        {
          line.Script.WriteTo(writer);
        }
        writer.WriteEndElement();
      }
    }



    private XmlWriter GetWriter(string path)
    {
      var filePath = Path.Combine(_baseFolderPath, path);
      var dirPath = Path.GetDirectoryName(filePath);
      if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
      var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
      return new CustXmlWriter(stream);
    }

    private class CustXmlWriter : ConfigurableXmlWriter
    {
      private XmlWriter _base;
      private bool _blockAttr = false;
      private bool _convertAdd = false;
      private bool _blockWhere = false;

      public CustXmlWriter(Stream stream) : base(stream) 
      {
        this.AttributeProcessor = (prefix, localName, ns, writer) =>
        {
          if (localName == "_config_id")
          {
            writer.WriteStartAttribute(prefix, "id", ns);
            _blockWhere = true;
            return ProcessState.RenderRemaining;
          }
          else if (localName.StartsWith("_") || _blockWhere && localName == "where")
          {
            _blockWhere = false;
            return ProcessState.DontRender;
          }
          else
          {
            _convertAdd = localName == "action";
            return ProcessState.RenderAll;
          }
        };
      }
      
      public override void WriteEndAttribute()
      {
        base.WriteEndAttribute();
        _convertAdd = false;
      }
      public override void WriteString(string text)
      {
        if (_convertAdd && text == "merge")
        {
          _base.WriteString("add");
        }
        else
        {
          base.WriteString(text);
        }
      }
    }
  }
}
