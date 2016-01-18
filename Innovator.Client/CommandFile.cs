using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal class CommandFile
  {
    private string _aml;
    private byte[] _data;
    private byte[] _header;
    private string _id;
    private long _length;
    private string _path;

    public string Aml     { get { return _aml; } }
    public string Id      { get { return _id; } }
    public byte[] Header  { get { return _header; } }
    public long Length    { get { return _length; } }
    public string Path    { get { return _path; } }

    public CommandFile(string id, string path, string vaultId, bool isNew = true)
    {
      _id = id;
      _path = NormalizePath(path);
      if (!File.Exists(_path)) throw new IOException("File " + _path + " does not exist");
      _aml = GetFileItem(id, path, vaultId, isNew);
      _length = new FileInfo(_path).Length;
    }
    public CommandFile(string id, string path, Stream data, string vaultId, bool isNew = true)
    {
      _id = id;
      _path = NormalizePath(path);
      
      if (data.CanSeek) data.Position = 0;
      _data = new byte[data.Length];
      data.Read(_data, 0, _data.Length);
      _aml = GetFileItem(id, path, vaultId, isNew);
      _length = _data.Length;
    }

    private string GetFileItem(string id, string path, string vaultId, bool isNew)
    {
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = false;

      using (var writer = new StringWriter())
      using (var xml = XmlWriter.Create(writer, settings))
      {
        xml.WriteStartElement("Item");
        xml.WriteAttributeString("type", "File");
        xml.WriteAttributeString("action", isNew ? "add" : "edit");
        xml.WriteAttributeString("id", id);
        xml.WriteElementString("actual_filename", path);
        xml.WriteElementString("checkedout_path", System.IO.Path.GetDirectoryName(path));
        xml.WriteElementString("filename", System.IO.Path.GetFileName(path));

        xml.WriteStartElement("Relationships");
        xml.WriteStartElement("Item");
        xml.WriteAttributeString("type", "Located");
        if (isNew)
        {
          xml.WriteAttributeString("action", "add");
          xml.WriteAttributeString("id", Guid.NewGuid().ToString("N").ToUpperInvariant());
          xml.WriteElementString("file_version", "1");
          xml.WriteElementString("related_id", vaultId);
          xml.WriteElementString("source_id", id);
        }
        else
        {
          xml.WriteAttributeString("action", "merge");
          xml.WriteAttributeString("where", string.Format("[Located].[related_id]='{0}'", vaultId));
          xml.WriteElementString("related_id", vaultId);
        }
        xml.WriteEndElement();
        xml.WriteEndElement();

        xml.WriteEndElement();
        xml.Flush();
        writer.Flush();
        return writer.ToString();
      }
    }

    public Stream GetStream(bool async)
    {
      if (_data == null)
      {
        return new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, async);
      }
      else
      {
        return new MemoryStream(_data);
      }
    }

    public int SetHeader(string brline)
    {
      _header = Encoding.UTF8.GetBytes(string.Format("{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\n\r\n", brline, _id, _path));
      return _header.Length;
    }

    public static string NormalizePath(string path)
    {
      return System.IO.Path.GetFullPath(Environment.ExpandEnvironmentVariables(path));
    }
  }
}
