using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin
{
  public static class Package
  {
    private const int ZIP_LEAD_BYTES = 0x04034b50;

    public static IEnumerable<IPackage> Create(params string[] paths)
    {
      var repos = new Dictionary<string, GitRepo>(StringComparer.OrdinalIgnoreCase);
      foreach (var path in paths)
      {
        if (path.StartsWith("git://", StringComparison.OrdinalIgnoreCase))
        {
          var query = new QueryString("file://" + path.Substring(6));

          var filePath = query.Uri.LocalPath;
          if (!repos.TryGetValue(filePath, out var repo))
          {
            repo = new GitRepo(filePath);
            repos[filePath] = repo;
          }

          var options = new GitDirectorySearch()
          {
            Sha = query["commit"].ToString(),
            Path = query["path"].ToString()
          };
          foreach (var branch in query["branch"])
            options.BranchNames.Add(branch);
          if (string.Equals(options.Sha, "tip", StringComparison.OrdinalIgnoreCase))
            options.Sha = null;

          yield return repo.GetDirectory(options);
        }
        else if (string.Equals(Path.GetExtension(path), ".innpkg", StringComparison.OrdinalIgnoreCase))
        {
          var isFile = false;
          using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
          {
            var bytes = new byte[4];
            if (stream.Read(bytes, 0, 4) == 4 && BitConverter.ToInt32(bytes, 0) == ZIP_LEAD_BYTES)
              isFile = true;
          }

          yield return isFile
            ? new ZipPackage(path)
            : new DirectoryPackage(path);
        }
        else if (string.Equals(Path.GetExtension(path), ".mf", StringComparison.OrdinalIgnoreCase))
        {
          yield return new DirectoryPackage(Path.GetDirectoryName(path));
        }
        else
        {
          yield return new DirectoryPackage(path);
        }
      }
    }

    public static InstallScript Read(this IPackage package)
    {
      var logger = new ExceptionLogger();
      if (!TryRead(package, logger, out var result))
        logger.AssertNoError();
      return result;
    }

    private class ExceptionLogger : ILogger
    {
      private List<Exception> _exceptions = new List<Exception>();

      public IDisposable BeginScope<TState>(TState state)
      {
        throw new NotSupportedException();
      }

      public bool IsEnabled(LogLevel logLevel)
      {
        return true;
      }

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
      {
        if (logLevel == LogLevel.Error)
          _exceptions.Add(new InvalidOperationException(formatter(state, exception), exception));
      }

      public void AssertNoError()
      {
        if (_exceptions.Count == 1)
        {
          throw _exceptions[0];
        }
        else if (_exceptions.Count > 1)
        {
          throw new AggregateException(_exceptions);
        }
      }
    }

    public static bool TryRead(this IPackage package, ILogger logger, out InstallScript installScript)
    {
      using (SharedUtils.StartActivity("Package.TryRead", "Read an Innovator Package from disk"))
      {
        var manifestFile = package.Manifest(false);
        if (manifestFile.Path.EndsWith(".mf", StringComparison.OrdinalIgnoreCase))
          return TryReadLegacyManifest(manifestFile, package, logger, out installScript);
        else
          return TryReadPackage(manifestFile, package, logger, out installScript);
      }
    }

    private static bool TryReadLegacyManifest(IPackageFile manifestFile, IPackage package, ILogger logger, out InstallScript installScript)
    {
      installScript = new InstallScript();
      var scripts = new List<InstallItem>();
      var manifest = new XmlDocument();
      using (var manifestStream = manifestFile.Open())
        manifest.Load(manifestStream);

      foreach (var pkg in manifest.DocumentElement.Elements("package"))
      {
        if (string.IsNullOrEmpty(installScript.Title))
          installScript.Title = pkg.Attribute("name", "");
        var folderPath = pkg.Attribute("path");
        if (folderPath == ".\\")
          folderPath = Utils.CleanFileName(pkg.Attribute("name", "")).Replace('.', '\\');
        foreach (var file in package.Files()
          .Where(f => f.Path.StartsWith(folderPath + "/", StringComparison.OrdinalIgnoreCase)
            && f.Path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)))
        {
          try
          {
            var doc = new XmlDocument(manifest.NameTable);
            using (var stream = file.Open())
              doc.Load(stream);

            var items = doc.DocumentElement.LocalName == "Item"
              ? new[] { doc.DocumentElement }
              : doc.DocumentElement.Elements("Item");
            foreach (var item in items)
            {
              scripts.Add(InstallItem.FromScript(item, file.Path));
            }
          }
          catch (Exception ex)
          {
            ex.Data["path"] = file.Path;
            throw;
          }
        }
      }

      installScript.Lines = scripts;
      return CleanKeyedNames(installScript.Lines, logger);
    }
    
    private static bool TryReadPackage(IPackageFile manifestFile, IPackage package, ILogger logger, out InstallScript installScript)
    {
      installScript = new InstallScript();
      var result = true;
      var scripts = new List<InstallItem>();
      var manifest = new XmlDocument();
      using (var manifestStream = manifestFile.Open())
        manifest.Load(manifestStream);

      if (manifest.DocumentElement.HasAttribute("created"))
        installScript.Created = DateTime.Parse(manifest.DocumentElement.GetAttribute("created"));
      installScript.Creator = manifest.DocumentElement.GetAttribute("creator");
      installScript.Description = manifest.DocumentElement.GetAttribute("description");
      if (manifest.DocumentElement.HasAttribute("modified"))
        installScript.Modified = DateTime.Parse(manifest.DocumentElement.GetAttribute("modified"));
      installScript.Version = manifest.DocumentElement.GetAttribute("revision");
      installScript.Title = manifest.DocumentElement.GetAttribute("title");
      if (manifest.DocumentElement.HasAttribute("website"))
        installScript.Website = new Uri(manifest.DocumentElement.GetAttribute("website"));

      foreach (var child in manifest.DocumentElement.ChildNodes.OfType<XmlElement>())
      {
        if (child.LocalName == "Item")
        {
          scripts.Add(InstallItem.FromScript(child));
        }
        else
        {
          var currPath = child.GetAttribute("path");
          var files = Enumerable.Empty<IPackageFile>();
          if (currPath == "*")
          {
            files = package.Files();
            installScript.DependencySorted = false;
          }
          else if (!string.IsNullOrEmpty(currPath))
          {
            if (package.TryAccessFile(currPath, false, out var file))
            {
              files = new[] { file };
            }
            else
            {
              result = false;
              logger.LogError("The file {Path} is referenced in the manifest, but not found in the package.", currPath);
              continue;
            }
          }

          var reportXmlPaths = new HashSet<string>(files
            .Where(f => f.Path.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
            .Select(f => f.Path + ".xml"), StringComparer.OrdinalIgnoreCase);

          foreach (var file in files
            .Where(f => !reportXmlPaths.Contains(f.Path)))
          {
            try
            {
              var doc = new XmlDocument(manifest.NameTable);
              using (var writer = doc.CreateNavigator().AppendChild())
                file.WriteAml(package, writer);
              var items = doc.DocumentElement.LocalName == "Item"
                ? new[] { doc.DocumentElement }
                : doc.DocumentElement.Elements("Item");
              foreach (var item in items)
              {
                scripts.Add(InstallItem.FromScript(item, file.Path));
              }
            }
            catch (XmlException ex)
            {
              result = false;
              logger.LogError(ex, "The AML script at {Path} is malformed", file.Path);
            }
            catch (Exception ex)
            {
              ex.Data["path"] = file.Path;
              throw;
            }
          }
        }
      }

      installScript.Lines = scripts;
      result = CleanKeyedNames(installScript.Lines, logger) || result;
      return result;
    }

    private static bool CleanKeyedNames(IEnumerable<InstallItem> lines, ILogger logger)
    {
      var groups = lines.Where(l => l.Type == InstallType.Create)
        .GroupBy(l => $"{l.Reference.Unique}|{l.Reference.Type}");
      var result = true;
      foreach (var duplicate in groups.Where(g => g.Skip(1).Any()))
      {
        result = false;
        logger.LogError("The package has duplicate entries for creating {Type} {Name} with ID {ID} at {Paths}", duplicate.First().Reference.Type, duplicate.First().Reference.KeyedName, duplicate.First().Reference.Unique, duplicate.Select(i => i.Path));
      }

      var existing = groups
        .ToDictionary(g => g.Key, g => g.First());
      InstallItem item;
      foreach (var line in lines.Where(l => l.Type == InstallType.Script))
      {
        if (existing.TryGetValue(line.InstalledId, out item))
        {
          line.Reference.KeyedName = InstallItem.RenderAttributes(line.Script, item.Reference.KeyedName);
        }
      }

      return result;
    }

    public static void Write(this IPackage package, InstallScript script)
    {
      if (package is ZipPackage zip)
      {
        zip.Properties.Created = script.Created;
        zip.Properties.Creator = script.Creator;
        zip.Properties.Description = script.Description;
        zip.Properties.Modified = script.Modified;
        zip.Properties.Revision = script.Version;
        zip.Properties.Title = script.Title;
        if (script.Website != null)
          zip.Properties.Identifier = script.Website.ToString();
      }

      var existingPaths = new HashSet<string>();

      // Record the import order
      var settings = new XmlWriterSettings()
      {
        OmitXmlDeclaration = true,
        Indent = true,
        IndentChars = "  "
      };
      using (var manifestStream = package.Manifest(true).Open())
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
            WriteReport(package, line, line.Path);
          }
          else if (package.TryAccessFile(line.Path, true, out var file))
          {
            using (var stream = file.Open())
            using (var writer = GetWriter(stream))
            {
              writer.WriteStartElement("AML");
              line.Script.WriteTo(writer);
              writer.WriteEndElement();
            }
          }
        };

        var scriptLines = script.Lines.Where(l => l.Type != InstallType.Warning && l.Type != InstallType.DependencyCheck);
        if (package is DirectoryPackage)
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

    private static string GetPath(InstallItem line, HashSet<string> existingPaths)
    {
      if (line.Reference.Type == "Report" && line.Type != InstallType.Script)
        return line.FilePath(existingPaths, ".xslt");
      else
        return line.FilePath(existingPaths);
    }

    private static XmlWriter GetWriter(Stream stream)
    {
      var settings = new XmlWriterSettings
      {
        OmitXmlDeclaration = true,
        Indent = true,
        IndentChars = "  ",
        CloseOutput = true
      };

      return XmlTextWriter.Create(stream, settings);
    }

    private const string _reportDataEnd = "REPORT-DATA-END-->";
    private const string _reportDataStart = "<!--REPORT-DATA-START";
    private const string _reportEnd = "METADATA-END-->";
    private const string _reportStart = "<!--METADATA-START";

    private static void WriteReport(IPackage package, InstallItem line, string path)
    {
      var dataFile = "<Result><Item></Item></Result>";

      if (!package.TryAccessFile(path, true, out var xsltFile))
        throw new InvalidOperationException();
      using (var writer = new StreamWriter(xsltFile.Open()))
      {
        var aml = new StringBuilder();

        using (var amlWriter = new System.IO.StringWriter(aml))
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

      if (!package.TryAccessFile(path + ".xml", true, out var xmlFile))
        throw new InvalidOperationException();
      using (var writer = new StreamWriter(xmlFile.Open()))
      {
        writer.Write(dataFile);
      }
    }

    public static void WriteAml(this IPackageFile file, IPackage package, XmlWriter writer)
    {
      if (file.Path.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
      {
        string xsltContent;
        using (var stream = file.Open())
        using (var reader = new StreamReader(stream))
        {
          xsltContent = reader.ReadToEnd();
        }
        string xml = null;
        if (package.TryAccessFile(file.Path + ".xml", false, out var dataFile))
        {
          using (var stream = dataFile.Open())
          using (var reader = new StreamReader(stream))
          {
            xml = reader.ReadToEnd();
          }
        }

        var metaStart = xsltContent.IndexOf(_reportStart);
        var metaEnd = xsltContent.IndexOf(_reportEnd);

        if (metaStart >= 0 && metaEnd >= 0)
        {
          var stack = new Stack<bool>();
          using (var strReader = new StringReader(xsltContent.Substring(metaStart + _reportStart.Length, metaEnd - metaStart - _reportStart.Length).Trim()))
          using (var reader = XmlReader.Create(strReader, new XmlReaderSettings()
          {
            IgnoreWhitespace = true,
          }))
          {
            while (reader.Read())
            {
              switch (reader.NodeType)
              {
                case XmlNodeType.Element:
                  writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                  writer.WriteAttributes(reader, false);
                  if (reader.IsEmptyElement)
                    writer.WriteEndElement();
                  else if (reader.LocalName == "Item" && reader.GetAttribute("type") == "Report")
                    stack.Push(true);
                  else
                    stack.Push(false);
                  break;
                case XmlNodeType.Text:
                  writer.WriteString(reader.Value);
                  break;
                case XmlNodeType.CDATA:
                  writer.WriteCData(reader.Value);
                  break;
                case XmlNodeType.EntityReference:
                  writer.WriteEntityRef(reader.Name);
                  break;
                case XmlNodeType.ProcessingInstruction:
                case XmlNodeType.XmlDeclaration:
                  writer.WriteProcessingInstruction(reader.Name, reader.Value);
                  break;
                case XmlNodeType.Comment:
                  writer.WriteComment(reader.Value);
                  break;
                case XmlNodeType.DocumentType:
                  writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                  break;
                //case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                  writer.WriteWhitespace(reader.Value);
                  break;
                case XmlNodeType.EndElement:
                  if (stack.Pop())
                  {
                    var xslt = xsltContent.Substring(metaEnd + _reportEnd.Length).Trim();
                    if (!string.IsNullOrEmpty(xml))
                      xslt += Environment.NewLine + Environment.NewLine + _reportDataStart + Environment.NewLine + xml + Environment.NewLine + _reportDataEnd;
                    writer.WriteElementString("xsl_stylesheet", xslt);
                  }
                  writer.WriteFullEndElement();
                  break;
              }
            }
          }
        }
        else if (xsltContent.StartsWith("<AML>"))
        {
          using (var strReader = new StringReader(xsltContent))
          using (var reader = XmlReader.Create(strReader, new XmlReaderSettings()
          {
            IgnoreWhitespace = true,
          }))
          {
            writer.WriteNode(reader, false);
          }
        }
        else
        {
          throw new ArgumentException("Invalid xslt file");
        }
      }
      else
      {
        using (var stream = file.Open())
        using (var reader = XmlReader.Create(stream, new XmlReaderSettings()
        {
          IgnoreWhitespace = true,
        }))
        {
          writer.WriteNode(reader, false);
        }
      }
    }

    public delegate XmlWriter AmlMergeCallback(string path, int progress, XElement baseScript);

    /// <summary>
    /// Writes the difference between two directories
    /// </summary>
    /// <param name="baseDir">The base directory which needs to be transformed</param>
    /// <param name="compareDir">The comparison directory which is what the base directoy should look like after transformation</param>
    /// <param name="callback">Gets an XML writer to write the merge script given the path and current progress (integer between 0 and 100)</param>
    /// <returns>Metadata regarding the target state</returns>
    public static PackageMetadataProvider WriteAmlMergeScripts(this IPackage baseDir, IPackage compareDir
      , AmlMergeCallback callback)
    {
      Func<IPackageFile, IComparable> keyGetter = i => i.Path.ToUpperInvariant().Replace('\\', '/').TrimStart('/');
      var basePaths = baseDir.Files().OrderBy(keyGetter).ToList();
      var comparePaths = compareDir.Files().OrderBy(keyGetter).ToList();
      var completed = 0;
      var total = basePaths.Count + comparePaths.Count;
      var metadata = new PackageMetadataProvider();

      var baseScripts = new List<AmlScript>();
      var compareScripts = new List<AmlScript>();

      Action<MergeType, AmlScript, AmlScript> mergeScripts = (type, baseScript, compareScript) =>
      {
        switch (type)
        {
          case MergeType.StartOnly: // Delete
            completed++;
            using (var writer = callback(baseScript.Path, completed * 100 / total, baseScript.Script))
            {
              var elem = AmlDiff.GetMergeScript(baseScript.Script, null);
              if (elem.Elements().Any())
                elem.WriteTo(writer);
            }
            break;
          case MergeType.DestinationOnly: // Add
            completed++;
            using (var writer = callback(compareScript.Path, completed * 100 / total, null))
            {
              compareScript.Script.WriteTo(writer);
              metadata.Add(compareScript.Script);
            }
            break;
          default:
            total--;
            completed++;

            var path = compareScript.Path;
            if (!string.Equals(baseScript.Path, compareScript.Path, StringComparison.OrdinalIgnoreCase)
              && !string.IsNullOrEmpty(compareScript.Id)
              && baseScript.Path.IndexOf(compareScript.Id) >= 0)
            {
              path = baseScript.Path;
            }
            using (var writer = callback(path, completed * 100 / total, null))
            {
              if (compareScript.Script.Elements("Item").Any(e => (string)e.Attribute("action") != "delete"))
                metadata.Add(compareScript.Script);
              var elem = AmlDiff.GetMergeScript(baseScript.Script, compareScript.Script);
              if (elem.Elements().Any())
                elem.WriteTo(writer);
            }
            break;
        }
      };

      basePaths.MergeSorted(comparePaths, keyGetter, (type, baseFile, compareFile) =>
      {
        try
        {
          var path = (compareFile ?? baseFile).Path;
          if (path.EndsWith(".xslt.xml", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".innpkg", StringComparison.OrdinalIgnoreCase))
            return;

          switch (type)
          {
            case MergeType.StartOnly: // Delete
              var baseScript = new AmlScript(baseFile, baseDir);
              if (baseScript.Key == null)
                mergeScripts(type, baseScript, null);
              else
                baseScripts.Add(baseScript);
              break;
            case MergeType.DestinationOnly: // Add
              var compareScript = new AmlScript(compareFile, compareDir);
              IdentityMergeToAdd(compareScript.Script);
              if (ItemAddToIgnore(compareScript.Script))
                return;

              if (compareScript.Key == null)
                mergeScripts(type, null, compareScript);
              else
                compareScripts.Add(compareScript);
              break;
            default: // Edit
              baseScript = new AmlScript(baseFile, baseDir);
              compareScript = new AmlScript(compareFile, compareDir);
              if (baseScript.Key == compareScript.Key)
              {
                mergeScripts(type, baseScript, compareScript);
              }
              else
              {
                baseScripts.Add(baseScript);
                compareScripts.Add(compareScript);
              }
              break;
          }
        }
        catch (XmlException ex)
        {
          ex.Data["MergeType"] = type.ToString();
          if (baseFile != null)
            ex.Data["BasePath"] = baseFile.Path;
          if (compareFile != null)
            ex.Data["ComparePath"] = compareFile.Path;
          throw;
        }
      });

      baseScripts = baseScripts.OrderBy(s => s.Key).ToList();
      compareScripts = compareScripts.OrderBy(s => s.Key).ToList();

      baseScripts.MergeSorted(compareScripts, s => s.Key, mergeScripts);
      return metadata;
    }

    /// <summary>
    /// Change action='merge' to action='add' for identities
    /// </summary>
    private static void IdentityMergeToAdd(XElement aml)
    {
      var item = aml.DescendantsAndSelf("Item").FirstOrDefault();
      if (item?.Attribute("type")?.Value != "Identity")
        return;
      item.SetAttributeValue("action", "add");
    }

    /// <summary>
    /// Indicates that the add should be ignored.
    /// </summary>
    private static bool ItemAddToIgnore(XElement aml)
    {
      return IsAddMorphaeToFileContainerItems(aml);
    }

    private static bool IsAddMorphaeToFileContainerItems(XElement aml)
    {
      var item = aml.DescendantsAndSelf("Item").FirstOrDefault();
      if (item?.Attribute("type")?.Value != "ItemType" || item.Attribute("action")?.Value != "edit")
        return false;
      if (item.Elements().Count() != 1 || item.Elements().First().Name.LocalName != "Relationships")
        return false;
      var relationships = item.Elements().First();
      if (relationships.Elements().Count() != 1 || item.Elements().First().Name.LocalName == "Item")
        return false;
      var relation = relationships.Elements().First();
      if (relation.Attribute("type")?.Value != "Morphae" || relation.Element("source_id")?.Value != "41EF49EFD2ED4F6EAB04C047681F33AC")
        return false;
      return true;
    }

    public static void WriteAmlMergeScripts(this IPackage baseDir, IPackage compareDir
      , string outputDirectory, Action<int> progressCallback = null)
    {
      WriteAmlMergeScripts(baseDir, compareDir, (path, progress, deletedScript) =>
      {
        progressCallback?.Invoke(progress);

        var savePath = Path.Combine(outputDirectory, path);
        Directory.CreateDirectory(Path.GetDirectoryName(savePath));

        var settings = new XmlWriterSettings()
        {
          OmitXmlDeclaration = true,
          Indent = true,
          IndentChars = "  "
        };
        var stream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
        return XmlWriter.Create(stream, settings);
      });
    }

    private class AmlScript
    {
      public string Key { get; }
      public string Path { get; }
      public string Id { get; }
      public XElement Script { get; }

      public AmlScript(IPackageFile file, IPackage package)
      {
        Path = file.Path;
        var doc = new XDocument();
        using (var writer = doc.CreateWriter())
          file.WriteAml(package, writer);
        Script = doc.Root;

        var item = Script.DescendantsAndSelf("Item").FirstOrDefault();
        if (item != null
          && (item.Parent == null
            || !item.Parent.Elements("Item").Skip(1).Any()))
        {
          Id = (string)item.Attribute("id");
          var parts = new[] {
              ((string)(item.Attribute("id") ?? item.Attribute("where")))?.ToUpperInvariant(),
              (string)item.Attribute(XmlFlags.Attr_ScriptType),
              (string)item.Attribute("action")
            };
          if (parts[2] == "add" || parts[2] == "create")
            parts[2] = "merge";
          Key = string.Join(".", parts);
        }
      }
    }
  }
}
