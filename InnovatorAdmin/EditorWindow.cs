using Aras.AutoComplete;
using Aras.Tools.InnovatorAdmin.Connections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Linq;
using System.Diagnostics;

namespace Aras.Tools.InnovatorAdmin
{
  public partial class EditorWindow : Form
  {
    private object _current;
    private IArasConnection _conn;
    private DataTable _outputTable;
    private string _outputXml;

    public bool AllowRun
    {
      get { return !splitEditors.Panel2Collapsed; }
      set { splitEditors.Panel2Collapsed  = !value; }
    }
    public string Aml
    {
      get { return inputEditor.Text; }
      set { inputEditor.Text = value; }
    }
    public Func<object, string> AmlGetter { get; set; }
    public Action<object, string> AmlSetter { get; set; }
    public object DataSource
    {
      get { return lstItems.DataSource; }
      set
      {
        lstItems.DataSource = value;
        splitMain.Panel1Collapsed = (value == null);
      }
    }
    public string DisplayMember
    {
      get { return lstItems.DisplayMember; }
      set { lstItems.DisplayMember = value; }
    }

    public EditorWindow()
    {
      InitializeComponent();

      inputEditor.Helper = new Editor.AmlEditorHelper();
      cmbSoapAction.SelectedItem = "ApplyAML";
    }

    public void SetConnection(IArasConnection conn, string name = null)
    {
      if (conn == null) throw new ArgumentNullException("inn");
      _conn = conn;
      ((Editor.AmlEditorHelper)inputEditor.Helper).InitializeConnection(ApplyAction);
      lblConnectionName.Text = string.Format("{0} ({1})", name ?? conn.GetDatabaseName(), conn.GetIomVersion());
    }
    public void SetConnection(ConnectionData conn)
    {
      string msg;
      var arasConn = ConnectionEditor.Login(conn, out msg);
      if (arasConn == null)
      {
        MessageBox.Show(msg);
      }
      else
      {
        _conn = arasConn;
        ((Editor.AmlEditorHelper)inputEditor.Helper).InitializeConnection(ApplyAction);
      }
      lblConnectionName.Text = string.Format("{0} ({1})", conn.ConnectionName, _conn.GetIomVersion());
    }

