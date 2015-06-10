using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Collections;

namespace Aras.Tools.InnovatorAdmin.Controls
{
  public partial class ImportMapping : UserControl, IWizardStep
  {
    private const int PageCount = 50;

    private Cache _cache;
    private static Property _emptyProperty = new Property("string");
    private int _extractorCount = -1;
    private static List<Property> _fileProperties = new List<Property>() { new Property("string") { Name = "actual_filename", Label = "Full Path" } };
    private BindingList<DataMapping> _mappings = new BindingList<DataMapping>();
    private XNamespace _nsXsl = "http://www.w3.org/1999/XSL/Transform";
    private DataTableWriter _tableWriter = new DataTableWriter();
    private IWizard _wizard;
    private XmlDataWriter _xmlWriter = new XmlDataWriter();


    public IDataExtractor Extractor { get; set; }

    public ImportMapping()
    {
      InitializeComponent();

      xsltEditor.Helper = new Editor.ImportXsltHelper();

      string xslt = null;
      if (File.Exists(Utils.GetAppFilePath(AppFileType.XsltAutoSave)))
        xslt = File.ReadAllText(Utils.GetAppFilePath(AppFileType.XsltAutoSave));
      if (string.IsNullOrWhiteSpace(xslt))
        xslt = Properties.Resources.Xslt_BaseImport;
      xsltEditor.Text = xslt;
    }

    public void Configure(IWizard wizard)
    {
      _wizard = wizard;
      _wizard.NextEnabled = true;
      _wizard.NextLabel = "Import";
      _wizard.Message = "Configure how the data should be imported.";
      _cache = new Cache(new Connection(_wizard.Connection));
      ConfigureUi((int)nudBatchSize.Value);
    }

    public void GoNext()
    {
      AutoSaveXslt();
      var xslt = xsltEditor.Text;
      var prog = new ImportProgress();
      prog.MethodInvoke = i =>
      {
        i.ProcessImport(this.Extractor, xslt, (int)nudBatchSize.Value);
      };
      _wizard.GoToStep(prog);
    }

    private void AddMapping(DataMapping mapping)
    {
      var i = 0;
      while (i < _mappings.Count
        && (_mappings[i].Type.Level < mapping.Type.Level
          || ((_mappings[i].Property ?? _emptyProperty).ToString().CompareTo((mapping.Property ?? _emptyProperty).ToString()) < 0
            && _mappings[i].Type == mapping.Type))) i++;
      if (i + 1 < _mappings.Count && (_mappings[i + 1].Property == mapping.Property || _mappings[i + 1].Property == null))
      {
        _mappings[i + 1] = mapping;
      }
      else
      {
        _mappings.Insert(i, mapping);
      }
    }

    private void AutoSaveXslt()
    {
      File.WriteAllText(Utils.GetAppFilePath(AppFileType.XsltAutoSave), xsltEditor.Text);
    }

    private string BuildXslt()
    {
      XElement tree;

      if (chkPreventDuplicateFiles.Checked && _mappings.Any(m => m.Type.ItemType.Name == "File" && m.Property == _fileProperties[0]))
      {
        if (chkPreventDuplicateDocs.Checked && _mappings.Any(m => m.Type.ItemType.Name == "Document"))
        {
          tree = XElement.Parse(Properties.Resources.Xslt_DocumentMergeImport);
          var template = tree.Descendants(_nsXsl + "template").Where(e => e.AttributeValue("name") == "RenderAml").First();
          var choose = template.Element(_nsXsl + "choose");
          choose.Remove();
          var current = PopulateTree(template, m =>
          {
            switch (m.Type.ItemType.Name)
            {
              case "Document":
              case "Document File":
              case "File":
                return choose.Descendants("Item").Where(e => e.AttributeValue("type") == m.Type.ItemType.Name && e.AttributeValue("action") == "add");
              default:
                return null;
            }
          }, m =>
          {
            switch (m.Type.ItemType.Name)
            {
              case "Document":
              case "Document File":
              case "File":
                return "$row/" + m.Value;
              default:
                return m.Value;
            }
          });
          current.Add(choose);
        }
        else
        {
          tree = XElement.Parse(Properties.Resources.Xslt_FileMergeImport);
          var template = tree.Descendants(_nsXsl + "template").Where(e => e.AttributeValue("name") == "RenderAml").First();
          var choose = template.Element(_nsXsl + "choose");
          choose.Remove();
          var current = PopulateTree(template, m =>
          {
            switch (m.Type.ItemType.Name)
            {
              case "File":
                return choose.Descendants("Item").Where(e => e.AttributeValue("type") == m.Type.ItemType.Name && e.AttributeValue("action") == "add");
              default:
                return null;
            }
          }, m =>
          {
            switch (m.Type.ItemType.Name)
            {
              case "File":
                return "$row/" + m.Value;
              default:
                return m.Value;
            }
          });
          current.Add(choose);
        }
      }
      else
      {
        tree = XElement.Parse(Properties.Resources.Xslt_BaseImport);
        PopulateTree(tree.Descendants(_nsXsl + "for-each").First(), null, null);
      }

      return tree.ToString();
    }

