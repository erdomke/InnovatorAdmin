using System;
using System.Windows.Forms;
using Aras.Tools.InnovatorAdmin.Connections;
using System.Collections.Generic;
using System.Linq;

namespace Aras.Tools.InnovatorAdmin
{
  public partial class ConnectionEditor : UserControl
  {
    private BindingSource _bs = new BindingSource();
    private static int _newConnNumber = 0;


    public event EventHandler SelectionChanged;

    public bool MultiSelect { get; set; }
    public IEnumerable<ConnectionData> SelectedConnections
    {
      get
      {
        return lstConnections.Selected.OfType<ConnectionData>();
      }
      set
      {
        lstConnections.SetSelection(value.ToArray());
      }
    }

    public ConnectionEditor()
    {
      InitializeComponent();
      this.MultiSelect = false;
      _bs.CurrentChanged += _bs_CurrentChanged;

      cboIomVersion.Items.Add(string.Empty);
      foreach (var version in Iom.Versions())
      {
        cboIomVersion.Items.Add(version);
      }
    }

    void _bs_CurrentChanged(object sender, EventArgs e)
    {
      try
      {
        var connData = _bs.Current as ConnectionData;
        if (connData != null && txtUrl.Text != _lastDatabaseUrl && !string.IsNullOrEmpty(connData.Database))
        {
          cmbDatabase.Items.Clear();
          cmbDatabase.Items.Add(connData.Database);
          cmbDatabase.SelectedIndex = 0;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
    protected virtual void OnSelectionChanged(EventArgs e)
    {
      if (SelectionChanged != null)
      {
        SelectionChanged(this, e);
      }
    }

    public void LoadConnectionLibrary(ConnectionLibrary library)
    {
      _bs.DataSource = library.Connections;
      if (!DesignMode)
      {
        lstConnections.DisplayMember = "ConnectionName";
        lstConnections.DataSource = _bs;

        txtName.DataBindings.Add("Text", _bs, "ConnectionName");
        cmbDatabase.DataBindings.Add("Text", _bs, "Database");
        txtPassword.DataBindings.Add("Text", _bs, "Password");
        txtUrl.DataBindings.Add("Text", _bs, "Url");
        txtUser.DataBindings.Add("Text", _bs, "UserName");
        cboIomVersion.DataBindings.Add("Text", _bs, "IomVersion");

        if (lstConnections.Items.Count > 0 && !this.MultiSelect)
          lstConnections.SetItemSelected(0, true);
      }
    }

    private void btnTest_Click(object sender, EventArgs e)
    {
      try
      {
        lblMessage.Text="Testing...";
        Application.DoEvents();
        string msg;
        var inn = Login((ConnectionData)_bs.Current, out msg);
        lblMessage.Text = msg;
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void ClearMessage()
    {
      lblMessage.Text="";
    }

    public static IArasConnection Login(ConnectionData credentials, out string messageText)
    {
      messageText = "";
      if (string.IsNullOrEmpty(credentials.Url))
      {
        messageText = resources.Messages.UrlNotSpecified;
        return null;
      }

      return Iom.GetFactory(credentials.IomVersion).Login(credentials.Url, credentials.Database, credentials.UserName, credentials.Password, out messageText);
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      try
      {
        ClearMessage();
        _bs.Add(new ConnectionData()
        {
          ConnectionName = "New Connection " + _newConnNumber++
        });
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      try
      {
        ClearMessage();
        _bs.RemoveCurrent();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnCopy_Click(object sender, EventArgs e)
    {
      try
      {
        ClearMessage();
        _bs.Add(((ConnectionData)_bs.Current).Clone());
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnMoveUp_Click(object sender, EventArgs e)
    {
      try
      {
        var pos = _bs.Position;
        if (pos > 0)
        {
          var curr = _bs.Current;
          _bs.RemoveAt(pos);
          _bs.Insert(pos - 1, curr);
          _bs.Position = pos - 1;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnMoveDown_Click(object sender, EventArgs e)
    {
      try
      {
        var pos = _bs.Position;
        if (pos < (_bs.Count - 1))
        {
          var curr = _bs.Current;
          _bs.RemoveAt(pos);
          _bs.Insert(pos + 1, curr);
          _bs.Position = pos + 1;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    //private void lstConnections_ItemCheck(object sender, ItemCheckEventArgs e)
    //{
    //  try
    //  {
    //    _itemCheckChanged = true;
    //    if (e.NewValue == CheckState.Checked && !this.MultiSelect)
    //    {
    //      try
    //      {
    //        clstConnections.ItemCheck -= lstConnections_ItemCheck;
    //        for (int i = 0; i < clstConnections.Items.Count; i++)
    //        {
    //          if (i != e.Index) clstConnections.SetItemCheckState(i, CheckState.Unchecked);
    //        }
    //      }
    //      finally
    //      {
    //        clstConnections.ItemCheck += lstConnections_ItemCheck;
    //      }
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    Utils.HandleError(ex);
    //  }
    //}

    private string _lastDatabaseUrl;
    private bool _itemCheckChanged;

    private void cmbDatabase_DropDown(object sender, EventArgs e)
    {
      try
      {
        if (txtUrl.Text != _lastDatabaseUrl)
        {
          var selected = (cmbDatabase.Items.Count > 0 ? cmbDatabase.SelectedItem : null);

          _lastDatabaseUrl = txtUrl.Text;
          cmbDatabase.Items.Clear();

          //get dbs from test connection
          try
          {
            foreach (var db in Iom.GetFactory(null).AvailableDatabases(txtUrl.Text))
            {
              cmbDatabase.Items.Add(db);
            }

            if (selected != null) cmbDatabase.SelectedItem = selected;
          }
          catch (Exception err)
          {
            Utils.HandleError(err);
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void lstConnections_MouseUp(object sender, MouseEventArgs e)
    {
      try
      {
        if (_itemCheckChanged) OnSelectionChanged(EventArgs.Empty);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void lstConnections_MouseDown(object sender, MouseEventArgs e)
    {
      try
      {
        _itemCheckChanged = false;
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

  }
}
