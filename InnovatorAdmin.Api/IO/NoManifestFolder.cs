using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace InnovatorAdmin
{
  public class NoManifestFolder : InnovatorPackage
  {
    private string _path;

    public NoManifestFolder(string path)
    {
      _path = path;
    }

    public override InstallScript Read()
    {
      var result = new InstallScript();
      var scripts = new List<InstallItem>();

      XmlDocument doc = null;
      foreach (var file in Directory.GetFiles(_path, "*.*", SearchOption.AllDirectories))
      {
        if (file.EndsWith(".xslt.xml", StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }
        else if (file.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
        {
          doc = InnovatorPackage.ReadReport(file, GetExistingStream);
        }
        else if (file.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
        {
          doc = new XmlDocument(doc == null ? null : doc.NameTable);
          doc.Load(GetExistingStream(file));
        }
        else
        {
          continue;
        }

        foreach (var item in doc.DocumentElement.Elements("Item"))
        {
          scripts.Add(InstallItem.FromScript(item));
        }
      }

      result.Lines = scripts;
      return result;
    }

    public override void Write(InstallScript script)
    {
      throw new NotSupportedException();
    }

    protected override System.IO.Stream GetExistingStream(string path)
    {
      return new FileStream(path, FileMode.Open, FileAccess.Read);
    }

    protected override System.IO.Stream GetNewStream(string path)
    {
      throw new NotImplementedException();
    }
  }
}
