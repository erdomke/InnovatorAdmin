using ICSharpCode.AvalonEdit.Document;
using Innovator.Client;
using Innovator.Client.Connection;
using InnovatorAdmin.Connections;
using InnovatorAdmin.Controls;
using InnovatorAdmin.Editor;
using Nancy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Text;
using System.ComponentModel;
using System.Net.Http;

namespace InnovatorAdmin
{
  public partial class EditorWindow : FormBase, IUpdateListener
  {
    private const string GeneratedPage = "__GeneratedPage_";

    private Timer _clock = new Timer();
    private UiCommandManager _commands;
    private IPromise<IResultObject> _currentQuery;
    private bool _disposeProxy = true;
    private Editor.AmlLinkElementGenerator _linkGenerator;
    private bool _loadingConnection = false;
    private DataSet _outputSet;
    private bool _outputTextSet = false;
    private bool _panelCollapsed;
    private Dictionary<string, QueryParameter> _paramCache = new Dictionary<string, QueryParameter>();
    private IEditorProxy _proxy;
    private IResultObject _result;
    private string _soapAction;
    private DateTime _start = DateTime.UtcNow;
    private string _uid;
    private bool _updateCheckComplete = false;
    private HttpClient _webService = new HttpClient();
    private ConnectionType _oldConnType = ConnectionType.Innovator;

    public bool AllowRun
    {
      get { return !splitEditors.Panel2Collapsed; }
      set { splitEditors.Panel2Collapsed = !value; }
    }
    public OutputType PreferredMode { get; set; }
    public IEditorProxy Proxy
    {
      get { return _proxy; }
      set { SetProxy(value); }
    }

    public string Script
    {
      get { return inputEditor.Text; }
      set { inputEditor.Text = value; }
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
    public string Uid { get { return _uid; } }

    private Color ConnectionColor
    {
      get { return pnlConnectionShadow.ShadowColor; }
      set
      {
        pnlConnectionShadow.ShadowColor = value;
        pnlLeft.ShadowColor = value;
        pnlRight.ShadowColor = value;

        var logo = new Logo(value);
        picLogo.Image = logo.Image;
        this.Icon = logo.Icon;
      }
    }
    public FullEditor InputEditor { get { return inputEditor; } }
    public FullEditor OutputEditor { get { return outputEditor; } }

    public EditorWindow() : base()
    {
      InitializeComponent();

      this.TitleLabel = lblTitle;
      this.MaximizeLabel = lblMaximize;
      this.MinimizeLabel = lblMinimize;
      this.CloseLabel = lblClose;
      this.LeftBorderPanel = pnlLeft;
      this.TopLeftCornerPanel = pnlTopLeft;
      this.TopLeftPanel = pnlLeftTop;
      this.TopBorderPanel = pnlTop;
      this.TopRightCornerPanel = pnlTopRight;
      this.TopRightPanel = pnlRightTop;
      this.RightBorderPanel = pnlRight;
      this.BottomRightCornerPanel = pnlBottomRight;
      this.BottomBorderPanel = pnlBottom;
      this.BottomLeftCornerPanel = pnlBottomLeft;
      this.InitializeTheme();

      InitializeDpi();

      menuStrip.Renderer = new SimpleToolstripRenderer() { BaseColor = Color.White };
      lblConnection.Font = FontAwesome.Font;
      lblConnection.Text = FontAwesome.Fa_cloud.ToString();
      lblSoapAction.Font = FontAwesome.Font;
      lblSoapAction.Text = FontAwesome.Fa_bolt.ToString();
      picLogo.Image = Properties.Resources.logo_black;
      btnPanelToggle.BackColor = Color.White;
      picLogo.MouseDown += SystemLabel_MouseDown;
      picLogo.MouseUp += SystemLabel_MouseUp;

      this.KeyPreview = true;
      this.PreferredMode = OutputType.Any;

      _uid = GetUid();
      var assy = Assembly.GetExecutingAssembly().GetName().Version;
      this.lblVersion.Text = string.Format("v{0}, [port {1}]", assy.ToString(), Program.PortNumber);

      tbcOutputView.TabsVisible = false;

      btnSoapAction.Visible = false;
      lblSoapAction.Visible = false;
      exploreButton.Visible = false;
      btnSubmit.Visible = false;

      treeItems.CanExpandGetter = m => ((IEditorTreeNode)m).HasChildren;
      treeItems.ChildrenGetter = m => ((IEditorTreeNode)m).GetChildren();
      colName.ImageGetter = m => ((IEditorTreeNode)m).Image.Key;

      foreach (var icon in Icons.All)
      {
        treeItems.SmallImageList.Images.Add(icon.Key, icon.Gdi);
      }

      _clock.Interval = 250;
      _clock.Tick += _clock_Tick;
      _panelCollapsed = Properties.Settings.Default.EditorWindowPanelCollapsed;
      UpdatePanelCollapsed();

      _linkGenerator = new Editor.AmlLinkElementGenerator();
      _linkGenerator.AmlLinkClicked += _linkGenerator_AmlLinkClicked;

      tbcOutputView.SelectedTab = pgTools;

      inputEditor.SelectionChanged += inputEditor_SelectionChanged;
      inputEditor.FindAllAction = res => SetResult(res, 0);
      inputEditor.BindToolStripItem(mniCut, System.Windows.Input.ApplicationCommands.Cut);
      inputEditor.BindToolStripItem(mniCopy, System.Windows.Input.ApplicationCommands.Copy);
      inputEditor.BindToolStripItem(mniPaste, System.Windows.Input.ApplicationCommands.Paste);
      inputEditor.BindToolStripItem(mniUndo, System.Windows.Input.ApplicationCommands.Undo);
      inputEditor.BindToolStripItem(mniRedo, System.Windows.Input.ApplicationCommands.Redo);

      UpdateTitle(null);

      if (_recentDocs.Count < 1 && !string.IsNullOrEmpty(Properties.Settings.Default.RecentDocument))
      {
        try
        {
          lock (_lock)
          {
            _recentDocs.RaiseListChangedEvents = false;
            foreach (var path in (Properties.Settings.Default.RecentDocument ?? "").Split('|').Where(d => !string.IsNullOrEmpty(d)))
            {
              _recentDocs.Add(path);
            }
          }
        }
        finally
        {
          _recentDocs.RaiseListChangedEvents = true;
        }
      }
      BuildRecentDocsMenu();
      _recentDocs.ListChanged += (s, e) => BuildRecentDocsMenu();

      // Wire up the commands
      _commands = new UiCommandManager(this);
      inputEditor.KeyDown += _commands.OnKeyDown;
      outputEditor.KeyDown += _commands.OnKeyDown;
      _commands.Add<Control>(btnEditConnections, e => e.KeyCode == Keys.Q && e.Modifiers == Keys.Control, ChangeConnection);
      _commands.Add<Control>(btnSoapAction, e => e.KeyCode == Keys.M && e.Modifiers == Keys.Control, ChangeSoapAction);
      _commands.Add<Editor.FullEditor>(mniNewDocument, e => e.KeyCode == Keys.N && e.Modifiers == (Keys.Control), c =>
      {
        c.NewFile();
        UpdateTitle(null);
      });
      _commands.Add<Control>(mniNewWindow, e => e.KeyCode == Keys.N && e.Modifiers == (Keys.Control | Keys.Shift), c => NewWindow().Show());
      _commands.Add<Editor.FullEditor>(mniOpen, e => e.KeyCode == Keys.O && e.Modifiers == Keys.Control, c =>
      {
        using (var dialog = new OpenFileDialog())
        {
          if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
          {
            OpenFile(c, dialog.FileName);
          }
        }
      });
      _commands.Add<Editor.FullEditor>(mniSave, e => e.KeyCode == Keys.S && e.Modifiers == Keys.Control, c => Save(c, false));
      _commands.Add<Editor.FullEditor>(mniSaveAs, e => e.KeyCode == Keys.S && e.Modifiers == (Keys.Control | Keys.Shift), c => Save(c, true));
      _commands.Add<Editor.FullEditor>(mniFind, null, c => c.Find());
      _commands.Add<Editor.FullEditor>(mniFindNext, null, c => c.FindNext());
      _commands.Add<Editor.FullEditor>(mniFindPrevious, null, c => c.FindPrevious());
      _commands.Add<Editor.FullEditor>(mniReplace, null, c => c.Replace());
      _commands.Add<Editor.FullEditor>(mniGoTo, e => e.KeyCode == Keys.G && e.Modifiers == Keys.Control, c =>
      {
        using (var dialog = new InputBox())
        {
          dialog.Caption = "Go To Line";
          dialog.Message = string.Format("Line Number (1 - {0}):", c.Editor.LineCount);
          int line;
          if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK
            && int.TryParse(dialog.Value, out line)
            && line >= 1
            && line <= c.Editor.LineCount)
          {
            var targetLine = c.Editor.TextArea.TextView.GetOrConstructVisualLine(c.Editor.Document.GetLineByNumber(line));
            var docHeight = c.Editor.TextArea.TextView.DocumentHeight;
            var winHeight = c.Editor.TextArea.TextView.ActualHeight;
            var target = Math.Min(docHeight, Math.Max(0, (int)(targetLine.VisualTop - (winHeight - targetLine.Height) / 2.0)));
            c.Editor.ScrollToVerticalOffset(target);
            c.Editor.TextArea.Caret.Line = line;
          }
        }
      });
      _commands.Add<Editor.FullEditor>(mniTidy, e => e.KeyCode == Keys.T && e.Modifiers == Keys.Control, c => TransformSelection(c, c.Helper.Format));
      _commands.Add<Editor.FullEditor>(mniMinify, null, c => TransformSelection(c, c.Helper.Minify));
      _commands.Add<Editor.FullEditor>(mniMd5Encode, null, c => c.ReplaceSelectionSegments(t => ConnectionDataExtensions.CalcMD5(t)));
      _commands.Add<Editor.FullEditor>(mniBase64Encode, null, c => c.ReplaceSelectionSegments(t => Convert.ToBase64String(Encoding.UTF8.GetBytes(t))));
      _commands.Add<Editor.FullEditor>(mniBase64Decode, null, c => c.ReplaceSelectionSegments(t => Encoding.UTF8.GetString(Convert.FromBase64String(t))));
      _commands.Add<Editor.FullEditor>(mniDoubleToSingleQuotes, null, c => c.ReplaceSelectionSegments(t => t.Replace('"', '\'')));
      _commands.Add<Editor.FullEditor>(mniSingleToDoubleQuotes, null, c => c.ReplaceSelectionSegments(t => t.Replace('\'', '"')));
      _commands.Add<Editor.FullEditor>(mniUppercase, null, c => c.TransformUppercase());
      _commands.Add<Editor.FullEditor>(mniLowercase, null, c => c.TransformLowercase());
      _commands.Add<Editor.FullEditor>(mniMoveUpCurrentLine, null, c => c.MoveLineUp());
      _commands.Add<Editor.FullEditor>(mniMoveDownCurrentLine, null, c => c.MoveLineDown());
      _commands.Add<Editor.FullEditor>(mniToggleSingleLineComment, e => e.Modifiers == Keys.Control && e.KeyCode == Keys.Q, LineToggleComment);
      _commands.Add<Editor.FullEditor>(mniSingleLineComment, e => e.Modifiers == Keys.Control && e.KeyCode == Keys.K, LineComment);
      _commands.Add<Editor.FullEditor>(mniSingleLineUncomment, e => e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.K, LineUncomment);
      _commands.Add<Editor.FullEditor>(mniBlockComment, e => e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.Q, BlockComment);
      _commands.Add<Editor.FullEditor>(mniBlockUncomment, null, BlockUncomment);
      _commands.Add<Editor.FullEditor>(mniInsertNewGuid, null, c => c.ReplaceSelectionSegments(t => Guid.NewGuid().ToString("N").ToUpperInvariant()));
      _commands.Add<Editor.FullEditor>(mniXmlToEntity, null, c => c.ReplaceSelectionSegments(t =>
      {
        try
        {
          var sb = new System.Text.StringBuilder();
          var settings = new XmlWriterSettings();
          settings.Indent = false;
          settings.OmitXmlDeclaration = true;
          using (var strWriter = new StringWriter(sb))
          using (var writer = XmlWriter.Create(strWriter, settings))
          {
            writer.WriteStartElement("a");
            writer.WriteValue(t);
            writer.WriteEndElement();
          }
          return sb.ToString(3, sb.Length - 7);
        }
        catch (XmlException)
        {
          return t.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
        }
      }));
      _commands.Add<Editor.FullEditor>(mniEntityToXml, null, c => c.ReplaceSelectionSegments(t =>
      {
        try
        {
          var xml = "<a>" + t + "</a>";
          using (var strReader = new StringReader(xml))
          using (var reader = XmlReader.Create(strReader))
          {
            while (reader.Read())
            {
              if (reader.NodeType == XmlNodeType.Text)
              {
                return reader.Value;
              }
            }
          }
        }
        catch (XmlException)
        {
          return t.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");
        }
        return t;
      }));
      _commands.Add<Control>(mniPreferences, null, c =>
      {
        using (var dialog = new Dialog.SettingsDialog())
        {
          dialog.Message = "Configure Innovator Admin";
          dialog.ShowDialog();
        }
      });
      _commands.Add<Control>(horizontalSplitToolStripMenuItem, null, c => splitViewHorizontally());
      _commands.Add<Control>(verticalSplitToolStripMenuItem, null, c => splitViewVertically());
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);

