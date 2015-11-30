using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Innovator.Client;

namespace Aras.Tools.InnovatorAdmin.Controls
{
  public partial class ExportSelect : UserControl, IWizardStep
  {
    private FullBindingList<ItemReference> _availableRefs = new FullBindingList<ItemReference>();
    private InstallScript _existingScript = null;
    private Action _findAction;
    private List<ItemReference> _itemTypes;
    private string _lastQuery;
    private ItemReference _searchMessage = new ItemReference("***", "***") { KeyedName = "Enter a more restrictive search, and tap find." };
    private FullBindingList<ItemReference> _selectedRefs = new FullBindingList<ItemReference>();
    private IWizard _wizard;
    private IAsyncConnection _conn;


    public ExportSelect()
    {
      InitializeComponent();
      gridAvailable.AutoGenerateColumns = false;
      gridSelected.AutoGenerateColumns = false;
      gridAvailable.DataSource = _availableRefs;
      gridSelected.DataSource = _selectedRefs;
      _selectedRefs.ListChanged += _selectedRefs_ListChanged;

      tbcSearch.TabPages.Remove(pgResults);
    }
    public void Configure(IWizard wizard)
    {
      _wizard = wizard;
      _wizard.NextEnabled = _selectedRefs.Any();
      _wizard.NextLabel = "&Export";
      _wizard.Message = "Select the items you would like to include in the package.";
      _conn = _wizard.Connection;
    }

    public void GoNext()
    {
      if (_selectedRefs.Any())
      {
        var prog = new ProgressStep<ExportProcessor>(_wizard.ExportProcessor);
        prog.MethodInvoke = e => {
          _wizard.InstallScript = _existingScript ?? new InstallScript();
          _wizard.InstallScript.ExportUri = new Uri(_wizard.ConnectionInfo.First().Url);
          _wizard.InstallScript.ExportDb = _wizard.ConnectionInfo.First().Database;
          _wizard.InstallScript.Lines = Enumerable.Empty<InstallItem>();
          e.Export(_wizard.InstallScript, _selectedRefs);
        };
        prog.GoNextAction = () => _wizard.GoToStep(new ExportResolve());
        _wizard.GoToStep(prog);
      }
      else
      {
        MessageBox.Show(resources.Messages.SelectItemsExport);
      }
    }

    private void ApplyDefaultSort()
    {
      if (string.IsNullOrEmpty(txtFind.Text))
      {
        _availableRefs.RemoveSort();
      }
      else
      {
        _availableRefs.ApplySort((x, y) =>
        {
          var compare = SortGroup(x.KeyedName).CompareTo(SortGroup(y.KeyedName));
          if (compare == 0) compare = (x.KeyedName ?? x.Unique).CompareTo(y.KeyedName ?? y.Unique);
          return compare;
        });
      }
    }

