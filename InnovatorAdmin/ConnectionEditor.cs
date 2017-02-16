using System;
using System.Windows.Forms;
using InnovatorAdmin.Connections;
using System.Collections.Generic;
using System.Linq;
using Innovator.Client;
using System.Data.SqlClient;
using System.Drawing;

namespace InnovatorAdmin
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

      var iconFont = new Font(FontAwesome.Family, 12.0F);
      btnNew.Font = iconFont;
      btnNew.Text = FontAwesome.Fa_plus_circle.ToString();
      btnDelete.Font = iconFont;
      btnDelete.Text = FontAwesome.Fa_minus_circle.ToString();
      btnCopy.Font = iconFont;
      btnCopy.Text = FontAwesome.Fa_copy.ToString();
      btnMoveDown.Font = iconFont;
      btnMoveDown.Text = FontAwesome.Fa_arrow_down.ToString();
      btnMoveUp.Font = iconFont;
      btnMoveUp.Text = FontAwesome.Fa_arrow_up.ToString();

      lblUrl.Font = FontAwesome.Font;
      lblUrl.Text = FontAwesome.Fa_globe.ToString();
      lblUser.Font = FontAwesome.Font;
      lblUser.Text = FontAwesome.Fa_user.ToString();
      lblPassword.Font = FontAwesome.Font;
      lblPassword.Text = FontAwesome.Fa_key.ToString();
      lblName.Font = FontAwesome.Font;
      lblName.Text = FontAwesome.Fa_bookmark.ToString();
      lblDatabase.Font = FontAwesome.Font;
      lblDatabase.Text = FontAwesome.Fa_database.ToString();
      lblType.Font = FontAwesome.Font;
      lblType.Text = FontAwesome.Fa_tag.ToString();

      this.MultiSelect = false;
      _bs.CurrentChanged += _bs_CurrentChanged;
      cmbType.DataSource = Enum.GetValues(typeof(ConnectionType));
    }

    public void InitializeFocus()
    {
      this.ActiveControl = lstConnections;
      lstConnections.InitializeFocus();
    }

    void _bs_CurrentChanged(object sender, EventArgs e)
    {
      try
      {
        var connData = _bs.Current as ConnectionData;
        if (connData != null)
        {
          if (connData.Url != _lastDatabaseUrl
          && !string.IsNullOrEmpty(connData.Database))
          {
            _lastDatabaseUrl = null;
            cmbDatabase.Items.Clear();
            cmbDatabase.Items.Add(connData.Database);
            cmbDatabase.SelectedIndex = 0;
          }
          SetAuthentication(connData.Authentication);
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
        cmbType.DataBindings.Add("SelectedItem", _bs, "Type");

        if (lstConnections.Items.Count > 0 && !this.MultiSelect)
          lstConnections.SetItemSelected(0, true);
      }
    }

    private void btnTest_Click(object sender, EventArgs e)
    {
      try
      {
        btnTest.Text = "Testing...";


        var connData = (ConnectionData)_bs.Current;
        switch (connData.Type)
        {
          case ConnectionType.Innovator:
            ((ConnectionData)_bs.Current).ArasLogin(true)
              .UiPromise(this)
              .Done(c => btnTest.Text = "Success. Test Again.")
              .Fail(ex => btnTest.Text = ex.Message);
            break;
          case ConnectionType.SqlServer:
            try
            {
              using (var conn = Editor.SqlEditorProxy.GetConnection(connData, "master"))
              {
                conn.Open();
              }
              btnTest.Text = "Success. Test Again.";
            }
            catch (Exception ex)
            {
              btnTest.Text = ex.Message;
            }
            break;
          default:
            ProxyFactory.FromConn(connData)
              .UiPromise(this)
              .Done(c => btnTest.Text = "Success. Test Again.")
              .Fail(ex => btnTest.Text = ex.Message);
            break;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void ClearMessage()
    {
      btnTest.Text = "Test";
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
      if (txtUrl.Text == _lastDatabaseUrl)
        return;

      try
      {
        var selected = (cmbDatabase.Items.Count > 0 ? cmbDatabase.SelectedItem : null);
        var data = (ConnectionData)_bs.Current;

        _lastDatabaseUrl = data.Url;
        cmbDatabase.Items.Clear();

        switch (data.Type)
        {
          case ConnectionType.Innovator:
            foreach (var db in Factory.GetConnection(_lastDatabaseUrl, "InnovatorAdmin").GetDatabases())
            {
              cmbDatabase.Items.Add(db);
            }
            break;
          case ConnectionType.SqlServer:
            using (var conn = Editor.SqlEditorProxy.GetConnection(data, "master"))
            {
              conn.Open();
              // Set up a command with the given query and associate
              // this with the current connection.
              using (var cmd = new SqlCommand("SELECT name from sys.databases order by name", conn))
              {
                using (var dr = cmd.ExecuteReader())
                {
                  while (dr.Read())
                  {
                    cmbDatabase.Items.Add(dr[0].ToString());
                  }
                }
              }
            }
            break;
        }

        if (selected != null) cmbDatabase.SelectedItem = selected;
      }
      catch (Exception err)
      {
        Utils.HandleError(err);
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
        if (ConnectionSelected != null && !lstConnections.Multiselect)
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
        using (var dialog = new Dialog.ColorDialog())
        {
          if (dialog.ShowDialog(this) == DialogResult.OK)
          {
            connData.Color = dialog.SelectedColor;
            btnColor.BackColor = dialog.SelectedColor;
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
        exploreButton.Text = "Opening Browser...";
        Application.DoEvents();
        ((ConnectionData)_bs.Current).Explore();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
      finally
      {
        exploreButton.Text = "Explore";
      }
    }

    private bool _programChangingCheck;

    private void chkPassword_CheckedChanged(object sender, EventArgs e)
    {
      try
      {
        SetAuthentication(Connections.Authentication.Explicit);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void chkWindows_CheckedChanged(object sender, EventArgs e)
    {
      try
      {
        SetAuthentication(Connections.Authentication.Windows);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void chkAnonymous_CheckedChanged(object sender, EventArgs e)
    {
      try
      {
        SetAuthentication(Connections.Authentication.Anonymous);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void SetAuthentication(Connections.Authentication value)
    {
      if (_programChangingCheck) return;

      try
      {
        _programChangingCheck = true;
        ((ConnectionData)_bs.Current).Authentication = value;
        chkAnonymous.Checked = false;
        chkPassword.Checked = false;
        chkWindows.Checked = false;
        switch (value)
        {
          case Connections.Authentication.Anonymous:
            chkAnonymous.Checked = true;
            break;
          case Connections.Authentication.Windows:
            chkWindows.Checked = true;
            break;
          default:
            chkPassword.Checked = true;
            break;
        }
        txtUser.Enabled = (value == Connections.Authentication.Explicit);
        txtPassword.Enabled = txtUser.Enabled;
      }
      finally
      {
        _programChangingCheck = false;
      }
    }

    private void btnAdvanced_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new Dialog.ConnectionAdvancedDialog())
        {
          dialog.ConnData = (Connections.ConnectionData)_bs.Current;
          dialog.ShowDialog();
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void txtName_Validated(object sender, EventArgs e)
    {
      try
      {
        _bs.ResetBindings(false);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }


  }
}
