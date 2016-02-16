using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Aras
{
  public class DataItemReader : IPipeOutput<IDataItem>, IPipeInput<IItem>, IPipeInput<Xml.IXmlNode>
  {
    private IEnumerable<IItem> _reader;

    public Action<IError> ErrorHandler { get; set; }

    public IEnumerator<IDataItem> GetEnumerator()
    {
      foreach(var item in _reader)
      {
        if (item is IError)
        {
          if (this.ErrorHandler != null) this.ErrorHandler((IError)item);
        }
        else
        {
          var dataItem = item as IDataItem;
          if (dataItem != null) yield return dataItem;
        }
      }
    }

    public IEnumerable<IResult> GetResults()
    {
      foreach (var item in _reader)
      {
        if (item is IError)
        {
          if (this.ErrorHandler != null) this.ErrorHandler((IError)item);
        }
        else
        {
          var resultItem = item as IResult;
          if (resultItem != null) yield return resultItem;
        }
      }
    }

    public IEnumerable<IEvent> GetEvents()
    {
      foreach (var item in _reader)
      {
        if (item is IError)
        {
          if (this.ErrorHandler != null) this.ErrorHandler((IError)item);
        }
        else
        {
          var eventItem = item as IEvent;
          if (eventItem != null) yield return eventItem;
        }
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Initialize(IEnumerable<IItem> source)
    {
      _reader = source;
    }

    public void Initialize(IEnumerable<Xml.IXmlNode> source)
    {
      if (_reader == null)
      {
        _reader = new ItemXmlReader();
        ((ItemXmlReader)_reader).Initialize(source);
      }
    }
  }
}
