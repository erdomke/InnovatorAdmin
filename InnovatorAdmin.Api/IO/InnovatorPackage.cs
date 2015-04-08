using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public class InnovatorPackage : IDisposable
  {
    //private readonly Uri _scriptUri;
    private const string _manifestPath = "manifest.txt";
    private bool _isDisposed;
    private Package _package;
    private List<string> _paths = new List<string>();

    public InnovatorPackage(string path)
    {
      _package = System.IO.Packaging.Package.Open(path, FileMode.OpenOrCreate);
    }

    public InstallScript Read()
    {
      var result = new InstallScript();
      result.Created = _package.PackageProperties.Created;
      result.Creator = _package.PackageProperties.Creator;
      result.Description = _package.PackageProperties.Description;
      result.Modified = _package.PackageProperties.Modified;
      result.Version = _package.PackageProperties.Revision;
      result.Title = _package.PackageProperties.Title;
      if (!string.IsNullOrEmpty(_package.PackageProperties.Identifier))
        result.Website = new Uri(_package.PackageProperties.Identifier);

      string path;
      XmlDocument doc;
      var scripts = new List<InstallItem>();
      using (var reader = new StreamReader(GetExistingStream(_manifestPath)))
      {
        while (!reader.EndOfStream)
        {
          path = reader.ReadLine();
          if (!string.IsNullOrEmpty(path))
          {
            doc = new XmlDocument();
            doc.Load(GetExistingStream(path));
            foreach (var item in doc.DocumentElement.Elements("Item"))
            {
              scripts.Add(InstallItem.FromScript(item));
            }
          }
        }
      }
      result.Lines = scripts;

      return result;
    }
    public void Write(InstallScript script)
    {
      _package.PackageProperties.Created = script.Created;
      _package.PackageProperties.Creator = script.Creator;
      _package.PackageProperties.Description = script.Description;
      _package.PackageProperties.Modified = script.Modified;
      _package.PackageProperties.Revision = script.Version;
      _package.PackageProperties.Title = script.Title;
      if (script.Website != null)
        _package.PackageProperties.Identifier = script.Website.ToString();

      _paths.Clear();
      script.WriteLines(GetWriter);

      using (var writer = new StreamWriter(GetNewStream(_manifestPath)))
      {
        foreach (var path in _paths)
        {
          writer.WriteLine(path);
        }
      }
    }

    private Stream GetExistingStream(string path)
    {
      path = ".\\" + path;
      var uri = PackUriHelper.CreatePartUri(new Uri(path, UriKind.Relative));
      var part = _package.GetPart(uri);
      return part.GetStream();
    }
    private Stream GetNewStream(string path)
    {
      path = ".\\" + path;
      var uri = PackUriHelper.CreatePartUri(new Uri(path, UriKind.Relative));
      if (_package.PartExists(uri))
      {
        _package.DeletePart(uri);
      }
      var part = _package.CreatePart(uri, "", CompressionOption.Normal);

      return part.GetStream();
    }

    private XmlWriter GetWriter(string path)
    {
      _paths.Add(path);
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";

      return XmlTextWriter.Create(GetNewStream(path), settings);
    }

    //private const long BUFFER_SIZE = 4096;

    //private static void CopyStream(System.IO.FileStream inputStream, System.IO.Stream outputStream)
    //{
    //  long bufferSize = inputStream.Length < BUFFER_SIZE ? inputStream.Length : BUFFER_SIZE;
    //  byte[] buffer = new byte[bufferSize];
    //  int bytesRead = 0;
    //  long bytesWritten = 0;
    //  while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
    //  {
    //    outputStream.Write(buffer, 0, bytesRead);
    //    bytesWritten += bufferSize;
    //  }
    //}

    public void Dispose()
    {
      if (!_isDisposed) ((IDisposable)_package).Dispose();
      _isDisposed = true;
    }
  }
}
