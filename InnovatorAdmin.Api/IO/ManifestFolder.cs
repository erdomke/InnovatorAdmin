﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace InnovatorAdmin
{
  public class ManifestFolder
  {
    private string _path;
    private string _baseFolderPath;
    private XmlWriterSettings _settings;

    public ManifestFolder(string path)
    {
      _path = path;

      _settings = new XmlWriterSettings
      {
        OmitXmlDeclaration = true,
        Indent = true,
        IndentChars = "  "
      };
    }

    public XmlDocument Read(out string title)
    {
      var result = new XmlDocument();
      var pkgDoc = new XmlDocument(result.NameTable);
      pkgDoc.Load(_path);
      string folderPath;
      var root = result.AppendChild(result.CreateElement("AML"));
      
      title = null;
      foreach (var pkg in pkgDoc.DocumentElement.Elements("package"))
      {
        title = pkg.Attribute("name", "");
        folderPath = pkg.Attribute("path");
        if (folderPath == ".\\") folderPath = Utils.CleanFileName(title).Replace('.', '\\');
        foreach (var file in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(_path), folderPath), "*.xml", SearchOption.AllDirectories))
        {
          using (var writer = root.CreateNavigator().AppendChild())
          using (var reader = XmlReader.Create(file))
          {
            writer.WriteNode(reader, false);
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
            _baseFolderPath = Utils.CleanFileName(script.Title).Substring(28).Replace('.', '\\') + "\\Import";
            xml.WriteAttributeString("path", _baseFolderPath);
          }
          else
          {
            _baseFolderPath = Utils.CleanFileName(script.Title).Replace('.', '\\');
            xml.WriteAttributeString("path", ".\\");
          }
        }
        else
        {
          _baseFolderPath = Utils.CleanFileName(script.Title) + "\\Import";
          xml.WriteAttributeString("path", _baseFolderPath);
        }

        xml.WriteEndElement();
        xml.WriteEndElement();
      }

      _baseFolderPath = Path.Combine(Path.GetDirectoryName(_path), _baseFolderPath);

      XmlWriter writer;
      var existingPaths = new HashSet<string>();
      string newPath;
      foreach (var line in script.Lines.Where(l => l.Type == InstallType.Create || l.Type == InstallType.Script))
      {
        newPath = line.FilePath(existingPaths);
        writer = GetWriter(newPath);
        try
        {
          writer.WriteStartElement("AML");
          line.Script.WriteTo(writer);
          writer.WriteEndElement();
          writer.Flush();
        }
        finally
        {
          writer.Close();
        }

        existingPaths.Add(newPath);
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
      private bool _convertAdd = false;
      private bool _blockWhere = false;

      public CustXmlWriter(Stream stream) : base(stream)
      {
        this.AttributeProcessor = (prefix, localName, ns, writer) =>
        {
          if (localName == XmlFlags.Attr_ConfigId)
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
