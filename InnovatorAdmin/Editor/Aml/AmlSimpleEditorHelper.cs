using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin.Editor
{
  public class AmlSimpleEditorHelper : XmlEditorHelper
  {
    protected IAsyncConnection _conn;

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
        return GetScripts(_conn, item.Type, item.Id);
      }
      return Enumerable.Empty<IEditorScript>();
    }

    public override IEnumerable<IEditorScript> GetScripts(IEnumerable<System.Data.DataRow> rows)
    {
      if (rows.Count() == 1)
      {
        var row = rows.First();
        if (row.Table.Columns.Contains(Extensions.AmlTable_TypeName)
          && row.Table.Columns.Contains("id")
          && !row.IsNull(Extensions.AmlTable_TypeName)
          && !row.IsNull("id"))
        {
          string relatedId = null;
          if (row.Table.Columns.Contains("related_id") && !row.IsNull("related_id"))
            relatedId = (string)row["related_id"];

          return GetScripts(_conn, (string)row[Extensions.AmlTable_TypeName], (string)row["id"], relatedId);
        }
      }

      return Enumerable.Empty<IEditorScript>();
    }

    private ItemData GetCurrentItem(ITextSource text, int offset)
    {
      var result = new Stack<ItemData>();

      XmlUtils.ProcessFragment(text, (r, o, st) =>
      {
        if (o > offset)
          return false;

        switch (r.NodeType)
        {
          case XmlNodeType.Element:

            if (r.LocalName == "Item")
            {
              result.Push(new ItemData()
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
            break;
          case XmlNodeType.EndElement:
            if (r.LocalName == "Item")
            {
              if (offset < (o + 6))
                return false;
              result.Pop();
            }
            break;
        }
        return true;
      });

      if (result.Any())
        return result.Pop();
      return null;
    }

    private class ItemData
    {
      public string Type { get; set; }
      public string Id { get; set; }
      public string Action { get; set; }
    }

    public static IEnumerable<IEditorScript> GetScripts(IAsyncConnection conn, string type, string id, string relatedId = null)
    {
      if (!string.IsNullOrEmpty(id))
      {
        ArasMetadataProvider metadata = null;
        ItemType itemType = null;
        if (conn != null)
        {
          metadata = ArasMetadataProvider.Cached(conn);
          itemType = metadata.ItemTypeByName(type);
        }

        if (metadata != null)
        {
          yield return new EditorScript()
          {
            Name = "View \"" + (itemType.Label ?? itemType.Name) + "\"",
            Action = "ApplyItem",
            Script = string.Format("<Item type='{0}' id='{1}' action='get' levels='1'></Item>", type, id),
            AutoRun = true,
            PreferredOutput = OutputType.Table
          };
          if (!string.IsNullOrEmpty(relatedId) && itemType.Related != null)
          {
            yield return new EditorScript()
            {
              Name = "View \"" + (itemType.Related.Label ?? itemType.Related.Name) + "\"",
              Action = "ApplyItem",
              Script = string.Format("<Item type='{0}' id='{1}' action='get' levels='1'></Item>", itemType.Related.Name, relatedId),
              AutoRun = true,
              PreferredOutput = OutputType.Table
            };
          }
        }
        yield return new EditorScript()
        {
          Name = "------"
        };
        if (conn != null)
        {
          yield return ArasEditorProxy.ItemTypeAddScript(conn, itemType);
        }
        yield return new EditorScript()
        {
          Name = "Edit",
          Action = "ApplyItem",
          Script = string.Format("<Item type='{0}' id='{1}' action='edit'></Item>", type, id)
        };
        yield return new EditorScript()
        {
          Name = "Delete",
          Action = "ApplyItem",
          Script = string.Format("<Item type='{0}' id='{1}' action='delete'></Item>", type, id)
        };
        yield return new EditorScript()
        {
          Name = "------"
        };
        if (metadata != null)
        {
          var actions = new EditorScript()
          {
            Name = "Actions"
          };

          var serverActions = metadata.ServerItemActions(type)
            .OrderBy(l => l.Label ?? l.Value, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
          foreach (var action in serverActions)
          {
            actions.Add(new EditorScript()
            {
              Name = (action.Label ?? action.Value),
              Action = "ApplyItem",
              Script = string.Format("<Item type='{0}' id='{1}' action='{2}'></Item>", type, id, action.Value),
              AutoRun = true
            });
          }

          if (serverActions.Any())
            yield return actions;

          var reports = new EditorScript()
          {
            Name = "Reports"
          };

          var serverReports = metadata.ServerReports(type)
            .OrderBy(l => l.Label ?? l.Value, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
          foreach (var report in serverReports)
          {
            reports.Add(new EditorScript()
            {
              Name = (report.Label ?? report.Value),
              Action = "ApplyItem",
              Script = @"<Item type='Method' action='Run Report'>
  <report_name>" + report.Value + @"</report_name>
  <AML>
    <Item type='" + itemType.Name + "' typeId='" + itemType.Id + "' id='" + id + @"' />
  </AML>
</Item>",
              AutoRun = true
            });
          }

          if (serverReports.Any())
            yield return reports;
        }
        yield return new EditorScriptExecute()
        {
          Name = "Copy ID",
          Execute = () =>
          {
            if (string.IsNullOrEmpty(id))
            {
              System.Windows.Clipboard.Clear();
            }
            else
            {
              System.Windows.Clipboard.SetText(id);
            }
          }
        };
      }
    }
  }
}
