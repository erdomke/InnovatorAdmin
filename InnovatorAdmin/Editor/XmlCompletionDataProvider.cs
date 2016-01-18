// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2130 $</version>
// </file>

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Linq;

namespace InnovatorAdmin.Editor
{
  /// <summary>
  /// Provides the autocomplete (intellisense) data for an
  /// xml document that specifies a known schema.
  /// </summary>
  public class XmlCompletionDataProvider : XmlEditorHelper
  {
    //public static XmlCompletionDataProvider FromResource(string path)
    //{
    //  var schema = new XmlSchema();
    //  using (Stream stream = typeof(XmlCompletionDataProvider).Assembly.GetManifestResourceStream("InnovatorAdmin.Editor." + path))
    //  {
    //    using (var reader = XmlReader.Create(stream))
    //    {
    //      schema = XmlSchema.Read(reader, new ValidationEventHandler(SchemaValidation));
    //      schema.Compile(new ValidationEventHandler(SchemaValidation));
    //    }
    //  }
    //  return new XmlCompletionDataProvider(schema);
    //}

    ///// <summary>
    ///// Handler for schema validation errors.
    ///// </summary>
    //private static void SchemaValidation(object source, ValidationEventArgs e)
    //{
    //  // Do nothing.
    //}

    XmlSchemaCompletionDataCollection schemaCompletionDataItems;
    XmlSchemaCompletionData defaultSchemaCompletionData;
    string defaultNamespacePrefix = String.Empty;

    public XmlCompletionDataProvider(XmlSchemaSet schemaSet, string defaultNamespace
      , string prefix = null
      , Func<string> namespaceWriter = null
      , Func<XmlSchemaElement, bool> filter = null)
    {
      var def = schemaSet.Schemas(defaultNamespace).OfType<XmlSchema>().First();

      this.schemaCompletionDataItems = new XmlSchemaCompletionDataCollection();
      foreach (var schema in schemaSet.Schemas().OfType<XmlSchema>().Where(s => s != def))
      {
        this.schemaCompletionDataItems.Add(new XmlSchemaCompletionData(schema)
        {
          ElementFilter = filter
        });
      }
      this.defaultSchemaCompletionData = new XmlSchemaCompletionData(def)
      {
        RelatedSchemas = this.schemaCompletionDataItems,
        NamespaceWriter = namespaceWriter,
        ElementFilter = filter
      };
      this.defaultNamespacePrefix = prefix ?? string.Empty;
    }

    public XmlCompletionDataProvider(XmlSchema schema)
    {
      this.schemaCompletionDataItems = new XmlSchemaCompletionDataCollection();
      this.defaultSchemaCompletionData = new XmlSchemaCompletionData(schema);
      this.defaultNamespacePrefix = string.Empty;
    }

    public virtual IEnumerable<ICompletionData> HandleTextEntered(ITextSource source, int caret, string insertText = null)
    {
      insertText = insertText ?? source.GetText(caret - 1, 1);
      var text = source.GetText(0, caret);
      ICompletionData[] result = null;
      IEnumerable<XmlElementPath> parentPaths;
      switch (insertText)
      {
        case "=":
          // Namespace intellisense.
          if (XmlParser.IsNamespaceDeclaration(text, text.Length))
          {
            result = schemaCompletionDataItems.GetNamespaceCompletionData();
          }
          break;
        case "<":
          // Child element intellisense.
          parentPaths = XmlParser.GetParentElementPaths(text);
          if (parentPaths.Any())
          {
            foreach (var path in parentPaths)
            {
              result = GetChildElementCompletionData(path);
              if (result.Any()) break;
            }
          }
          else if (defaultSchemaCompletionData != null)
          {
            result = defaultSchemaCompletionData.GetElementCompletionData(defaultNamespacePrefix).ToArray();
          }
          break;

        case " ":
          // Attribute intellisense.
          if (!XmlParser.IsInsideAttributeValue(text, text.Length))
          {
            XmlElementPath path = XmlParser.GetActiveElementStartPath(text, text.Length);
            if (path.Elements.Count > 0)
            {
              result = GetAttributeCompletionData(path);
            }
          }
          break;
        default:

          // Attribute value intellisense.
          if (XmlParser.IsAttributeValueChar(insertText[0]))
          {
            string attributeName = XmlParser.GetAttributeName(text, text.Length);
            if (attributeName.Length > 0)
            {
              XmlElementPath elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
              if (elementPath.Elements.Count > 0)
              {
                //preSelection = insertText.ToString();
                result = GetAttributeValueCompletionData(elementPath, attributeName);
              }
            }
          }
          break;
      }

      if (result != null)
        return result.OrderBy(x => x.Text);

      return Enumerable.Empty<ICompletionData>();
    }

