using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  internal static class Utils
  {
    public static StringBuilder AppendSeparator(this StringBuilder builder, string separator, object value)
    {
      if (builder.Length > 0) builder.Append(separator);
      return builder.Append(value);
    }

    public static bool StringEquals(string x, string y)
    {
      return StringEquals(x, y, StringComparison.CurrentCulture);
    }
    public static bool StringEquals(string x, string y, StringComparison compare)
    {
      if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
      {
        return true;
      }
      else if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
      {
        return false;
      }
      else
      {
        return string.Compare(x, y, compare) == 0;
      }
    }

    public static XmlElement AppendElement(this XmlNode node, string localName)
    {
      if (node == null) return null;
      var newElem = node.OwnerDocument.CreateElement(localName);
      node.AppendChild(newElem);
      return newElem;
    }
    public static string Attribute(this XmlNode elem, string localName, string defaultValue = null)
    {
      if (elem == null || elem.Attributes == null) return defaultValue;
      var attr = elem.Attributes[localName];
      if (attr == null)
      {
        return defaultValue;
      }
      else
      {
        return attr.Value;
      }
    }
    public static void Detatch(this XmlNode node)
    {
      if (node != null && node.ParentNode != null) node.ParentNode.RemoveChild(node);
    }
    public static XmlElement Element(this XmlNode node, string localName)
    {
      if (node == null) return null;
      return node.ChildNodes.OfType<XmlElement>().SingleOrDefault(e => e.LocalName == localName);
    }
    public static string Element(this XmlNode node, string localName, string defaultValue)
    {
      if (node == null) return defaultValue;
      var elem = node.Element(localName);
      if (elem == null) return defaultValue;
      return elem.InnerText;
    }
    public static XmlElement Element(this IEnumerable<XmlElement> nodes, string localName)
    {
      if (nodes == null) return null;
      return nodes.SelectMany(n => n.ChildNodes.OfType<XmlElement>()).SingleOrDefault(e => e.LocalName == localName);
    }
    public static IEnumerable<XmlElement> Elements(this XmlNode node)
    {
      return node.ChildNodes.OfType<XmlElement>();
    }
    public static IEnumerable<XmlElement> Elements(this XmlNode node, string localName)
    {
      return node.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == localName);
    }
    public static IEnumerable<XmlElement> Elements(this XmlNode node, Func<XmlElement, bool> predicate)
    {
      return node.ChildNodes.OfType<XmlElement>().Where(predicate);
    }
    public static IEnumerable<XmlElement> Elements(this IEnumerable<XmlElement> nodes, Func<XmlElement, bool> predicate)
    {
      return nodes.SelectMany(n => n.ChildNodes.OfType<XmlElement>()).Where(predicate);
    }
    public static IEnumerable<XmlElement> Elements(this IEnumerable<XmlElement> nodes, string localName)
    {
      return nodes.SelectMany(n => n.ChildNodes.OfType<XmlElement>()).Where(e => e.LocalName == localName);
    }
    public static IEnumerable<XmlElement> ElementsByXPath(this XmlNode node, string xPath)
    {
      return node.SelectNodes(xPath).OfType<XmlElement>();
    }
    public static XmlNode Parent(this XmlNode node)
    {
      if (node == null) return null;
      return node.ParentNode;
    }
    public static IEnumerable<XmlElement> Parents(this XmlNode node)
    {
      if (node == null) yield break;

      var parent = node.ParentNode as XmlElement;
      while (parent != null)
      {
        yield return parent;
        parent = parent.ParentNode as XmlElement;
      }
    }

    public static IEnumerable<T> DependencySort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies, bool throwOnCycle = false)
    {
      IList<T> cycle = new List<T>();
      return DependencySort(source, dependencies, ref cycle, throwOnCycle);
    }
    public static IEnumerable<T> DependencySort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies, ref IList<T> cycle, bool throwOnCycle = false)
    {
      var sorted = new List<T>();
      var visited = new HashSet<T>();

      foreach (var item in source)
        Visit(item, visited, sorted, dependencies, cycle, throwOnCycle);

      return sorted;
    }

    private static bool Visit<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> dependencies, IList<T> cycle, bool throwOnCycle)
    {
      var hasCycle = false;
      if (!hasCycle) cycle.Add(item);

      if (!visited.Contains(item))
      {
        visited.Add(item);

        foreach (var dep in (dependencies(item) ?? Enumerable.Empty<T>()).Where(d => !Object.ReferenceEquals(d, item)))
          hasCycle = Visit(dep, visited, sorted, dependencies, cycle, throwOnCycle) || hasCycle;

        sorted.Add(item);
      }
      else
      {
        if (!sorted.Contains(item))
        {
          System.Diagnostics.Debug.Print("Cyclic dependency found");
          if (throwOnCycle)
          {
            var ex = new Exception("Cyclic dependency found");
            ex.Data["Cycle"] = cycle;
            throw ex;
          }
          else
          {
            return true;
          }
        }
      }

      if (!hasCycle) cycle.RemoveAt(cycle.Count - 1);
      return hasCycle;
    }
  }
}
