using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;

namespace InnovatorAdmin
{
  public class ZipPackage : IPackage
  {
    private const string _manifestPath = "manifest.innpkg";
    private System.IO.Packaging.Package _package;

    public PackageProperties Properties => _package.PackageProperties;

    public ZipPackage(string path)
    {
      _package = System.IO.Packaging.Package.Open(path, FileMode.OpenOrCreate);
    }

    public ZipPackage(Stream data)
    {
      _package = System.IO.Packaging.Package.Open(data, FileMode.OpenOrCreate);
    }

    public IEnumerable<IPackageFile> Files()
    {
      return _package.GetParts()
        .Select(p => new PackageFile(p))
        .Where(p => !string.Equals(p.Path, _manifestPath, StringComparison.OrdinalIgnoreCase));
    }

    public IPackageFile Manifest(bool create)
    {
      return TryAccessFile(_manifestPath, create, out var file) ? file : null;
    }

    public bool TryAccessFile(string path, bool create, out IPackageFile file)
    {
      var uri = PackUriHelper.CreatePartUri(new Uri(".\\" + path, UriKind.Relative));
      if (create)
      {
        if (_package.PartExists(uri))
          _package.DeletePart(uri);
        var part = _package.CreatePart(uri, "", CompressionOption.Normal);
        file = new PackageFile(part);
        return true;
      }
      else if (_package.PartExists(uri))
      {
        file = new PackageFile(_package.GetPart(uri));
        return true;
      }
      else
      {
        file = null;
        return false;
      }
    }

    public void Dispose()
    {
      ((IDisposable)_package).Dispose();
    }

    private class PackageFile : IPackageFile
    {
      private PackagePart _part;

      public string Path => _part.Uri.ToString().TrimStart('/');

      public PackageFile(PackagePart part)
      {
        _part = part;
      }

      public Stream Open()
      {
        return _part.GetStream();
      }
    }
  }
}