    protected override void OnLoad(EventArgs e)
    {
      try
      {
        base.OnLoad(e);
        btnOk.Visible = this.Modal;
        btnCancel.Visible = this.Modal;

        lblConnection.Visible = _conn == null;
        lblConnectionName.Visible = lblConnection.Visible;
        btnEditConnections.Visible = lblConnection.Visible;

        if (_conn == null)
        {
          SetConnection(ConnectionManager.Current.Library.Connections.First());
        }

      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private XmlNode ApplyAction(string action, XmlNode input)
    {
      return XmlUtils.DocFromXml(_conn.CallAction(action, input.OuterXml)).DocumentElement;
    }

    private void EnsureDataTable()
    {
      if (!string.IsNullOrEmpty(_outputXml) && _outputTable == null && tbcOutputView.SelectedTab == pgTableOutput)
      {
        var doc = new XmlDocument();
        doc.LoadXml(_outputXml);
        _outputTable = Extensions.GetItemTable(doc);
        dgvItems.DataSource = _outputTable;
      }
    }

    private string IndentXml(string xmlContent, out int itemCount)
    {
      itemCount = 0;
      char[] writeNodeBuffer = null;
      var levels = new int[64];
      int level = 0;

      using (var strReader = new StringReader(xmlContent))
      {
        using (var reader = XmlReader.Create(strReader))
        {
          using (var writer = new StringWriter())
          {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.CheckCharacters = true;
            using (var xmlWriter = XmlWriter.Create(writer, settings))
            {
              bool canReadValueChunk = reader.CanReadValueChunk;
              while (reader.Read())
              {
                switch (reader.NodeType)
                {
                  case XmlNodeType.Element:
                    if (reader.LocalName == "Item") levels[level]++;
                    xmlWriter.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                    xmlWriter.WriteAttributes(reader, false);
                    if (reader.IsEmptyElement)
                    {
                      xmlWriter.WriteEndElement();
                    }
                    else
                    {
                      level++;
                    }
                    break;
                  case XmlNodeType.Text:
                    if (canReadValueChunk)
                    {
                      if (writeNodeBuffer == null)
                      {
                        writeNodeBuffer = new char[1024];
                      }
                      int count;
                      while ((count = reader.ReadValueChunk(writeNodeBuffer, 0, 1024)) > 0)
                      {
                        xmlWriter.WriteChars(writeNodeBuffer, 0, count);
                      }
                    }
                    else
                    {
                      xmlWriter.WriteString(reader.Value);
                    }
                    break;
                  case XmlNodeType.CDATA:
                    xmlWriter.WriteCData(reader.Value);
                    break;
                  case XmlNodeType.EntityReference:
                    xmlWriter.WriteEntityRef(reader.Name);
                    break;
                  case XmlNodeType.ProcessingInstruction:
                  case XmlNodeType.XmlDeclaration:
                    xmlWriter.WriteProcessingInstruction(reader.Name, reader.Value);
                    break;
                  case XmlNodeType.Comment:
                    xmlWriter.WriteComment(reader.Value);
                    break;
                  case XmlNodeType.DocumentType:
                    xmlWriter.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                    break;
                  case XmlNodeType.Whitespace:
                  case XmlNodeType.SignificantWhitespace:
                    xmlWriter.WriteWhitespace(reader.Value);
                    break;
                  case XmlNodeType.EndElement:
                    xmlWriter.WriteFullEndElement();
                    level--;
                    break;
                }
              }
              
              xmlWriter.Flush();
            }
            itemCount = levels.FirstOrDefault(i => i > 0);
            return writer.ToString();
          }
        }
      }
    }

    private void Submit(string query)
    {
      if (_conn != null)
      {
        try
        {
          outputEditor.Text = "Processing...";
          lblItems.Text = "Processing...";
          var _amlText = query;

          if (_amlText.IndexOf("<AML>") < 0 && (string)cmbSoapAction.SelectedItem == "ApplyAML")
          {
            _amlText = "<AML>" + _amlText + "</AML>";
          }
          
          _outputTable = null;
          dgvItems.DataSource = null;
          callAction.RunWorkerAsync(new AmlAction()
          {
            SoapAction = cmbSoapAction.SelectedItem.ToString(),
            Aml = _amlText
          });
        }
        catch (Exception err)
        {
          MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }
    }

    private void btnEditConnections_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new ConnectionEditorForm())
        {
          dialog.Multiselect = false;
          if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
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
    private void callAction_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      var action = (AmlAction)e.Argument;
      action.Output = _conn.CallAction(action.SoapAction, action.Aml);
      e.Result = action;
    }

    private void callAction_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      try
      {
        var action = e.Result as AmlAction;
        if (action != null)
        {
          var milliseconds = action.Stopwatch.ElapsedMilliseconds;
          int itemCount = 0;
          string indented = action.Output;
          try
          {
            indented = IndentXml(action.Output, out itemCount);
          }
          catch (Exception ex)
          {
            MessageBox.Show(ex.Message);
          }

          if (itemCount > 0)
          {
            lblItems.Text = string.Format("{0} item(s) found in {1} ms.", itemCount, milliseconds);
          }
          else
          {
            lblItems.Text = string.Format("No items found in {0} ms.", milliseconds);
          }
          outputEditor.Text = indented;

          if (itemCount > 1 && outputEditor.Editor.LineCount > 100)
          {
            outputEditor.CollapseAll();
          }
          _outputXml = action.Output;
        }
        EnsureDataTable();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void lstItems_SelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        if (_current != null && this.AmlSetter != null) this.AmlSetter.Invoke(_current, inputEditor.Text);
        if (this.AmlGetter != null) inputEditor.Text = this.AmlGetter(lstItems.SelectedItem);
        _current = lstItems.SelectedItem;
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
        if (dialog.ShowDialog() == DialogResult.OK
          && dialog._outputTable.Columns.Contains("type")
          && dialog._outputTable.Columns.Contains("id"))
        {
          return dialog.dgvItems.SelectedRows
                       .OfType<DataGridViewRow>()
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
  }
}
