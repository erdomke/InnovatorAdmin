using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public class InstallScript
  {
    private List<Version> _supportedVersions = new List<Version>();

    public bool AddPackage { get; set; }
    public DateTime? Created { get; set; }
    public string Creator { get; set; }
    public string Description { get; set; }
    public Uri ExportUri { get; set; }
    public string ExportDb { get; set; }
    public IEnumerable<InstallItem> Lines { get; set; }
    public DateTime? Modified { get; set; }
    public List<Version> SupportedVersions { get { return _supportedVersions; } }
    public string Title { get; set; }
    public string Version { get; set; }
    public Uri Website { get; set; }

    public void WriteLines(Func<string, XmlWriter> writerGetter, Func<InstallItem, bool> predicate = null)
    {
      bool first = true;
      XmlWriter curr = null;
      var linesToExport = Lines.Where(l => l.Script != null && l.Type != InstallType.Warning);
      if (predicate != null) linesToExport = linesToExport.Where(predicate);

      foreach (var line in linesToExport)
      {
        if (line.Type == InstallType.Script && !first)
        {
          line.Script.WriteTo(curr);
        }
        else
        {
          if (curr != null)
          {
            curr.WriteEndElement();
            curr.Flush();
            curr.Close();
          }

          curr = writerGetter(line.Reference.Type + "\\" + (line.Reference.KeyedName ?? line.Reference.Unique) + ".xml");
          curr.WriteStartElement("AML");
          line.Script.WriteTo(curr);
        }

        first = false;
      }

      if (curr != null)
      {
        curr.WriteEndElement();
        curr.Flush();
        curr.Close();
      }
    }
  }
}
