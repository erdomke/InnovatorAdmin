using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal class ResultWriter : BaseAmlWriter
  {
    private Result _result;
    private Stack<string> _names;

    public Result Result { get { return _result; } }

    public ResultWriter(ElementFactory factory, string database, string query) : base(factory)
    {
      _result = new Result(factory, database, query);
      _names = new Stack<string>();
    }

    public override void WriteEndElement()
    {
      if (_base != null)
      {
        _base.WriteEndElement();
        return;
      }

      if (_names.Any() && _names.Peek() == "Result")
      {
        if (_value != null)
          _result.Value = ConsumeValue();
        if (_parent != null) _parent.SignalCompletion();
      }
    }
    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      if (_base != null)
      {
        _base.WriteStartElement(prefix, localName, ns);
        return;
      }
      ConsumeValue();
      var name = GetName(localName, ns);
      switch (name)
      {
        case "Item":
          if (!_names.Any() || _names.Peek() != "Message")
          {
            _base = new ItemElementWriter(_factory, this);
            _base.WriteStartElement(prefix, localName, ns);
          }
          break;
        case "SOAP-ENV:Fault":
          _base = new AmlElementWriter(_factory, this);
          _base.WriteStartElement(prefix, localName, ns);
          break;
      }
      _names.Push(name);
    }
    public override void SignalCompletion()
    {
      var itemWriter = _base as ItemElementWriter;
      if (itemWriter != null)
      {
        _result.AddReadOnly(itemWriter.Result as IReadOnlyItem);
      }
      else
      {
        var elemWriter = _base as AmlElementWriter;
        if (elemWriter != null)
        {
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
          if (_parent != null) _parent.SignalCompletion();
        }
        else
        {
          throw new InvalidOperationException();
        }
      }
      base.SignalCompletion();
    }
  }

  internal class ItemElementWriter : BaseAmlWriter
  {
    private Stack<IElement> _stack;
    private IElement _result;

    public IElement Result { get { return _result; } }

    public ItemElementWriter(ElementFactory factory, BaseAmlWriter parent) : base(factory)
    {
      _stack = new Stack<IElement>();
      _parent = parent;
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

    public override void WriteEndAttribute()
    {
      if (DefaultEndAttribute())
        return;

      switch (_attrName)
      {
        case "id":
          var val = ConsumeValue();
          if (_stack.Peek() is Item && val.IsGuid())
            AddAttribute(new IdAnnotation(_stack.Peek(), new Guid(val)));
          else
            AddAttribute(new Attribute(_stack.Peek() as Element, _attrName, val));
          break;
        default:
          AddAttribute(new Attribute(_stack.Peek() as Element, _attrName, ConsumeValue()));
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


    public override void WriteEndElement()
    {
      if (_base != null)
      {
        _base.WriteEndElement();
        return;
      }
      if (_value != null)
        _stack.Peek().Add(ConsumeValue());
      _stack.Pop();
      if (!_stack.Any())
        _parent.SignalCompletion();
    }

    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      if (_base != null)
      {
        _base.WriteStartElement(prefix, localName, ns);
        return;
      }
      _value = null;
      var name = GetName(localName, ns);
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
  }

  internal class AmlElementWriter : BaseAmlWriter
  {
    private Element _root;
    private Element _curr;

    public Element Root { get { return _root; } }

    public AmlElementWriter(ElementFactory factory, BaseAmlWriter parent) : base(factory)
    {
      _parent = parent;
    }

    public override void WriteEndAttribute()
    {
      if (DefaultEndAttribute())
        return;
      _curr.Add(new Attribute(_attrName, ConsumeValue()));
    }

    public override void WriteEndElement()
    {
      if (_base != null)
      {
        _base.WriteEndElement();
        return;
      }
      if (_value != null)
      {
        _curr.Value = ConsumeValue();
      }
      _curr = _curr.Parent as Element;
      if (_curr == null
        || (!_curr.Exists && _root != _curr))
        _parent.SignalCompletion();
    }

    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      if (_base != null)
      {
        _base.WriteStartElement(prefix, localName, ns);
        return;
      }
      _value = null;
      var newElem = new AmlElement(_factory, GetName(localName, ns));
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
  }

  internal class BaseAmlWriter : XmlWriter
  {
    public const string IgnoreAttribute = "~`IGNORE`~";
    protected ElementFactory _factory;
    protected BaseAmlWriter _parent;
    protected XmlWriter _base;
    protected string _attrName;
    protected string _value;
    private bool _inElement;

    public BaseAmlWriter(ElementFactory factory)
    {
      _factory = factory;
    }

    public override WriteState WriteState
    {
      get
      {
        if (_base != null) return _base.WriteState;
        throw new NotSupportedException();
      }
    }

    public override void Close()
    {
      if (_base != null)
      {
        _base.Close();
        return;
      }
      throw new NotImplementedException();
    }

    public override void Flush()
    {
      if (_base != null)
      {
        _base.Flush();
        return;
      }
      throw new NotImplementedException();
    }

    public override string LookupPrefix(string ns)
    {
      if (_base != null)
      {
        return _base.LookupPrefix(ns);
      }
      else
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
    }

    public override void WriteBase64(byte[] buffer, int index, int count)
    {
      if (_base != null)
      {
        _base.WriteBase64(buffer, index, count);
        return;
      }
      throw new NotSupportedException();
    }

    public override void WriteCData(string text)
    {
      if (_base != null)
      {
        _base.WriteCData(text);
        return;
      }
      _value = (_value ?? "") + text;
    }

    public override void WriteCharEntity(char ch)
    {
      if (_base != null)
      {
        _base.WriteCharEntity(ch);
        return;
      }
      _value = (_value ?? "") + new string(ch, 1);
    }

    public override void WriteChars(char[] buffer, int index, int count)
    {
      if (_base != null)
      {
        _base.WriteChars(buffer, index, count);
        return;
      }
      _value = (_value ?? "") + new string(buffer, index, count);
    }

    public override void WriteComment(string text)
    {
      if (_base != null) _base.WriteComment(text);
    }

    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {
      if (_base != null) _base.WriteDocType(name, pubid, sysid, subset);
    }

    public override void WriteEndAttribute()
    {
      DefaultEndAttribute();
    }

    protected bool DefaultEndAttribute()
    {
      if (_base != null)
      {
        _base.WriteEndAttribute();
        return true;
      }
      else if (_attrName == IgnoreAttribute)
      {
        _value = null;
        return true;
      }
      return false;
    }

    protected string ConsumeValue()
    {
      var result = _value;
      _value = null;
      return result;
    }

    public override void WriteEndDocument()
    {
      if (_base != null) _base.WriteEndDocument();
    }

    public override void WriteEndElement()
    {
      if (_base == null)
      {
        _inElement = false;
      }
      else
      {
        _base.WriteEndElement();
      }
    }

    public override void WriteEntityRef(string name)
    {
      if (_base != null)
      {
        _base.WriteEntityRef(name);
      }
      else if (name == "amp")
      {
        _value = (_value ?? "") + "&";
      }
      else if (name == "apos")
      {
        _value = (_value ?? "") + "'";
      }
      else if (name == "gt")
      {
        _value = (_value ?? "") + ">";
      }
      else if (name == "lt")
      {
        _value = (_value ?? "") + "<";
      }
      else if (name == "quot")
      {
        _value = (_value ?? "") + "\"";
      }
      else
      {
        throw new NotSupportedException();
      }
    }

    public override void WriteFullEndElement()
    {
      if (_base != null)
      {
        _base.WriteFullEndElement();
      }
      else
      {
        WriteEndElement();
      }
    }

    public override void WriteProcessingInstruction(string name, string text)
    {
      if (_base != null) _base.WriteProcessingInstruction(name, text);
    }

    public override void WriteRaw(string data)
    {
      if (_base != null)
      {
        _base.WriteRaw(data);
        return;
      }
      _value = (_value ?? "") + data;
    }

    public override void WriteRaw(char[] buffer, int index, int count)
    {
      if (_base != null)
      {
        _base.WriteRaw(buffer, index, count);
        return;
      }
      _value = (_value ?? "") + new string(buffer, index, count);
    }

    public override void WriteStartAttribute(string prefix, string localName, string ns)
    {
      if (_base != null)
      {
        _base.WriteStartAttribute(prefix, localName, ns);
        return;
      }
      _attrName = GetName(localName, ns);
      _value = null;
    }

    public override void WriteStartDocument()
    {
      if (_base != null) _base.WriteStartDocument();
    }

    public override void WriteStartDocument(bool standalone)
    {
      if (_base != null) _base.WriteStartDocument(standalone);
    }

    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      if (_base != null)
      {
        _base.WriteStartElement(prefix, localName, ns);
        return;
      }
      _value = null;
      _inElement = true;
    }

    public override void WriteString(string text)
    {
      if (_base != null)
      {
        _base.WriteString(text);
        return;
      }
      _value = (_value ?? "") + text;
    }

    public override void WriteSurrogateCharEntity(char lowChar, char highChar)
    {
      if (_base != null)
      {
        _base.WriteSurrogateCharEntity(lowChar, highChar);
        return;
      }
      _value = (_value ?? "") + new string(new char[] { highChar, lowChar });
    }

    public override void WriteWhitespace(string ws)
    {
      if (_base != null)
      {
        _base.WriteWhitespace(ws);
        return;
      }
      if (_inElement) _value = (_value ?? "") + ws;
    }

    protected string GetName(string localName, string ns)
    {
      switch ((ns ?? "").TrimEnd('/'))
      {
        case "http://www.w3.org/2000/xmlns":
          return IgnoreAttribute;
        default:
          var prefix = LookupPrefix(ns);
          if (!string.IsNullOrEmpty(prefix))
            return prefix + ":" + localName;
          return localName;
      }
    }

    public virtual void SignalCompletion()
    {
      _base = null;
    }
  }
}
