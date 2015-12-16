using InnovatorAdmin.Connections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Linq;
using System.Diagnostics;
using Innovator.Client;
using InnovatorAdmin.Controls;
using System.Globalization;

namespace InnovatorAdmin
{
  public partial class EditorWindow : Form
  {
    private bool _panelCollapsed;
    private bool _autoTabSelect = true;
    private bool _loadingConnection = false;
    private bool _disposeProxy = true;
    private IEditorProxy _proxy;
    private IResultObject _result;
    private DataTable _outputTable;
    private string _soapAction;
    private Dictionary<string, QueryParameter> _paramCache = new Dictionary<string, QueryParameter>();
    private IPromise _currentQuery;
    private string _locale;
    private string _timeZone;
    private int _timeout = Innovator.Client.Connection.DefaultHttpService.DefaultTimeout;
    private Timer _clock = new Timer();
    private DateTime _start = DateTime.UtcNow;

    public bool AllowRun
    {
      get { return !splitEditors.Panel2Collapsed; }
      set { splitEditors.Panel2Collapsed  = !value; }
    }
    public string Script
    {
      get { return inputEditor.Text; }
      set { inputEditor.Text = value; }
    }
    public IEditorProxy Proxy
    {
      get { return _proxy; }
      set { SetConnection(value); }
    }
    public string SoapAction
    {
      get { return _soapAction; }
      set
      {
        _soapAction = value;
        if (_proxy != null) _proxy.Action = value;
        btnSoapAction.Text = value + " ▼";
      }
    }

    public EditorWindow()
    {
      InitializeComponent();

      inputEditor.Helper = new Editor.AmlEditorHelper();
      var lastQuery = SnippetManager.Instance.LastQuery;
      inputEditor.Text = lastQuery.Text;
      this.SoapAction = lastQuery.Action;
      menuStrip.Renderer = new SimpleToolstripRenderer();

      treeItems.CanExpandGetter = m => ((IEditorTreeNode)m).HasChildren;
      treeItems.ChildrenGetter = m => ((IEditorTreeNode)m).GetChildren();
      colName.ImageGetter = m => ((IEditorTreeNode)m).ImageKey;

      treeItems.SmallImageList.Images.Add("class-16", InnovatorAdmin.Ide.TreeImages.class_16);
      treeItems.SmallImageList.Images.Add("folder-16", InnovatorAdmin.Ide.TreeImages.folder_16);
      treeItems.SmallImageList.Images.Add("folder-special-16", InnovatorAdmin.Ide.TreeImages.folder_special_16);
      treeItems.SmallImageList.Images.Add("property-16", InnovatorAdmin.Ide.TreeImages.property_16);
      treeItems.SmallImageList.Images.Add("xml-tag-16", InnovatorAdmin.Ide.TreeImages.xml_tag_16);

      _clock.Interval = 250;
      _clock.Tick += _clock_Tick;
      _panelCollapsed = Properties.Settings.Default.EditorWindowPanelCollapsed;
      UpdatePanelCollapsed();
    }

    void _clock_Tick(object sender, EventArgs e)
    {
      lblItems.Text = string.Format(@"Processing... {0:hh\:mm\:ss}", DateTime.UtcNow - _start);
    }

