using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes.Data;
using Pipes.Xml;
using System.Xml.Linq;

namespace Pipes.Aras
{
  public class ItemXmlReader : IPipeOutput<IItem>, IPipeInput<Xml.IXmlNode>
  {
    private IEnumerable<Xml.IXmlNode> _reader;

    public ItemXmlReader() { }

    public IEnumerator<IItem> GetEnumerator()
    {
      var elementPath = new Stack<string>();
      var results = new Stack<object>();
      IXmlNode node;
      bool foundItem = false;

      using (var e = _reader.GetEnumerator())
      {
        while (e.MoveNext())
        {
          node = e.Current;
          switch (node.Type)
          {
            case Xml.XmlNodeType.EmptyElement:
            case Xml.XmlNodeType.Element:
              if (node.Type == Xml.XmlNodeType.Element) elementPath.Push(node.Name.ToString());
              if (node.Name.ToString() == "SOAP-ENV:Fault")
              {
                results.Push(new Error());
              }
              else if (node.Name.LocalName == "Item")
              {
                foundItem = true;
                var item = new Item();
                foreach (var attr in (IEnumerable<Data.IFieldValue>)node)
                {
                  item.Attribute(attr.Name, attr.Value);
                }

                if (results.Count == 0)
                {
                  if (node.Type == Xml.XmlNodeType.EmptyElement)
                  {
                    yield return item;
                  }
                  else
                  {
                    results.Push(item);
                  }
                }
                else
                {
                  var prop = results.Peek() as Property;
                  if (prop == null)
                  {
                    throw new InvalidOperationException("An Item element can only appear within a property element.");
                  }
                  else
                  {
                    PropSetOrAppend<IDataItem>(prop, item);
                  }

                  if (node.Type != Xml.XmlNodeType.EmptyElement) results.Push(item);
                }
              }
              else if (node.Name.LocalName == "Result" && node.Type == Xml.XmlNodeType.EmptyElement && !foundItem)
              {
                yield return new Result(null);
                yield break;
              }
              else if (node.Name.LocalName == "detail" && results.Peek() is IError)
              {
                XElement detailElem = new XElement("FAULT");
                var elemWriter = new XElementWriter(detailElem);
                e.WriteTo(elemWriter);
                var wrapper = new XElementWrapper(detailElem.Elements().First());
                ((Error)results.Peek()).Data = wrapper;

                var itemElem = (from el in wrapper.Elements() where el.Name.ToString() == "af:item" select el).SingleOrDefault();
                if (itemElem != null)
                {
                  using (var writer = new System.IO.StringWriter())
                  {
                    var settings = new Xml.XmlWriterSettings();
                    settings.OmitXmlDeclaration = true;
                    using (var xml = Xml.XmlTextWriter.Create(writer, settings))
                    {
                      xml.Element("Item");
                      foreach (var attr in itemElem.OfType<IXmlFieldValue>())
                      {
                        if (attr.XmlName.Prefix != "xmlns") xml.Attribute(attr.XmlName.LocalName, attr.Value);
                      }
                      xml.ElementEnd();
                    }
                    ((Error)results.Peek()).Detail = writer.ToString();
                  }
                }

                var legacyStringElem = (from el in wrapper.Elements() where el.Name.ToString() == "af:legacy_faultstring" select el).SingleOrDefault();
                if (legacyStringElem != null)
                {
                  ((Error)results.Peek()).Detail = (legacyStringElem.Value == null ? string.Empty : legacyStringElem.Value.ToString());
                }
              }
              else if (results.Count > 0)
              {
                var newProp = new Property();
                foreach (var attr in (IEnumerable<Data.IFieldValue>)node)
                {
                  newProp.Attribute(attr.Name, attr.Value);
                }
                newProp.Name = node.Name.ToString();

                var item = results.Peek() as Item;
                if (item == null)
                {
                  var prop = results.Peek() as Property;
                  if (prop != null)
                  {
                    PropSetOrAppend<IProperty>(prop, newProp);
                    if (node.Type != Xml.XmlNodeType.EmptyElement) results.Push(newProp);
                  }
                }
                else
                {
                  item.Property(node.Name.ToString(), newProp);
                  if (node.Type != Xml.XmlNodeType.EmptyElement) results.Push(newProp);
                }
              }
              break;
            case Xml.XmlNodeType.EndElement:
              elementPath.Pop();
              if (results.Count > 0)
              {
                if (node.Name.ToString() == "SOAP-ENV:Fault" && results.Peek() is IError)
                {
                  yield return (IError)results.Peek();
                  yield break;
                }
                else if (node.Name.LocalName == "Item" && results.Peek() is IDataItem)
                {
                  var item = results.Pop() as IDataItem;
                  if (results.Count == 0) yield return item;
                }
                else
                {
                  var prop = results.Peek() as Property;
                  if (prop != null)
                  {
                    if (prop.Name == node.Name.ToString())
                    {
                      results.Pop();
                      PropSet(prop, prop.Value);
                    }
                  }
                }
              }
              break;
            case Xml.XmlNodeType.Text:
              if (elementPath.Peek() == "Result" && !foundItem)
              {
                yield return new Result(node.Value == null ? string.Empty : node.Value.ToString());
                yield break;
              }
              else if (elementPath.Peek() == "event" && !node.Status("name").IsNullOrEmpty())
              {
                yield return new Event(node.Item("name").ToString(), node.Value == null ? string.Empty : node.Value.ToString());
              }
              else if (results.Count > 0)
              {
                if (results.Peek() is IError)
                {
                  if (elementPath.Peek() == "faultcode")
                  {
                    ((Error)results.Peek()).Code = (node.Value == null ? string.Empty : node.Value.ToString());
                  }
                  else if (elementPath.Peek() == "faultstring")
                  {
                    var detail = (node.Value == null ? string.Empty : node.Value.ToString());
                    if (detail == "Aras.Server.Core.ItemNotFoundException")
                    {
                      detail = "No items found.";
                    }
                    ((Error)results.Peek()).Message = detail;
                  }
                }
                else
                {
                  var prop = results.Peek() as Property;
                  if (prop != null) PropSet(prop, node.Value);
                }
              }
              break;
          }
        }
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Initialize(IEnumerable<Xml.IXmlNode> source)
    {
      _reader = source;
    }

    private void PropSetOrAppend<T>(Property prop, T value)
    {
      if (prop.Value == null || (prop.Value is string && (string)prop.Value == ""))
      {
        PropSet(prop, value);
      }
      else if (prop.Value is T)
      {
        prop.Value = new List<T>() { (T)prop.Value, value };
      }
      else if (prop.Value is IList<T>)
      {
        ((IList<T>)prop.Value).Add(value);
      }
      else if (prop.Value is IList)
      {
        ((IList)prop.Value).Add(value);
      }
      else
      {
        prop.Value = new List<object>() { prop.Value, value };
      }
    }
    private void PropSet(Property prop, object value)
    {
      if (value == null && (string)prop.Attribute("is_null") != "1")
      {
        prop.Value = string.Empty;
      }
      else
      {
        prop.Value = value;
      }
    }
  }
}
