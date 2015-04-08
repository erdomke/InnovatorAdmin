using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aras.Tools.InnovatorAdmin
{
  public static class Utils
  {
    public static void HandleError(Exception ex)
    {
      MessageBox.Show(ex.Message);
    }

    public static IEnumerable<Item> AsEnum(this Item item)
    {
      for (var i = 0; i < item.getItemCount(); i++)
      {
        yield return item.getItemByIndex(i);
      }
    }
    public static void UiThreadInvoke(this Control control, Action code)
    {
      if (control.InvokeRequired && control.Parent != null)
      {
        control.Invoke(code);
      }
      else
      {
        code.Invoke();
      }
    }
  }
}
