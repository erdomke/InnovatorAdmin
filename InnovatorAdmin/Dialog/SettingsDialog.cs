using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Dialog
{
  public class SettingsDialog : ConfigDialog<Settings>
  {
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (this.DataSource == null)
      {
        this.DataSource = Settings.Current;
      }
    }

    protected override void OnOk()
    {
      base.OnOk();
      this.DataSource.Save();
    }
  }
}
