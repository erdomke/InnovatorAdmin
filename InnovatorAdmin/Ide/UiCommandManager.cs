using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public class UiCommandManager
  {
    private List<IUiCommand> _commands = new List<IUiCommand>();
    private Form _form;

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

    public UiCommandManager Add<T>(ToolStripItem item, Func<KeyEventArgs, bool> keyFilter, Action<T> callback)
    {
      _commands.Add(new ToolStripCommand<T>(item, _form)
      {
        KeyFilter = keyFilter,
        Callback = callback
      });
      return this;
    }

    private interface IUiCommand : IDisposable
    {
      Func<KeyEventArgs, bool> KeyFilter { get; }
      void Execute();
    }

    private class ToolStripCommand<T> : IUiCommand
    {
      private ToolStripItem _item;
      private Form _form;

      public Func<KeyEventArgs, bool> KeyFilter { get; set; }
      public Action<T> Callback { get; set; }

      public ToolStripCommand(ToolStripItem item, Form form)
      {
        _item = item;
        _form = form;
        _item.Click += OnClick;
      }

      public void Execute()
      {
        var ctrl = _form.FindFocusedControl().ParentsAndSelf().OfType<T>().FirstOrDefault();
        if (ctrl != null)
        {
          this.Callback.Invoke(ctrl);
        }
      }

      private void OnClick(object sender, EventArgs e)
      {
        try
        {
          this.Execute();
        }
        catch (Exception ex)
        {
          Utils.HandleError(ex);
        }
      }

      public void Dispose()
      {
        _item.Click -= OnClick;
      }
    }
  }
}
