using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public class XmlPath : ICollection<IXmlName>
  {
    private List<IXmlName> _path = new List<IXmlName>();

    public int Count
    {
      get { return _path.Count; }
    }
    bool ICollection<IXmlName>.IsReadOnly
    {
      get { return false; }
    }
    public IXmlName this[int index]
    {
      get { return _path[index]; }
    }

    public void Add(IXmlName item)
    {
      _path.Add(item);
    }
    public void Clear()
    {
      _path.Clear();
    }
    public bool Contains(IXmlName item)
    {
      return _path.Contains(item);
    }
    public void CopyTo(IXmlName[] array, int arrayIndex)
    {
      _path.CopyTo(array, arrayIndex);
    }
    public bool LocalNameEndsWith(params string[] localNames)
    {
      var offset = _path.Count - localNames.Length;
      if (offset >= 0)
      {
        for (var i = 0; i < localNames.Length; i++)
        {
          if (_path[i + offset].LocalName != localNames[i]) return false;
        }
        return true;
      }
      else
      {
        return false;
      }
    }
    public IEnumerator<IXmlName> GetEnumerator()
    {
      return _path.GetEnumerator();
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _path.GetEnumerator();
    }
    public bool Remove(IXmlName item)
    {
      return _path.Remove(item);
    }
    public void Track(IXmlNode node)
    {
      if (node.Type == XmlNodeType.Element) _path.Add(node.Name);
      if (node.Type == XmlNodeType.EndElement) _path.RemoveAt(_path.Count - 1);
    }
  }
}
