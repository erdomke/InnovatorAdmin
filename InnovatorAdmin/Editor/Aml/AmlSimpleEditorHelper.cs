using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin.Editor
{
  public class AmlSimpleEditorHelper : XmlEditorHelper
  {
    protected IAsyncConnection _conn;
    protected InnovatorAdmin.Connections.ConnectionData _connData;

    public AmlSimpleEditorHelper() : base()
    {
      _foldingStrategy = new AmlFoldingStrategy() { ShowAttributesWhenFolded = true };
    }

    internal static IHighlightingDefinition _highlighter;

    static AmlSimpleEditorHelper()
    {
      using (var stream = System.Reflection.Assembly.GetExecutingAssembly()
        .GetManifestResourceStream("InnovatorAdmin.resources.Aml.xshd"))
      {
        using (var reader = new System.Xml.XmlTextReader(stream))
        {
          _highlighter =
              ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
              ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
        }
      }
    }

    public override IHighlightingDefinition GetHighlighting()
    {
      return _highlighter;
    }

    public override IEnumerable<IEditorScript> GetScripts(ICSharpCode.AvalonEdit.Document.ITextSource text, int offset)
    {
      var item = GetCurrentItem(text, offset);
      if (item != null)
      {
        var generator = new ScriptMenuGenerator()
        {
          Conn = _conn,
          ConnData = _connData,
          Items = new[] { item }
        };
        return generator.GetScripts().Concat(Enumerable.Repeat(new EditorScriptExecute() {
          Name = "Transform: Criteria to Where Clause",
          Execute = () =>
          {
            var doc = text as IDocument;
            if (doc != null)
            {
              var segment = GetCurrentQuerySegment(text, offset);
              var elem = XElement.Load(text.CreateReader(segment.Offset, segment.Length));
              AmlTransforms.CriteriaToWhereClause(elem);
              doc.Replace(segment.Offset, segment.Length, elem.ToString());
            }
          }
        }, 1));
      }
      return Enumerable.Empty<IEditorScript>();
    }


    public override IEnumerable<IEditorScript> GetScripts(IEnumerable<DataRow> rows, string column)
    {
      var generator = new ScriptMenuGenerator()
      {
        Column = column,
        Conn = _conn,
        ConnData = _connData,
        Items = rows.Select(r => new DataRowItemData(r))
      };
      return generator.GetScripts();
    }

    private EditorItemData GetCurrentItem(ITextSource text, int offset)
    {
      var result = new Stack<EditorItemData>();
      string lastElement = null;

      XmlUtils.ProcessFragment(text, (r, o, st) =>
      {
        if (o > offset)
          return false;

        switch (r.NodeType)
        {
          case XmlNodeType.Element:

            if (r.LocalName == "Item")
            {
              result.Push(new EditorItemData()
              {
                Action = r.GetAttribute("action"),
                Type = r.GetAttribute("type"),
                Id = r.GetAttribute("id")
              });

              if (r.IsEmptyElement)
              {
                var end = text.IndexOf("/>", o, text.TextLength - o, StringComparison.Ordinal) + 2;
                if (offset >= o && offset < end)
                {
                  return false;
                }
                else
                {
                  result.Pop();
                }
              }
            }
            else if (!r.IsEmptyElement)
            {
              lastElement = r.LocalName;
            }
            break;
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
            if (result.Any() && !string.IsNullOrEmpty(lastElement))
            {
              result.Peek().Property(lastElement, r.Value);
            }
            break;
          case XmlNodeType.EndElement:
            if (r.LocalName == "Item")
            {
              if (offset < (o + 6))
                return false;
              result.Pop();
            }
            else
            {
              lastElement = null;
            }
            break;
        }
        return true;
      });

      if (result.Any())
        return result.Pop();
      return null;
    }


    private class EditorItemData : IItemData
    {
      private Dictionary<string, string> _propertyData =
        new Dictionary<string, string>();

      public string Type { get; set; }
      public string Id { get; set; }
      public string Action { get; set; }

      public object Property(string name)
      {
        string result;
        if (_propertyData.TryGetValue(name, out result))
          return result;
        return null;
      }
      internal void Property(string name, string value)
      {
        _propertyData[name] = value;
      }
    }
  }
}
