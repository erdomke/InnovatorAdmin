using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace InnovatorAdmin
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
    public bool DependencySorted { get; set; }

    public InstallScript()
    {
      this.DependencySorted = true;
    }

    //public IEnumerable<IEnumerable<InstallItem>> GroupLines(Func<InstallItem, bool> predicate = null)
    //{
    //  var linesToExport = Lines.Where(l => l.Script != null && l.Type != InstallType.Warning);
    //  if (predicate != null) linesToExport = linesToExport.Where(predicate);

    //  bool first = true;
    //  List<InstallItem> buffer = null;
    //  foreach (var line in linesToExport)
    //  {
    //    if (line.Type == InstallType.Script && !first)
    //    {
    //      buffer.Add(line);
    //    }
    //    else
    //    {
    //      if (buffer != null) yield return buffer;

    //      buffer = new List<InstallItem>();
    //      buffer.Add(line);
    //    }

    //    first = false;
    //  }

    //  if (buffer != null) yield return buffer;
    //}
  }
}
