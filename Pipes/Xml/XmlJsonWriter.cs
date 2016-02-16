using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public class XmlJsonWriter : Sgml.ISgmlWriter, Sgml.ISgmlGroupWriter
  {
    private Dictionary<string, string> _nsDict = new Dictionary<string, string>();
    private WriterState _state = WriterState.startElem;
    private Stack<Tag> _tags = new Stack<Tag>();
    private Json.IJsonWriter _writer;

    private enum WriterState
    {
      startElem,
      elemData,
      singleValue
    }
    private class Tag
    {
      public string Name { get; set; }
      public bool Group { get; set; }
    }

    public XmlJsonWriter(Json.IJsonWriter writer)
    {
      _writer = writer;
    }

    public Sgml.ISgmlWriter Attribute(string name, object value)
    {
      if (_state == WriterState.startElem) _writer.Object();
      _writer.Prop("@" + name, value);
      _state = WriterState.elemData;
      return this;
    }
    public Sgml.ISgmlWriter Attribute(string name, string ns, object value)
    {
      if (string.IsNullOrEmpty(ns))
      {
        return Attribute(name, value);
      }
      else
      {
        string prefix = null;
        if (_nsDict.TryGetValue(ns, out prefix))
        {
          return Attribute(prefix + ":" + name, value);
        }
        else
        {
          throw new ArgumentException("Undeclared namespace");
        }
      }
    }
    public Sgml.ISgmlWriter Attribute(string prefix, string name, string ns, object value)
    {
      if (string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(ns))
      {
        return Attribute(name, value);
      }
      else
      {
        _nsDict[ns] = prefix;
        return Attribute(prefix + ":" + name, value);
      }
    }
    public Sgml.ISgmlWriter Comment(string value)
    {
      throw new NotImplementedException();
    }
    public Sgml.ISgmlWriter Element(string name, object value)
    {
      return Element(name).Value(value).ElementEnd();
    }
    public Sgml.ISgmlWriter Element(string name)
    {
      if (!(_tags.Count > 0 && _tags.Peek().Group && _tags.Peek().Name == name))
      {
        if (_state == WriterState.startElem) _writer.Object();
        _writer.Prop(name);
      }
      _tags.Push(new Tag() { Group = false, Name = name });
      _state = WriterState.startElem;
      return this;
    }
    public Sgml.ISgmlWriter ElementEnd()
    {
      if (_state == WriterState.startElem)
      {
        _writer.Value(null);
      }
      else if (_state != WriterState.singleValue)
      {
        _writer.ObjectEnd();
      }
      _tags.Pop();
      if (_tags.Count == 0) _writer.ObjectEnd();
      _state = WriterState.elemData;
      return this;
    }
    public Sgml.ISgmlGroupWriter ElementGroup(string name)
    {
      _tags.Push(new Tag() { Group = true, Name = name });
      if (_state == WriterState.startElem) _writer.Object();
      _writer.Prop(name);
      _writer.Array();
      return this;
    }
    public Sgml.ISgmlGroupWriter ElementGroupEnd()
    {
      _tags.Pop();
      _writer.ArrayEnd();
      return this;
    }
    public void Flush()
    {
      _writer.Flush();
    }
    public Sgml.ISgmlWriter NsElement(string name, string ns)
    {
      string prefix = null;
      if (_nsDict.TryGetValue(ns, out prefix))
      {
        return Element(prefix + ":" + name);
      }
      else
      {
        throw new ArgumentException("Undeclared namespace");
      }
    }
    public Sgml.ISgmlWriter NsElement(string prefix, string name, string ns)
    {
      _nsDict[ns] = prefix;
      return Element(prefix + ":" + name);
    }
    public Sgml.ISgmlWriter Raw(string value)
    {
      _writer.Raw(value);
      return this;
    }
    public Sgml.ISgmlWriter Value(object value)
    {
      if (_state == WriterState.startElem)
      {
        _writer.Value(value);
        _state = WriterState.singleValue;
      }
      else
      {
        _writer.Prop("#text", value);
        _state = WriterState.elemData;
      }
      return this;
    }
    
  }
}