      var resizeMargin = this.WindowState == FormWindowState.Maximized ? 0 : 3;
      tblMain.ColumnStyles[0].Width = resizeMargin;
      tblMain.ColumnStyles[tblMain.ColumnStyles.Count - 1].Width = resizeMargin;
      tblMain.RowStyles[0].Height = resizeMargin;
      tblMain.RowStyles[tblMain.RowStyles.Count - 1].Height = resizeMargin;
    }

    private void Save(FullEditor editor, bool forceFilePrompt)
    {
      lblProgress.Text = "Saving file...";
      editor.Save(forceFilePrompt).ContinueWith(t =>
      {
        if (t.IsFaulted)
          lblProgress.Text = t.Exception.Message;
        else if (t.IsCanceled || !t.Result)
          lblProgress.Text = "";
        else
        {
          lblProgress.Text = "File saved";
          if (_proxy != null && _proxy.ConnData != null)
          {
            SnippetManager.Instance.SetLastQueryByConnection(_proxy.ConnData.ConnectionName, new Snippet()
            {
              Action = this.SoapAction,
              Text = editor.Text
            });
            Properties.Settings.Default.LastConnection = _proxy.ConnData.ConnectionName;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
          }
        }
        UpdateTitle(null);
      }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void SetConnection(IAsyncConnection conn, string name = null)
    {
      if (conn == null) throw new ArgumentNullException("conn");
      exploreButton.Visible = true;
      SetProxy(new ArasEditorProxy(conn, name ?? conn.Database));
      _disposeProxy = false;
      _oldConnType = ConnectionType.Innovator;
    }

    public async Task<bool> SetConnection(ConnectionData conn)
    {
      if (!_loadingConnection && !string.IsNullOrEmpty(conn.Url))
      {
        _loadingConnection = true;
        btnSoapAction.Visible = false;
        lblSoapAction.Visible = false;
        exploreButton.Visible = false;
        btnSubmit.Visible = false;

        if (inputEditor.Document.TextLength <= 0 || conn.Type != _oldConnType)
        {
          var lastQuery = SnippetManager.Instance.GetLastQueryByConnection(conn.ConnectionName);
          inputEditor.Text = lastQuery.Text;
          this.SoapAction = lastQuery.Action;
          inputEditor.CleanUndoStack();
        }
        _oldConnType = conn.Type;

        try
        {
          btnEditConnections.Text = "Connecting... ▼";
          var proxy = await ProxyFactory.FromConn(conn).ToTask();
          SetProxy(proxy);
          _disposeProxy = true;
        }
        catch (Exception ex)
        {
          lblProgress.Text = ex.Message;
          btnEditConnections.Text = "Not Connected ▼";
          lblConnection.Visible = true;
          btnEditConnections.Visible = lblConnection.Visible;
        }
        _loadingConnection = false;
        return true;
      }
      return false;
    }

    public void SetProxy(IEditorProxy proxy)
    {
      DisposeProxy();

      _proxy = proxy;
      UpdateTitle(null);
      if (proxy == null)
        return;

      btnSubmit.Visible = true;
      if (_proxy.GetActions().Any())
      {
        _proxy.Action = _soapAction;
        btnSoapAction.Visible = true;
      }
      else
      {
        btnSoapAction.Visible = false;
      }
      lblSoapAction.Visible = btnSoapAction.Visible;

      inputEditor.Helper = _proxy.GetHelper();
      outputEditor.Helper = _proxy.GetOutputHelper();
      btnEditConnections.Text = string.Format("{0} ▼", _proxy.Name);

      if (proxy.ConnData != null)
      {
        lblConnection.Visible = true;
        btnEditConnections.Visible = lblConnection.Visible;
        this.ConnectionColor = proxy.ConnData.Color;
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

    public static IEnumerable<ItemReference> GetItems(IAsyncConnection conn, string query, int offset)
    {
      return GetItems(d =>
      {
        d.SetConnection(conn);
        d.Script = query;
        d.inputEditor.Editor.CaretOffset = offset;
      });
    }
    public static IEnumerable<ItemReference> GetItems(Connections.ConnectionData conn)
    {
      return GetItems(d => d.SetConnection(conn));
    }
    public static IEnumerable<ItemReference> GetItems(Action<EditorWindow> setConn)
    {
      using (var dialog = new EditorWindow())
      {
        dialog.dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dialog.tbcOutputView.TabsVisible = false;
        dialog.tbcOutputView.SelectedTab = dialog.pgTableOutput;
        dialog.PreferredMode = OutputType.Table;
        setConn(dialog);
        if (dialog.ShowDialog() == DialogResult.OK
          && dialog._outputSet != null
          && dialog._outputSet.Tables.Count > 0
          && dialog._outputSet.Tables[0].Columns.Contains(Extensions.AmlTable_TypeName)
          && dialog._outputSet.Tables[0].Columns.Contains("id"))
        {
          return dialog.dgvItems.SelectedRows
                       .OfType<DataGridViewRow>()
                       .Where(r => r.Index != r.DataGridView.NewRowIndex)
                       .Select(r => ((DataRowView)r.DataBoundItem).Row)
                       .Select(r => new ItemReference((string)r[Extensions.AmlTable_TypeName], (string)r["id"])
                       {
                         KeyedName = dialog._outputSet.Tables[0].Columns.Contains("keyed_name") ? (string)r["keyed_name"] : null
                       }).ToList();
        }
        return Enumerable.Empty<ItemReference>();
      }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
      base.OnFormClosed(e);
      SaveFormBounds();
      DisposeProxy();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
      _commands.OnKeyDown(this, e);
    }

    protected override void OnLoad(EventArgs e)
    {
      try
      {
        base.OnLoad(e);

        if (!this.Modal)
        {
          var col1 = tblMain.GetColumn(btnOk);
          var col2 = tblMain.GetColumn(btnCancel);
          tblMain.Controls.Remove(btnOk);
          tblMain.Controls.Remove(btnCancel);
          tblMain.ColumnStyles[col1].SizeType = SizeType.Absolute;
          tblMain.ColumnStyles[col1].Width = 0;
          tblMain.ColumnStyles[col2].SizeType = SizeType.Absolute;
          tblMain.ColumnStyles[col2].Width = 0;
        }

        //lblConnection.Visible = _proxy != null && _proxy.ConnData != null;
        lblConnection.Visible = true;
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
        if (bounds.Width < 200 || bounds.Height < 200)
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

        inputEditor.Editor.TextArea.Focus();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
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

    void inputEditor_SelectionChanged(object sender, Editor.SelectionChangedEventArgs e)
    {
      try
      {
        if (e.SelectionLength < 1)
        {
          lblSelection.Text = string.Format("Ln: {0}  Col: {1}  Sel: {2} ['' = 0x0 = 0]"
            , e.CaretLine, e.CaretColumn, e.SelectionLength);
        }
        else
        {
          var text = e.GetText(1);
          lblSelection.Text = string.Format("Ln: {0}  Col: {1}  Sel: {2} ['{3}' = 0x{4:x} = {4}]"
            , e.CaretLine, e.CaretColumn, e.SelectionLength, text, (int)text[0]);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }


    void _linkGenerator_AmlLinkClicked(object sender, Editor.AmlLinkClickedEventArgs e)
    {
      try
      {
        var window = NewWindow();
        var query = "<Item type='" + e.Type + "' action='get' id='" + e.Id + "' levels='1' />";
        window.inputEditor.Text = query;
        window.SoapAction = "ApplyItem";
        window.Submit(query);
        window.Show();
        Task.Delay(200).ContinueWith(_ => window.Activate(), TaskScheduler.FromCurrentSynchronizationContext());
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    void _clock_Tick(object sender, EventArgs e)
    {
      lblClock.Text = string.Format(@"[{0:hh\:mm\:ss}]", DateTime.UtcNow - _start);
    }
    private void ConfigureRequest(IHttpRequest req)
    {
      if (_proxy != null && _proxy.ConnData != null)
      {
        foreach (var param in _proxy.ConnData.Params)
        {
          req.SetHeader(param.Name, param.Value);
        }
        req.Timeout = TimeSpan.FromMilliseconds(_proxy.ConnData.Timeout);
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

    private class TableError
    {
      public string Message { get; set; }
      public string Table { get; set; }
    }

    private void EnsureDataTable()
    {
      if (_outputSet == null && _result != null && tbcOutputView.SelectedTab == pgTableOutput)
      {
        _outputSet = _result.GetDataSet();
        if (_outputSet.Tables.Count > 0)
        {
          var errors = _outputSet.Tables.OfType<DataTable>()
            .SelectMany(t => t.GetErrors().Select(r => new TableError()
            {
              Message = r.RowError,
              Table = t.TableName
            }))
            .GroupBy(e => e.Message)
            .Select(g => g.Key
              + " in tables: "
              + g.GroupBy(t => t.Table)
                .Select(t => t.Key + " (" + t.Count() + " row(s))")
                .GroupConcat(", "));

          if (errors.Any())
          {
            using (var dialog = new Dialog.MessageDialog())
            {
              dialog.Message = errors.GroupConcat(Environment.NewLine);
              dialog.OkText = "&Keep Going";
              dialog.Caption = "Validation Error";
              dialog.CaptionColor = System.Drawing.Color.Red;
              dialog.ShowDialog();
            }
          }

          dgvItems.AutoGenerateColumns = false;
          dgvItems.DataSource = _outputSet.Tables[0];
          FormatDataGrid(dgvItems);
          pgTableOutput.Text = _outputSet.Tables[0].TableName;

          var i = 1;
          foreach (var tbl in _outputSet.Tables.OfType<DataTable>().Skip(1))
          {
            var pg = new TabPage(tbl.TableName);
            pg.Name = GeneratedPage + i.ToString();
            var grid = new Controls.DataGrid();
            grid.AutoGenerateColumns = false;
            grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.ContextMenuStrip = this.conTable;
            grid.DataSource = tbl;
            grid.Dock = System.Windows.Forms.DockStyle.Fill;
            grid.Location = new System.Drawing.Point(0, 0);
            grid.Margin = new System.Windows.Forms.Padding(0);
            grid.TabIndex = 0;
            grid.MouseDoubleClick += Grid_MouseDoubleClick;
            pg.Controls.Add(grid);
            tbcOutputView.TabPages.Add(pg);
            FormatDataGrid(grid);
            i++;
          }
        }
      }
    }

    private void Grid_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (this.Modal)
      {
        var grid = (DataGridView)sender;
        if (grid.HitTest(e.X, e.Y).RowIndex >= 0)
        {
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
      }
    }

    private void EnsureTextResult()
    {
      if (!_outputTextSet && _result != null && tbcOutputView.SelectedTab == pgTextOutput)
      {
        _outputTextSet = true;

        var text = _result.GetTextSource();
        outputEditor.Document.Replace(0, outputEditor.Document.TextLength, text);

        if (_result.ItemCount > 1 && outputEditor.Editor.LineCount > 100)
        {
          outputEditor.CollapseAll();
        }
      }
    }

    private void FormatDataGrid(DataGridView grid)
    {
      var tbl = (DataTable)grid.DataSource;
      grid.RowTemplate.Height = (int)(DpiScale * 22);
      var arasProxy = _proxy as ArasEditorProxy;
      ArasMetadataProvider metadata = null;
      if (arasProxy != null)
        metadata = ArasMetadataProvider.Cached(arasProxy.Connection);

      grid.Columns.Clear();
      foreach (var dataCol in tbl.Columns.OfType<DataColumn>())
      {
        DataGridViewColumn col;
        if (dataCol.DataType == typeof(bool))
        {
          col = new DataGridViewCheckBoxColumn();
        }
        else if (dataCol.DataType == typeof(DateTime))
        {
          col = new Controls.DataGridViewCalendarColumn();
        }
        else
        {
          var prop = dataCol.PropMetadata();
          if (prop != null && metadata != null
            && string.Equals(prop.TypeName, "list", StringComparison.OrdinalIgnoreCase))
          {
            var combo = new DataGridViewListColumn();
            combo.DisplayMember = "Label";
            combo.ValueMember = "Value";
            metadata.ListValues(prop.DataSource)
              .Done(v => combo.DataSource = v.ToArray());
            col = combo;
          }
          else
          {
            col = new DataGridViewTextBoxColumn();
          }
        }

        col.DataPropertyName = dataCol.ColumnName;
        col.ValueType = dataCol.DataType;
        col.HeaderText = dataCol.Caption;
        col.Visible = dataCol.IsUiVisible();
        grid.Columns.Add(col);
      }

      grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader);
      var minWidths = grid.Columns.OfType<DataGridViewColumn>().Select(c => c.Width).ToArray();
      grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
      var maxWidths = grid.Columns.OfType<DataGridViewColumn>().Select(c => c.Width).ToArray();
      var maxWidth = (int)(grid.Width * 0.8);

      //DataColumn boundColumn;
      for (var i = 0; i < grid.Columns.Count; i++)
      {
        grid.Columns[i].Width = Math.Min(maxWidths[i] < 100 ? maxWidths[i] :
            (maxWidths[i] < minWidths[i] + 60 ? maxWidths[i] : minWidths[i] + 60)
          , maxWidth);
        grid.Columns[i].DefaultCellStyle.Alignment =
          (IsNumericType(grid.Columns[i].ValueType)
            ? DataGridViewContentAlignment.TopRight
            : DataGridViewContentAlignment.TopLeft);
        //boundColumn = ((DataTable)grid.DataSource).Columns[grid.Columns[i].DataPropertyName];
        //grid.Columns[i].HeaderText = boundColumn.Caption;
        //grid.Columns[i].Visible = boundColumn.IsUiVisible();
      }

      if (!grid.Columns.OfType<DataGridViewColumn>().Any(c => c.Visible))
      {
        foreach (var col in grid.Columns.OfType<DataGridViewColumn>())
        {
          col.Visible = true;
        }
      }

      var orderedColumns = grid.Columns.OfType<DataGridViewColumn>()
        .Select(c => new
        {
          Column = c,
          SortOrder = GetSortOrder(c)
        })
        .OrderBy(c => c.SortOrder)
        .ThenBy(c => c.Column.HeaderText)
        .Select((c, i) => new { Column = c.Column, Index = i })
        .ToArray();
      foreach (var col in orderedColumns)
      {
        col.Column.DisplayIndex = col.Index;
      }

      if (this.Modal)
      {
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      }
      else
      {
        grid.AllowUserToAddRows = _outputSet != null
          && ((DataTable)grid.DataSource).Columns.Contains("id")
          && ((DataTable)grid.DataSource).Columns.Contains(Extensions.AmlTable_TypeName);
        grid.AllowUserToDeleteRows = grid.AllowUserToAddRows;
        grid.ReadOnly = !grid.AllowUserToAddRows;
        grid.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
      }
    }

    private int GetSortOrder(DataGridViewColumn c)
    {
      var col = ((DataTable)c.DataGridView.DataSource).Columns[c.DataPropertyName];
      var prop = col.PropMetadata();
      if (prop == null)
        return col.Ordinal;
      return prop.SortOrder;
    }

    private string GetUid()
    {
      return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace('+', '-').Replace('/', '_');
    }
    private void InitializeUi(ArasEditorProxy proxy)
    {
      if (proxy == null || proxy.Connection.AmlContext == null)
      {
        outputEditor.ElementGenerators.Remove(_linkGenerator);
        return;
      }

      exploreButton.Visible = true;

      if (!outputEditor.ElementGenerators.Contains(_linkGenerator))
        outputEditor.ElementGenerators.Add(_linkGenerator);

      var remote = proxy.Connection as IRemoteConnection;

      if (remote != null)
      {
        remote.DefaultSettings(ConfigureRequest);
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

    private void LineComment(Editor.FullEditor editor)
    {
      if (editor.Helper == null || editor.ReadOnly) return;
      if (string.IsNullOrWhiteSpace(editor.Helper.LineComment)
        && !string.IsNullOrWhiteSpace(editor.Helper.BlockCommentStart))
      {
        BlockComment(editor);
        return;
      }
      if (string.IsNullOrWhiteSpace(editor.Helper.LineComment))
        return;

      ActOnSelectedLines(editor, start => editor.Document.Insert(start, editor.Helper.LineComment + " "));
    }

    private void LineUncomment(Editor.FullEditor editor)
    {
      if (editor.Helper == null || editor.ReadOnly) return;
      if (string.IsNullOrWhiteSpace(editor.Helper.LineComment)
        && !string.IsNullOrWhiteSpace(editor.Helper.BlockCommentStart))
      {
        BlockUncomment(editor);
        return;
      }
      if (string.IsNullOrWhiteSpace(editor.Helper.LineComment))
        return;

      ActOnSelectedLines(editor, start =>
      {
        var length = editor.Helper.LineComment.Length;
        if (start + length < editor.Document.TextLength
          && editor.Document.GetText(start, length) == editor.Helper.LineComment)
        {
          if (start + length + 1 < editor.Document.TextLength
            && editor.Document.GetCharAt(start + length) == ' ')
            length++;
          editor.Document.Remove(start, length);
        }
      });
    }

    private void LineToggleComment(Editor.FullEditor editor)
    {
      if (editor.Helper == null || editor.ReadOnly) return;
      if (string.IsNullOrWhiteSpace(editor.Helper.LineComment))
        return;

      ActOnSelectedLines(editor, start =>
      {
        var length = editor.Helper.LineComment.Length;
        if (start + length < editor.Document.TextLength
          && editor.Document.GetText(start, length) == editor.Helper.LineComment)
        {
          if (start + length + 1 < editor.Document.TextLength
            && editor.Document.GetCharAt(start + length) == ' ')
            length++;
          editor.Document.Remove(start, length);
        }
        else
        {
          editor.Document.Insert(start, editor.Helper.LineComment + " ");
        }
      });
    }

    private void ActOnSelectedLines(Editor.FullEditor editor, Action<int> callback)
    {
      var firstLine = editor.Document.GetLineByOffset(editor.Editor.SelectionLength > 0
        ? editor.Editor.SelectionStart : editor.Editor.CaretOffset);
      var lastLine = editor.Editor.SelectionLength > 0
        ? editor.Document.GetLineByOffset(editor.Editor.SelectionStart + editor.Editor.SelectionLength)
        : firstLine;
      var curr = firstLine;
      int start;
      while (curr.LineNumber <= lastLine.LineNumber)
      {
        start = curr.Offset;
        while (start < curr.EndOffset && char.IsWhiteSpace(editor.Document.GetCharAt(start)))
          start++;
        callback(start);
        curr = curr.NextLine;
      }
    }

    private void BlockComment(Editor.FullEditor editor)
    {
      if (editor.Helper == null || editor.ReadOnly) return;
      if (string.IsNullOrWhiteSpace(editor.Helper.BlockCommentStart)
        && !string.IsNullOrWhiteSpace(editor.Helper.LineComment))
      {
        LineComment(editor);
        return;
      }
      if (string.IsNullOrWhiteSpace(editor.Helper.BlockCommentStart))
        return;

      var segment = SelectionSegment(editor);
      var text = editor.Helper.BlockCommentStart + " "
        + editor.Document.GetText(segment)
          .Replace(editor.Helper.BlockCommentEnd, NewBlockCommentEnd(editor.Helper.BlockCommentEnd))
        + " " + editor.Helper.BlockCommentEnd;
      editor.Document.Replace(segment, text);
    }

    private void BlockUncomment(Editor.FullEditor editor)
    {
      if (editor.Helper == null || editor.ReadOnly) return;
      if (string.IsNullOrWhiteSpace(editor.Helper.BlockCommentStart)
        && !string.IsNullOrWhiteSpace(editor.Helper.LineComment))
      {
        LineUncomment(editor);
        return;
      }
      if (string.IsNullOrWhiteSpace(editor.Helper.BlockCommentStart))
        return;

      var segment = SelectionSegment(editor);
      // If the selection is on the inside of the comment
      if (editor.Document.GetText(segment.Offset, editor.Helper.BlockCommentStart.Length) != editor.Helper.BlockCommentStart
        && segment.Offset > editor.Helper.BlockCommentStart.Length
        && editor.Document.GetText(segment.Offset - editor.Helper.BlockCommentStart.Length - 1, editor.Helper.BlockCommentStart.Length + 1)
          == (editor.Helper.BlockCommentStart + " "))
      {
        segment = new TextSegment()
        {
          StartOffset = segment.Offset - (editor.Helper.BlockCommentStart.Length + 1),
          EndOffset = segment.EndOffset + editor.Helper.BlockCommentEnd.Length + 1
        };
      }
      var text = editor.Document.GetText(segment);
      var output = new System.Text.StringBuilder();
      var newEnd = NewBlockCommentEnd(editor.Helper.BlockCommentEnd);
      var endWithSpace = " " + editor.Helper.BlockCommentEnd;
      bool inComment = false;

      for (var i = 0; i < text.Length; i++)
      {
        if (!inComment && IsNext(text, editor.Helper.BlockCommentStart, i))
        {
          inComment = true;
          i += editor.Helper.BlockCommentStart.Length - 1;
          if (IsNext(text, " ", i + 1)) i++;
        }
        else if (inComment && IsNext(text, newEnd, i))
        {
          output.Append(editor.Helper.BlockCommentEnd);
          i += newEnd.Length - 1;
        }
        else if (inComment && IsNext(text, endWithSpace, i))
        {
          inComment = false;
          i += endWithSpace.Length - 1;
        }
        else if (inComment && IsNext(text, editor.Helper.BlockCommentEnd, i))
        {
          inComment = false;
          i += editor.Helper.BlockCommentEnd.Length - 1;
        }
        else
        {
          output.Append(text[i]);
        }
      }

      editor.Document.Replace(segment, output.ToString());
    }

    private ISegment SelectionSegment(Editor.FullEditor editor)
    {
      if (editor.Editor.SelectionLength > 0)
      {
        return new TextSegment()
        {
          StartOffset = editor.Editor.SelectionStart,
          Length = editor.Editor.SelectionLength
        };
      }
      else
      {
        var line = editor.Document.GetLineByOffset(editor.Editor.CaretOffset);
        var start = line.Offset;
        while (start < line.EndOffset && char.IsWhiteSpace(editor.Document.GetCharAt(start)))
          start++;
        return new TextSegment()
        {
          StartOffset = start,
          EndOffset = line.EndOffset
        };
      }
    }

    private bool IsNext(string search, string value, int pos)
    {
      if (search.Length < (pos + value.Length)) return false;
      for (var i = 0; i < value.Length; i++)
      {
        if (search[pos + i] != value[i]) return false;
      }
      return true;
    }

    private string NewBlockCommentEnd(string oldEnd)
    {
      var newEnd = new char[oldEnd.Length + 1];
      oldEnd.CopyTo(0, newEnd, 0, oldEnd.Length);
      newEnd[newEnd.Length - 1] = newEnd[newEnd.Length - 2];
      newEnd[newEnd.Length - 2] = '`';
      return new string(newEnd);
    }

    private void TransformSelection(Editor.FullEditor editor, Action<TextReader, TextWriter> action)
    {
      if (editor.Editor.SelectionLength > 0)
      {
        using (var reader = new StringReader(editor.Editor.SelectedText))
        using (var writer = new StringWriter())
        {
          action.Invoke(reader, writer);
          writer.Flush();
          editor.Editor.SelectedText = writer.ToString();
        }
      }
      else
      {
        using (var reader = editor.Document.CreateReader())
        using (var writer = new RopeWriter())
        {
          action.Invoke(reader, writer);
          writer.Flush();
          editor.Document.Replace(0, editor.Document.TextLength,
            new ICSharpCode.AvalonEdit.Document.RopeTextSource(writer.Rope));
        }
      }
    }

    private void SaveFormBounds()
    {
      if (this.WindowState == FormWindowState.Normal)
      {
        Properties.Settings.Default.EditorWindow_Bounds = this.DesktopBounds;
        Properties.Settings.Default.Save();
        Properties.Settings.Default.Reload();
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
        Submit(inputEditor.Helper.GetCurrentQuery(inputEditor.Document, inputEditor.Editor.CaretOffset)
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
        var query = inputEditor.Helper.GetCurrentQuery(inputEditor.Document, inputEditor.Editor.CaretOffset)
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
      window.SetProxy(_proxy.Clone());
      window.SoapAction = this.SoapAction;
      window._disposeProxy = false;
      return window;
    }

    private IPromise<IResultObject> Submit(string query, OutputType preferred = OutputType.Any, int batchSize = 0, int concurrentCount = 0)
    {
      if (_currentQuery != null)
      {
        outputEditor.Text = "";
        _currentQuery.Cancel();
        _currentQuery = null;
        lblClock.Text = "";
        progQuery.Visible = false;
        _clock.Enabled = false;
        btnSubmit.Text = "► Run";
        lblProgress.Text = "Canceled";
        return null;
      }

      if (_proxy.ConnData != null && _proxy.ConnData.Confirm)
      {
        if (Dialog.MessageDialog.Show("Do you want to run this query on " + _proxy.ConnData.ConnectionName + "?", "Confirm Execution"
          , "&Run Query", "&Cancel") != DialogResult.OK)
        {
          return null;
        }
      }
      try
      {
        var cmd = _proxy.NewCommand()
          .WithQuery(query)
          .WithAction(this.SoapAction)
          .WithStatementCount(batchSize)
          .WithConcurrentCount(concurrentCount);
        var queryParams = _proxy.GetHelper().GetParameterNames(query)
          .Select(p => GetCreateParameter(p)).ToList();
        if (queryParams.Any() && this.SoapAction != ArasEditorProxy.UnitTestAction)
        {
          using (var dialog = new Dialog.ParameterDialog(queryParams))
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
                return null;
            }
          }
        }

        outputEditor.Text = "Processing...";
        lblProgress.Text = "Processing...";
        _start = DateTime.UtcNow;
        lblClock.Text = "";
        progQuery.Visible = true;
        _clock.Enabled = true;
        _outputTextSet = false;
        btnSubmit.Text = "Cancel";

        // Only reset if we are getting a new result
        if (preferred != OutputType.None)
        {
          _result = null;
          _outputSet = null;
          CleanupGrids();
        }

        if (_proxy.ConnData != null)
        {
          SnippetManager.Instance.SetLastQueryByConnection(_proxy.ConnData.ConnectionName, new Snippet()
          {
            Action = this.SoapAction,
            Text = inputEditor.Text
          });
          Properties.Settings.Default.LastConnection = _proxy.ConnData.ConnectionName;
          Properties.Settings.Default.Save();
          Properties.Settings.Default.Reload();
        }

        var st = Stopwatch.StartNew();
        _currentQuery = _proxy
          .Process(cmd, true, (p, m) => this.UiThreadInvoke(() =>
          {
            lblProgress.Text = m;
            progQuery.Value = p;
          }))
          .UiPromise(this)
          .Done(result =>
          {
            try
            {
              var milliseconds = st.ElapsedMilliseconds;
              _clock.Enabled = false;

              SetResult(result, milliseconds, preferred);
            }
            catch (Exception ex)
            {
              Utils.HandleError(ex);
            }
          }).Fail(ex =>
          {
            outputEditor.Text = ex.Message;
            tbcOutputView.SelectedTab = pgTextOutput;
            lblProgress.Text = "Error";
          })
          .Always(() =>
          {
            lblClock.Text = "";
            _clock.Enabled = false;
            _currentQuery = null;
            progQuery.Visible = false;
            btnSubmit.Text = "► Run";
          });

        // Handle the synchronous option
        if (!_clock.Enabled)
          _currentQuery = null;
      }
      catch (Exception err)
      {
        outputEditor.Text = err.Message;
        tbcOutputView.SelectedTab = pgTextOutput;
        _clock.Enabled = false;
        progQuery.Visible = false;
        lblClock.Text = "";
        lblProgress.Text = "Error";
        _currentQuery = null;
        btnSubmit.Text = "► Run";
      }
      return _currentQuery;
    }

    private void CleanupGrids()
    {
      var pagesToRemove = tbcOutputView.TabPages.OfType<TabPage>()
                  .Where(p => p.Name.StartsWith(GeneratedPage)).ToArray();
      foreach (var page in pagesToRemove)
      {
        var grid = page.Controls.OfType<DataGridView>().FirstOrDefault();
        if (grid != null)
          grid.MouseDoubleClick -= Grid_MouseDoubleClick;
        tbcOutputView.TabPages.Remove(page);
      }
      pgTableOutput.Text = "Table";
      dgvItems.DataSource = null;
    }

    private void SetResult(IResultObject result, long milliseconds, OutputType preferred = OutputType.Any)
    {
      var mode = this.PreferredMode;
      if (mode == OutputType.Any)
        mode = preferred;
      if (mode == OutputType.Any)
        mode = result.PreferredMode;
      tbcOutputView.TabsVisible = this.PreferredMode == OutputType.Any;

      if (result.ItemCount > 0)
      {
        lblProgress.Text = string.Format("{0} item(s) found in {1} ms.", result.ItemCount, milliseconds);
      }
      else
      {
        lblProgress.Text = string.Format("No items found in {0} ms.", milliseconds);
      }

      if (mode != OutputType.None)
      {
        _outputTextSet = false;
        dgvItems.DataSource = null;
        _outputSet = null;
        _result = result;
        if (mode == OutputType.Table && result.ItemCount > 0)
        {
          tbcOutputView.SelectedTab = pgTableOutput;
          EnsureDataTable();
        }
        else if (mode == OutputType.Html)
        {
          browser.Navigate(GetReportUri().ToString());
          tbcOutputView.SelectedTab = pgHtml;
        }
        else
        {
          tbcOutputView.SelectedTab = pgTextOutput;
          EnsureTextResult();
        }
      }

      inputEditor.Editor.Focus();

      UpdateTitle(result.Title);
    }
    #endregion

    private void UpdateTitle(string resultTitle)
    {
      var name = string.Empty;
      if (string.IsNullOrWhiteSpace(inputEditor.Document.FileName))
      {
        name += resultTitle ?? "";
      }
      else
      {
        name += Path.GetFileName(inputEditor.Document.FileName);
      }

      if (_proxy != null && _proxy.ConnData != null && !string.IsNullOrWhiteSpace(_proxy.ConnData.ConnectionName))
      {
        if (string.IsNullOrWhiteSpace(name))
        {
          name = _proxy.ConnData.ConnectionName;
        }
        else
        {
          name += " (" + _proxy.ConnData.ConnectionName + ")";
        }
      }

      if (string.IsNullOrWhiteSpace(name))
      {
        this.Text = "Innovator Admin";
      }
      else
      {
        this.Text = name + " [Innovator Admin]";
      }
    }

    private QueryParameter GetCreateParameter(string name)
    {
      QueryParameter result;
      if (_paramCache.TryGetValue(name, out result))
        return result;
      result = new QueryParameter() { Name = name };
      _paramCache[name] = result;
      return result;
    }

    private void ChangeConnection(Control active)
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

    private void tbcOutputView_SelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        EnsureDataTable();
        EnsureTextResult();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
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

    private void ChangeSoapAction(Control active)
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
    #region Copy Actions

    private void mniTableCopyWithoutHeader_Click(object sender, EventArgs e)
    {
      try
      {
        var grid = tbcOutputView.SelectedTab.Controls.OfType<DataGridView>().Single();
        DataGridViewClipboardCopyMode oldMode = grid.ClipboardCopyMode;
        grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
        Clipboard.SetDataObject(grid.GetClipboardContent());
        grid.ClipboardCopyMode = oldMode;
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniTableCopyWithHeader_Click(object sender, EventArgs e)
    {
      try
      {
        var grid = tbcOutputView.SelectedTab.Controls.OfType<DataGridView>().Single();
        DataGridViewClipboardCopyMode oldMode = grid.ClipboardCopyMode;
        bool oldHeaders = grid.RowHeadersVisible;
        grid.RowHeadersVisible = false;
        grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
        Clipboard.SetDataObject(grid.GetClipboardContent());
        grid.ClipboardCopyMode = oldMode;
        grid.RowHeadersVisible = oldHeaders;

      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    #endregion

    #region Table Handling

    private void mniResetChanges_Click(object sender, EventArgs e)
    {
      try
      {
        _outputSet.RejectChanges();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniSave_Click(object sender, EventArgs e)
    {
      try
      {
        var grid = tbcOutputView.SelectedTab.Controls.OfType<DataGridView>().Single();
        this.Validate();
        var aml = GetTableChangeAml((DataTable)grid.DataSource);
        if (!string.IsNullOrEmpty(aml))
        {
          this.SoapAction = "ApplyAML";
          var result = Submit(aml, OutputType.None);
          if (result != null)
            result.Done(r =>
            {
              try
              {
                using (var reader = r.GetTextSource().CreateReader())
                using (var xml = XmlReader.Create(reader))
                {
                  while (xml.Read())
                  {
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                      if (xml.LocalName == "Fault" && xml.Prefix == "SOAP-ENV")
                        return;
                      if (xml.LocalName == "Result" || xml.LocalName == "Item")
                        break;
                    }
                  }
                }
              }
              catch (XmlException) { }
              ((DataTable)grid.DataSource).AcceptChanges();
            });
        }
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
        var grid = tbcOutputView.SelectedTab.Controls.OfType<DataGridView>().Single();
        this.Validate();
        var aml = GetTableChangeAml((DataTable)grid.DataSource);
        if (!string.IsNullOrEmpty(aml))
          Clipboard.SetText(aml);
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
            System.IO.File.WriteAllText(dialog.FileName, GetTableChangeAml((DataTable)tbcOutputView.SelectedTab.Controls.OfType<DataGridView>().Single().DataSource));
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniTableToFile_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new SaveFileDialog())
        {
          if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
          {
                var grid = tbcOutputView.SelectedTab.Controls.OfType<DataGridView>().Single();
         
                int columnCount = grid.Columns.Count;
                string columnNames = "";
                string outputCsv ="";

            using (System.IO.StreamWriter file =
                  new System.IO.StreamWriter(dialog.FileName))
            {
              for (int i = 0; i < columnCount; i++)
              {
                columnNames += grid.Columns[i].HeaderText.ToString() + "\t";
              }
              outputCsv += columnNames;
              file.WriteLine(outputCsv);
              outputCsv = "";
              for (int i = 1; (i - 1) < grid.Rows.Count; i++)
              {
                for (int j = 0; j < columnCount; j++)
                {
                  outputCsv += grid.Rows[i - 1].Cells[j].Value.ToString() + "\t";
                }
                file.WriteLine(outputCsv);
                outputCsv = "";
              }

            }   
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
        window.inputEditor.Text = GetTableChangeAml((DataTable)tbcOutputView.SelectedTab.Controls.OfType<DataGridView>().Single().DataSource);
        window.SoapAction = this.SoapAction;
        window.Show();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private string GetTableChangeAml(DataTable table)
    {
      var arasProxy = _proxy as ArasEditorProxy;
      var context = arasProxy == null
        ? ElementFactory.Local.LocalizationContext
        : arasProxy.Connection.AmlContext.LocalizationContext;

      var changes = table.GetChanges(DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);
      if (changes == null)
        return string.Empty;

      var settings = new System.Xml.XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";

      var types = table.AsEnumerable()
        .Select(r => r.CellValue(Extensions.AmlTable_TypeName).ToString())
        .Where(t => !string.IsNullOrEmpty(t))
        .Distinct().ToList();
      var singleType = types.Count == 1 ? types[0] : null;
      object newValue;

      using (var writer = new System.IO.StringWriter())
      using (var xml = XmlWriter.Create(writer, settings))
      {
        xml.WriteStartElement("AML");
        foreach (var row in changes.AsEnumerable())
        {
          xml.WriteStartElement("Item");
          xml.WriteAttributeString("type", singleType ?? row.CellValue(Extensions.AmlTable_TypeName).ToString());
          xml.WriteAttributeString("id", row.CellIsNull("id")
            ? Guid.NewGuid().ToString("N").ToUpperInvariant()
            : row.CellValue("id").ToString());

          switch (row.RowState)
          {
            case DataRowState.Added:
              xml.WriteAttributeString("action", "add");
              foreach (var column in changes.Columns.OfType<DataColumn>())
              {
                if (!column.ColumnName.Contains('/')
                  && column.ColumnName != Extensions.AmlTable_TypeName
                  && !row.IsNull(column))
                {
                  xml.WriteElementString(column.ColumnName, context.Format(row[column]));
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
                  && column.ColumnName != Extensions.AmlTable_TypeName
                  && (IsChanged(row, column, out newValue) || column.ColumnName == "related_id" || column.ColumnName == "source_id"))
                {
                  xml.WriteElementString(column.ColumnName, context.Format(newValue));
                }
              }
              break;
          }
          xml.WriteEndElement();
        }
        xml.WriteEndElement();
        xml.Flush();
        return writer.ToString() ?? string.Empty;
      }
    }


    private bool IsChanged(DataRow row, DataColumn col, out object newValue)
    {
      newValue = null;
      if (!row.HasVersion(DataRowVersion.Original) || !row.HasVersion(DataRowVersion.Current))
        return false;
      var orig = row[col, DataRowVersion.Original];
      var curr = row[col, DataRowVersion.Current];

      if (orig == DBNull.Value && curr == DBNull.Value)
        return false;

      newValue = curr;
      if (orig == DBNull.Value && curr == DBNull.Value)
        return true;

      return !orig.Equals(curr);
    }

    #endregion

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

    //private void mniTimeZone_Click(object sender, EventArgs e)
    //{
    //  try
    //  {
    //    using (var dialog = new FilterSelect<string>())
    //    {
    //      dialog.DataSource = TimeZoneInfo.GetSystemTimeZones().Select(t => t.Id).ToList();
    //      dialog.Message = "Select a time zone";
    //      if (dialog.ShowDialog(this) ==
    //        DialogResult.OK && dialog.SelectedItem != null)
    //      {
    //        _timeZone = dialog.SelectedItem;
    //        mniTimeZone.ShortcutKeyDisplayString = _timeZone;
    //      }
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    Utils.HandleError(ex);
    //  }
    //}

    //private void mniLocale_Click(object sender, EventArgs e)
    //{
    //  try
    //  {
    //    using (var dialog = new FilterSelect<string>())
    //    {
    //      dialog.DataSource = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
    //        .Select(c => c.Name).ToList();
    //      dialog.Message = "Select a locale";
    //      if (dialog.ShowDialog(this) ==
    //        DialogResult.OK && dialog.SelectedItem != null)
    //      {
    //        _locale = dialog.SelectedItem;
    //        mniLocale.ShortcutKeyDisplayString = _locale;
    //      }
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    Utils.HandleError(ex);
    //  }
    //}

    private void exploreButton_Click(object sender, EventArgs e)
    {
      try
      {
        if (_proxy != null && _proxy.ConnData != null)
        {
          var connData = _proxy.ConnData;
          connData.Explore();
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }


    private void treeItems_CellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
    {
      try
      {
        var node = e.Model as IEditorTreeNode;
        if (node != null)
        {
          var scripts = node.GetScripts();
          if (scripts.Any())
          {
            var con = new ContextMenuStrip();
            EditorScript.BuildMenu(con.Items, scripts, Execute);
            con.Show(treeItems.PointToScreen(e.Location));
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    public async Task Execute(IEditorScript script)
    {
      this.SoapAction = script.Action;
      var text = await script.GetScript();
      inputEditor.Document.Insert(0, text + Environment.NewLine + Environment.NewLine);
      if (script.AutoRun)
        Submit(text, script.PreferredOutput);
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

    private void treeItems_ModelDoubleClick(object sender, ModelDoubleClickEventArgs e)
    {
      try
      {
        var node = e.Model as IEditorTreeNode;
        if (node != null && node.GetScripts().Any())
        {
          var script = node.GetScripts().First();
          Execute(script);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
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
        Properties.Settings.Default.Reload();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnInstall_Click(object sender, EventArgs e)
    {
      InstallCommand();
    }

    private void mniInstall_Click(object sender, EventArgs e)
    {
      InstallCommand();
    }

    private void InstallCommand()
    {
      try
      {
        var main = new Main();
        main.GoToStep(new InstallSource());
        main.Show();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnCreate_Click(object sender, EventArgs e)
    {
      CreateCommand();
    }

    private void mniCreate_Click(object sender, EventArgs e)
    {
      CreateCommand();
    }

    private void CreateCommand()
    {
      try
      {
        var main = new Main();
        var connSelect = new ConnectionSelection();
        connSelect.MultiSelect = false;
        connSelect.GoNextAction = () => main.GoToStep(new ExportSelect());
        main.GoToStep(connSelect);
        main.Show();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }


    private void btnCompare_Click(object sender, EventArgs e)
    {
      CompareCommand();
    }

    private void mniCompare_Click(object sender, EventArgs e)
    {
      CompareCommand();
    }

    private void CompareCommand()
    {
      try
      {
        var main = new Main();
        var compareSel = new CompareSelect();
        main.GoToStep(compareSel);
        main.Show();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private async void lnkCreateModelFiles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {

    }

    private void mniColumns_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new Dialog.ColumnSelect())
        {
          var source = ((ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent()).SourceControl;
          dialog.DataSource = (DataGridView)source;
          dialog.ShowDialog(this);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    public Response GetResponse(NancyContext context, string rootPath)
    {
      if (context.Request.Method != "GET" || _result == null)
        return new Response().WithStatusCode(HttpStatusCode.InternalServerError);

      if (string.Equals(context.Request.Url.Path, GetReportUri().LocalPath))
      {
        var resp = new Response().WithStatusCode(HttpStatusCode.OK);
        resp.ContentType = "text/html";
        resp.Contents = s =>
        {
          using (var writer = new StreamWriter(s))
          {
            writer.Write(_result.Html);
          }
        };
        return resp;
      }
      else
      {
        var parts = context.Request.Url.Path.Split('/');
        if (parts.Length > 4 && parts[2] == "ProcessMapXml")
        {
          var type = parts[3];
          var id = parts[4];
          var cmd = _proxy.NewCommand();
          switch (type.ToLowerInvariant())
          {
            case "life cycle map":
              cmd.WithAction("ApplyItem")
                .WithQuery("<Item type='Life Cycle Map' action='get' id='@id' levels='1'></Item>")
                .WithParam("id", id);
              var resultObj = _proxy.Process(cmd, false, null).Wait();
              var aml = ElementFactory.Local;
              var map = aml.FromXml(resultObj.GetTextSource().CreateReader()).AssertItem();

              break;
          }
        }
        else
        {
          var arasProxy = _proxy as ArasEditorProxy;
          if (arasProxy != null && arasProxy.Connection != null)
          {
            var reportUrlBase = GetReportUri().LocalPath;
            var idx = reportUrlBase.IndexOf("/Client/") + 8;
            var relativeUrl = "../" + context.Request.Url.Path.Substring(idx);
            var absUrl = arasProxy.Connection.MapClientUrl(relativeUrl);
            var pResp = _webService.GetAsync(absUrl).Result;
            var resp = new Response().WithStatusCode((int)pResp.StatusCode);
            resp.ContentType = pResp.Headers.GetValues("Content-Type").FirstOrDefault();
            resp.Contents = s => pResp.Content.ReadAsStreamAsync().Result.CopyTo(s);
            return resp;
          }
        }
      }

      return new Response().WithStatusCode(HttpStatusCode.NotFound);
    }

    private Uri GetReportUri()
    {
      return new Uri("http://localhost:" + Program.PortNumber + "/" + Uid + "/Client/Scripts/report.html");
    }

    public void UpdateCheckComplete(Version latestVersion)
    {
      try
      {
        _updateCheckComplete = true;
        var currVer = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        if (latestVersion == default(Version))
        {
          this.lblVersion.Text = string.Format("v{0} (No updates available)", currVer);
        }
        else
        {
          var newVer = latestVersion.ToString();

          if (newVer != currVer)
          {
            this.lblVersion.Text = string.Format("v{0} (Restart to install v{1}!)", currVer, newVer);
          }
          else
          {
            this.lblVersion.Text = string.Format("v{0} (No updates available)", currVer);
          }
        }
      }
      catch (Exception) { }
    }

    public void UpdateCheckProgress(int progress)
    {
      try
      {
        if (!_updateCheckComplete)
        {
          var currVer = Assembly.GetExecutingAssembly().GetName().Version.ToString();
          this.lblVersion.Text = string.Format("v{0} (Checking updates: {1}%)", currVer, progress);
        }
      }
      catch (Exception) { }
    }

    private void conTable_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
      try
      {
        if (_proxy == null)
          return;

        var grid = (DataGridView)((ContextMenuStrip)sender).SourceControl;
        var sel = new DataGridViewSelection(grid);

        var dataRows = sel.Rows.Where(r => r.DataBoundItem is DataRowView)
          .Select(r => ((DataRowView)r.DataBoundItem).Row)
          .OfType<DataRow>()
          .Concat(sel.Rows.Where(r => r.IsNewRow)
            .Select(r => ((DataTable)r.DataGridView.DataSource).NewRow()))
          .ToArray();
        var scripts = _proxy.GetHelper().GetScripts(dataRows, sel.ColumnPropertyName);

        conTable.Items.Clear();
        EditorScript.BuildMenu(conTable.Items, scripts, Execute);
        if (scripts.Any())
          conTable.Items.Add(new ToolStripSeparator());
        conTable.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
          this.mniColumns,
          new ToolStripSeparator(),
          this.mniTableCopyActions,
          new ToolStripSeparator(),
          this.mniSaveTableToFile,
          new ToolStripSeparator(),
          this.mniSaveTableEdits,
          this.mniScriptEdits,
          this.mniResetChanges});
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniRunBatch_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new Dialog.SettingsDialog())
        {
          dialog.Filter.Add(s => s.BatchSize);
          dialog.Filter.Add(s => s.ThreadCount);
          dialog.DataSource = Settings.Current;
          dialog.Message = "Please verify the batch size and thread count settings";
          if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            return;
        }
        Submit(inputEditor.Helper.GetCurrentQuery(inputEditor.Document, inputEditor.Editor.CaretOffset)
          ?? inputEditor.Text, OutputType.Any, Settings.Current.BatchSize, Settings.Current.ThreadCount);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    protected override void OnDpiChanged(float scale, float oldScale)
    {
      var size = (int)(3 * scale);
      tblMain.ColumnStyles[0].Width = size;
      tblMain.ColumnStyles[tblMain.ColumnStyles.Count - 1].Width = size;
      tblMain.RowStyles[0].Height = size;
      tblMain.RowStyles[tblMain.RowStyles.Count - 1].Height = size;
      btnInstall.MinimumSize = new Size((int)(120 * scale), (int)(40 * scale));
      btnCreate.MinimumSize = btnInstall.MinimumSize;
    }

    private static BindingList<string> _recentDocs = new BindingList<string>();
    private static object _lock = new object();

    private void AddRecentDocument(string path)
    {
      try
      {
        lock (_lock)
        {
          _recentDocs.RaiseListChangedEvents = false;
          _recentDocs.Remove(path);
          _recentDocs.Insert(0, path);
          while (_recentDocs.Count > 10)
          {
            _recentDocs.RemoveAt(_recentDocs.Count - 1);
          }
        }
      }
      finally
      {
        _recentDocs.RaiseListChangedEvents = true;
        _recentDocs.ResetBindings();
      }
      Properties.Settings.Default.RecentDocument = string.Join("|", _recentDocs);
      Properties.Settings.Default.Save();
    }
    private void BuildRecentDocsMenu()
    {
      if (!mniFile.DropDownItems.OfType<RecentDocItem>().Any())
      {
        var start = mniFile.DropDownItems.IndexOf(mniRecentDocsStart) + 1;
        for (var i = 0; i < 10; i++)
        {
          mniFile.DropDownItems.Insert(start, new RecentDocItem(this) { Index = i + 1 });
          start++;
        }
      }

      var menuItems = mniFile.DropDownItems.OfType<RecentDocItem>().ToArray();

      for (var i = 0; i < Math.Min(menuItems.Length, _recentDocs.Count); i++)
      {
        menuItems[i].Path = _recentDocs[i];
      }
    }

    private void OpenFile(FullEditor control, string path)
    {
      lblProgress.Text = "Opening file...";
      control.OpenFile(path).ContinueWith(t =>
      {
        if (t.IsCanceled)
          lblProgress.Text = "";
        else if (t.IsFaulted)
          lblProgress.Text = t.Exception.Message;
        else
        {
          lblProgress.Text = "File opened";
          AddRecentDocument(path);
        }
        UpdateTitle(null);
      }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private class RecentDocItem : ToolStripMenuItem
    {
      private EditorWindow _parent;
      private string _path;
      private int _index;

      public RecentDocItem(EditorWindow parent)
      {
        _parent = parent;
        this.Visible = false;
      }

      public string Path
      {
        get { return _path; }
        set
        {
          _path = value;
          UpdateText();
          this.Visible = !string.IsNullOrEmpty(_path);
        }
      }
      public int Index
      {
        get { return _index; }
        set
        {
          _index = value;
          UpdateText();
        }
      }

      private void UpdateText()
      {
        this.Text = this.Index + ": "
          + (string.IsNullOrEmpty(_path) ? "" : Utils.Ellipsis(_path, 300, 9, TextFormatFlags.PathEllipsis).Replace("&", "&&"));
      }

      protected override void OnClick(EventArgs e)
      {
        try
        {
          _parent.OpenFile(_parent.inputEditor, this.Path);
        }
        catch (Exception ex)
        {
          Utils.HandleError(ex);
          throw;
        }
      }
    }

    private void lnkGitMergeHelper_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {

    }

    private class MergeSettings
    {
      [DisplayName("Continue Last Merge")]
      public bool ContinueLast { get; set; }
      [DisplayName("Git Repository Path"), ParamControl(typeof(Editor.FilePathControl))]
      public string RepoPath { get; set; }
      [DisplayName("Local Branch Name")]
      public string LocalBranch { get; set; }
      [DisplayName("Remote Branch Name")]
      public string RemoteBranch { get; set; }
    }

    private void lnkWriteMergeScripts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {

    }

    private class ScriptWriterSettings
    {
      [DisplayName("Git Repository Path"), ParamControl(typeof(Editor.FilePathControl))]
      public string RepoPath { get; set; }
      [DisplayName("Initial Commit")]
      public string InitCommit { get; set; }
      [DisplayName("Destination Commit")]
      public string DestCommit { get; set; }
      [DisplayName("Script Save Directory"), ParamControl(typeof(Editor.FilePathControl))]
      public string SaveDirectory { get; set; }
    }

    private void splitViewHorizontally()
    {
      splitEditors.Orientation = Orientation.Horizontal;
      splitEditors.SplitterDistance = splitEditors.Size.Height / 2;
    }

    private void splitViewVertically()
    {
      splitEditors.Orientation = Orientation.Vertical;
      splitEditors.SplitterDistance = splitEditors.Size.Width / 2;
    }

    private void mniGitMergeHelper_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new Dialog.ConfigDialog<MergeSettings>())
        {
          var settings = new MergeSettings();
          settings.ContinueLast = true;
          dialog.DataSource = settings;
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            var mergeOp = new GitRepo(settings.RepoPath).GetMerge(settings.LocalBranch, settings.RemoteBranch);
            var main = new Main();
            var step = new MergeInterface();
            step.ContinueLastMerge = settings.ContinueLast;
            main.GoToStep(step.Initialize(mergeOp));
            main.Show();
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private async void mniCreateModelFiles_Click(object sender, EventArgs e)
    {
      try
      {
        var output = new OutputModelClasses(_proxy.ConnData.ArasLogin());
        await output.Run();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniMergeScripts_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new Dialog.ConfigDialog<ScriptWriterSettings>())
        {
          var settings = new ScriptWriterSettings();
          dialog.DataSource = settings;
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            var repo = new GitRepo(settings.RepoPath);
            var initDir = repo.GetDirectory(new GitDirectorySearch() { Sha = settings.InitCommit });
            var destDir = repo.GetDirectory(new GitDirectorySearch() { Sha = settings.DestCommit });

            var manifestPath = Path.Combine(settings.SaveDirectory, "MergeScript.innpkg");
            var pkg = new InnovatorPackageFolder(manifestPath);
            ProgressDialog.Display(this, d =>
            {
              var processor = new MergeProcessor()
              {
                SortDependencies = true
              };
              processor.ProgressChanged += (s, ev) => d.SetProgress(ev.Progress);
              var script = processor.Merge(initDir, destDir);
              pkg.Write(script);
            });
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
