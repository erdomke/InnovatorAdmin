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
      connectionEditor.ConnectionSelected += connectionEditor_ConnectionSelected;
      this.TopMost = true;
    }

    void connectionEditor_ConnectionSelected(object sender, EventArgs e)
    {
      Accept();
    }

    public void SetSelected(params Connections.ConnectionData[] connections)
    {
      connectionEditor.SelectedConnections = connections;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.TopMost = false;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      Accept();
    }

    private void Accept()
    {
      ConnectionManager.Current.Save();
      this.DialogResult = System.Windows.Forms.DialogResult.OK;
    }


  }
}
