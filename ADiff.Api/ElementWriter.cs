using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ADiff
{
  internal struct ElementWriter
  {
    private LineWriter lines;
    private NamespaceResolver resolver;
    private XmlWriter writer;
    public int CurrentLine { get { return lines.Line; } }

    public ElementWriter(XmlWriterSettings settings)
    {
      this.lines = new LineWriter();
      this.writer = XmlWriter.Create(lines, settings);
      this.resolver = default(NamespaceResolver);
    }

    public void WriteElement(XElement e)
    {
      e.WriteTo(this.writer);
      this.writer.Flush();
    }

    public void WriteEndElement()
    {
      this.writer.WriteEndElement();
      this.writer.Flush();
    }

    public void WriteStartElement(XElement e)
    {
      var ns = e.Name.Namespace;
      this.writer.WriteStartElement(this.GetPrefixOfNamespace(ns, true), e.Name.LocalName, ns.NamespaceName);
      for (var xAttribute = e.FirstAttribute; xAttribute != null; xAttribute = xAttribute.NextAttribute)
      {
        ns = xAttribute.Name.Namespace;
        string localName = xAttribute.Name.LocalName;
        string namespaceName = ns.NamespaceName;
        this.writer.WriteAttributeString(this.GetPrefixOfNamespace(ns, false), localName, (namespaceName.Length == 0 && localName == "xmlns") ? "http://www.w3.org/2000/xmlns/" : namespaceName, xAttribute.Value);
      }
      this.writer.Flush();
    }

    public void WriteStartOrElement(XElement e)
    {
      if (e.Elements().Any())
      {
        WriteStartElement(e);
      }
      else
      {
        WriteElement(e);
      }
    }

    public override string ToString()
    {
      return this.lines.ToString();
    }

    private string GetPrefixOfNamespace(XNamespace ns, bool allowDefaultNamespace)
    {
      string namespaceName = ns.NamespaceName;
      if (namespaceName.Length == 0)
      {
        return string.Empty;
      }
      string prefixOfNamespace = this.resolver.GetPrefixOfNamespace(ns, allowDefaultNamespace);
      if (prefixOfNamespace != null)
      {
        return prefixOfNamespace;
      }
      if (namespaceName == "http://www.w3.org/XML/1998/namespace")
      {
        return "xml";
      }
      if (namespaceName == "http://www.w3.org/2000/xmlns/")
      {
        return "xmlns";
      }
      return null;
    }

    internal struct NamespaceResolver
    {
      private class NamespaceDeclaration
      {
        public string prefix;
        public XNamespace ns;
        public int scope;
        public NamespaceResolver.NamespaceDeclaration prev;
      }
      private int scope;
      private NamespaceResolver.NamespaceDeclaration declaration;
      private NamespaceResolver.NamespaceDeclaration rover;

      public void PushScope()
      {
        this.scope++;
      }
      public void PopScope()
      {
        var prev = this.declaration;
        if (prev != null)
        {
          do
          {
            prev = prev.prev;
            if (prev.scope != this.scope)
            {
              break;
            }
            if (prev == this.declaration)
            {
              this.declaration = null;
            }
            else
            {
              this.declaration.prev = prev.prev;
            }
            this.rover = null;
          }
          while (prev != this.declaration && this.declaration != null);
        }
        this.scope--;
      }
      public void Add(string prefix, XNamespace ns)
      {
        var namespaceDeclaration = new NamespaceResolver.NamespaceDeclaration();
        namespaceDeclaration.prefix = prefix;
        namespaceDeclaration.ns = ns;
        namespaceDeclaration.scope = this.scope;
        if (this.declaration == null)
        {
          this.declaration = namespaceDeclaration;
        }
        else
        {
          namespaceDeclaration.prev = this.declaration.prev;
        }
        this.declaration.prev = namespaceDeclaration;
        this.rover = null;
      }
      public void AddFirst(string prefix, XNamespace ns)
      {
        var namespaceDeclaration = new NamespaceResolver.NamespaceDeclaration();
        namespaceDeclaration.prefix = prefix;
        namespaceDeclaration.ns = ns;
        namespaceDeclaration.scope = this.scope;
        if (this.declaration == null)
        {
          namespaceDeclaration.prev = namespaceDeclaration;
        }
        else
        {
          namespaceDeclaration.prev = this.declaration.prev;
          this.declaration.prev = namespaceDeclaration;
        }
        this.declaration = namespaceDeclaration;
        this.rover = null;
      }

      public string GetPrefixOfNamespace(XNamespace ns, bool allowDefaultNamespace)
      {
        if (this.rover != null && this.rover.ns == ns && (allowDefaultNamespace || this.rover.prefix.Length > 0))
        {
          return this.rover.prefix;
        }
        var prev = this.declaration;
        if (prev != null)
        {
          while (true)
          {
            prev = prev.prev;
            if (prev.ns == ns)
            {
              var prev2 = this.declaration.prev;
              while (prev2 != prev && prev2.prefix != prev.prefix)
              {
                prev2 = prev2.prev;
              }
              if (prev2 == prev)
              {
                if (allowDefaultNamespace)
                {
                  break;
                }
                if (prev.prefix.Length > 0)
                {
                  return prev.prefix;
                }
              }
            }
            if (prev == this.declaration)
            {
              return null;
            }
          }
          this.rover = prev;
          return prev.prefix;
        }

        return null;
      }
    }
  }
}
