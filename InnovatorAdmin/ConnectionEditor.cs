using System;
using System.Windows.Forms;
using Aras.Tools.InnovatorAdmin.Connections;
using Aras.IOM;
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
        return lstConnections.CheckedItems.OfType<ConnectionData>();
      }
    }
    
    public ConnectionEditor()
    {
      InitializeComponent();
      lstConnections.CheckOnClick = true;
      this.MultiSelect = false;
      _bs.CurrentChanged += _bs_CurrentChanged;
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

        if (lstConnections.Items.Count > 0 && !this.MultiSelect)
          lstConnections.SetItemChecked(0, true);
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

    public static Innovator Login(ConnectionData credentials, out string messageText)
    {
      messageText = "";
      if (string.IsNullOrEmpty(credentials.Url))
      {
        messageText = resources.Messages.UrlNotSpecified;
        return null;
      }

      var conn = IomFactory.CreateHttpServerConnection(credentials.Url, credentials.Database, credentials.UserName, credentials.Password);
      var loginResult = conn.Login();
      if (loginResult.isError())
      {
        //get details of error
        var errorStr = loginResult.getErrorString();

        //Interpret message string  - remove header text before : symbol
        var pos = errorStr.IndexOf(':') + 1;
        if (pos > 0) errorStr = errorStr.Substring(pos);

        //If error contains keyword clean up message text
        if (errorStr.Contains("Authentication")) errorStr = resources.Messages.InvalidCredentials;
        if (errorStr.Contains("Database")) errorStr = resources.Messages.DatabaseUnavailable;

        messageText = string.Format(resources.Messages.LoginFailed, errorStr);
        return null;
      }

      messageText = resources.Messages.LoginSuccess;
      return IomFactory.CreateInnovator(conn);
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

    private void lstConnections_ItemCheck(object sender, ItemCheckEventArgs e)
    {
      try
      {
        _itemCheckChanged = true;
        if (e.NewValue == CheckState.Checked && !this.MultiSelect)
        {
          try
          {
            lstConnections.ItemCheck -= lstConnections_ItemCheck;
            for (int i = 0; i < lstConnections.Items.Count; i++)
            {
              if (i != e.Index) lstConnections.SetItemCheckState(i, CheckState.Unchecked);
            }
          }
          finally
          {
            lstConnections.ItemCheck += lstConnections_ItemCheck;
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

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
            var conn = IomFactory.CreateHttpServerConnection(txtUrl.Text, cmbDatabase.Text, txtUser.Text, txtPassword.Text);
            string[] databases = conn.GetDatabases();
            for (int i = 0; i < databases.Length; i++)
            {
              cmbDatabase.Items.Add(databases[i]);
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
