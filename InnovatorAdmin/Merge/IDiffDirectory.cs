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
      , string outputDirectory, Action<int> progressCallback = null)
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
          var savePath = Path.Combine(outputDirectory, (b ?? c).Path);
          if (savePath.EndsWith(".xslt.xml"))
            return;

          Directory.CreateDirectory(Path.GetDirectoryName(savePath));
          switch (i)
          {
            case -1:
              SaveFile(AmlDiff.GetMergeScript(ReadFile(b,
                p => basePaths.FirstOrDefault(f => f.Path == p)), null), savePath);
              break;
            case 1:
              using (var baseStream = c.OpenRead())
              using (var outStream = File.OpenWrite(savePath))
              {
                baseStream.CopyTo(outStream);
              }
              break;
            default:
              SaveFile(AmlDiff.GetMergeScript(
                ReadFile(b, p => basePaths.FirstOrDefault(f => f.Path == p)),
                ReadFile(c, p => comparePaths.FirstOrDefault(f => f.Path == p))), savePath);
              total--;
              break;
          }
          completed++;
          if (progressCallback != null) progressCallback(completed * 100 / total);
        }
        catch (XmlException ex)
        {
          throw new XmlException(string.Format("{0} ({1}, {2}, {3})", ex.Message, i, b.Path, c.Path), ex);
        }
      });
    }

    private static string ReadFile(IDiffFile file, Func<string, IDiffFile> fileGetter)
    {
      if (file.Path.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
      {
        return InnovatorPackage.ReadReport(file.Path, p => 
        {
          var f = fileGetter(p);
          if (f != null)
            return f.OpenRead();
          return new MemoryStream(Encoding.UTF8.GetBytes("<Result><Item></Item></Result>"));
        }).OuterXml;
      }
      else
      {
        using (var baseStream = file.OpenRead())
        using (var baseRead = new StreamReader(baseStream))
        {
          return baseRead.ReadToEnd();
        }
      }
      
    }

    private static void SaveFile(XElement elem, string path)
    {
      if (elem.Elements().Any())
      {
        var settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = true;
        settings.Indent = true;
        settings.IndentChars = "  ";
        using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
        using (var writer = XmlWriter.Create(stream, settings))
        {
          elem.Save(writer);
        }
      }
    }
  }
}
