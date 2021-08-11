﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin
{
  public abstract class InnovatorPackage : IDisposable
  {
    private List<string> _paths = new List<string>();
    protected bool _parallel = false;

    public virtual InstallScript Read()
    {
      var result = new InstallScript();

      XmlDocument doc;
      var scripts = new List<InstallItem>();
      var manifest = new XmlDocument();
      string currPath;
      IEnumerable<string> paths;
      manifest.Load(GetExistingStream(null));

      if (manifest.DocumentElement.HasAttribute("created"))
        result.Created = DateTime.Parse(manifest.DocumentElement.GetAttribute("created"));
      result.Creator = manifest.DocumentElement.GetAttribute("creator");
      result.Description = manifest.DocumentElement.GetAttribute("description");
      if (manifest.DocumentElement.HasAttribute("modified"))
        result.Modified = DateTime.Parse(manifest.DocumentElement.GetAttribute("modified"));
      result.Version = manifest.DocumentElement.GetAttribute("revision");
      result.Title = manifest.DocumentElement.GetAttribute("title");
      if (manifest.DocumentElement.HasAttribute("website"))
        result.Website = new Uri(manifest.DocumentElement.GetAttribute("website"));

      foreach (var child in manifest.DocumentElement.ChildNodes.OfType<XmlElement>())
      {
        if (child.LocalName == "Item")
        {
          scripts.Add(InstallItem.FromScript(child));
        }
        else
        {
          currPath = child.GetAttribute("path");
          paths = string.IsNullOrEmpty(currPath)
            ? Enumerable.Empty<string>()
            : (currPath == "*"
              ? GetPaths()
              : Enumerable.Repeat(currPath, 1));

          if (currPath == "*") result.DependencySorted = false;
          var reportXmlPaths = new HashSet<string>(paths
            .Where(p => p.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
            .Select(p => p + ".xml"), StringComparer.OrdinalIgnoreCase);

          foreach (var path in paths
            .Where(p => !reportXmlPaths.Contains(p)))
          {
            if (path.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
            {
              doc = ReadReport(path);
            }
            else
            {
              try
              {
                doc = new XmlDocument(manifest.NameTable);
                var stream = GetExistingStream(path);
                if (stream == null)
                  throw new FileNotFoundException("A referenced file was not found in the package", path);
                using (stream)
                using (var reader = new StreamReader(stream))
                {
                  var text = reader.ReadToEnd();
                  doc.LoadXml(text);
                }
              }
              catch (Exception ex) when (ex is XmlException || ex is IOException)
              {
                throw new InvalidOperationException($"Error reading the file {path}: {ex.Message}", ex);
              }
            }

            var items = doc.DocumentElement.LocalName == "Item"
              ? new[] { doc.DocumentElement }
              : doc.DocumentElement.Elements("Item");
            foreach (var item in items)
            {
              scripts.Add(InstallItem.FromScript(item, path));
            }
          }
        }
      }

      result.Lines = scripts;
      result.Lines.CleanKeyedNames();

      return result;
    }

    public virtual void Write(InstallScript script)
    {
      var existingPaths = new HashSet<string>();

      // Record the import order
      var settings = new XmlWriterSettings()
      {
        OmitXmlDeclaration = true,
        Indent = true,
        IndentChars = "  "
      };
      using (var manifestStream = GetNewStream(null))
      {
        using (var manifestWriter = XmlWriter.Create(manifestStream, settings))
        {
          manifestWriter.WriteStartElement("Import");

          if (script.Created.HasValue) manifestWriter.WriteAttributeString("created", script.Created.Value.ToString("s"));
          manifestWriter.WriteAttributeString("creator", script.Creator);
          manifestWriter.WriteAttributeString("description", script.Description);
          if (script.Modified.HasValue) manifestWriter.WriteAttributeString("modified", script.Modified.Value.ToString("s"));
          manifestWriter.WriteAttributeString("revision", script.Version);
          manifestWriter.WriteAttributeString("title", script.Title);
          if (script.Website != null)
            manifestWriter.WriteAttributeString("website", script.Website.ToString());

          // Achieve consistent file names regardless of the ordering.
          foreach (var line in script.Lines)
            line.Path = GetPath(line, existingPaths);
          existingPaths.UnionWith(script.Lines.Select(l => l.Path));

          foreach (var group in script.Lines.GroupBy(l => l.Path).Where(g => g.Skip(1).Any()))
          {
            foreach (var line in group.OrderBy(l => l.Reference.Unique).Skip(1))
            {
              line.Path = GetPath(line, existingPaths);
            }
          }


          foreach (var line in script.Lines)
          {
            if (line.Type == InstallType.Warning)
            {
              // Do nothing
            }
            else if (line.Type == InstallType.DependencyCheck)
            {
              line.Script.WriteTo(manifestWriter);
            }
            else
            {
              manifestWriter.WriteStartElement("Path");
              manifestWriter.WriteAttributeString("path", line.Path);
              manifestWriter.WriteEndElement();
            }
          }
          manifestWriter.WriteEndElement();

          Action<InstallItem> writeFunc = line =>
          {
            if (line.Reference.Type == "Report" && line.Type != InstallType.Script)
            {
              WriteReport(line, line.Path);
            }
            else
            {
              using (var stream = GetNewStream(line.Path))
              {
                using (var writer = GetWriter(stream))
                {
                  writer.WriteStartElement("AML");
                  line.Script.WriteTo(writer);
                  writer.WriteEndElement();
                }
              }
            }
          };

          var scriptLines = script.Lines.Where(l => l.Type != InstallType.Warning && l.Type != InstallType.DependencyCheck);
          if (_parallel)
          {
            Parallel.ForEach(scriptLines, writeFunc);
          }
          else
          {
            foreach (var line in scriptLines)
              writeFunc(line);
          }
        }
      }
    }

    private string GetPath(InstallItem line, HashSet<string> existingPaths)
    {
      if (line.Reference.Type == "Report" && line.Type != InstallType.Script)
        return line.FilePath(existingPaths, ".xslt");
      else
        return line.FilePath(existingPaths);
    }

    #region "Report XSLT Handling"

    private const string _reportDataEnd = "REPORT-DATA-END-->";
    private const string _reportDataStart = "<!--REPORT-DATA-START";
    private const string _reportEnd = "METADATA-END-->";
    private const string _reportStart = "<!--METADATA-START";

    public static XmlDocument ReadReport(string path, Func<string, Stream> fileGetter)
    {
      try
      {
        string file;
        using (var reader = new StreamReader(fileGetter(path)))
        {
          file = reader.ReadToEnd();
        }
        string dataFile = null;
        var dataFileStream = fileGetter(path + ".xml");
        if (dataFileStream != null)
        {
          using (var reader = new StreamReader(dataFileStream))
          {
            dataFile = reader.ReadToEnd();
          }
        }

        var metaStart = file.IndexOf(_reportStart);
        var metaEnd = file.IndexOf(_reportEnd);
        var result = new XmlDocument();

        if (metaStart >= 0 && metaEnd >= 0)
        {
          result.LoadXml(file.Substring(metaStart + _reportStart.Length, metaEnd - metaStart - _reportStart.Length).Trim());
          var reportItem = result.SelectSingleNode("//Item[@type='Report']") as XmlElement;
          var xsltElem = result.CreateElement("xsl_stylesheet");
          var xslt = file.Substring(metaEnd + _reportEnd.Length).Trim();
          if (!string.IsNullOrEmpty(dataFile))
          {
            xslt += Environment.NewLine + Environment.NewLine + _reportDataStart + Environment.NewLine + dataFile + Environment.NewLine + _reportDataEnd;
          }
          xsltElem.InnerText = xslt;
          reportItem.AppendChild(xsltElem);
        }
        else if (file.StartsWith("<AML>"))
        {
          result.LoadXml(file);
        }
        else
        {
          throw new ArgumentException("Invalid xslt file");
        }

        return result;
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException($"Error reading the report the report at {path}: {ex.Message}", ex);
      }
    }

    private XmlDocument ReadReport(string path)
    {
      return ReadReport(path, GetExistingStream);
    }
    private void WriteReport(InstallItem line, string path)
    {
      var dataFile = "<Result><Item></Item></Result>";

      using (var writer = new StreamWriter(GetNewStream(path)))
      {
        var aml = new StringBuilder();

        using (var amlWriter = new System.IO.StringWriter(aml))
        {
          using (var xml = new ConfigurableXmlWriter(amlWriter))
          {
            xml.ElementProcessor = (prefix, localName, ns, w) =>
            {
              if (localName == "xsl_stylesheet") return ProcessState.DontRender;
              return ProcessState.RenderAll;
            };

            xml.WriteStartElement("AML");
            line.Script.WriteTo(xml);
            xml.WriteEndElement();
          }
        }

        writer.WriteLine(_reportStart);
        writer.WriteLine(aml);
        writer.WriteLine(_reportEnd);

        var xsltElem = line.Script.SelectSingleNode(".//xsl_stylesheet") as XmlElement;
        if (xsltElem != null)
        {
          var xslt = xsltElem.InnerText;
          var dataStart = xslt.IndexOf(_reportDataStart);
          var dataEnd = xslt.IndexOf(_reportDataEnd);
          if (dataStart >= 0 && dataEnd >= 0)
          {
            dataFile = xslt.Substring(dataStart + _reportDataStart.Length, dataEnd - dataStart - _reportDataStart.Length).Trim();
            xslt = xslt.Substring(0, dataStart).Trim() + Environment.NewLine + xslt.Substring(dataEnd + _reportDataEnd.Length).Trim();
          }
          else
          {
            dataFile = xsltElem.Parent().Element("report_query", "<Result><Item></Item></Result>");
          }
          dataFile = XmlUtils.RemoveComments(dataFile);

          writer.WriteLine(xslt);
        }
      }

      using (var writer = new StreamWriter(GetNewStream(path + ".xml")))
      {
        writer.Write(dataFile);
      }
    }

    #endregion

    protected abstract Stream GetExistingStream(string path);
    protected abstract Stream GetNewStream(string path);
    protected abstract bool PathExists(string path);
    protected abstract IEnumerable<string> GetPaths();

    private XmlWriter GetWriter(Stream stream)
    {
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";
      settings.CloseOutput = true;

      return XmlTextWriter.Create(stream, settings);
    }

    private const int ZIP_LEAD_BYTES = 0x04034b50;
    public static InnovatorPackage Load(string path)
    {
      var isFile = false;
      using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
      {
        var bytes = new byte[4];
        if (stream.Read(bytes, 0, 4) == 4 && BitConverter.ToInt32(bytes, 0) == ZIP_LEAD_BYTES)
          isFile = true;
      }

      return isFile ? (InnovatorPackage)new InnovatorPackageFile(path) : new InnovatorPackageFolder(path);
    }

    public virtual void Dispose()
    {
      // Do nothing by default
    }
  }
}
