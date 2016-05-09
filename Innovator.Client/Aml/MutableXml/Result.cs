using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal class Result : Element, IResult
  {
    private string _query;
    private IConnection _conn;
    private IReadOnlyItem _errorContext;
    private List<string> _errors;
    private List<string> _properties;

    public ServerException Exception
    {
      get
      {
        if (_errors != null && _errors.Any())
        {
          if (_errorContext == null)
          {
            return _factory.ServerException(_errors.GroupConcat(Environment.NewLine))
              .SetDetails(_conn == null ? null : _conn.Database, _query);
          }
          else
          {
            var props = (_properties ?? Enumerable.Empty<string>()).ToArray();
            return _factory.ValidationException(_errors.GroupConcat(Environment.NewLine), _errorContext, props)
              .SetDetails(_conn == null ? null : _conn.Database, _query);
          }
        }

        if (_node.LocalName == "Envelope"
          && (_node.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || string.IsNullOrEmpty(_node.NamespaceURI))
          && _node.HasChildNodes)
        {
          var node = _node.ChildNodes.OfType<XmlElement>().First();
          if (node.LocalName == "Body"
            && (node.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || string.IsNullOrEmpty(node.NamespaceURI))
            && node.HasChildNodes)
          {
            node = node.ChildNodes.OfType<XmlElement>().First();
            if (node.LocalName == "Fault"
              && (node.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || string.IsNullOrEmpty(node.NamespaceURI))
              && node.HasChildNodes)
            {
              return _factory.ServerException(node).SetDetails(_conn == null ? null : _conn.Database, _query);
            }
          }
        }
        return null;
      }
    }
    public new string Value
    {
      get
      {
        var exception = this.Exception;
        if (exception != null) throw exception;
        var node = FirstDescendantOrSelf("Result");
        if (node == null) return null;
        return node.InnerText;
      }
      set
      {
        var node = FindOrCreateResult();
        if (node != null) node.InnerText = value;
      }
    }

    internal Result(ElementFactory factory)
      : this(factory, "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\"><SOAP-ENV:Body></SOAP-ENV:Body></SOAP-ENV:Envelope>")
    {
      _node = _node.ChildNodes.OfType<XmlElement>().SingleOrDefault();
    }
    internal Result(ElementFactory factory, string xml) : base(factory, GetElem(xml)) { }
    internal Result(ElementFactory factory, string xml, string query, IConnection conn) : base(factory, GetElem(xml))
    {
      _query = query;
      _conn = conn;
    }
    internal Result(ElementFactory factory, XmlNode xml, string query, IConnection conn) : base(factory, GetElem(xml))
    {
      _query = query;
      _conn = conn;
    }
    internal Result(ElementFactory factory, XmlNode node) : base(factory, GetElem(node)) {}

    private static XmlElement GetElem(string xml)
    {
      var doc = new XmlDocument(Element.BufferDocument.NameTable);
      doc.LoadXml(xml);
      return doc.DocumentElement;
    }
    private static XmlElement GetElem(XmlNode node)
    {
      var doc = node as XmlDocument;
      if (doc != null)
      {
        return doc.DocumentElement;
      }
      else
      {
        return (XmlElement)node;
      }
    }

    public IResult Add(IItem content)
    {
      return Add((Item)content);
    }
    public IResult Add(Item content)
    {
      var result = FindOrCreateResult();
      result.AppendChild(result.GetLocalNode(content.Node));
      return this;
    }
    public override IElement Add(object content)
    {
      var origNode = _node;
      IElement result = null;
      try
      {
        _node = (XmlElement)FindOrCreateResult();
        result = base.Add(content);
      }
      finally
      {
        _node = origNode;
      }
      return result;
    }

    /// <summary>
    /// Throws any contained exception
    /// </summary>
    public IResult AssertNoError()
    {
      var exception = this.Exception;
      if (exception != null) throw exception;
      return this;
    }
    /// <summary>
    /// Throws an exception if the result does not represent a single item specified
    /// </summary>
    /// <returns>A single item</returns>
    public IItem AssertItem(string type = null)
    {
      var item = (IItem)_factory.ElementFromXml(AssertItemNode().ParentNode.ChildNodes.OfType<XmlElement>().Single(
        e => e.LocalName == "Item",
        i => i < 1
          ? NewNoItemsException()
          : NewServerException("Multiple items found when only one was expected")
      ));
      if (string.IsNullOrEmpty(type) || item.Type().Value == type) return item;
      throw NewServerException("Found item of type '{0}' when expecting item of type '{1}'", item.Type().Value, type);
    }
    /// <summary>
    /// Asserts that there are one or more items present
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IReadOnlyItem> AssertItems()
    {
      var result = this.Items();
      if (result.Any()) return result;
      throw NewNoItemsException();
    }
    /// <summary>
    /// Asserts that there is no error (other than a no items found error).  Returns the items (if any) or an empty enumerable otherwise.
    /// </summary>
    public IEnumerable<IReadOnlyItem> Items()
    {
      var exception = this.Exception;
      if (exception is NoItemsFoundException) return Enumerable.Empty<IReadOnlyItem>();
      if (exception != null) throw exception;
      var node = FirstDescendantOrSelf("Item");
      if (node == null) return Enumerable.Empty<IReadOnlyItem>();
      return node.ParentNode.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == "Item").Select(e => (IReadOnlyItem)_factory.ElementFromXml(e));
    }

    public override string ToString()
    {
      var ex = this.Exception;
      if (ex == null) return _node.OuterXml;
      return ex.AsAmlString();
    }

    private XmlNode AssertItemNode()
    {
      var exception = this.Exception;
      if (exception != null) throw exception;
      var node = FirstDescendantOrSelf("Item");
      if (node == null) throw NewNoItemsException();
      return node;
    }
    private XmlNode FindOrCreateResult()
    {
      var node = FirstDescendantOrSelf("Result");
      if (node == null && _node.LocalName == "Body" && (_node.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || string.IsNullOrEmpty(_node.NamespaceURI)))
        node = _node.AppendChild(_node.OwnerDocument.CreateElement("Result"));
      return node;
    }
    private XmlNode FirstDescendantOrSelf(string name)
    {
      var node = _node;
      while (node != null && node.LocalName != name) node = node.ChildNodes.OfType<XmlElement>().FirstOrDefault();
      return node;
    }

    private ServerException NewNoItemsException()
    {
      return (this.Exception as NoItemsFoundException) ??
        _factory.NoItemsFoundException("?", _query ?? "?").SetDetails(_conn == null ? null : _conn.Database, _query);
    }
    private ServerException NewServerException(string message)
    {
      return _factory.ServerException(message).SetDetails(_conn == null ? null : _conn.Database, _query);
    }
    private ServerException NewServerException(string format, params object[] args)
    {
      return NewServerException(string.Format(format, args));
    }

    IReadOnlyResult IReadOnlyResult.AssertNoError()
    {
      return AssertNoError();
    }

    public IErrorBuilder ErrorContext(IReadOnlyItem item)
    {
      _errorContext = item;
      return this;
    }

    public IErrorBuilder ErrorMsg(string message)
    {
      if (_errors == null) _errors = new List<string>();
      _errors.Add(message);
      return this;
    }

    public IErrorBuilder ErrorMsg(string message, params string[] properties)
    {
      return ErrorMsg(message, (IEnumerable<string>)properties);
    }

    public IErrorBuilder ErrorMsg(string message, IEnumerable<string> properties)
    {
      if (_errors == null) _errors = new List<string>();
      _errors.Add(message);
      if (properties.Any())
      {
        if (_properties == null) _properties = new List<string>();
        _properties.AddRange(properties);
      }
      return this;
    }

    IReadOnlyItem IReadOnlyResult.AssertItem(string type)
    {
      return this.AssertItem(type);
    }

    public override string ToAml()
    {
      var ex = this.Exception;
      if (ex != null)
        return ex.AsAmlString();
      return base.ToAml();
    }
  }
}