    public override void HandleTextEntered(EditorWinForm control, string insertText)
    {
      switch (insertText)
      {
        case ">":
          var doc = control.Editor.Document;
          var caret = control.Editor.CaretOffset;
          var text = doc.GetText(0, caret);
          var elementName = XmlParser.GetOpenElement(text);
          if (!string.IsNullOrEmpty(elementName))
          {
            var insert = "</" + elementName + ">";
            if (!doc.GetText(caret, doc.TextLength - caret).Trim().StartsWith(insert))
            {
              doc.Insert(caret, insert, AnchorMovementType.BeforeInsertion);
            }
          }
          break;
        default:
          var result = HandleTextEntered(control.Editor.Document, control.Editor.CaretOffset, insertText);
          if (result.Any())
          {
            control.ShowCompletionWindow(result, 0);
          }
          break;
      }
    }

    public override string GetCurrentQuery(ITextSource text, int offset)
    {
      return text.Text;
    }

    public IEnumerable<IEditorScript> GetScripts(ITextSource text, int offset)
    {
      return Enumerable.Empty<IEditorScript>();
    }

    /// <summary>
    /// Finds the schema given the xml element path.
    /// </summary>
    public XmlSchemaCompletionData FindSchema(XmlElementPath path)
    {
      if (path.Elements.Count > 0) {
        string namespaceUri = path.Elements[0].Namespace;
        if (namespaceUri.Length > 0) {
          var result = schemaCompletionDataItems[namespaceUri];
          if (result == null && defaultSchemaCompletionData.NamespaceUri == namespaceUri) result = defaultSchemaCompletionData;
          return result;
        } else if (defaultSchemaCompletionData != null) {

          // Use the default schema namespace if none
          // specified in a xml element path, otherwise
          // we will not find any attribute or element matches
          // later.
          foreach (QualifiedName name in path.Elements) {
            if (name.Namespace.Length == 0) {
              name.Namespace = defaultSchemaCompletionData.NamespaceUri;
            }
          }
          return defaultSchemaCompletionData;
        }
      }
      return null;
    }

    /// <summary>
    /// Finds the schema given a namespace URI.
    /// </summary>
    public XmlSchemaCompletionData FindSchema(string namespaceUri)
    {
      return schemaCompletionDataItems[namespaceUri];
    }

    /// <summary>
    /// Gets the schema completion data that was created from the specified
    /// schema filename.
    /// </summary>
    public XmlSchemaCompletionData FindSchemaFromFileName(string fileName)
    {
      return schemaCompletionDataItems.GetSchemaFromFileName(fileName);
    }

    ICompletionData[] GetChildElementCompletionData(XmlElementPath path)
    {
      ICompletionData[] completionData = null;

      XmlSchemaCompletionData schema = FindSchema(path);
      if (schema != null) {
        completionData = schema.GetChildElementCompletionData(path);
      }

      return completionData;
    }

    ICompletionData[] GetAttributeCompletionData(XmlElementPath path)
    {
      ICompletionData[] completionData = null;

      XmlSchemaCompletionData schema = FindSchema(path);
      if (schema != null) {
        completionData = schema.GetAttributeCompletionData(path);
      }

      return completionData;
    }

    ICompletionData[] GetAttributeValueCompletionData(XmlElementPath path, string name)
    {
      ICompletionData[] completionData = null;

      XmlSchemaCompletionData schema = FindSchema(path);
      if (schema != null) {
        completionData = schema.GetAttributeValueCompletionData(path, name);
      }

      return completionData;
    }
  }
}
