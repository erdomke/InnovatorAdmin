using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin.Controls
{
  public enum FlatButtonTheme
  {
    Red,
    LightGray,
    DarkGray
  }
  public class FlatButton : Button
  {
    private FlatButtonTheme _theme = FlatButtonTheme.LightGray;

    public FlatButtonTheme Theme 
    {
      get { return _theme; }
      set
      {
        switch (value)
        {
          case FlatButtonTheme.Red:
            this.BackColor = System.Drawing.Color.FromArgb(204, 0, 51);
            this.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(119, 3, 32);
            this.ForeColor = System.Drawing.Color.White;
            break;
          case FlatButtonTheme.DarkGray:
            break;
          default:
            this.BackColor = System.Drawing.Color.FromArgb(221, 221, 221);
            this.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(196, 196, 196);
            this.ForeColor = System.Drawing.Color.Black;
            break;
        }
        this.FlatAppearance.MouseDownBackColor = this.FlatAppearance.MouseOverBackColor;
        _theme = value;
      }
    }

    public FlatButton()
    {
      this.AutoSize = true;
      this.FlatAppearance.BorderSize = 0;
      this.Padding = new System.Windows.Forms.Padding(2);
      this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.UseVisualStyleBackColor = false;
      this.Theme = FlatButtonTheme.LightGray;
    }
    
    protected override void OnEnabledChanged(EventArgs e)
    {
      base.OnEnabledChanged(e);
      if (this.Enabled)
      {
        this.Theme = _theme;
      }
      else
      {
        switch (_theme)
        {
          case FlatButtonTheme.Red:
            this.BackColor = System.Drawing.Color.FromArgb(234, 186, 198);
            break;
          case FlatButtonTheme.DarkGray:
            break;
          default:
            this.BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
            break;
        }
      }
    }
  }
}
