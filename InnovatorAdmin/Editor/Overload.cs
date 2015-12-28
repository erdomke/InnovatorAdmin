using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class Overload
  {
    public object Header { get; set; }
    public object Content { get; set; }

    public Overload(object header, object content)
    {
      this.Header = header;
      this.Content = content;
    }
  }

  public class OverloadList : IOverloadProvider, IEnumerable<Overload>
  {
    private int _i;
    private List<Overload> _overloads = new List<Overload>();

    public void Add(object header, object content)
    {
      _overloads.Add(new Overload(header, content));
    }

    public int Count
    {
      get { return _overloads.Count; }
    }

    public object CurrentContent
    {
      get { return _overloads[_i].Content; }
    }

    public object CurrentHeader
    {
      get { return _overloads[_i].Header; }
    }

    public string CurrentIndexText
    {
      get { return (_i + 1) + " of " + this.Count; }
    }

    public int SelectedIndex
    {
      get { return _i; }
      set
      {
        if (value != _i)
        {
          _i = value;
          OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentContent"));
          OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentHeader"));
          OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentIndexText"));
          OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedIndex"));
        }
      }
    }

    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (PropertyChanged != null)
        PropertyChanged.Invoke(this, e);
    }

    public IEnumerator<Overload> GetEnumerator()
    {
      return _overloads.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
