using System;
using System.Windows.Forms;
using Aras.Tools.InnovatorAdmin.Connections;
using System.Collections.Generic;
using System.Linq;
using Innovator.Client;

namespace Aras.Tools.InnovatorAdmin
{
  public partial class ConnectionEditor : UserControl
  {
    private BindingSource _bs = new BindingSource();
    private static int _newConnNumber = 0;


    public event EventHandler SelectionChanged;
    public event EventHandler ConnectionSelected;

    public bool MultiSelect
    {
      get { return lstConnections.Multiselect; }
      set { lstConnections.Multiselect = value; }
    }
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
        btnColor.BackColor = connData.Color;
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
          lstConnections.SetItemSelected(0, true);
      }
    }

    private void btnTest_Click(object sender, EventArgs e)
    {
      try
      {
        lblMessage.Text="Testing...";
        Login((ConnectionData)_bs.Current, true)
          .UiPromise(this)
          .Done(c => lblMessage.Text = "Success")
          .Fail(ex => lblMessage.Text = ex.Message);
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

    public static IAsyncConnection Login(ConnectionData credentials)
    {
      var conn = Factory.GetConnection(credentials.Url, "InnovatorAdmin");
      conn.Login(new ExplicitCredentials(credentials.Database, credentials.UserName, credentials.Password));
      return conn;
    }
    public static IPromise<IAsyncConnection> Login(ConnectionData credentials, bool async)
    {
      return Factory.GetConnection(credentials.Url
        , new ConnectionPreferences() { UserAgent = "InnovatorAdmin" }
        , async)
      .Continue(c =>
      {
        return c.Login(new ExplicitCredentials(credentials.Database, credentials.UserName, credentials.Password), async)
          .Convert(u => (IAsyncConnection)c);
      });
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

    private string _lastDatabaseUrl;

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
            foreach (var db in Factory.GetConnection(_lastDatabaseUrl, "InnovatorAdmin").GetDatabases())
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

    private void lstConnections_SelectionChanged(object sender, EventArgs e)
    {
      OnSelectionChanged(EventArgs.Empty);
    }

    private void lstConnections_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        if (ConnectionSelected != null)
          ConnectionSelected.Invoke(this, e);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnColor_Click(object sender, EventArgs e)
    {
      try
      {
        var connData = _bs.Current as ConnectionData;
        using (var dialog = new ColorDialog())
        {
          dialog.Color = connData.Color;
          if (dialog.ShowDialog(this) == DialogResult.OK)
          {
            connData.Color = dialog.Color;
            btnColor.BackColor = dialog.Color;
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

        private void exploreButton_Click(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "Opening Browser...";
                Application.DoEvents();
                string arasUrl;
                ConnectionData cd = (ConnectionData)_bs.Current;

                arasUrl = cd.Url + "?database=" + cd.Database + "&username=" + cd.UserName + "&password=" + ConnectionData.ScalcMD5(cd.Password);
                System.Diagnostics.Process.Start(arasUrl);
                lblMessage.Text = "";
            }
            catch (Exception ex)
            {
                Utils.HandleError(ex);
            }

        }
    }
}
