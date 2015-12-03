using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin
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
