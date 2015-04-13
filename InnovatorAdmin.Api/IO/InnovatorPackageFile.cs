using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public class InnovatorPackageFile : InnovatorPackage, IDisposable
  {
    private const string _manifestPath = "manifest.innpkg";
    private bool _isDisposed;
    private Package _package;
    
    public InnovatorPackageFile(string path)
    {
      _package = System.IO.Packaging.Package.Open(path, FileMode.OpenOrCreate);
    }
    public InnovatorPackageFile(Stream data)
    {
      _package = System.IO.Packaging.Package.Open(data, FileMode.OpenOrCreate);
    }

    public override InstallScript Read()
    {
      var result = base.Read();
      result.Created = _package.PackageProperties.Created;
      result.Creator = _package.PackageProperties.Creator;
      result.Description = _package.PackageProperties.Description;
      result.Modified = _package.PackageProperties.Modified;
      result.Version = _package.PackageProperties.Revision;
      result.Title = _package.PackageProperties.Title;
      if (!string.IsNullOrEmpty(_package.PackageProperties.Identifier))
        result.Website = new Uri(_package.PackageProperties.Identifier);
      return result;
    }

    public override void Write(InstallScript script)
    {
      _package.PackageProperties.Created = script.Created;
      _package.PackageProperties.Creator = script.Creator;
      _package.PackageProperties.Description = script.Description;
      _package.PackageProperties.Modified = script.Modified;
      _package.PackageProperties.Revision = script.Version;
      _package.PackageProperties.Title = script.Title;
      if (script.Website != null)
        _package.PackageProperties.Identifier = script.Website.ToString();

      base.Write(script);
    }

    protected override Stream GetExistingStream(string path)
    {
      if (string.IsNullOrEmpty(path)) path = _manifestPath;
      path = ".\\" + path;
      var uri = PackUriHelper.CreatePartUri(new Uri(path, UriKind.Relative));
      var part = _package.GetPart(uri);
      return part.GetStream();
    }
    protected override Stream GetNewStream(string path)
    {
      if (string.IsNullOrEmpty(path)) path = _manifestPath;
      path = ".\\" + path;
      var uri = PackUriHelper.CreatePartUri(new Uri(path, UriKind.Relative));
      if (_package.PartExists(uri))
      {
        _package.DeletePart(uri);
      }
      var part = _package.CreatePart(uri, "", CompressionOption.Normal);

      return part.GetStream();
    }


    public override void Dispose()
    {
      if (!_isDisposed) ((IDisposable)_package).Dispose();
      _isDisposed = true;
    }
  }
}
