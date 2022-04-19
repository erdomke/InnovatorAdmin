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
  public interface IDiffDirectory
  {
    IEnumerable<IDiffFile> GetFiles();
  }

  public static class DirectoryExtensions
  {
    /// <summary>
    /// Writes the difference between two directories
    /// </summary>
    /// <param name="baseDir">The base directory which needs to be transformed</param>
    /// <param name="compareDir">The comparison directory which is what the base directoy should look like after transformation</param>
    /// <param name="callback">Gets an XML writer to write the merge script given the path and current progress (integer between 0 and 100)</param>
    /// <returns>Metadata regarding the target state</returns>
    public static PackageMetadataProvider WriteAmlMergeScripts(this IDiffDirectory baseDir, IDiffDirectory compareDir
      , Func<string, int, XmlWriter> callback)
    {
      Func<IDiffFile, IComparable> keyGetter = i => i.Path.ToUpperInvariant().Replace('\\', '/').TrimStart('/');
      var basePaths = baseDir.GetFiles().OrderBy(keyGetter).ToList();
      var comparePaths = compareDir.GetFiles().OrderBy(keyGetter).ToList();
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
            using (var writer = callback(baseScript.Path, completed * 100 / total))
            {
              var elem = AmlDiff.GetMergeScript(baseScript.Script, null);
              if (elem.Elements().Any())
                elem.WriteTo(writer);
            }
            break;
          case MergeType.DestinationOnly: // Add
            completed++;
            using (var writer = callback(compareScript.Path, completed * 100 / total))
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
            using (var writer = callback(path, completed * 100 / total))
            {
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
              var baseScript = new AmlScript(baseFile, basePaths);
              if (baseScript.Key == null)
                mergeScripts(type, baseScript, null);
              else
                baseScripts.Add(baseScript);
              break;
            case MergeType.DestinationOnly: // Add
              var compareScript = new AmlScript(compareFile, comparePaths);
              IdentityMergeToAdd(compareScript.Script);
              if (ItemAddToIgnore(compareScript.Script))
                return;

              if (compareScript.Key == null)
                mergeScripts(type, null, compareScript);
              else
                compareScripts.Add(compareScript);
              break;
            default: // Edit
              baseScript = new AmlScript(baseFile, basePaths);
              compareScript = new AmlScript(compareFile, comparePaths);
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

    public static void WriteAmlMergeScripts(this IDiffDirectory baseDir, IDiffDirectory compareDir
      , string outputDirectory, Action<int> progressCallback = null)
    {
      WriteAmlMergeScripts(baseDir, compareDir, (path, progress) =>
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

      public AmlScript(IDiffFile file, IEnumerable<IDiffFile> others)
      {
        Path = file.Path;
        using (var stream = ReadFile(file, path => others.FirstOrDefault(f => string.Equals(f.Path, path, StringComparison.OrdinalIgnoreCase))))
        {
          Script = Utils.LoadXml(stream);
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

      private static Stream ReadFile(IDiffFile file, Func<string, IDiffFile> fileGetter)
      {
        if (file.Path.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
        {
          var report = InnovatorPackage.ReadReport(file.Path, p =>
          {
            var f = fileGetter(p);
            if (f != null)
              return f.OpenRead();
            return new MemoryStream(Encoding.UTF8.GetBytes("<Result><Item></Item></Result>"));
          });

          var result = new MemoryStream();
          using (var writer = XmlWriter.Create(result, new XmlWriterSettings
          {
            OmitXmlDeclaration = true,
            Indent = true,
            IndentChars = "  "
          }))
          {
            report.WriteTo(writer);
          };
          result.Position = 0;
          return result;
        }
        else
        {
          return file.OpenRead();
        }
      }
    }
  }
}
