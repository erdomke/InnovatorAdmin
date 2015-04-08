using Aras.AutoComplete;
using Aras.IOM;
using Aras.Tools.InnovatorAdmin.Connections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Linq;

namespace Aras.Tools.InnovatorAdmin
{
  public partial class Editor : Form
  {
    private object _current;
    private Innovator _inn;
    private DataTable _outputTable;
    private XmlDocument _outputXml;

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

    public Editor()
    {
      InitializeComponent();

      cmbSoapAction.SelectedItem = "ApplyAML";
    }

    public void SetConnection(Innovator inn, string name = null)
    {
      if (inn == null) throw new ArgumentNullException("inn");
      _inn = inn;
      inputEditor.InitializeConnection(ApplyAction);
      lblConnectionName.Text = name ?? inn.getConnection().GetDatabaseName();
    }
    public void SetConnection(ConnectionData conn)
    {
      string msg;
      var inn = ConnectionEditor.Login(conn, out msg);
      if (inn == null)
      {
        MessageBox.Show(msg);
      }
      else
      {
        _inn = inn;
        inputEditor.InitializeConnection(ApplyAction);
      }
      lblConnectionName.Text = conn.ConnectionName;
    }

    protected override void OnLoad(EventArgs e)
    {
      try
      {
        base.OnLoad(e);
        btnOk.Visible = this.Modal;
        btnCancel.Visible = this.Modal;

        lblConnection.Visible = _inn == null;
        lblConnectionName.Visible = lblConnection.Visible;
        btnEditConnections.Visible = lblConnection.Visible;

        if (_inn == null)
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
      var output = new XmlDocument();
      output.AppendChild(output.CreateElement("Item"));
      var inDoc = input as XmlDocument;
      if (inDoc == null && input == input.OwnerDocument.DocumentElement) inDoc = input.OwnerDocument;
      if (inDoc == null)
      {
        inDoc = new XmlDocument();
        inDoc.LoadXml(input.OuterXml);
      }

      _inn.getConnection().CallAction(action, inDoc, output);
      return output.DocumentElement;
    }

    private void EnsureDataTable()
    {
      if (_outputXml != null && _outputTable == null && tbcOutputView.SelectedTab == pgTableOutput)
      {
        _outputTable = Extensions.GetItemTable(_outputXml);
        dgvItems.DataSource = _outputTable;
      }
    }

    /// <summary>
    /// Takes an XML document and renders it as a formatted (indented) string
    /// </summary>
    /// <param name="doc">XML Document</param>
    /// <returns>Formatted (indented) XML string</returns>
    private Exception IndentXml(XmlDocument doc, out string formattedString)
    {
      try
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
            // write dom xml to the xmltextwriter
            doc.WriteContentTo(xmlWriter);
            xmlWriter.Flush();
            formattedString = writer.ToString();
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
        formattedString = string.Empty;
        return ex;
      }
      return null;
    }

    private void Submit(string query)
    {
      if (_inn != null)
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

          XmlDocument input = new XmlDocument();
          input.LoadXml(_amlText);

          _outputTable = null;
          dgvItems.DataSource = null;
          callAction.RunWorkerAsync(new AmlAction()
          {
            SoapAction = cmbSoapAction.SelectedItem.ToString(),
            Aml = input
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
      var output = new XmlDocument();
      _inn.getConnection().CallAction(action.SoapAction, action.Aml, output);
      e.Result = output;
    }

    private void callAction_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      try
      {
        var output = e.Result as XmlDocument;
        if (output != null)
        {
          var node = output.DocumentElement;
          while (node != null && node.LocalName != "Item") node = node.ChildNodes.OfType<XmlElement>().FirstOrDefault();
          
          if (node == null)
          {
            lblItems.Text = "No items found.";
          }
          else
          {
            lblItems.Text = string.Format("{0} item(s) found.", node.ParentNode.ChildNodes.OfType<XmlElement>().Where(n => n.LocalName == "Item").Count());
          }

          string viewerText;
          if (IndentXml(output, out viewerText) == null)
          {
            outputEditor.Text = viewerText;
          }
          else
          {
            outputEditor.Text = output.OuterXml;
          }
          _outputXml = output;
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
      using (var dialog = new Editor())
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
      public string SoapAction { get; set; }
      public XmlDocument Aml { get; set; }
    }

    private void inputEditor_RunRequested(object sender, RunRequestedEventArgs e)
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
