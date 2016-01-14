using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using System.Xml.Linq;

namespace InnovatorAdmin.Testing
{
  public class Query : ICommand
  {
    public string Comment { get; set; }
    public string Text { get; set; }

    public async Task Run(TestContext context)
    {
      var cmd = new Command().WithAml(this.Text);
      cmd.Action = CommandAction.ApplyItem;
      if (this.Text.StartsWith("<sql") || this.Text.StartsWith("<SQL"))
        cmd.Action = CommandAction.ApplySQL;
      foreach (var kvp in context.Parameters)
      {
        cmd.WithParam(kvp.Key, kvp.Value);
      }

      var stream = await context.Connection.Process(cmd, true).ToTask();
      context.LastResult = XElement.Load(stream);
    }
  }
}
