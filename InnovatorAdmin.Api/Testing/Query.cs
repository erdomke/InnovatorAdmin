using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using System.Xml.Linq;

namespace InnovatorAdmin.Testing
{
  /// <summary>
  /// Execute a query
  /// </summary>
  public class Query : ICommand
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
      var elem = XElement.Parse(Text);
      Command cmd;
      var files = elem.DescendantsAndSelf("Item")
        .Where(e => e.Attributes("type").Any(a => a.Value == "File")
                  && e.Elements("actual_filename").Any(p => !string.IsNullOrEmpty(p.Value))
                  && e.Attributes("id").Any(p => !string.IsNullOrEmpty(p.Value))
                  && e.Attributes("action").Any(p => !string.IsNullOrEmpty(p.Value)));
      if (files.Any())
      {
        var upload = context.Connection.CreateUploadCommand();
        upload.AddFileQuery(Text);
        upload.WithAction(CommandAction.ApplyItem);
        cmd = upload;
      }
      else
      {
        cmd = new Command().WithAml(this.Text);
        cmd.Action = CommandAction.ApplyItem;
        if (this.Text.StartsWith("<sql") || this.Text.StartsWith("<SQL"))
          cmd.Action = CommandAction.ApplySQL;
        else if (this.Text.StartsWith("<GetNextSequence"))
          cmd.Action = CommandAction.GetNextSequence;
      }
      foreach (var kvp in context.Parameters)
      {
        cmd.WithParam(kvp.Key, kvp.Value);
      }

      var stream = await context.Connection.Process(cmd, true).ToTask();
      context.LastResult = XElement.Load(stream);
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
