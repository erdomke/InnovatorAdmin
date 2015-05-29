using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin.Controls
{
  public partial class RecentlyModifiedSearch : Form
  {
    private BindingList<ItemReference> _availTypes = new BindingList<ItemReference>();
    private List<ItemReference> _results = new List<ItemReference>();
    private BindingList<ItemReference> _selectedTypes = new BindingList<ItemReference>();
    private Connection _conn;
    private string _currUserKeyedName;
    
    public IEnumerable<ItemReference> Results
    {
      get { return _results; }
    }
    
    public RecentlyModifiedSearch()
    {
      InitializeComponent();
    }

    public void SetConnection(Connection conn)
    {
      _conn = conn;
      _currUserKeyedName = conn.GetCurrUserInfo().Element("keyed_name", "");
      txtModifiedBy.Text = _currUserKeyedName;
    }
    public void SetItemTypes(IEnumerable<ItemReference> itemTypes)
    {
      try
      {
        _selectedTypes.RaiseListChangedEvents = false;
        _availTypes.RaiseListChangedEvents = false;
        foreach (var itemType in itemTypes)
        {
          if (IsCoreType(itemType))
          {
            _selectedTypes.Add(itemType);
          }
          else
          {
            _availTypes.Add(itemType);
          }
        }
      }
      finally
      {
        _selectedTypes.RaiseListChangedEvents = true;
        _availTypes.RaiseListChangedEvents = true;
      }
      lstItemTypes.DataSource = _selectedTypes;
    }

    private bool IsCoreType(ItemReference itemRef)
    {
      switch (itemRef.KeyedName)
      {
        case "ItemType":
        case "RelationshipType":
        case "Action":
        case "Report":
        case "Identity":
        case "List":
        case "Team":
        case "Method":
        case "Permission":
        case "Sequence":
        case "UserMessage":
        case "Workflow Promotion":
        case "Grid":
        case "User":
        case "Preference":
        case "Form":
        case "Life Cycle Map":
        case "Workflow Map":
          return true;
        default:
          return false;
      }
    }

    private void btnAddItemTypes_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new FilterSelect<ItemReference>())
        {
          dialog.DataSource = _availTypes;
          dialog.DisplayMember = "KeyedName";
          dialog.Message = resources.Messages.ItemTypeSelect;
          if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedItem != null)
          {
            _selectedTypes.Add(dialog.SelectedItem);
            _availTypes.Remove(dialog.SelectedItem);
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnRemove_Click(object sender, EventArgs e)
    {
      try
      {
        var selected = lstItemTypes.SelectedItem as ItemReference;
        if (selected != null)
        {
          _selectedTypes.Remove(selected);
          _availTypes.Add(selected);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      try
      {
        _results.Clear();
        IEnumerable<XmlElement> queryResults;
        foreach (var itemType in _selectedTypes)
        {
          if (txtModifiedBy.Text == _currUserKeyedName)
          {
            queryResults = _conn.GetItems("ApplyItem", string.Format(Properties.Resources.RecentItems_UserId, 
              itemType.KeyedName, 
              _conn.GetCurrUserInfo().Attribute("id"), 
              DateTime.Today.AddDays(-1 * (double)nudDays.Value).ToString("s")));
          }
          else 
          {
            queryResults = _conn.GetItems("ApplyItem", string.Format(Properties.Resources.RecentItems_UserKeyedName, 
              itemType.KeyedName, 
              txtModifiedBy.Text, 
              DateTime.Today.AddDays(-1 * (double)nudDays.Value).ToString("s")));
          }

          foreach (var qr in queryResults)
          {
            _results.Add(ItemReference.FromFullItem(qr, true));
          }
        }
        this.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.Close();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
