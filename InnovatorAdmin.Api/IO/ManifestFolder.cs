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

      script.WriteLines(GetWriter, i => i.Type != InstallType.DependencyCheck);
    }



    private XmlWriter GetWriter(string path)
    {
      var filePath = Path.Combine(_baseFolderPath, path);
      var dirPath = Path.GetDirectoryName(filePath);
      if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
      var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
      return new CustXmlWriter(stream);
    }

    private class CustXmlWriter : XmlWriter
    {
      private XmlWriter _base;
      private bool _blockAttr = false;
      private bool _convertAdd = false;
      private bool _blockWhere = false;

      public CustXmlWriter(Stream stream)
      {
        var settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = true;
        settings.Indent = true;
        settings.IndentChars = "  ";
        _base = XmlWriter.Create(stream, settings);
      }

      public override void Close()
      {
        _base.Close();
      }

      public override void Flush()
      {
        _base.Flush();
      }

      public override string LookupPrefix(string ns)
      {
        return _base.LookupPrefix(ns);
      }

      public override void WriteBase64(byte[] buffer, int index, int count)
      {
        _base.WriteBase64(buffer, index, count);
      }

      public override void WriteCData(string text)
      {
        _base.WriteCData(text);
      }

      public override void WriteCharEntity(char ch)
      {
        _base.WriteCharEntity(ch);
      }

      public override void WriteChars(char[] buffer, int index, int count)
      {
        _base.WriteChars(buffer, index, count);
      }

      public override void WriteComment(string text)
      {
        _base.WriteComment(text);
      }

      public override void WriteDocType(string name, string pubid, string sysid, string subset)
      {
        _base.WriteDocType(name, pubid, sysid, subset);
      }

      public override void WriteEndAttribute()
      {
        if (!_blockAttr) _base.WriteEndAttribute();
        _blockAttr = false;
        _convertAdd = false;
      }

      public override void WriteEndDocument()
      {
        _base.WriteEndDocument();
      }

      public override void WriteEndElement()
      {
        _base.WriteEndElement();
      }

      public override void WriteEntityRef(string name)
      {
        _base.WriteEntityRef(name);
      }

      public override void WriteFullEndElement()
      {
        _base.WriteFullEndElement();
      }

      public override void WriteProcessingInstruction(string name, string text)
      {
        _base.WriteProcessingInstruction(name, text);
      }

      public override void WriteRaw(string data)
      {
        _base.WriteRaw(data);
      }

      public override void WriteRaw(char[] buffer, int index, int count)
      {
        _base.WriteRaw(buffer, index, count);
      }

      public override void WriteStartAttribute(string prefix, string localName, string ns)
      {
        if (localName == "_config_id")
        {
          _base.WriteStartAttribute(prefix, "id", ns);
          _blockWhere = true;
        }
        else if (localName.StartsWith("_") || _blockWhere && localName == "where")
        {
          _blockAttr = true;
          _blockWhere = false;
        }
        else
        {
          _base.WriteStartAttribute(prefix, localName, ns);
          _convertAdd = localName == "action";
        }
      }

      public override void WriteStartDocument(bool standalone)
      {
        _base.WriteStartDocument(standalone);
      }

      public override void WriteStartDocument()
      {
        _base.WriteStartDocument();
      }

      public override void WriteStartElement(string prefix, string localName, string ns)
      {
        _base.WriteStartElement(prefix, localName, ns);
      }

      public override WriteState WriteState
      {
        get { return _base.WriteState; }
      }

      public override void WriteString(string text)
      {
        if (_convertAdd && text == "merge")
        {
          _base.WriteString("add");
        }
        else if (!_blockAttr)
        {
          _base.WriteString(text);
        }
      }

      public override void WriteSurrogateCharEntity(char lowChar, char highChar)
      {
        _base.WriteSurrogateCharEntity(lowChar, highChar);
      }

      public override void WriteWhitespace(string ws)
      {
        _base.WriteWhitespace(ws);
      }
    }
  }
}
