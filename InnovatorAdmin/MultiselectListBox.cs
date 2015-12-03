using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public partial class MultiselectListBox : UserControl
  {
    private ListBox _list;
    private CheckedListBox _checked;
    private ObjectCollection _items;
    private bool _multiselect;
    private bool _itemChecked;

    public event EventHandler SelectionChanged;

    public object DataSource
    {
      get
      {
        return _checked.DataSource;
      }
      set
      {
        _checked.DataSource = value;
        _list.DataSource = value;
      }
    }
    public string DisplayMember
    {
      get
      {
        return _checked.DisplayMember;
      }
      set
      {
        _checked.DisplayMember = value;
        _list.DisplayMember = value;
      }
    }
    public IList<object> Items
    {
      get { return _items; }
    }
    public bool Multiselect
    {
      get
      {
        return _multiselect;
      }
      set
      {
        _multiselect = value;
        this.Controls.Clear();
        if (value)
        {
          this.Controls.Add(_checked);
        }
        else
        {
          this.Controls.Add(_list);
        }
      }
    }
    public IEnumerable<object> Selected
    {
      get
      {
        return (_multiselect ? _checked.CheckedItems.OfType<object>() : Enumerable.Repeat<object>(_list.SelectedItem, 1));
      }
    }
    public string ValueMember
    {
      get
      {
        return _checked.ValueMember;
      }
      set
      {
        _checked.ValueMember = value;
        _list.ValueMember = value;
      }
    }

    public MultiselectListBox()
    {
      InitializeComponent();
      _list = new ListBox();
      _checked = new CheckedListBox();
      _items = new ObjectCollection(this);

      _list.Dock = DockStyle.Fill;
      _checked.Dock = DockStyle.Fill;

      _checked.CheckOnClick = true;

      this.Controls.Add(_list);
      _list.SelectedIndexChanged += _list_SelectedIndexChanged;
      _list.MouseDoubleClick += _list_MouseDoubleClick;
      _checked.ItemCheck += _checked_ItemCheck;
      _checked.MouseDown += _checked_MouseDown;
      _checked.MouseUp += _checked_MouseUp;
      _checked.MouseDoubleClick += _list_MouseDoubleClick;
    }

    void _list_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      OnMouseDoubleClick(e);
    }

    public void SetItemSelected(int index, bool state)
    {
      if (state && !_multiselect)
      {
        for (var i = 0; i < _checked.Items.Count; i++)
        {
          _checked.SetItemChecked(i, false);
        }
      }
      _checked.SetItemChecked(index, state);
      if (state && _list.SelectedIndex != index)
      {
        _list.SelectedIndex = index;
      }
    }
    public void SetSelection(params object[] items)
    {
      if (items == null || items.Length < 1) return;
      if (items.Length > 1) this.Multiselect = true;
      _list.SelectedItem = items[0];

      for (var i = 0; i < _checked.Items.Count; i++)
      {
        _checked.SetItemChecked(i, items.Contains(_checked.Items[i]));
      }
    }

    protected virtual void OnSelectionChanged(EventArgs e)
    {
      if (SelectionChanged != null) SelectionChanged.Invoke(this, e);
    }

    void _checked_ItemCheck(object sender, ItemCheckEventArgs e)
    {
      _itemChecked = true;
    }

    void _checked_MouseDown(object sender, MouseEventArgs e)
    {
      _itemChecked = false;
    }

    void _checked_MouseUp(object sender, MouseEventArgs e)
    {
      if (_itemChecked) OnSelectionChanged(e);
    }

    void _list_SelectedIndexChanged(object sender, EventArgs e)
    {
      OnSelectionChanged(e);
    }

    private class ObjectCollection : IList<object>
    {
      private MultiselectListBox _parent;

      public ObjectCollection(MultiselectListBox parent)
      {
        _parent = parent;
      }

      public int IndexOf(object item)
      {
        return _parent._list.Items.IndexOf(item);
      }

      public void Insert(int index, object item)
      {
        _parent._checked.Items.Insert(index, item);
        _parent._list.Items.Insert(index, item);
      }

      public void RemoveAt(int index)
      {
        _parent._checked.Items.RemoveAt(index);
        _parent._list.Items.RemoveAt(index);
      }

      public object this[int index]
      {
        get
        {
          return _parent._checked.Items[index];
        }
        set
        {
          _parent._checked.Items[index] = value;
          _parent._list.Items[index] = value;
        }
      }

      public void Add(object item)
      {
        _parent._checked.Items.Add(item);
        _parent._list.Items.Add(item);
      }

      public void Clear()
      {
        _parent._checked.Items.Clear();
        _parent._list.Items.Clear();
      }

      public bool Contains(object item)
      {
        return _parent._checked.Items.Contains(item);
      }

      public void CopyTo(object[] array, int arrayIndex)
      {
        _parent._checked.Items.CopyTo(array, arrayIndex);
      }

      public int Count
      {
        get { return _parent._checked.Items.Count; }
      }

      public bool IsReadOnly
      {
        get { return false; }
      }

      public bool Remove(object item)
      {
        var idx = _parent._checked.Items.IndexOf(item);
        if (idx < 0) return false;
        _parent._checked.Items.RemoveAt(idx);
        _parent._list.Items.RemoveAt(idx);
        return true;
      }

      public IEnumerator<object> GetEnumerator()
      {
        return _parent._checked.Items.OfType<object>().GetEnumerator();
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        return _parent._checked.Items.GetEnumerator();
      }
    }


  }
}
