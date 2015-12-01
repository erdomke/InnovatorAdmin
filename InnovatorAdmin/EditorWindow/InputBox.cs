using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aras.Tools.InnovatorAdmin
{
  public partial class InputBox : Form
  {
    public string Caption
    {
      get { return this.Text; }
      set { this.Text = value; }
    }
    public string Message
    {
      get { return lblMessage.Text; }
      set { lblMessage.Text = value; }
    }
    public string Value
    {
      get { return txtInput.Text; }
      set { txtInput.Text = value; }
    }

    public InputBox()
    {
      InitializeComponent();

      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;
    }

    public DialogResult ShowDialog(IWin32Window owner, Rectangle bounds)
    {
      this.StartPosition = FormStartPosition.Manual;
      var screenDim = SystemInformation.VirtualScreen;
      var newX = Math.Min(Math.Max(bounds.X, 0), screenDim.Width - this.DesktopBounds.Width);
      var newY = Math.Min(Math.Max(bounds.Y - 30, 0), screenDim.Height - this.DesktopBounds.Height);
      this.DesktopLocation = new Point(newX, newY);
      return this.ShowDialog(owner);
    }
  }
}
