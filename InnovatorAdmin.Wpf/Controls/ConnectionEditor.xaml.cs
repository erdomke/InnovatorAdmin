using Innovator.Client;
using InnovatorAdmin.Connections;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InnovatorAdmin.Controls
{
  /// <summary>
  /// Interaction logic for ConnectionEditor.xaml
  /// </summary>
  public partial class ConnectionEditor : UserControl
  {
    private string _lastDatabaseUrl;

    public ConnectionEditor()
    {
      InitializeComponent();

      this.DataContext = ConnectionManager.Current.Library;
    }

    private void lstConnections_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var conn = e.AddedItems.OfType<Connections.ConnectionData>().First();
      txtPassword.Password = conn.Password.UseString((ref string p) => new string(p.ToCharArray()));
      if (conn.Url != _lastDatabaseUrl
          && !string.IsNullOrEmpty(conn.Database))
      {
        var db = conn.Database;
        _lastDatabaseUrl = null;
        cboDatabase.Items.Clear();
        cboDatabase.Items.Add(db);
        cboDatabase.SelectedIndex = 0;
      }
    }

    private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
      var conn = (ConnectionData)lstConnections.SelectedItem;
      conn.Password = txtPassword.SecurePassword;
    }

    private void cboDatabase_DropDownOpened(object sender, EventArgs e)
    {
      if (txtUrl.Text == _lastDatabaseUrl)
        return;

      try
      {
        var selected = (cboDatabase.Items.Count > 0 ? cboDatabase.SelectedItem : null);
        var data = (ConnectionData)lstConnections.SelectedItem;

        _lastDatabaseUrl = data.Url;
        cboDatabase.Items.Clear();

        switch (data.Type)
        {
          case ConnectionType.Innovator:
            foreach (var db in Factory.GetConnection(_lastDatabaseUrl, "InnovatorAdmin").GetDatabases())
            {
              cboDatabase.Items.Add(db);
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
                    cboDatabase.Items.Add(dr[0].ToString());
                  }
                }
              }
            }
            break;
        }

        if (selected != null) cboDatabase.SelectedItem = selected;
      }
      catch (Exception err)
      {
        Utils.HandleError(err);
      }
    }
  }
}
