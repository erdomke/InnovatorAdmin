using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using InnovatorAdmin.Connections;

namespace InnovatorAdmin.Controls
{
  public partial class ConnectionSelection : UserControl, IWizardStep
  {
    private IWizard _wizard;

    public bool MultiSelect
    {
      get { return connEditor.MultiSelect; }
      set { connEditor.MultiSelect = value; }
    }
    public Action GoNextAction { get; set; }

    public ConnectionSelection()
    {
      InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      connEditor.LoadConnectionLibrary(ConnectionManager.Current.Library);
    }

    public void Configure(IWizard wizard)
    {
      _wizard = wizard;
      _wizard.Message = "Select a connection to use";
      _wizard.NextEnabled = connEditor.SelectedConnections.Any();
    }

    public void GoNext()
    {
      ConnectionManager.Current.Save();

      if (!connEditor.SelectedConnections.Any())
      {
        MessageBox.Show(resources.Messages.NoConnectionSelected);
      }
      else
      {
        _wizard.ConnectionInfo = connEditor.SelectedConnections;
        try
        {
          var conn = _wizard.ConnectionInfo.First().ArasLogin();
          _wizard.Connection = conn;
          this.GoNextAction();
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
      }
    }

    private void connEditor_SelectionChanged(object sender, EventArgs e)
    {
      if (_wizard != null) _wizard.NextEnabled = connEditor.SelectedConnections.Any();
    }

    private void connEditor_ConnectionSelected(object sender, EventArgs e)
    {
      try
      {
        this.GoNext();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