    private void DefaultFindAction()
    {
      if (string.IsNullOrEmpty(txtFind.Text))
      {
        _availableRefs.RemoveFilter();
      }
      else
      {
        _availableRefs.ApplyFilter(r => (r.KeyedName ?? "").IndexOf(txtFind.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        ApplyDefaultSort();
      }
    }

    private void EnsureResultsTab()
    {
      if (!tbcSearch.TabPages.Contains(pgResults)) tbcSearch.TabPages.Add(pgResults);
    }

    private void FindByItem(string type)
    {
      if (!string.IsNullOrEmpty(_lastQuery) && !string.IsNullOrEmpty(txtFind.Text) && txtFind.Text.StartsWith(_lastQuery))
      {
        DefaultFindAction();
      }
      else
      {
        _availableRefs.Clear();

        var results = _conn.Apply(@"<Item type='@0' action='get' maxRecords='1000' orderBy='keyed_name' select='id,source_id,related_id'>
                                      <keyed_name condition='like'>@1</keyed_name>
                                    </Item>"
          , type, "*" + txtFind.Text + "*").Items();
        if (results.Count() >= 1000)
        {
          _availableRefs.Add(_searchMessage);
        }
        else
        {
          foreach (var result in results)
          {
            _availableRefs.Add(ItemReference.FromFullItem(result, true));
          }
          _lastQuery = txtFind.Text;
        }
      }
    }

    private IEnumerable<ItemReference> GetSelected(DataGridView grid)
    {
      return grid.SelectedRows.OfType<DataGridViewRow>().Select(r => (ItemReference)r.DataBoundItem).ToList();
    }

    private int SortGroup(string value)
    {
      if (value.StartsWith(txtFind.Text, StringComparison.OrdinalIgnoreCase)) return 1;
      return 2;
    }

    private void SelectItems()
    {
      foreach (var itemRef in GetSelected(gridAvailable))
      {
        _selectedRefs.Add(itemRef);
        _availableRefs.Remove(itemRef);
      }
    }

    private void UnselectItems()
    {
      foreach (var itemRef in GetSelected(gridSelected))
      {
        _availableRefs.Add(itemRef);
        _selectedRefs.Remove(itemRef);
      }
    }

    void _selectedRefs_ListChanged(object sender, ListChangedEventArgs e)
    {
      _wizard.NextEnabled = _selectedRefs.Any();
    }

    private void btnDbPackage_Click(object sender, EventArgs e)
    {
      try
      {
        var items = _conn.Apply(@"<Item type='PackageDefinition' action='get' select='id' />").Items();
        var refs = new List<ItemReference>();

        foreach (var item in items)
        {
          refs.Add(ItemReference.FromFullItem(item, true));
        }

        using (var dialog = new FilterSelect<ItemReference>())
        {
          dialog.DataSource = refs;
          dialog.DisplayMember = "KeyedName";
          dialog.Message = resources.Messages.PackageSelect;
          if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedItem != null)
          {
            txtFind.Text = "";
            _findAction = DefaultFindAction;
            items = _conn.Apply(@"<Item type='PackageElement' action='get' select='element_id,element_type,name' orderBy='element_type,name,element_id'>
                                    <source_id condition='in'>(select id
                                      from innovator.PACKAGEGROUP
                                      where SOURCE_ID = @0)</source_id>
                                  </Item>", dialog.SelectedItem.Unique).Items();
            _availableRefs.Clear();
            ItemReference newRef;
            foreach (var item in items)
            {
              newRef = new ItemReference()
              {
                Type = item.Property("element_type").AsString(""),
                Unique = item.Property("element_id").AsString(""),
                KeyedName = item.Property("name").AsString("")
              };
              if (!_selectedRefs.Contains(newRef)) _selectedRefs.Add(newRef);
            }

            _existingScript = _existingScript ?? new InstallScript();
            _existingScript.Title = dialog.SelectedItem.KeyedName;

            EnsureResultsTab();
            tbcSearch.SelectedTab = pgResults;
            txtFind.Focus();
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }

    }
    private void btnAmlStudio_Click(object sender, EventArgs e)
    {
      try
      {
        foreach (var newRef in EditorWindow.GetItems(_wizard.ConnectionInfo.First()))
        {
          if (!_selectedRefs.Contains(newRef)) _selectedRefs.Add(newRef);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void EnsureItemTypes()
    {
      if (_itemTypes == null)
      {
        _itemTypes = _conn.Apply("<Item type='ItemType' action='get' select='id' />")
          .Items()
          .Select(i => ItemReference.FromFullItem(i, true))
          .ToList();
      }
    }

    private void btnItem_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new FilterSelect<ItemReference>())
        {
          EnsureItemTypes();
          dialog.DataSource = _itemTypes;
          dialog.DisplayMember = "KeyedName";
          dialog.Message = resources.Messages.ItemTypeSelect;
          if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedItem != null)
          {
            _lastQuery = null;
            txtFind.Text = "";
            _availableRefs.Clear();

            var items = _conn.Apply("<Item type='@0' action='get' maxRecords='1000' orderBy='keyed_name' select='id,source_id,related_id' />", dialog.SelectedItem.KeyedName).Items();
            if (items.Count() >= 1000)
            {
              _findAction = () => FindByItem(dialog.SelectedItem.KeyedName);
              _availableRefs.Add(_searchMessage);
            }
            else
            {
              ItemReference newRef;
              _findAction = DefaultFindAction;
              foreach (var result in items)
              {
                newRef = ItemReference.FromFullItem(result, true);
                if (!_selectedRefs.Contains(newRef)) _availableRefs.Add(newRef);
              }
            }
            EnsureResultsTab();
            tbcSearch.SelectedTab = pgResults;
            txtFind.Focus();
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
    private void btnFind_Click(object sender, EventArgs e)
    {
      try
      {
        _findAction.Invoke();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnPackageFile_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new OpenFileDialog())
        {
          dialog.Filter = "Innovator Package (.innpkg)|*.innpkg|Manifest (.mf)|*.mf";
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            if (Path.GetExtension(dialog.FileName) == ".innpkg")
            {
              using (var pkg = InnovatorPackage.Load(dialog.FileName))
              {
                var installScript = pkg.Read();

                _availableRefs.Clear();
                foreach (var item in installScript.Lines.Where(l => l.Type == InstallType.Create).Select(l => l.Reference))
                {
                  if (!_selectedRefs.Contains(item)) _selectedRefs.Add(item);
                }

                _existingScript = installScript;
                _existingScript.Lines = null;
              }
            }
            else
            {
              var pkg = new ManifestFolder(dialog.FileName);
              string title;
              var doc = pkg.Read(out title);

              foreach (var item in ItemReference.FromFullItems(doc.DocumentElement, true))
              {
                if (!_selectedRefs.Contains(item)) _selectedRefs.Add(item);
              }

              _existingScript = _existingScript ?? new InstallScript();
              _existingScript.Title = title;
            }

            EnsureResultsTab();
            tbcSearch.SelectedTab = pgResults;
            txtFind.Focus();
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnSelect_Click(object sender, EventArgs e)
    {
      try
      {
        SelectItems();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnSelectAll_Click(object sender, EventArgs e)
    {
      try
      {
        _selectedRefs.AddRange(_availableRefs);
        _availableRefs.ClearVisible();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnUnselect_Click(object sender, EventArgs e)
    {
      try
      {
        UnselectItems();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnUnselectAll_Click(object sender, EventArgs e)
    {
      try
      {
        _availableRefs.AddRange(_selectedRefs);
        _selectedRefs.ClearVisible();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void gridAvailable_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      try
      {
        SelectItems();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void gridAvailable_KeyDown(object sender, KeyEventArgs e)
    {
      try
      {
        if (e.KeyCode == Keys.Enter)
        {
          SelectItems();
          txtFind.Focus();
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void gridSelected_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      try
      {
        UnselectItems();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void txtFind_KeyDown(object sender, KeyEventArgs e)
    {
      try
      {
        if (e.KeyCode == Keys.Enter)
        {
          var origRowCount = gridAvailable.RowCount;
          _findAction.Invoke();
          if (gridAvailable.RowCount == origRowCount && origRowCount > 0)
          {
            gridAvailable.Rows[0].Selected = true;
            SelectItems();
          }
        }
        else if (e.KeyCode == Keys.Down)
        {
          gridAvailable.Focus();
          if (gridAvailable.RowCount > 0) gridAvailable.Rows[0].Selected = true;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void txtFind_TextChanged(object sender, EventArgs e)
    {
      try
      {
        if (_availableRefs.Count == 1 && _availableRefs[0].Equals(_searchMessage)) return;
        _findAction.Invoke();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnAdvanced_Click(object sender, EventArgs e)
    {
      ShowContextMenu(btnAdvanced.Parent.PointToScreen(new Point(btnAdvanced.Left, btnAdvanced.Bottom)));
    }

    private void ShowContextMenu(Point pt)
    {
      var items = gridSelected.SelectedRows.OfType<DataGridViewRow>().Select(r => (ItemReference)r.DataBoundItem).ToList();

      if (items.Any())
      {
        foreach (var menu in mniLevels.DropDownItems.OfType<ToolStripMenuItem>()) menu.Checked = false;
        ((ToolStripMenuItem)mniLevels.DropDownItems[items.Select(i => i.Levels < 0 ? 1 : i.Levels).Min()]).Checked = true;

        var sysProps = items.Select(i => i.SystemProps).Aggregate((p, c) => p & c);
        mniHistory.Checked = ((sysProps & SystemPropertyGroup.History) == SystemPropertyGroup.History);
        mniPermissions.Checked = ((sysProps & SystemPropertyGroup.Permission) == SystemPropertyGroup.Permission);
        mniState.Checked = ((sysProps & SystemPropertyGroup.State) == SystemPropertyGroup.State);
        mniVersion.Checked = ((sysProps & SystemPropertyGroup.Versioning) == SystemPropertyGroup.Versioning);

        conStrip.Show(pt);
      }
    }

    private void conStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
    {
      try
      {
        var items = gridSelected.SelectedRows.OfType<DataGridViewRow>().Select(r => (ItemReference)r.DataBoundItem).ToList();

        if (items.Any())
        {
          var selectedLevelItem = mniLevels.DropDownItems.OfType<ToolStripMenuItem>().Where(m => m.Checked).FirstOrDefault();
          if (selectedLevelItem != null)
          {
            var level = mniLevels.DropDownItems.IndexOf(selectedLevelItem);
            foreach (var item in items) item.Levels = level;
          }

          var sysProps = (mniHistory.Checked ? SystemPropertyGroup.History : 0)
            | (mniPermissions.Checked ? SystemPropertyGroup.Permission : 0)
            | (mniState.Checked ? SystemPropertyGroup.State : 0)
            | (mniVersion.Checked ? SystemPropertyGroup.Versioning : 0);

          foreach (var item in items) item.SystemProps = sysProps;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void gridSelected_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      try
      {
        if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
        {
          if (!gridSelected[e.ColumnIndex, e.RowIndex].Selected)
          {
            gridSelected.ClearSelection();
            gridSelected.Rows[e.RowIndex].Selected = true;
          }

          ShowContextMenu(Cursor.Position);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniLevelItem_Click(object sender, MouseEventArgs e)
    {
      foreach (var menu in mniLevels.DropDownItems.OfType<ToolStripMenuItem>()) menu.Checked = false;
      ((ToolStripMenuItem)sender).Checked = true;
    }

    private void mniSysProps_MouseDown(object sender, MouseEventArgs e)
    {
      ((ToolStripMenuItem)sender).Checked = !((ToolStripMenuItem)sender).Checked;
    }

    private void btnRecentlyModified_Click(object sender, EventArgs e)
    {
      try
      {
        EnsureItemTypes();
        using (var dialog = new RecentlyModifiedSearch())
        {
          dialog.SetConnection(_conn);
          dialog.SetItemTypes(_itemTypes);
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            _availableRefs.Clear();
            _availableRefs.AddRange(dialog.Results);

            EnsureResultsTab();
            tbcSearch.SelectedTab = pgResults;
            txtFind.Focus();
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
