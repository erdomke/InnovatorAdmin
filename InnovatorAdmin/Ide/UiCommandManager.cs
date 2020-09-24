using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public class UiCommandManager
  {
    private List<UiCommand> _commands = new List<UiCommand>();
    private readonly Form _form;

    public UiCommandManager(Form form)
    {
      _form = form;
    }

    public void OnKeyDown(object sender, KeyEventArgs e)
    {
      try
      {
        var cmd = _commands.FirstOrDefault(c => c.KeyFilter != null && c.KeyFilter(e));
        if (cmd != null)
        {
          cmd.Execute();
          e.Handled = true;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    public UiCommandManager Add(UiCommand command)
    {
      _commands.Add(command);
      return this;
    }

    public UiCommandManager Add<T>(ToolStripItem item, Func<KeyEventArgs, bool> keyFilter, Action<T> callback)
    {
      _commands.Add(new UiCommand(item.Text, null).Bind(item, _form, keyFilter, callback));
      return this;
    }
  }
}
