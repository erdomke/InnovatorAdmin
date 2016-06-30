using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal class ResultWriter : XmlWriter
  {
    public const string IgnoreAttribute = "~`IGNORE`~";

    public Result Result { get { return _result; } }

    protected ElementFactory _factory;
    protected string _attrName;
    protected string _value;
    private bool _inElement;
    private IElementWriter _base;

    private Result _result;
    private Stack<string> _names;

    public ResultWriter(ElementFactory factory, string database, string query)
    {
      _factory = factory;
      _result = new Result(factory, database, query);
      _names = new Stack<string>();
    }

    public override WriteState WriteState
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    public override void Close()
    {
      throw new NotImplementedException();
    }

    public override void Flush()
    {
      throw new NotImplementedException();
    }

    public override string LookupPrefix(string ns)
    {
      switch ((ns ?? "").TrimEnd('/'))
      {
        case "http://schemas.xmlsoap.org/soap/envelope":
          return "SOAP-ENV";
        case "http://www.w3.org/XML/1998/namespace":
          return "xml";
        case "http://www.aras.com/InnovatorFault":
          return "af";
        default:
          return "";
      }
    }

    public override void WriteBase64(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException();
    }

    public override void WriteCData(string text)
    {
      AddString(text);
    }

    public override void WriteCharEntity(char ch)
    {
      AddString(new string(ch, 1));
    }

    public override void WriteChars(char[] buffer, int index, int count)
    {
      AddString(new string(buffer, index, count));
    }

    public override void WriteComment(string text)
    {
    }

    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {
    }

    public override void WriteEndAttribute()
    {
      if (_attrName == IgnoreAttribute)
      {
        _value = null;
      }
      else if (_base != null)
      {
        _base.WriteEndAttribute(_attrName, _value);
        _value = null;
      }
    }

    public override void WriteEndDocument()
    {
    }

    public override void WriteEndElement()
    {
      _inElement = false;
      if (_base != null)
      {
        _base.WriteEndElement(_value);
      }
      else if (_names.Any() && _names.Peek() == "Result")
      {
        if (_value != null)
          _result.Value = _value;
      }
      _value = null;
    }

    public override void WriteEntityRef(string name)
    {
      if (name == "amp")
      {
        AddString("&");
      }
      else if (name == "apos")
      {
        AddString("'");
      }
      else if (name == "gt")
      {
        AddString(">");
      }
      else if (name == "lt")
      {
        AddString("<");
      }
      else if (name == "quot")
      {
        AddString("\"");
      }
      else
      {
        throw new NotSupportedException();
      }
    }

    public override void WriteFullEndElement()
    {
      WriteEndElement();
    }

    public override void WriteProcessingInstruction(string name, string text)
    {
    }

    public override void WriteRaw(string data)
    {
      AddString(data);
    }

    public override void WriteRaw(char[] buffer, int index, int count)
    {
      AddString(new string(buffer, index, count));
    }

    public override void WriteStartAttribute(string prefix, string localName, string ns)
    {
      _attrName = GetName(localName, ns);
      _value = null;
    }

    public override void WriteStartDocument()
    {
    }

    public override void WriteStartDocument(bool standalone)
    {
    }

    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      _value = null;
      _inElement = true;
      var name = GetName(localName, ns);
      if (_base != null)
      {
        _base.WriteStartElement(name);
      }
      else
      {
        switch (name)
        {
          case "Item":
            if (_names.Count < 1 || _names.Peek() != "Message")
            {
              _base = new ItemElementWriter(_factory);
              _base.Complete += OnComplete;
              _base.WriteStartElement(name);
            }
            break;
          case "SOAP-ENV:Fault":
            _base = new AmlElementWriter(_factory);
            _base.Complete += OnComplete;
            _base.WriteStartElement(name);
            break;
        }
        _names.Push(name);
      }
    }

    public void OnComplete(object sender, EventArgs e)
    {
      var itemWriter = _base as ItemElementWriter;
      if (itemWriter != null)
      {
        _result.AddReadOnly(itemWriter.Result as IReadOnlyItem);
        itemWriter.Complete -= OnComplete;
      }
      else
      {
        var elemWriter = _base as AmlElementWriter;
        if (elemWriter != null)
        {
          elemWriter.Complete -= OnComplete;
          var fault = elemWriter.Root;
          if (fault == null)
            throw new InvalidOperationException();
          switch (fault.ElementByName("faultcode").Value)
          {
            case "0":
              _result.Exception = new NoItemsFoundException(fault);
              break;
            case "1001":
              if (fault.ElementByName("detail").ElementByName("error_resolution_report").Exists)
              {
                _result.Exception = new ValidationReportException(fault);
              }
              else
              {
                _result.Exception = new ValidationException(fault);
              }
              break;
            default:
              _result.Exception = new ServerException(fault);
              break;
          }
        }
        else
        {
          throw new InvalidOperationException();
        }
      }

      _base = null;
    }

    public override void WriteString(string text)
    {
      AddString(text);
    }

    public override void WriteSurrogateCharEntity(char lowChar, char highChar)
    {
      AddString(new string(new char[] { highChar, lowChar }));
    }

    public override void WriteWhitespace(string ws)
    {
      if (_inElement) AddString(ws);
    }

    private void AddString(string value)
    {
      if (_value == null)
        _value = value;
      else
        _value += value;
    }

    protected string GetName(string localName, string ns)
    {
      if (string.IsNullOrEmpty(ns))
        return localName;
      if (ns.TrimEnd('/') == "http://www.w3.org/2000/xmlns")
        return IgnoreAttribute;
      var prefix = LookupPrefix(ns);
      if (string.IsNullOrEmpty(prefix))
        return localName;
      return prefix + ":" + localName;
    }

    private interface IElementWriter
    {
      event EventHandler Complete;
      void WriteStartElement(string name);
      void WriteEndElement(string value);
      void WriteEndAttribute(string name, string value);
    }

    private class ItemElementWriter : IElementWriter
    {
      public event EventHandler Complete;

      private ElementFactory _factory;
      private Stack<IElement> _stack;
      private IElement _result;

      public IElement Result { get { return _result; } }

      public ItemElementWriter(ElementFactory factory)
      {
        _factory = factory;
        _stack = new Stack<IElement>();
      }

      public void WriteEndAttribute(string name, string value)
      {
        var peek = _stack.Peek();
        switch (name)
        {
          case "id":
            if (peek is Item && value.IsGuid())
              AddAttribute(new IdAnnotation(peek, new Guid(value)));
            else
              AddAttribute(new Attribute(peek as Element, name, value));
            break;
          default:
            AddAttribute(new Attribute(peek as Element, name, value));
            break;
        }
      }

      private void AddAttribute(ILinkedAnnotation attr)
      {
        var elem = _stack.Peek() as Element;
        if (elem == null)
        {
          _stack.Peek().Add(attr);
        }
        else
        {
          elem.QuickAddAttribute(attr);
        }
      }

      public void WriteEndElement(string value)
      {
        if (value != null)
          _stack.Peek().Add(value);
        _stack.Pop();
        if (_stack.Count < 1)
          OnComplete(EventArgs.Empty);
      }

      public void WriteStartElement(string name)
      {
        IElement curr;
        switch (name)
        {
          case "Item":
            curr = new Item(_factory);
            break;
          case "Relationships":
            curr = new Relationships(_stack.Peek());
            break;
          case "and":
          case "or":
          case "not":
            curr = new Logical(_stack.Peek(), name);
            break;
          default:
            curr = new Property(_stack.Peek(), name);
            break;
        }
        if (_stack.Count > 0)
        {
          var elem = _stack.Peek() as Element;
          if (elem == null)
          {
            _stack.Peek().Add(curr);
          }
          else
          {
            elem.QuickAddElement((ILinkedElement)curr);
          }
        }
        else
        {
          _result = curr;
        }
        _stack.Push(curr);
      }

      protected virtual void OnComplete(EventArgs e)
      {
        if (Complete != null)
          Complete.Invoke(this, e);
      }

      private Item Normalize(Item item)
      {
        var idProp = item.Property("id");
        if (idProp.Exists && idProp.Value.IsGuid())
        {
          var typeAttr = item.Type();
          var idTypeAttr = idProp.Type();
          if (idTypeAttr.HasValue() && !typeAttr.HasValue())
          {
            typeAttr.Set(idTypeAttr.Value);
          }

          var keyedNameProp = item.KeyedName();
          var idKeyedNameAttr = idProp.KeyedName();
          if (idKeyedNameAttr.HasValue() && !keyedNameProp.HasValue())
          {
            keyedNameProp.Set(idKeyedNameAttr.Value);
          }

          idProp.Remove();
        }

        return item;
      }
    }

    private class AmlElementWriter : IElementWriter
    {
      private ElementFactory _factory;
      private Element _root;
      private Element _curr;

      public event EventHandler Complete;

      public Element Root { get { return _root; } }

      public AmlElementWriter(ElementFactory factory)
      {
        _factory = factory;
      }

      public void WriteEndAttribute(string name, string value)
      {
        _curr.Add(new Attribute(name, value));
      }

      public void WriteEndElement(string value)
      {
        if (value != null)
        {
          _curr.Value = value;
        }
        _curr = _curr.Parent as Element;
        if (_curr == null
          || (!_curr.Exists && _root != _curr))
          OnComplete(EventArgs.Empty);
      }

      public void WriteStartElement(string name)
      {
        var newElem = new AmlElement(_factory, name);
        if (_curr == null)
        {
          _root = newElem;
        }
        else
        {
          _curr.Add(newElem);
        }
        _curr = newElem;
      }

      protected virtual void OnComplete(EventArgs e)
      {
        if (Complete != null)
          Complete.Invoke(this, e);
      }
    }
  }
}
