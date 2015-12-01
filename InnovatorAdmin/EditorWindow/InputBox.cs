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
  }
}
