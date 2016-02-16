using Innovator.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InnovatorAdmin.Testing
{
  /// <summary>
  /// Download a file from the vault
  /// </summary>
  public class DownloadFile : ITestCommand
  {
    /// <summary>
    /// Comment preceding the command in the script
    /// </summary>
    public string Comment { get; set; }
    /// <summary>
    /// Text of the query to execute
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Code for executing the command
    /// </summary>
    public async Task Run(TestContext context)
    {
      var cmd = new Command().WithAml(this.Text);
      cmd.Action = CommandAction.DownloadFile;
      foreach (var kvp in context.Parameters)
      {
        cmd.WithParam(kvp.Key, kvp.Value);
      }

      var stream = await context.Connection.Process(cmd, true).ToTask();
      var memStream = await stream.ToMemoryStream();
      var start = await memStream.ReadStart(500);
      memStream.Position = 0;
      if (start.IndexOf("http://www.aras.com/InnovatorFault") > 0)
      {
        context.LastResult = XElement.Load(stream);
      }
      else
      {
        context.LastResult = new XElement("Result", new XElement("Item", Convert.ToBase64String(memStream.ToArray())));
      }
    }

    /// <summary>
    /// Visit this object for the purposes of rendering it to an output
    /// </summary>
    public void Visit(ITestVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