    private IList GetItemTypeList(MappedType mappedType = null)
    {
      if (_mappings.Any() || mappedType != null)
      {
        var mappings = _mappings.Select(m => m.Type).Distinct();
        if (mappedType != null) mappings = mappings.Concat(Enumerable.Repeat(mappedType, 1));
        var itemTypes = mappings.OrderBy(m => m.Level).ToList();
        itemTypes.InsertRange(0, _cache.GetParents(itemTypes.First().ItemType).Select(i => new MappedType(i)).OrderBy(m => m.ItemType.ToString()));
        itemTypes.AddRange(GetUiChildren(itemTypes.Last().ItemType).Select(i => new MappedType(i) { Level = itemTypes.Last().Level + 1 }).OrderBy(m => m.ItemType.ToString()));
        return itemTypes;
      }
      else
      {
        return _cache.ItemTypes.OrderBy(i => i.ToString()).ToList();
      }
    }

    private IEnumerable<ItemType> GetUiChildren(ItemType itemType)
    {
      if (itemType.Name == "File") return Enumerable.Empty<ItemType>();
      return _cache.GetChildren(itemType);
    }
    private void ConfigureUi(int count = PageCount)
    {
      this.Extractor.Write(count, _tableWriter, _xmlWriter);
      if (gridPreview.DataSource == null) gridPreview.DataSource = _tableWriter.Table;
      xmlEditor.Text = _xmlWriter.ToString();
      btnLoadMore.Enabled = !this.Extractor.AtEnd;
      if (_extractorCount < 0 && !countWorker.IsBusy) countWorker.RunWorkerAsync();
      UpdateCountLabel();

      _cache.Build();
      cboItemTypes.DataSource = GetItemTypeList();
      ResetMapping();

      dgvMappings.DataSource = _mappings;
    }

    private ItemType ExtractItemType()
    {
      var itemType = cboItemTypes.SelectedItem as ItemType;
      if (itemType == null)
      {
        var mappedType = cboItemTypes.SelectedItem as MappedType;
        if (mappedType == null) return null;
        itemType = mappedType.ItemType;
      }
      return itemType;
    }

    private XElement PopulateTree(XElement current, Func<DataMapping, IEnumerable<XElement>> parentGetter, Func<DataMapping, string> calculatedGetter)
    {
      calculatedGetter = calculatedGetter ?? (Func<DataMapping, string>)(m => m.Value);

      MappedType lastType = null;
      IEnumerable<XElement> parents = Enumerable.Repeat(current, 1);
      foreach (var mapping in _mappings)
      {
        if (mapping.Type != lastType)
        {
          if (lastType != null)
          {
            if (mapping.Type.ItemType.Relationship == null)
            {
              current = current.AddAndReturn(new XElement("related_id"));
            }
            else
            {
              current = current.AddAndReturn(new XElement("Relationships"));
            }
          }

          parents = null;
          if (parentGetter != null) parents = parentGetter.Invoke(mapping);

          if (parents == null)
          {
            current = current.AddAndReturn(new XElement("Item",
              new XAttribute("type", mapping.Type.ItemType.Name),
              new XAttribute("typeid", mapping.Type.ItemType.Id)
            ));
            parents = Enumerable.Repeat(current, 1);
          }

          var id = _mappings.FirstOrDefault(m => m.Type == mapping.Type && m.Property != null && m.Property.Name == "id");
          if (id != null)
          {
            foreach (var parent in parents)
            {
              current.Add(new XAttribute("id", id.IsCalculated ? "{" + id.Value + "}" : id.Value));
            }
          }
        }

        if (mapping.Property != null)
        {
          foreach (var parent in parents)
          {
            switch (mapping.Property.Type)
            {
              case PropertyType.Item:
                parent.Add(new XElement(mapping.Property.Name,
                  new XElement("Item", new XAttribute("type", mapping.Property.ItemSource.Name), new XAttribute("action", "get"),
                    new XElement("keyed_name",
                      mapping.IsCalculated
                      ? (object)new XElement(_nsXsl + "value-of", new XAttribute("select", calculatedGetter.Invoke(mapping)))
                      : mapping.Value
                    )
                  )
                ));
                break;
              case PropertyType.Date:
                parent.Add(new XElement(mapping.Property.Name,
                  mapping.IsCalculated
                  ? (object)new XElement(_nsXsl + "value-of", new XAttribute("select", "arasx:FormatDate(" + calculatedGetter.Invoke(mapping) + ", 's')"))
                  : (string.IsNullOrEmpty(mapping.Value) ? "" : DateTime.Parse(mapping.Value).ToString("s"))
                ));
                break;
              default:
                parent.Add(new XElement(mapping.Property.Name,
                  mapping.IsCalculated ? (object)new XElement(_nsXsl + "value-of", new XAttribute("select", calculatedGetter.Invoke(mapping))) : mapping.Value
                ));
                break;
            }
          }
        }

        lastType = mapping.Type;
      }
      return current;
    }

