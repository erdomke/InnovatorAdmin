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

    public static IEnumerable<IEditorScript> GetScripts(IAsyncConnection conn, string type, string id)
    {
      if (!string.IsNullOrEmpty(id))
      {
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
        if (conn != null)
        {
          var metadata = ArasMetadataProvider.Cached(conn);
          var itemType = metadata.ItemTypeByName(type);
          foreach (var report in metadata.ServerReports(type).OrderBy(l => l.Label ?? l.Value, StringComparer.CurrentCultureIgnoreCase))
          {
            yield return new EditorScript()
            {
              Name = "Report: " + (report.Label ?? report.Value),
              Action = "ApplyItem",
              Script = @"<Item type='Method' action='Run Report'>
  <report_name>" + report.Value + @"</report_name>
  <AML>
    <Item type='" + itemType.Name + "' typeId='" + itemType.Id + "' id='" + id + @"' />
  </AML>
</Item>",
              AutoRun = true
            };
          }
        }
        yield return new EditorScript()
        {
          Name = "Script: Edit",
          Action = "ApplyItem",
          Script = string.Format("<Item type='{0}' id='{1}' action='edit'></Item>", type, id)
        };
        
      }
    }
  }
}
