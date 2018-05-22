using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Innovator.Client;
using Innovator.Client.Model;
using Innovator.Client.QueryModel;
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

    public override IEnumerable<IEditorScript> GetScripts(ITextSource text, int offset, bool readOnly)
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

        var extras = new List<IEditorScript>();
        if (!readOnly)
        {
          extras.Add(new EditorScriptExecute()
          {
            Name = "Transform: Criteria to Where Clause",
            Execute = () =>
            {
              var doc = text as IDocument;
              if (doc != null)
              {
                try
                {
                  var segment = GetCurrentQuerySegment(text, offset);
                  var queryItem = _conn.AmlContext.FromXml(text.CreateReader(segment.Offset, segment.Length)).AssertItem();
                  queryItem.QueryType().Set("ignore");
                  queryItem.Where().Remove();
                  var settings = new SqlSettings(_conn)
                  {
                    RenderOption = AmlSqlRenderOption.WhereClause
                  };

                  var elem = XElement.Load(text.CreateReader(segment.Offset, segment.Length));
                  var whereClause = elem.Attribute("where")?.Value ?? "";
                  if (whereClause != "")
                    whereClause += " and ";
                  whereClause += queryItem.ToQueryItem().ToSql(settings);
                  elem.SetAttributeValue("where", whereClause);

                  foreach (var child in elem.Elements().ToArray())
                  {
                    child.Remove();
                  }

                  doc.Replace(segment.Offset, segment.Length, elem.ToString());
                }
                catch (Exception ex)
                {
                  // Do nothing
                }
              }
              return Task.FromResult(true);
            }
          });
        }

        return generator.GetScripts().Concat(extras);
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
              if (offset < (o + 6) && result.Count == 1)
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

    private class SqlSettings : IAmlSqlWriterSettings
    {
      private IAsyncConnection _conn;

      public SqlSettings(IAsyncConnection conn)
      {
        _conn = conn;
      }

      public string IdentityList => string.Empty;

      public AmlSqlPermissionOption PermissionOption { get; set; }

      public AmlSqlRenderOption RenderOption { get; set; }

      public string UserId => _conn.UserId;

      public IDictionary<string, IPropertyDefinition> GetProperties(string itemType)
      {
        var meta = ArasMetadataProvider.Cached(_conn);
        var type = meta.ItemTypeByName(itemType);
        return meta.GetProperties(type).Wait()
          .Select(p => p.ToItem(_conn.AmlContext))
          .ToDictionary(p => p.NameProp().Value);
      }
    }
  }
}