    public void SetConnection(IAsyncConnection conn, string name = null)
    {
      if (conn == null) throw new ArgumentNullException("conn");
      exploreButton.Visible = true;
      mniLocale.Visible = true;
      mniTimeZone.Visible = true;
      SetConnection(new ArasEditorProxy(conn, name ?? conn.Database));
      _disposeProxy = false;
    }
    public void SetConnection(ConnectionData conn)
    {
      if (!_loadingConnection)
      {
        _loadingConnection = true;

        try
        {

          btnEditConnections.Text = "Connecting... ▼";
          exploreButton.Visible = conn.Type == ConnectionType.Innovator;
          mniLocale.Visible = exploreButton.Visible;
          mniTimeZone.Visible = exploreButton.Visible;
          ProxyFactory.FromConn(conn)
            .UiPromise(this)
            .Done(proxy =>
            {
              SetConnection(proxy);
              _disposeProxy = true;
            })
            .Fail(ex =>
            {
              lblItems.Text = ex.Message;
              btnEditConnections.Text = "Not Connected ▼";
              lblConnection.Visible = true;
              btnEditConnections.Visible = lblConnection.Visible;
            }).Always(() => _loadingConnection = false);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
      }
    }

    private void DisposeProxy()
    {
      if (_proxy != null)
      {
        var arasProxy = _proxy as ArasEditorProxy;
        if (arasProxy != null)
        {
          var remote = arasProxy.Connection as IRemoteConnection;
          if (remote != null)
            remote.DefaultSettings(r => { });
        }
        if (_disposeProxy) _proxy.Dispose();
      }
    }

    public void SetConnection(IEditorProxy proxy)
    {
      DisposeProxy();

      _proxy = proxy;

      if (proxy == null)
        return;

      if (_proxy.GetActions().Any())
      {
        _proxy.Action = _soapAction;
        btnSoapAction.Visible = true;
      }
      else
      {
        btnSoapAction.Visible = true;
      }
      lblSoapAction.Visible = btnSoapAction.Visible;

      inputEditor.Helper = _proxy.GetHelper();
      outputEditor.Helper = _proxy.GetHelper();
      btnEditConnections.Text = string.Format("{0} ▼", _proxy.Name);

      if (proxy.ConnData != null)
      {
        lblConnection.Visible = true;
        btnEditConnections.Visible = lblConnection.Visible;
        lblConnColor.BackColor = proxy.ConnData.Color;
      }
      InitializeUi(_proxy as ArasEditorProxy);
      treeItems.Roots = null;
      treeItems.RebuildAll(false);
      _proxy.GetNodes()
        .UiPromise(this)
        .Done(r =>
        {
          treeItems.Roots = r;
        });
    }
    private void InitializeUi(ArasEditorProxy proxy)
    {
      if (proxy == null || proxy.Connection.AmlContext == null)
        return;

      var local = proxy.Connection.AmlContext.LocalizationContext;
      var remote = proxy.Connection as IRemoteConnection;
      _locale = local.Locale;
      _timeZone = local.TimeZone;
      mniLocale.ShortcutKeyDisplayString = "(" + _locale + ")";
      mniTimeZone.ShortcutKeyDisplayString = "(" + _timeZone + ")";
      mniTimeout.ShortcutKeyDisplayString = "(" + (_timeout / 1000) + "s)";

      mniLocale.Enabled = remote != null;
      mniTimeZone.Enabled = mniLocale.Enabled;
      mniTimeout.Visible = mniLocale.Enabled;

      if (remote != null)
      {
        remote.DefaultSettings(ConfigureRequest);
      }
    }

    private void ConfigureRequest(IHttpRequest req)
    {
      req.SetHeader("LOCALE", _locale);
      req.SetHeader("TIMEZONE_NAME", _timeZone);
      req.Timeout = _timeout;
    }

    protected override void OnLoad(EventArgs e)
    {
      try
      {
        base.OnLoad(e);
        btnOk.Visible = this.Modal;
        btnCancel.Visible = this.Modal;

        lblConnection.Visible = _proxy != null && _proxy.ConnData != null;
        btnEditConnections.Visible = lblConnection.Visible;

        if (_proxy == null)
        {
          var conn = ConnectionManager.Current.Library.Connections
            .FirstOrDefault(c => c.ConnectionName == Properties.Settings.Default.LastConnection)
            ?? ConnectionManager.Current.Library.Connections.FirstOrDefault();
          if (conn != null)
            SetConnection(conn);
        }

        var bounds = Properties.Settings.Default.EditorWindow_Bounds;
        if (bounds.Width < 100 || bounds.Height < 100)
        {
          // Do nothing
        }
        else if (bounds != Rectangle.Empty && bounds.IntersectsWith(SystemInformation.VirtualScreen))
        {
          this.DesktopBounds = bounds;
        }
        else
        {
          this.Size = bounds.Size;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void EnsureDataTable()
    {
      if (_result != null && tbcOutputView.SelectedTab == pgTableOutput)
      {
        _outputTable = _result.GetTable();
        dgvItems.DataSource = _outputTable;
        FormatDataGrid();

        dgvItems.AllowUserToAddRows = _outputTable != null
                && _outputTable.Columns.Contains("id")
                && _outputTable.Columns.Contains("type");
        dgvItems.AllowUserToDeleteRows = dgvItems.AllowUserToAddRows;
        dgvItems.ReadOnly = !dgvItems.AllowUserToAddRows;
      }
    }

    private void FormatDataGrid()
    {
      dgvItems.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader);
      var minWidths = dgvItems.Columns.OfType<DataGridViewColumn>().Select(c => c.Width).ToArray();
      dgvItems.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
      var maxWidths = dgvItems.Columns.OfType<DataGridViewColumn>().Select(c => c.Width).ToArray();
      var maxWidth = (int)(dgvItems.Width * 0.8);

      for (var i = 0; i < dgvItems.Columns.Count; i++)
      {
        dgvItems.Columns[i].Width = Math.Min(maxWidths[i] < 100 ? maxWidths[i] :
            (maxWidths[i] < minWidths[i] + 60 ? maxWidths[i] : minWidths[i] + 60)
          , maxWidth);
        dgvItems.Columns[i].DefaultCellStyle.Alignment =
          (IsNumericType(dgvItems.Columns[i].ValueType)
            ? DataGridViewContentAlignment.TopRight
            : DataGridViewContentAlignment.TopLeft);
        //dgvItems.Columns[i].nul
      }
    }
    private bool IsNumericType(Type type)
    {
      return type == typeof(byte)
        || type == typeof(short) || type == typeof(ushort)
        || type == typeof(int) || type == typeof(uint)
        || type == typeof(long) || type == typeof(ulong)
        || type == typeof(float) || type == typeof(double)
        || type == typeof(decimal);
    }

    #region Run Query
    private void btnSubmit_Click(object sender, System.EventArgs e)
    {
      try
      {
        Submit(inputEditor.Text);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }


    private void mniRunAll_Click(object sender, EventArgs e)
    {
      try
      {
        Submit(inputEditor.Editor.Text);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniRunCurrent_Click(object sender, EventArgs e)
    {
      try
      {
        Submit(inputEditor.Helper.GetCurrentQuery(inputEditor.Text, inputEditor.Editor.CaretOffset)
          ?? inputEditor.Text);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }


    private void mniRunCurrentNewWindow_Click(object sender, EventArgs e)
    {
      try
      {
        var window = NewWindow();
        var query = inputEditor.Helper.GetCurrentQuery(inputEditor.Text, inputEditor.Editor.CaretOffset)
          ?? inputEditor.Text;
        window.inputEditor.Text = query;
        window.SoapAction = this.SoapAction;
        window.Submit(query);
        window.Show();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private EditorWindow NewWindow()
    {
      var window = new EditorWindow();
      window.SetConnection(_proxy.Clone());
      window._disposeProxy = false;
      return window;
    }

    private void Submit(string query)
    {
      if (_currentQuery != null)
      {
        _currentQuery.Cancel();
        _currentQuery = null;
        btnSubmit.Text = "► Run";
        return;
      }

      try
      {
        outputEditor.Text = "Processing...";
        lblItems.Text = "Processing...";
        _start = DateTime.UtcNow;
        _clock.Enabled = true;
        btnSubmit.Text = "Cancel";

        var cmd = _proxy.NewCommand().WithQuery(query).WithAction(this.SoapAction);
        var queryParams = _proxy.GetHelper().GetParameterNames(query)
          .Select(p => GetCreateParameter(p)).ToList();
        if (queryParams.Any())
        {
          using (var dialog = new ParameterWindow(queryParams))
          {
            switch (dialog.ShowDialog(this))
            {
              case System.Windows.Forms.DialogResult.OK:
                foreach (var param in queryParams)
                {
                  cmd.WithParam(param.Name, param.GetValue());
                }
                break;
              case System.Windows.Forms.DialogResult.Ignore:
                break;
              default:
                return;
            }
          }
        }

        _result = null;
        _outputTable = null;
        dgvItems.DataSource = null;

        SnippetManager.Instance.LastQuery = new Snippet()
        {
          Action = this.SoapAction,
          Text = inputEditor.Text
        };
        Properties.Settings.Default.LastConnection = _proxy.ConnData.ConnectionName;
        Properties.Settings.Default.Save();

        var st = Stopwatch.StartNew();
        _currentQuery = _proxy.Process(cmd, true)
          .UiPromise(this)
          .Done(result =>
          {
            try
            {
              var milliseconds = st.ElapsedMilliseconds;

              _result = result;
              _clock.Enabled = false;
              if (result.ItemCount > 0)
              {
                lblItems.Text = string.Format("{0} item(s) found in {1} ms.", result.ItemCount, milliseconds);
              }
              else
              {
                lblItems.Text = string.Format("No items found in {0} ms.", milliseconds);
              }
              outputEditor.Text = result.GetText();

              if (result.ItemCount > 1 && outputEditor.Editor.LineCount > 100)
              {
                outputEditor.CollapseAll();
              }

              if (_autoTabSelect) tbcOutputView.SelectedTab = result.PreferTable ? pgTableOutput : pgTextOutput;
              EnsureDataTable();
            }
            catch (Exception ex)
            {
              Utils.HandleError(ex);
            }
          }).Fail(ex =>
          {
            outputEditor.Text = ex.Message;
            lblItems.Text = "Error";
          })
          .Always(() =>
          {
            _clock.Enabled = false;
            _currentQuery = null;
            btnSubmit.Text = "► Run";
          });
      }
      catch (Exception err)
      {
        outputEditor.Text = err.Message;
        _clock.Enabled = false;
        lblItems.Text = "Error";
        _currentQuery = null;
        btnSubmit.Text = "► Run";
      }
    }
    #endregion

    private QueryParameter GetCreateParameter(string name)
    {
      QueryParameter result;
      if (_paramCache.TryGetValue(name, out result))
        return result;
      result = new QueryParameter() { Name = name };
      _paramCache[name] = result;
      return result;
    }

    private void btnEditConnections_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new ConnectionEditorForm())
        {
          dialog.Multiselect = false;
          if (_proxy != null && _proxy.ConnData != null)
            dialog.SetSelected(_proxy.ConnData);
          if (dialog.ShowDialog(this, menuStrip.RectangleToScreen(btnEditConnections.Bounds)) ==
            System.Windows.Forms.DialogResult.OK)
          {
            SetConnection(dialog.SelectedConnections.First());
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void tbcOutputView_SelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        EnsureDataTable();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    public static IEnumerable<ItemReference> GetItems(Connections.ConnectionData conn)
    {
      using (var dialog = new EditorWindow())
      {
        dialog.dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dialog.tbcOutputView.Appearance = TabAppearance.FlatButtons;
        dialog.tbcOutputView.ItemSize = new Size(0, 1);
        dialog.tbcOutputView.SelectedTab = dialog.pgTableOutput;
        dialog.tbcOutputView.SizeMode = TabSizeMode.Fixed;
        dialog.SetConnection(conn);
        dialog._autoTabSelect = false;
        if (dialog.ShowDialog() == DialogResult.OK
          && dialog._outputTable.Columns.Contains("type")
          && dialog._outputTable.Columns.Contains("id"))
        {
          return dialog.dgvItems.SelectedRows
                       .OfType<DataGridViewRow>()
                       .Where(r => r.Index != r.DataGridView.NewRowIndex)
                       .Select(r => ((DataRowView)r.DataBoundItem).Row)
                       .Select(r => new ItemReference((string)r["type"], (string)r["id"]) {
                         KeyedName = dialog._outputTable.Columns.Contains("keyed_name") ? (string)r["keyed_name"] : null
                       }).ToList();
        }
        return Enumerable.Empty<ItemReference>();
      }
    }

    private class AmlAction
    {
      private Stopwatch _stopwatch = Stopwatch.StartNew();

      public string SoapAction { get; set; }
      public string Aml { get; set; }
      public string Output { get; set; }
      public Stopwatch Stopwatch
      {
        get { return _stopwatch; }
      }
    }

    private void inputEditor_RunRequested(object sender, Editor.RunRequestedEventArgs e)
    {
      try
      {
        Submit(e.Query);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnSoapAction_Click(object sender, EventArgs e)
    {
      try
      {
        if (_proxy == null) return;

        using (var dialog = new FilterSelect<string>())
        {
          dialog.DataSource = _proxy.GetActions();
          dialog.Message = "Select an action to perform";
          if (dialog.ShowDialog(this, menuStrip.RectangleToScreen(btnSoapAction.Bounds)) ==
            DialogResult.OK && dialog.SelectedItem != null)
          {
            this.SoapAction = dialog.SelectedItem;
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    #region Table Handling

    private void mniAcceptChanges_Click(object sender, EventArgs e)
    {
      try
      {
        _outputTable.AcceptChanges();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniResetChanges_Click(object sender, EventArgs e)
    {
      try
      {
        _outputTable.RejectChanges();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniTableEditsToClipboard_Click(object sender, EventArgs e)
    {
      try
      {
        Clipboard.SetText(GetTableChangeAml());
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniTableEditsToFile_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new SaveFileDialog())
        {
          if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
          {
            System.IO.File.WriteAllText(dialog.FileName, GetTableChangeAml());
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniTableEditsToQueryEditor_Click(object sender, EventArgs e)
    {
      try
      {
        var window = NewWindow();
        window.inputEditor.Text = GetTableChangeAml();
        window.SoapAction = this.SoapAction;
        window.Show();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private string GetTableChangeAml()
    {
      var changes = _outputTable.GetChanges(DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);
      if (changes == null)
        return null;

      var settings = new System.Xml.XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";

      var types = _outputTable.AsEnumerable()
        .Select(r => r.CellValue("type").ToString())
        .Where(t => !string.IsNullOrEmpty(t))
        .Distinct().ToList();
      var singleType = types.Count == 1 ? types[0] : null;
      string newValue;

      using (var writer = new System.IO.StringWriter())
      using (var xml = XmlWriter.Create(writer, settings))
      {
        xml.WriteStartElement("AML");
        foreach (var row in changes.AsEnumerable())
        {
          xml.WriteStartElement("Item");
          xml.WriteAttributeString("type", singleType ?? row.CellValue("type").ToString());
          xml.WriteAttributeString("id", row.CellIsNull("id")
            ? Guid.NewGuid().ToString("N").ToUpperInvariant()
            : row.CellValue("id").ToString());

          switch (row.RowState)
          {
            case DataRowState.Added:
              xml.WriteAttributeString("action", "add");
              foreach (var column in changes.Columns.OfType<DataColumn>())
              {
                if (!row.IsNull(column) && !column.ColumnName.Contains('/'))
                {
                  xml.WriteElementString(column.ColumnName, row[column].ToString());
                }
              }
              break;
            case DataRowState.Deleted:
              xml.WriteAttributeString("action", "delete");
              break;
            case DataRowState.Modified:
              xml.WriteAttributeString("action", "edit");
              foreach (var column in changes.Columns.OfType<DataColumn>())
              {
                if (!column.ColumnName.Contains('/')
                  && IsChanged(row, column, out newValue))
                {
                  xml.WriteElementString(column.ColumnName, newValue);
                }
              }
              break;
          }
          xml.WriteEndElement();
        }
        xml.WriteEndElement();
        xml.Flush();
        return writer.ToString();
      }
    }


    private bool IsChanged(DataRow row, DataColumn col, out string newValue)
    {
      newValue = null;
      if (!row.HasVersion(DataRowVersion.Original) || !row.HasVersion(DataRowVersion.Current))
        return false;
      var orig = row[col, DataRowVersion.Original];
      var curr = row[col, DataRowVersion.Current];

      if (orig == DBNull.Value && curr == DBNull.Value)
        return false;

      newValue = curr.ToString();
      if (orig == DBNull.Value && curr == DBNull.Value)
        return true;

      return !orig.Equals(curr);
    }

    #endregion


    private void mniTidyXml_Click(object sender, EventArgs e)
    {
      try
      {
        if (outputEditor.ContainsFocus)
        {
          outputEditor.TidyXml();
        }
        else
        {
          inputEditor.TidyXml();
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniNewWindow_Click(object sender, EventArgs e)
    {
      try
      {
        NewWindow().Show();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniClose_Click(object sender, EventArgs e)
    {
      try
      {
        this.Close();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniTimeZone_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new FilterSelect<string>())
        {
          dialog.DataSource = TimeZoneInfo.GetSystemTimeZones().Select(t => t.Id).ToList();
          dialog.Message = "Select a time zone";
          if (dialog.ShowDialog(this) ==
            DialogResult.OK && dialog.SelectedItem != null)
          {
            _timeZone = dialog.SelectedItem;
            mniTimeZone.ShortcutKeyDisplayString = _timeZone;
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniLocale_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new FilterSelect<string>())
        {
          dialog.DataSource = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(c => c.Name).ToList();
          dialog.Message = "Select a locale";
          if (dialog.ShowDialog(this) ==
            DialogResult.OK && dialog.SelectedItem != null)
          {
            _locale = dialog.SelectedItem;
            mniLocale.ShortcutKeyDisplayString = _locale;
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniTimeout_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new InputBox())
        {
          dialog.Caption = "Select Timeout";
          dialog.Message = "Specify the desired timeout (in seconds)";
          dialog.Value = (_timeout < 0 ? -1 : _timeout / 1000).ToString();
          int newTimeout;
          if (dialog.ShowDialog(this) == DialogResult.OK && int.TryParse(dialog.Value, out newTimeout))
          {
            if (newTimeout < 0)
            {
              _timeout = -1;
              mniTimeout.ShortcutKeyDisplayString = "(none)";
            }
            else
            {
              _timeout = newTimeout * 1000;
              mniTimeout.ShortcutKeyDisplayString = "(" + newTimeout.ToString() + "s)";
            }
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
      base.OnFormClosed(e);
      SaveFormBounds();
      DisposeProxy();
    }
    protected override void OnMove(EventArgs e)
    {
      base.OnMove(e);
      SaveFormBounds();
    }
    protected override void OnResizeEnd(EventArgs e)
    {
      base.OnResizeEnd(e);
      SaveFormBounds();
    }
    private void SaveFormBounds()
    {
      if (this.WindowState == FormWindowState.Normal)
      {
        Properties.Settings.Default.EditorWindow_Bounds = this.DesktopBounds;
        Properties.Settings.Default.Save();
      }
    }

    private void exploreButton_Click(object sender, EventArgs e)
    {
      try
      {
        var connData = _proxy.ConnData;
        connData.Explore();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void dgvItems_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      try
      {
        if (e.Value == e.CellStyle.DataSourceNullValue)
        {
          e.CellStyle.BackColor = Color.AntiqueWhite;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void treeItems_ModelDoubleClick(object sender, ModelDoubleClickEventArgs e)
    {
      try
      {
        var node = e.Model as IEditorTreeNode;
        if (node != null && node.GetScripts().Any())
        {
          var script = node.GetScripts().First();
          this.SoapAction = script.Action;
          this.Script = script.Script;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniUppercase_Click(object sender, EventArgs e)
    {
      try
      {
        inputEditor.TransformUppercase();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniLowercase_Click(object sender, EventArgs e)
    {
      try
      {
        inputEditor.TransformLowercase();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniMoveUpCurrentLine_Click(object sender, EventArgs e)
    {
      try
      {
        inputEditor.MoveLineUp();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniMoveDownCurrentLine_Click(object sender, EventArgs e)
    {
      try
      {
        inputEditor.MoveLineDown();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniInsertNewGuid_Click(object sender, EventArgs e)
    {
      try
      {
        inputEditor.ReplaceSelectionSegments(t => Guid.NewGuid().ToString("N").ToUpperInvariant());
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniXmlToEntity_Click(object sender, EventArgs e)
    {
      try
      {
        inputEditor.ReplaceSelectionSegments(t => t.Replace("<", "&lt;").Replace(">", "&gt;"));
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniEntityToXml_Click(object sender, EventArgs e)
    {
      try
      {
        inputEditor.ReplaceSelectionSegments(t => t.Replace("&lt;", "<").Replace("&gt;", ">"));
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void treeItems_CellToolTipShowing(object sender, BrightIdeasSoftware.ToolTipShowingEventArgs e)
    {
      try
      {
        var node = (IEditorTreeNode)e.Model;
        if (e.Column.AspectName == "Name" && !string.IsNullOrWhiteSpace(node.Description))
        {
          e.Text = node.Description;
        }
      }
      catch (Exception) { }
    }

    private void treeItems_Expanding(object sender, BrightIdeasSoftware.TreeBranchExpandingEventArgs e)
    {
      try
      {
        if (treeItems.CanExpand(e.Model) && !treeItems.GetChildren(e.Model).OfType<IEditorTreeNode>().Any())
          treeItems.RefreshObject(e.Model);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void UpdatePanelCollapsed()
    {
      splitMain.IsSplitterFixed = _panelCollapsed;
      treeItems.Visible = !_panelCollapsed;
      if (_panelCollapsed)
      {
        btnPanelToggle.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        btnPanelToggle.Orientation = Orientation.Vertical;
        splitMain.SplitterDistance = btnPanelToggle.Width;
      }
      else
      {
        btnPanelToggle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        btnPanelToggle.Orientation = Orientation.Horizontal;
        btnPanelToggle.Height = 25;
        splitMain.SplitterDistance = 220;
      }
    }

    private void btnPanelToggle_Click(object sender, EventArgs e)
    {
      try
      {
        _panelCollapsed = !_panelCollapsed;
        UpdatePanelCollapsed();
        Properties.Settings.Default.EditorWindowPanelCollapsed = _panelCollapsed;
        Properties.Settings.Default.Save();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

  }
}
