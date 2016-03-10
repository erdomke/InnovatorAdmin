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
    public static IEnumerable<FileDiff> GetDiffs(this IDiffDirectory baseDir, IDiffDirectory compareDir)
    {
      Func<IDiffFile, IComparable> keyGetter = i => i.Path;
      var basePaths = baseDir.GetFiles().OrderBy(keyGetter).ToArray();
      var comparePaths = compareDir.GetFiles().OrderBy(keyGetter).ToArray();

      var result = new List<FileDiff>();
      basePaths.MergeSorted(comparePaths, keyGetter, (i, b, c) =>
      {
        switch (i)
        {
          case -1:
            result.Add(new FileDiff()
            {
              Path = b.Path,
              InBase = FileStatus.Unchanged,
              InCompare = FileStatus.DoesntExist
            });
            break;
          case 1:
            result.Add(new FileDiff()
            {
              Path = c.Path,
              InBase = FileStatus.DoesntExist,
              InCompare = FileStatus.Unchanged
            });
            break;
          default:
            result.Add(new FileDiff()
            {
              Path = b.Path,
              InBase = FileStatus.Unchanged,
              InCompare = b.CompareKey.CompareTo(c.CompareKey) == 0
                ? FileStatus.Unchanged : FileStatus.Unchanged
            });
            break;
        }
      });
      return result;
    }

    public static void WriteAmlMergeScripts(this IDiffDirectory baseDir, IDiffDirectory compareDir
      , Func<string, int, XmlWriter> callback = null)
    {
      Func<IDiffFile, IComparable> keyGetter = i => i.Path;
      var basePaths = baseDir.GetFiles().OrderBy(keyGetter).ToArray();
      var comparePaths = compareDir.GetFiles().OrderBy(keyGetter).ToArray();
      var completed = 0;
      var total = basePaths.Length + comparePaths.Length;

      var result = new List<FileDiff>();

      basePaths.MergeSorted(comparePaths, keyGetter, (i, b, c) =>
      {
        try
        {
          var path = (b ?? c).Path;
          if (path.EndsWith(".xslt.xml"))
            return;

          completed++;
          switch (i)
          {
            case -1: // Delete
              using (var baseStream = ReadFile(b, p => basePaths.FirstOrDefault(f => f.Path == p)))
              using (var writer = callback(path, completed * 100 / total))
              {
                var elem = AmlDiff.GetMergeScript(baseStream, null);
                if (elem.Elements().Any())
                  elem.WriteTo(writer);
              }
              break;
            case 1: // Add
              using (var baseStream = c.OpenRead())
              using (var writer = callback(path, completed * 100 / total))
              {
                XElement.Load(baseStream).WriteTo(writer);
              }
              break;
            default: // Edit
              total--;
              using (var baseStream = ReadFile(b, p => basePaths.FirstOrDefault(f => f.Path == p)))
              using (var compareStream = ReadFile(c, p => comparePaths.FirstOrDefault(f => f.Path == p)))
              using (var writer = callback(path, completed * 100 / total))
              {
                var elem = AmlDiff.GetMergeScript(baseStream, compareStream);
                if (elem.Elements().Any())
                  elem.WriteTo(writer);
              }
              break;
          }
        }
        catch (XmlException ex)
        {
          throw new XmlException(string.Format("{0} ({1}, {2}, {3})", ex.Message, i, b.Path, c.Path), ex);
        }
      });
    }

    public static void WriteAmlMergeScripts(this IDiffDirectory baseDir, IDiffDirectory compareDir
      , string outputDirectory, Action<int> progressCallback = null)
    {
      WriteAmlMergeScripts(baseDir, compareDir, (path, progress) =>
      {
        if (progressCallback != null) progressCallback(progress);

        var savePath = Path.Combine(outputDirectory, path);
        Directory.CreateDirectory(Path.GetDirectoryName(savePath));

        var settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = true;
        settings.Indent = true;
        settings.IndentChars = "  ";
        var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        return XmlWriter.Create(stream, settings);
      });
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

        var settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = true;
        settings.Indent = true;
        settings.IndentChars = "  ";
        var result = new MemoryStream();
        using (var writer = XmlWriter.Create(result, settings))
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
