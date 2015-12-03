using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace InnovatorAdmin
{
  public class FullBindingList<T> : IList<T>, IBindingListView
  {
    private ListSortDescriptionCollection _descrips;
    protected IBindingList _list;
    private string _filterString = null;
    private List<int> _filterIds = new List<int>();

    private Func<T, bool> _filter;
    public bool AllowEdit
    {
      get { return _list.AllowEdit; }
    }
    public bool AllowNew
    {
      get { return true; }
    }
    public bool AllowRemove
    {
      get { return _list.AllowRemove; }
    }
    public int Count
    {
      get { return _filter != null ? _filterIds.Count : _list.Count; }
    }
    public bool IsFixedSize
    {
      get { return _list.IsFixedSize; }
    }
    public bool IsReadOnly
    {
      get { return _list.IsReadOnly; }
    }
    public bool IsSynchronized
    {
      get { return _list.IsSynchronized; }
    }

    public T this[int index]
    {
      get
      {
        if (IsFilteredOrSorted())
        {
          return (T)_list[_filterIds[index]];
        }
        else
        {
          return (T)_list[index];
        }
      }
      set
      {
        if (IsFilteredOrSorted())
        {
          _list[_filterIds[index]] = value;
        }
        else
        {
          _list[index] = value;
        }
      }
    }
    public bool RaiseListChangedEvents { get; set; }
    public bool SupportsChangeNotification
    {
      get { return _list.SupportsChangeNotification; }
    }
    public object SyncRoot
    {
      get { return _list.SyncRoot; }
    }
    public int TotalCount
    {
      get { return _list.Count; }
    }

    public FullBindingList()
    {
      _list = new BindingList<T>();
      _list.ListChanged += _list_ListChanged;
      this.RaiseListChangedEvents = true;
    }
    //public FullBindingList(IBindingList list)
    //{
    //  _list = list;
    //  ListChangedWeakEventHandler.Register(_list, (s, eh) =>, (s, eh) =>, this, (item, s, e) => item._list_ListChanged(s, e));
    //}

    private bool IsFilteredOrSorted()
    {
      return _filterIds.Count > 0 || _filter != null;
    }

    #region "Insert/Remove"

    public void Add(T item)
    {
      InsertItem(_list.Count, item);
    }
    public void AddRange(IEnumerable<T> items)
    {
      foreach (T item in items)
      {
        InsertItem(_list.Count, item);
      }
    }
    public virtual T AddNew()
    {
      if (_list.AllowNew)
      {
        return (T)_list.AddNew();
      }
      else
      {
        var result = Activator.CreateInstance<T>();
        _list.Add(result);
        return result;
      }
    }
    public void Clear()
    {
      _filterIds.Clear();
      _list.Clear();
    }
    public void ClearVisible()
    {
      if (IsFilteredOrSorted())
      {
        foreach (var i in _filterIds)
        {
          RemoveCore(_list[i]);
        }
        _filterIds.Clear();
      }
      else
      {
        _list.Clear();
      }
    }
    public void Insert(int index, T item)
    {
      InsertWithFilter(index, item);
    }
    public bool Remove(T item)
    {
      if (_list.Contains(item))
      {
        RemoveItem(item);
        return true;
      }
      return false;
    }
    public void RemoveAt(int index)
    {
      RemoveAtWithFilter(index);
    }
    public virtual int RemoveRange(System.Collections.IEnumerable items)
    {
      int countRemoved = 0;

      foreach (object item in items)
      {
        if (item is T)
        {
          if (this.Remove((T)item))
            countRemoved += 1;
        }
      }

      return countRemoved;
    }

    protected virtual void InsertItem(int index, T value)
    {
      _list.Insert(index, value);
    }
    protected virtual void RemoveItem(T item)
    {
      _list.Remove(item);
    }

    private int AddCore(object value)
    {
      InsertItem(_list.Count, (T)value);
      return 1;
    }
    int System.Collections.IList.Add(object value)
    {
      return AddCore(value);
    }
    private object AddNewCore()
    {
      return this.AddNew();
    }
    object System.ComponentModel.IBindingList.AddNew()
    {
      return AddNewCore();
    }
    private void InsertWithFilter(int index, object value)
    {
      if (IsFilteredOrSorted())
      {
        InsertItem(_filterIds[index], (T)value);
      }
      else
      {
        InsertItem(index, (T)value);
      }
    }
    void System.Collections.IList.Insert(int index, object value)
    {
      InsertWithFilter(index, value);
    }
    private void RemoveCore(object value)
    {
      RemoveItem((T)value);
    }
    void System.Collections.IList.Remove(object value)
    {
      RemoveCore(value);
    }
    private void RemoveAtWithFilter(int index)
    {
      if (IsFilteredOrSorted())
      {
        RemoveCore(_list[_filterIds[index]]);
      }
      else
      {
        RemoveCore(_list[index]);
      }
    }
    void System.Collections.IList.RemoveAt(int index)
    {
      RemoveAtWithFilter(index);
    }

    #endregion

    #region "Filtering"
    public string Filter
    {
      get { return _filterString; }
      set
      {
        throw new NotSupportedException();
      }
    }
    public bool SupportsFiltering
    {
      get { return false; }
    }

    public void ApplyFilter(Func<T, bool> filter)
    {
      this.ApplyFilter(filter, null);
    }
    public virtual void RemoveFilter()
    {
      if (_filter != null)
      {
        _filter = null;
        _filterString = null;
        _filterIds.Clear();
        ResetBindings();
      }
    }

    protected virtual void ApplyFilter(Func<T, bool> filter, string filterString)
    {
      if (!object.ReferenceEquals(_filter, filter) || _filterString != filterString)
      {
        _filterString = filterString;
        _filter = filter;
        RebuildIndex();
      }
    }
    #endregion

    #region "Searching"
    public bool SupportsSearching
    {
      get { return true; }
    }
    protected virtual int FindCore(System.ComponentModel.PropertyDescriptor prop, object key)
    {
      if (key == null)
        return -1;
      object propValue = null;

      for (int i = 0; i <= this.Count - 1; i++)
      {
        propValue = GetValue(prop, this[i]);
        if (propValue != null && key.Equals(propValue))
        {
          return i;
        }
      }

      return -1;
    }
    int System.ComponentModel.IBindingList.Find(System.ComponentModel.PropertyDescriptor prop, object key)
    {
      return FindCore(prop, key);
    }
    #endregion

    #region "Sorting"
    public bool IsSorted
    {
      get { return _list.IsSorted || (_descrips != null && _descrips.Count > 0); }
    }
    public System.ComponentModel.ListSortDescriptionCollection SortDescriptions
    {
      get { return _descrips; }
    }
    public System.ComponentModel.ListSortDirection SortDirection
    {
      get
      {
        if (_list.SupportsSorting)
        {
          return _list.SortDirection;
        }
        else if (_descrips != null && _descrips.Count > 0)
        {
          return _descrips[0].SortDirection;
        }
        else
        {
          return ListSortDirection.Ascending;
        }
      }
    }
    public System.ComponentModel.PropertyDescriptor SortProperty
    {
      get
      {
        if (_list.SupportsSorting)
        {
          return _list.SortProperty;
        }
        else if (_descrips != null && _descrips.Count > 0)
        {
          return _descrips[0].PropertyDescriptor;
        }
        else
        {
          return null;
        }
      }
    }
    public bool SupportsAdvancedSorting
    {
      get { return true; }
    }
    public bool SupportsSorting
    {
      get { return true; }
    }

    public void ApplySort(System.ComponentModel.PropertyDescriptor pd, System.ComponentModel.ListSortDirection direction)
    {
      if (_list.SupportsSorting)
      {
        _list.ApplySort(pd, direction);
      }
      else
      {
        ApplySort(new ListSortDescriptionCollection(new ListSortDescription[] { new ListSortDescription(pd, direction) }));
      }
    }
    public void ApplySort(string sortDefn)
    {
      var typedList = this as ITypedList;
      PropertyDescriptorCollection props = default(PropertyDescriptorCollection);
      if (typedList == null)
      {
        props = TypeDescriptor.GetProperties(typeof(T));
      }
      else
      {
        props = typedList.GetItemProperties(null);
      }

      ApplySort(new ListSortDescriptionCollection((from c in (sortDefn ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                   select ExtractSortDescrip(props, c)).ToArray()));
    }
    public void ApplySort(Comparison<T> comparison)
    {
      _descrips = new ListSortDescriptionCollection();
      if (comparison == null)
      {
        if (_filter == null)
        {
          _filterIds.Clear();
        }
        else
        {
          _filterIds.Sort();
        }
      }
      else
      {
        if (!_filterIds.Any() && _filter == null)
          _filterIds.AddRange(Enumerable.Range(0, _list.Count));
        _filterIds.Sort((x, y) => comparison((T)_list[x], (T)_list[y]));
      }

      ResetBindings();
    }
    public void ApplySort(System.ComponentModel.ListSortDescriptionCollection sorts)
    {
      _descrips = sorts;
      if (sorts == null || sorts.Count < 1)
      {
        if (_filter == null)
        {
          _filterIds.Clear();
        }
        else
        {
          _filterIds.Sort();
        }
      }
      else
      {
        if (!_filterIds.Any() && _filter == null)
          _filterIds.AddRange(Enumerable.Range(0, _list.Count));
        _filterIds.Sort((x, y) => CompareProp((T)_list[x], (T)_list[y]));
      }

      ResetBindings();
    }
    public void RemoveSort()
    {
      ApplySort((ListSortDescriptionCollection)null);
    }
    public void Sort(Comparison<T> comparison)
    {
      var origRaiseEvents = this.RaiseListChangedEvents;
      this.RaiseListChangedEvents = false;
      var newList = _list.OfType<T>().ToList();
      newList.Sort(comparison);
      _list.Clear();
      foreach (var value in newList)
      {
        _list.Add(value);
      }
      this.RaiseListChangedEvents = origRaiseEvents;
      if (!RebuildIndex())
      {
        ResetBindings();
      }
    }

    private ListSortDescription ExtractSortDescrip(PropertyDescriptorCollection props, string value)
    {
      value = value.Trim();
      var test = value.ToLowerInvariant();
      if (test.EndsWith(" desc"))
      {
        var name = value.Substring(0, value.Length - 5).Trim();
        return new ListSortDescription(props[name], ListSortDirection.Descending);
      }
      else if (test.EndsWith(" asc"))
      {
        var name = value.Substring(0, value.Length - 4).Trim();
        return new ListSortDescription(props[name], ListSortDirection.Ascending);
      }
      else
      {
        return new ListSortDescription(props[value], ListSortDirection.Ascending);
      }
    }

    private int CompareProp(T x, T y)
    {
      try
      {
        if (_descrips == null)
          return 0;

        int cmp = 0;
        int i = 0;
        IComparable v1 = null;
        IComparable v2 = null;

        while (cmp == 0 && i < _descrips.Count)
        {
          v1 = GetValue(_descrips[i].PropertyDescriptor, x) as IComparable;
          v2 = GetValue(_descrips[i].PropertyDescriptor, y) as IComparable;
          cmp = v1 == null && v2 == null ? 0 : v1 == null ? +1 : v2 == null ? -1 : v1.CompareTo(v2);
          if (_descrips[i].SortDirection == ListSortDirection.Descending)
            cmp *= -1;
          i += 1;
        }

        return cmp;
      }
      catch
      {
        // comparison failed...
        return 0;
      }
    }
    #endregion


    public void AddIndex(System.ComponentModel.PropertyDescriptor property)
    {
      _list.AddIndex(property);
    }
    public bool Contains(T item)
    {
      return ContainsCore(item);
    }
    private bool ContainsCore(object value)
    {
      return IndexOf((T)value) >= 0;
    }
    bool System.Collections.IList.Contains(object value)
    {
      return ContainsCore(value);
    }
    public void CopyTo(T[] array, int arrayIndex)
    {
      CopyToCore(array, arrayIndex);
    }
    private void CopyToCore(System.Array array, int index)
    {
      _list.CopyTo(array, index);
    }
    void System.Collections.ICollection.CopyTo(System.Array array, int index)
    {
      CopyToCore(array, index);
    }
    public System.Collections.Generic.IEnumerator<T> GetEnumerator()
    {
      if (IsFilteredOrSorted())
      {
        return new FilterEnumerator<T>(this);
      }
      else
      {
        return _list.OfType<T>().GetEnumerator();
      }
    }
    private System.Collections.IEnumerator GetEnumeratorCore()
    {
      return GetEnumerator();
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumeratorCore();
    }
    public int IndexOf(T item)
    {
      return IndexOfCore(item);
    }
    protected virtual int IndexOfCore(object value)
    {
      if (IsFilteredOrSorted())
      {
        return _filterIds.IndexOf(_list.IndexOf(value));
      }
      else
      {
        return _list.IndexOf(value);
      }
    }
    int System.Collections.IList.IndexOf(object value)
    {
      return IndexOfCore(value);
    }
    public void RemoveIndex(System.ComponentModel.PropertyDescriptor property)
    {
      _list.RemoveIndex(property);
    }

    private bool RebuildIndex()
    {
      if (_filter != null)
      {
        _filterIds.Clear();
        for (int i = 0; i <= _list.Count - 1; i++)
        {
          if (_filter.Invoke((T)_list[i]))
          {
            _filterIds.Add(i);
          }
        }
        ApplySort(_descrips);
        return true;
      }
      return false;
    }
    private bool RebuildIndexWithoutEvents()
    {
      var origRaiseEvents = this.RaiseListChangedEvents;
      try
      {
        this.RaiseListChangedEvents = false;
        return RebuildIndex();
      }
      finally
      {
        this.RaiseListChangedEvents = origRaiseEvents;
      }
    }

    public event ListChangedEventHandler ListChanged;
    public event ListChangedEventHandler CoreListChanged;

    public void ResetBindings()
    {
      FireListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }
    protected virtual void FireListChanged(System.ComponentModel.ListChangedEventArgs e)
    {
      if (this.RaiseListChangedEvents)
      {
        this.OnListChanged(e);
      }
    }

    protected virtual void OnListChanged(System.ComponentModel.ListChangedEventArgs e)
    {
      if (ListChanged != null)
      {
        ListChanged(this, e);
      }
    }

    private void _list_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
    {
      if (CoreListChanged != null)
      {
        CoreListChanged(sender, e);
      }
      ListChangedEventArgs args = e;
      int i = 0;

      switch (e.ListChangedType)
      {
        case ListChangedType.ItemAdded:
          if (IsFilteredOrSorted())
          {
            if (_filter == null || _filter((T)_list[e.NewIndex]))
            {
              args = null;

              while (i < _filterIds.Count)
              {
                if (_filterIds[i] >= e.NewIndex)
                {
                  if (args == null)
                  {
                    _filterIds.Insert(i, e.NewIndex);
                    args = new ListChangedEventArgs(ListChangedType.ItemAdded, i);
                  }
                  else
                  {
                    _filterIds[i] += 1;
                  }
                }
                i += 1;
              }
              if (args == null)
              {
                _filterIds.Add(e.NewIndex);
                args = new ListChangedEventArgs(ListChangedType.ItemAdded, _filterIds.Count - 1);
              }
            }
            else
            {
              return;
            }
          }
          break;
        case ListChangedType.ItemDeleted:
          if (_filterIds.Count > 0)
          {
            args = null;

            while (i < _filterIds.Count)
            {
              if (_filterIds[i] == e.NewIndex)
              {
                _filterIds.RemoveAt(i);
                args = new ListChangedEventArgs(ListChangedType.ItemDeleted, i);
              }
              else
              {
                if (_filterIds[i] > e.NewIndex)
                {
                  _filterIds[i] -= 1;
                }
                i += 1;
              }
            }

            if (args == null)
              return;
          }
          break;
        case ListChangedType.ItemChanged:
          if (_filter != null)
          {
            //Rebuild the index because the item might have its filter status changed as a result of the property change
            var currIndex = _filterIds.IndexOf(e.NewIndex);
            if ((currIndex >= 0) ^ _filter((T)_list[e.NewIndex]))
            {
              RebuildIndexWithoutEvents();
              var newIndex = _filterIds.IndexOf(e.NewIndex);

              if (currIndex != newIndex)
              {
                args = new ListChangedEventArgs(ListChangedType.Reset, -1);
              }
              else if (currIndex == -1)
              {
                //Item not shown, so suppress the message
                return;
              }
              else
              {
                args = new ListChangedEventArgs(ListChangedType.ItemChanged, currIndex, e.PropertyDescriptor);
              }
            }
            else
            {
              return;
            }
          }
          break;
        case ListChangedType.Reset:
          RebuildIndexWithoutEvents();
          break;
      }

      FireListChanged(args);
    }

    protected virtual object GetValue(PropertyDescriptor descrip, object obj)
    {
      return descrip.GetValue(obj);
    }

    private class FilterEnumerator<S> : IEnumerator<S>
    {

      private int _i = -1;

      private FullBindingList<S> _parent;
      public S Current
      {
        get { return (S)_parent._list[_parent._filterIds[_i]]; }
      }
      private object CurrentCore
      {
        get { return this.Current; }
      }
      object System.Collections.IEnumerator.Current
      {
        get { return CurrentCore; }
      }

      public bool MoveNext()
      {
        _i += 1;
        return _i < _parent._filterIds.Count;
      }

      public void Reset()
      {
        _i = -1;
      }

      public FilterEnumerator(FullBindingList<S> parent)
      {
        _parent = parent;
      }

      #region "IDisposable Support"
      // To detect redundant calls
      private bool disposedValue;

      // IDisposable
      protected virtual void Dispose(bool disposing)
      {
        if (!this.disposedValue)
        {
          if (disposing)
          {
            // TODO: dispose managed state (managed objects).
          }

          // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
          // TODO: set large fields to null.
        }
        this.disposedValue = true;
      }

      // TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
      //Protected Overrides Sub Finalize()
      //    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
      //    Dispose(False)
      //    MyBase.Finalize()
      //End Sub

      // This code added by Visual Basic to correctly implement the disposable pattern.
      public void Dispose()
      {
        // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(true);
        GC.SuppressFinalize(this);
      }
      #endregion

    }

    object System.Collections.IList.this[int index]
    {
      get
      {
        if (IsFilteredOrSorted())
        {
          return _list[_filterIds[index]];
        }
        else
        {
          return _list[index];
        }
      }
      set
      {
        if (IsFilteredOrSorted())
        {
          _list[_filterIds[index]] = value;
        }
        else
        {
          _list[index] = value;
        }
      }
    }
  }
}
