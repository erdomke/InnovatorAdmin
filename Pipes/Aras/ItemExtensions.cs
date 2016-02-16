using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes.Data;

namespace Pipes.Aras
{
  public static class ItemExtensions
  {
    public static string Id(this IDataItem item)
    {
      if (item.AttributeStatus("id").IsNullOrEmpty())
      {
        if (item.Status("id").IsNullOrEmpty())
        {
          return null;
        }
        else
        {
          var prop = item.Property("id");
          if (prop.AttributeStatus("condition").IsNullOrEmpty() || string.Compare(prop.Attribute("condition").ToString(), "eq", true) == 0 )
          {
            return prop.Value.ToString();
          }
          else 
          {
            return null;
          }
        }
      }
      else
      {
        return item.Attribute("id").ToString();
      }
    }
    public static T Property<T>(this IDataItem item, string name, T defaultValue)
    {
      if (item.Status(name).IsNullOrEmpty())
      {
        return defaultValue;
      }
      else
      {
        return (T)item.Item(name);
      }
    }
    public static IDataItem PropertyItem(this IDataItem item, string name)
    {
      if (item.Status(name).IsNullOrEmpty())
      {
        return null;
      }
      else
      {
        var prop = item.Property(name);
        if (prop.Value is IDataItem)
        {
          return (IDataItem)prop.Value;
        }
        else if (prop.AttributeStatus("type").IsNullOrEmpty())
        {
          return null;
        }
        else
        {
          var result = new Item();
          result.Attribute("type", prop.Attribute("type"));
          result.Attribute("id", prop.Value);
          var idProp = new Property() { Name = "id", Value = prop.Value };
          idProp.Attribute("type", prop.Attribute("type"));
          if (!prop.AttributeStatus("keyed_name").IsNullOrEmpty())
          {
            idProp.Attribute("keyed_name", prop.Attribute("keyed_name"));
            result.Property("keyed_name", prop.Attribute("keyed_name"));
          }
          result.Property("id", idProp);
          return result;
        }
      }
    }
    public static T PropertyAttribute<T>(this IDataItem item, string name, string attribute, T defaultValue)
    {
      if (item.Status(name).IsNullOrEmpty() || item.Property(name).AttributeStatus(attribute).IsNullOrEmpty())
      {
        return defaultValue;
      }
      else
      {
        return (T)item.Property(name).Attribute(attribute);
      }
    }
    public static void WriteTo(this IEnumerable<IItem> data, Sgml.ISgmlWriter writer, bool isResult)
    {
      int closeCount = 0;

      foreach (var item in data)
      {
        if (isResult)
        {
          writer.NsElement("SOAP-ENV", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
          writer.NsElement("Body", "http://schemas.xmlsoap.org/soap/envelope/");
          isResult = false;
          closeCount = 2;
          if (item is IDataItem) 
          { 
            writer.Element("Result");
            closeCount++;
          }
        }
        if (item is IDataItem)
        {
          ((IDataItem)item).WriteTo(writer);
        }
        else if (item is IResult)
        {
          ((IResult)item).WriteTo(writer);
        }
        else if (item is IError)
        {
          ((IError)item).WriteTo(writer);
        }
      }

      for (var i = 0; i < closeCount; i++ )
      {
        writer.ElementEnd();        
      }
    }
    public static void WriteTo(this IResult result, Sgml.ISgmlWriter writer)
    {
      writer.Element("Result", result.Value);
    }
    public static void WriteTo(this IError error, Sgml.ISgmlWriter writer)
    {
      writer.NsElement("Fault", "http://schemas.xmlsoap.org/soap/envelope/");
      writer.Element("faultcode", error.Code);
      writer.Element("faultstring", error.Message);
      writer.ElementEnd();
    }
    public static void WriteTo(this IDataItem item, Sgml.ISgmlWriter writer)
    {
      writer.Element("Item");
      foreach (var attr in item.Attributes)
      {
        writer.Attribute(attr.Name, attr.Value);
      }
      foreach (var prop in item)
      {
        if (prop is IProperty)
        {
          ((IProperty)prop).WriteTo(writer);
        }
        else
        {
          writer.Element(prop.Name, prop.Value);
        }
      }
      writer.ElementEnd();
    }
    public static void WriteTo(this IProperty prop, Sgml.ISgmlWriter writer)
    {
      writer.Element(prop.Name);
      foreach (var attr in prop.Attributes)
      {
        if (attr.Name.StartsWith("xml:"))
        {
          writer.Attribute("xml", attr.Name.Substring(4), "http://www.w3.org/XML/1998/namespace", attr.Value);
        }
        else
        {
          writer.Attribute(attr.Name, attr.Value);
        }
      }
      if (prop.Value is IDataItem)
      {
        ((IDataItem)prop.Value).WriteTo(writer);
      }
      else if (prop.Value is IEnumerable<IDataItem>)
      {
        ((IEnumerable<IDataItem>)prop.Value).Cast<IItem>().WriteTo(writer, false);
      }
      else
      {
        writer.Value(prop.Value);
      }
      writer.ElementEnd();
    }
  }
}
