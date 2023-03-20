using System;
using System.Collections.Generic;
using System.Linq;

namespace InnovatorAdmin
{
  public class InstallScript : IPackage
  {
    private List<Version> _supportedVersions = new List<Version>();
    private string _title;

    public DateTime? Created { get; set; }
    public string Creator { get; set; }
    public string Description { get; set; }
    public Uri ExportUri { get; set; }
    public string ExportDb { get; set; }
    public bool IsMerge { get; set; }
    public IEnumerable<InstallItem> Lines { get; set; }
    public DateTime? Modified { get; set; }
    public List<Version> SupportedVersions { get { return _supportedVersions; } }
    public string Title
    {
      get { return _title; }
      set { _title = Utils.CleanFileName(value); }
    }
    public string Version { get; set; }
    public Uri Website { get; set; }
    public bool DependencySorted { get; set; }

    public InstallScript()
    {
      this.DependencySorted = true;
    }

    IEnumerable<IPackageFile> IPackage.Files()
    {
      return Lines;
    }

    IPackageFile IPackage.Manifest(bool create)
    {
      throw new NotSupportedException();
    }

    bool IPackage.TryAccessFile(string path, bool create, out IPackageFile file)
    {
      if (create)
        throw new NotSupportedException();
      file = Lines
        .OfType<IPackageFile>()
        .FirstOrDefault(f => string.Equals(f.Path, path, StringComparison.OrdinalIgnoreCase));
      return file != null;
    }

    void IDisposable.Dispose()
    {
      // Do nothing
    }
  }
}
