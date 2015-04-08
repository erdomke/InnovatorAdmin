using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aras.Tools.InnovatorAdmin
{
  public partial class ConnectionEditorForm : Form
  {
    public bool Multiselect
    {
      get { return connectionEditor.MultiSelect; }
      set { connectionEditor.MultiSelect = value; }
    }
    public IEnumerable<Connections.ConnectionData> SelectedConnections
    {
      get
      {
        return connectionEditor.SelectedConnections;
      }
    }

    public ConnectionEditorForm()
    {
      InitializeComponent();
      connectionEditor.LoadConnectionLibrary(ConnectionManager.Current.Library);
      this.TopMost = true;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.TopMost = false;
    }

  }
}
