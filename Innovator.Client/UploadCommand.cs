using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Innovator.Client
{
  /// <summary>
  /// A request used for uploading files to the database
  /// </summary>
  public class UploadCommand : Command
  {
    private Vault _vault;
    private List<CommandFile> _files = new List<CommandFile>();

    /// <summary>
    /// The AML query.  If not explicitly set, it is built based on the files which have been added to the requested.
    /// </summary>
    public override string Aml
    {
      get
      {
        if (string.IsNullOrEmpty(base.Aml))
        {
          if (_files.Count == 1)
          {
            return _files[0].Aml;
          }
          else if (_files.Count > 1)
          {
            return "<AML>" + _files.GroupConcat("", f => f.Aml) + "</AML>";
          }
        }
        return base.Aml;
      }
      set
      {
        base.Aml = value;
      }
    }
    internal IEnumerable<CommandFile> Files { get { return _files; } }
    internal Vault Vault { get { return _vault; } }

    /// <summary>
    /// Create an upload command with the specified vault metadata
    /// </summary>
    /// <param name="vault">Vault metadata</param>
    public UploadCommand(Vault vault)
    {
      _vault = vault;
    }

    ///// <summary>
    ///// Merge another request into this request
    ///// </summary>
    ///// <param name="request">Request to merge into this one</param>
    //public override Command MergeWith(Command request)
    //{
    //  var upload = request as UploadCommand;
    //  if (upload != null)
    //  {
    //    _files.AddRange(upload._files);
    //  }
    //  return base.MergeWith(request);
    //}

    /// <summary>
    /// Add a file to the upload request
    /// </summary>
    /// <param name="id">Aras ID of the file</param>
    /// <param name="path">Physical path of the file</param>
    /// <param name="isNew">Is this a new file being added to the database for the first time?</param>
    /// <returns>AML file string useful for building a larger AML statement</returns>
    public string AddFile(string id, string path, bool isNew = true)
    {
      var file = new CommandFile(id, path, _vault.Id, isNew);
      _files.Add(file);
      return file.Aml;
    }

    /// <summary>
    /// Add a file to the upload request
    /// </summary>
    /// <param name="id">Aras ID of the file</param>
    /// <param name="path">Path (or pseudo path) of the file</param>
    /// <param name="data">Stream of data representing the file</param>
    /// <param name="isNew">Is this a new file being added to the database for the first time?</param>
    /// <returns>AML file string useful for building a larger AML statement</returns>
    public string AddFile(string id, string path, Stream data, bool isNew = true)
    {
      var file = new CommandFile(id, path, data, _vault.Id, isNew);
      _files.Add(file);
      return file.Aml;
    }

    /// <summary>
    /// Add a file to the request without specifying an ID
    /// </summary>
    /// <param name="path">Path (or pseudo path) of the file</param>
    /// <param name="data">Stream of data representing the file</param>
    /// <param name="isNew">Is this a new file being added to the database for the first time?</param>
    /// <returns>AML file string useful for building a larger AML statement</returns>
    public string AddFile(string path, Stream data, bool isNew = true)
    {
      return AddFile(Guid.NewGuid().ToString("N").ToUpperInvariant(), path, data, isNew);
    }

    /// <summary>
    /// Adds a file Item query to the request where the path to the file is specified as the actual_filename property
    /// </summary>
    /// <param name="query">Query to add to the request</param>
    public void AddFileQuery(string query)
    {
      var elem = XElement.Parse(query);
      var files = elem.DescendantsAndSelf("Item")
        .Where(e => e.Attributes("type").Any(a => a.Value == "File")
                  && e.Elements("actual_filename").Any(p => !string.IsNullOrEmpty(p.Value))
                  && e.Attributes("id").Any(p => !string.IsNullOrEmpty(p.Value))
                  && e.Attributes("action").Any(p => !string.IsNullOrEmpty(p.Value)));
      XElement newElem = null;
      foreach (var file in files.ToList())
      {
        var dataElem = file.Element("actual_data");
        if (dataElem == null)
        {
          newElem = XElement.Parse(AddFile(
            file.Attribute("id").Value,
            file.Element("actual_filename").Value,
            file.Attribute("action").Value == "add" || file.Attribute("action").Value == "create"));
        }
        else
        {
          var encoding = dataElem.Attribute("encoding");
          var data = encoding != null && string.Equals(encoding.Value, "base64", StringComparison.OrdinalIgnoreCase)
            ? Convert.FromBase64String(dataElem.Value)
            : Encoding.UTF8.GetBytes(dataElem.Value);
          var stream = new MemoryStream(data);
          newElem = XElement.Parse(AddFile(
            file.Attribute("id").Value,
            file.Element("actual_filename").Value,
            stream,
            file.Attribute("action").Value == "add" || file.Attribute("action").Value == "create"));
        }
        if (file.Parent != null)
        {
          MergeIfMissing(newElem, file);
          newElem = null;
        }
      }
      base.AddAml(newElem != null ? newElem.ToString() : elem.ToString());
    }

    private void MergeIfMissing(XElement source, XElement target)
    {
      XElement targetElem;
      foreach (var elem in source.Elements())
      {
        targetElem = target.Element(elem.Name);
        if (targetElem == null)
        {
          target.Add(elem);
        }
        else
        {
          MergeIfMissing(elem, targetElem);
        }
      }
    }
  }
}
