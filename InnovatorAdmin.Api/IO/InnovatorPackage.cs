using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace InnovatorAdmin
{
  public abstract class InnovatorPackage : IDisposable
  {
    private List<string> _paths = new List<string>();

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

          foreach (var path in paths)
          {
            if (path.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
            {
              doc = ReadReport(path);
            }
            else
            {
              doc = new XmlDocument(manifest.NameTable);
              doc.Load(GetExistingStream(path));
            }

            foreach (var item in doc.DocumentElement.Elements("Item"))
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
      string newPath;
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
              if (line.Reference.Type == "Report" && line.Type != InstallType.Script)
              {
                newPath = line.FilePath(existingPaths, ".xslt");
                WriteReport(line, newPath);
              }
              else
              {
                newPath = line.FilePath(existingPaths);

                using (var stream = GetNewStream(newPath))
                {
                  using (var writer = GetWriter(stream))
                  {
                    writer.WriteStartElement("AML");
                    line.Script.WriteTo(writer);
                    writer.WriteEndElement();
                  }
                }
              }

              existingPaths.Add(newPath);
              manifestWriter.WriteStartElement("Path");
              manifestWriter.WriteAttributeString("path", newPath);
              manifestWriter.WriteEndElement();
            }
          }
          manifestWriter.WriteEndElement();
        }
      }
    }

    #region "Report XSLT Handling"

    private const string _reportDataEnd = "REPORT-DATA-END-->";
    private const string _reportDataStart = "<!--REPORT-DATA-START";
    private const string _reportEnd = "METADATA-END-->";
    private const string _reportStart = "<!--METADATA-START";

    public static XmlDocument ReadReport(string path, Func<string, Stream> fileGetter)
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
          dataFile = RemoveComments(dataFile);

          writer.WriteLine(xslt);
        }
      }

      using (var writer = new StreamWriter(GetNewStream(path + ".xml")))
      {
        writer.Write(dataFile);
      }
    }

    private string RemoveComments(string xml)
    {
      using (var reader = new StringReader(xml))
      using (var xReader = XmlReader.Create(reader, new XmlReaderSettings()
      {
        IgnoreComments = true
      }))
      {
        var doc = new XmlDocument();
        doc.Load(reader);
        return doc.OuterXml;
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