    private void ResetMapping()
    {
      var itemType = ExtractItemType();
      if (itemType == null)
      {
        cboProperties.DataSource = null;
        cboProperties.Enabled = false;
      }
      chkCalculated.Checked = false;
      txtValue.Text = "";
    }

    private void ResetPreview(int minCount)
    {
      this.Extractor.Reset();
      _tableWriter.Reset();
      _xmlWriter.Reset();
      gridPreview.DataSource = null;
      ConfigureUi(Math.Max(minCount, (int)nudBatchSize.Value));
    }

    private void RunTest(string query)
    {
      outputEditor.Text = ArasXsltExtensions.Transform(query, _xmlWriter.ToString(), _wizard.Connection);
    }

    private void UpdateCountLabel()
    {
      lblCount.Text = string.Format("Showing {0} of {1}", this.Extractor.NumProcessed, (_extractorCount >= 0 ? _extractorCount.ToString() : "?"));
    }
    private void btnAdd_Click(object sender, EventArgs e)
    {
      try
      {
        var property = cboProperties.SelectedItem as Property;
        var mappedType = cboItemTypes.SelectedItem as MappedType;
        if (mappedType == null)
        {
          var itemType = cboItemTypes.SelectedItem as ItemType;
          if (itemType != null)
          {
            mappedType = new MappedType(itemType) { Level = 1 };
          }
        }

        if (mappedType != null && property != null && (property == _emptyProperty || !string.IsNullOrEmpty(txtValue.Text)))
        {
          if (mappedType.Level == 0)
          {
            mappedType.Level = 1;
            foreach (var m in _mappings)
            {
              m.Type.Level++;
            }
          }

          var mapping = new DataMapping()
          {
            IsCalculated = chkCalculated.Checked,
            Type = mappedType,
            Property = (property == _emptyProperty ? null : property),
            Value = txtValue.Text
          };
          if (!_mappings.Any(m => m.Type == mappedType))
          {
            cboItemTypes.DataSource = GetItemTypeList(mappedType);
            cboItemTypes.SelectedItem = mapping.Type;
          }
          AddMapping(mapping);
          ResetMapping();

          xsltEditor.Text = BuildXslt();
        }
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
        var rows = dgvMappings.SelectedCells.OfType<DataGridViewCell>().Select(c => c.OwningRow).Distinct();
        if (rows.Any())
        {
          var removingItems = rows.Select(r => ((DataMapping)r.DataBoundItem)).ToList();

          // Allow removing types on the edges (top or bottom of the list, but not middle types
          // This is so the XSLT generates properly by having a full tree of types
          var typesToKeep = removingItems.Select(r => r.Type).Distinct().ToList();

          var maxLevel = _mappings.Select(m => m.Type.Level).Max();
          while (typesToKeep.Any(t => t.Level == maxLevel))
          {
            typesToKeep.RemoveFilter(t => t.Level == maxLevel);
            maxLevel--;
          }

          var minLevel = 1;
          while (typesToKeep.Any(t => t.Level == minLevel))
          {
            typesToKeep.RemoveFilter(t => t.Level == minLevel);
            minLevel++;
          }

          try
          {
            _mappings.RaiseListChangedEvents = false;

            // Reset the levels
            if (minLevel > 1)
            {
              var offset = 1 - minLevel;
              foreach (var mapping in _mappings)
              {
                mapping.Type.Level += offset;
              }
            }
            // Remove the items
            foreach (var item in removingItems)
            {
              _mappings.Remove(item);
            }
            // Re-add the types as necessary
            foreach (var type in typesToKeep)
            {
              if (!_mappings.Any(m => m.Type == type))
              {
                AddMapping(new DataMapping() { Type = type });
              }
            }
          }
          finally
          {
            _mappings.RaiseListChangedEvents = true;
            _mappings.ResetBindings();
          }

          cboItemTypes.DataSource = GetItemTypeList();
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnLoadMore_Click(object sender, EventArgs e)
    {
      ConfigureUi((int)nudBatchSize.Value);
    }

    private void btnOpen_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new OpenFileDialog())
        {
          dialog.CheckPathExists = true;
          dialog.Filter = "XML Stylesheets (*.xslt, *.xsl)|*.xslt;*.xsl";
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            xsltEditor.Text = File.ReadAllText(dialog.FileName);
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new SaveFileDialog())
        {
          dialog.Filter = "XML Stylesheets (*.xslt)|*.xslt";
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            File.WriteAllText(dialog.FileName, xsltEditor.Text);
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnTest_Click(object sender, EventArgs e)
    {
      try
      {
        RunTest(xsltEditor.Text);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void cboItemTypes_DropDown(object sender, EventArgs e)
    {
      try
      {
        if (cboItemTypes.Items.Count == _cache.ItemTypeCount)
        {
          cboItemTypes.DroppedDown = false;
          using (var dialog = new FilterSelect<ItemType>())
          {
            dialog.DataSource = (IEnumerable<ItemType>)(cboItemTypes.DataSource);
            if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedItem != null)
            {
              cboItemTypes.SelectedItem = dialog.SelectedItem;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void cboItemTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        var itemType = ExtractItemType();
        if (itemType == null)
        {
          ResetMapping();
        }
        else
        {
          cboProperties.DataSource = null;
          if (itemType.Name == "File")
          {
            cboProperties.DataSource = _fileProperties;
          }
          else
          {
            var props = new List<Property>(itemType.Properties);
            props.Insert(0, _emptyProperty);
            cboProperties.DataSource = props;
          }
          System.Diagnostics.Debug.Print("{0}", ((IList<Property>)cboProperties.DataSource).Count);
          cboProperties.Enabled = true;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void chkCalculated_CheckedChanged(object sender, EventArgs e)
    {
      try
      {
        lblValue.Text = (chkCalculated.Checked ? "Select a column above or enter an expression" : "Constant value");
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void chkPreventDuplicateFiles_CheckedChanged(object sender, EventArgs e)
    {
      try
      {
        chkPreventDuplicateDocs.Enabled = chkPreventDuplicateFiles.Checked;
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void countWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      e.Result = this.Extractor.GetTotalCount();
    }

    private void countWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      _extractorCount = (int)e.Result;
      UpdateCountLabel();
    }

    private void gridPreview_SelectionChanged(object sender, EventArgs e)
    {
      try
      {
        if (chkCalculated.Checked)
        {
          txtValue.Text = XmlDataWriter.TransformName(gridPreview.CurrentCell.OwningColumn.DataPropertyName);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void timerAutoSave_Tick(object sender, EventArgs e)
    {
      try
      {
        AutoSaveXslt();
      }
      catch (Exception) { }
    }
    private void xsltEditor_RunRequested(object sender, Editor.RunRequestedEventArgs e)
    {
      try
      {
        RunTest(e.Query);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
    #region "Helper Classes"
    private class Cache
    {
      private Dictionary<string, ItemType> _typesById;
      private Dictionary<string, ItemType> _typesByName;
      private List<RelationshipType> _relTypes;
      private Connection _conn;

      public int ItemTypeCount
      {
        get { return _typesById.Count; }
      }
      public IEnumerable<ItemType> ItemTypes
      {
        get { return _typesById.Values; }
      }

      public Cache(Connection conn)
      {
        _conn = conn;
      }

      public void Build()
      {
        _typesById = _conn.GetItems("ApplyItem", "<Item type=\"ItemType\" action=\"get\" select=\"name,label\" />")
                          .Select(i => new ItemType(this) { Id = i.Element("id", ""), Name = i.Element("name", null), Label = i.Element("label", null)})
                          .ToDictionary(i => i.Id);
        _typesByName = _typesById.Values.ToDictionary(i => i.Name);
        _relTypes = _conn.GetItems("ApplyItem", "<Item type=\"RelationshipType\" action=\"get\" select=\"source_id,related_id,relationship_id,name\" related_expand=\"0\" />")
                         .Select(i => new RelationshipType()
                         {
                           Name = i.Element("name", ""),
                           Core = ById(i.Element("relationship_id", "")),
                           Source = ById(i.Element("source_id", "")),
                           Related = ById(i.Element("related_id", ""))
                         }).ToList();
        foreach (var rel in _relTypes)
        {
          rel.Core.Relationship = rel;
        }
      }
      public IEnumerable<ItemType> GetChildren(ItemType itemType)
      {
        if (itemType.Relationship == null)
        {
          return _relTypes.Where(r => r.Source == itemType).Select(r => r.Core);
        }
        else
        {
          return Enumerable.Repeat(itemType.Relationship.Related, 1);
        }
      }
      public IEnumerable<ItemType> GetParents(ItemType itemType)
      {
        if (itemType.Relationship == null)
        {
          return _relTypes.Where(r => r.Related == itemType).Select(r => r.Core);
        }
        else
        {
          return Enumerable.Repeat(itemType.Relationship.Source, 1);
        }
      }
      public IEnumerable<Property> GetProperties(ItemType itemType)
      {
        return _conn.GetItems("ApplyItem", "<Item type=\"Property\" action=\"get\" select=\"name,label,data_type,data_source\"><source_id>" + itemType.Id + "</source_id></Item>")
                    .Select(i => new Property(i.Element("data_type", "")) {
                      Id = i.Element("id", ""),
                      Label = i.Element("label", null),
                      Name = i.Element("name", null),
                      ItemSource = ById(i.Element("data_source", ""))
                    }).OrderBy(p => p.ToString())
                    .ToList();
      }

      private ItemType ById(string id)
      {
        ItemType result;
        if (string.IsNullOrEmpty(id) || !_typesById.TryGetValue(id, out result)) return null;
        return result;
      }

      public void Serialize(DataMapping mapping)
      {

      }
    }

    private class ItemType
    {
      private Cache _cache;
      private IEnumerable<Property> _properties;

      public string Id { get; set; }
      public string Label { get; set; }
      public string Name { get; set; }

      public IEnumerable<Property> Properties
      {
        get
        {
          if (_properties == null)
          {
            _properties = _cache.GetProperties(this);
          }
          return _properties;
        }
      }
      public RelationshipType Relationship { get; set; }

      public ItemType(Cache cache)
      {
        _cache = cache;
      }

      public override string ToString()
      {
        return (string.IsNullOrEmpty(this.Label) ? this.Name : this.Label);
      }
    }
    private class MappedType
    {
      private ItemType _baseType;

      public ItemType ItemType
      {
        get { return _baseType; }
      }

      public int Level { get; set; }

      public MappedType(ItemType baseType)
      {
        _baseType = baseType;
      }

      public override string ToString()
      {
        return new string(' ', this.Level * 2) + _baseType.ToString();
      }
    }

    private class RelationshipType
    {
      public string Name { get; set; }
      public ItemType Source { get; set; }
      public ItemType Related { get; set; }
      public ItemType Core { get; set; }
    }

    private enum PropertyType
    {
      String,
      Integer,
      Decimal,
      Date,
      Item
    }

    private class Property
    {
      private PropertyType _type;

      public string Id { get; set; }
      public ItemType ItemSource { get; set; }
      public string Label { get; set; }
      public string Name { get; set; }
      public PropertyType Type { get { return _type; } }

      public Property(string dataType)
      {
        switch (dataType)
        {
          case "integer":
            _type = PropertyType.Integer;
            break;
          case "float":
          case "decimal":
            _type = PropertyType.Decimal;
            break;
          case "date":
            _type = PropertyType.Date;
            break;
          case "item":
            _type = PropertyType.Item;
            break;
          default:
            _type = PropertyType.String;
            break;
        }
      }

      public override string ToString()
      {
        return (string.IsNullOrEmpty(this.Label) ? (this.Name ?? "") : this.Label + " (" + this.Name + ")");
      }
    }

    private class DataMapping
    {
      public MappedType Type { get; set; }
      public Property Property { get; set; }
      public bool IsCalculated { get; set; }
      public string Value { get; set; }
    }
    #endregion
  }
}
