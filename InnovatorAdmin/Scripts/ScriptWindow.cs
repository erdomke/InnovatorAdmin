using Innovator.Client;
using InnovatorAdmin.Connections;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin.Scripts
{
  public partial class ScriptWindow : Form
  {
    private IAsyncScript _script;
    private ConnectionData _connData;
    private Task<IAsyncConnection> _conn;
    private IPromise<string> _currentRun;

    public ScriptWindow()
    {
      InitializeComponent();

      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;

      menuStrip.Renderer = new SimpleToolstripRenderer();
    }

    public async Task SetConnection(ConnectionData connData)
    {
      _conn = null;
      _connData = null;
      btnEditConnection.Text = "No Connection ▼";
      lblConnColor.BackColor = Color.Transparent;

      if (connData.Type != ConnectionType.Innovator)
      {
        Dialog.MessageDialog.Show("Only Innovator connections are supported");
        return;
      }

      _connData = connData;
      btnEditConnection.Text = "Connecting... ▼";
      _conn = connData.ArasLogin(true);
      await _conn;
      btnEditConnection.Text = connData.ConnectionName + " ▼";
      lblConnColor.BackColor = connData.Color;
    }

    public ScriptWindow WithScript(IAsyncScript script)
    {
      _script = script;
      lblScriptName.Text = ScriptManager.Instance.GetName(script);
      propGrid.SelectedObject = _script;
      return this;
    }

    private void mniClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void mniOtherScripts_Click(object sender, EventArgs e)
    {
      try
      {
        var script = Scripts.ScriptManager.Instance.PromptForScript(this
          , this.RectangleToScreen(menuStrip.Bounds));
        if (script != null)
          new Scripts.ScriptWindow().WithScript(script).Show();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private async void btnSubmit_Click(object sender, EventArgs e)
    {
      try
      {
        if (_currentRun == null)
        {
          if (_conn == null)
          {
            outputEditor.Text = "Please select a connection first";
            return;
          }

          btnSubmit.Text = "► Cancel";
          outputEditor.Text = "Processing...";

          var conn = await _conn;
          _currentRun = _script.Execute(conn);
          outputEditor.Text = await _currentRun;
          _currentRun = null;
        }
        else
        {
          btnSubmit.Text = "► Run";
          _currentRun.Cancel();
          _currentRun = null;
        }
      }
      catch (Exception ex)
      {
        outputEditor.Text = ex.ToString();
      }
    }

    private void btnEditConnection_Click(object sender, EventArgs e)
    {
      using (var dialog = new ConnectionEditorForm())
      {
        dialog.Multiselect = false;
        if (_connData != null)
          dialog.SetSelected(_connData);
        if (dialog.ShowDialog(this, menuStrip.RectangleToScreen(btnEditConnection.Bounds)) ==
          System.Windows.Forms.DialogResult.OK)
        {
          SetConnection(dialog.SelectedConnections.First());
        }
      }
    }
  }
}
