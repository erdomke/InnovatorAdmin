using InnovatorAdmin.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace InnovatorAdmin.Editor
{
  internal static class Tooltips
  {
    public static object Property(Property property)
    {
      if (property == null)
        return null;

      return FormatText.Invoke(() =>
      {
        var block = new TextBlock()
        {
          TextWrapping = TextWrapping.Wrap
        };
        block.Inlines.Add(new Run("property ")
        {
          Foreground = Brushes.Gray
        });
        block.Inlines.Add(new Run(property.Name)
        {
          FontWeight = FontWeight.FromOpenTypeWeight(700)
        });

        if (!string.IsNullOrEmpty(property.Label))
          block.Inlines.Add(new Run(" (" + property.Label + ")"));
        block.Inlines.Add(new Run(": "));

        block.Inlines.Add(new Run(property.TypeDisplay())
        {
          Foreground = Brushes.Blue
        });

        if (!string.IsNullOrEmpty(property.Description))
        {
          block.Inlines.Add(new LineBreak());
          block.Inlines.Add(new Run(property.Description));
        }

        return block;
      });
    }

    public static object ItemType(ItemType itemType)
    {
      if (itemType == null)
        return null;

      return FormatText.Invoke(() =>
      {
        var block = new TextBlock()
        {
          TextWrapping = TextWrapping.Wrap
        };
        var typeName = "itemtype";
        if (itemType.IsPolymorphic)
          typeName = "poly item";
        else if (itemType.IsRelationship)
          typeName = "relationship";
        block.Inlines.Add(new Run(typeName + " ")
        {
          Foreground = Brushes.Gray
        });
        block.Inlines.Add(new Run(itemType.Name)
        {
          FontWeight = FontWeight.FromOpenTypeWeight(700)
        });

        if (!string.IsNullOrEmpty(itemType.Label ?? itemType.TabLabel))
          block.Inlines.Add(new Run(" (" + (itemType.Label ?? itemType.TabLabel) + ")"));

        if (itemType.IsRelationship)
        {
          block.Inlines.Add(new Run(": "));
          block.Inlines.Add(new Run((itemType.SourceTypeName ?? "null") + " -> " + (itemType.RelatedTypeName ?? "null"))
          {
            Foreground = Brushes.Blue
          });
        }

        if (!string.IsNullOrEmpty(itemType.Description))
        {
          block.Inlines.Add(new LineBreak());
          block.Inlines.Add(new Run(itemType.Description));
        }

        return block;
      });
    }

    public static object Documentation(OperationElement documentation, string type)
    {
      if (documentation == null)
        return null;

      return FormatText.Invoke(() =>
      {
        var block = new TextBlock()
        {
          TextWrapping = TextWrapping.Wrap
        };
        block.Inlines.Add(new Run(type + " ")
        {
          Foreground = Brushes.Gray
        });
        block.Inlines.Add(new Run(documentation.Name)
        {
          FontWeight = FontWeight.FromOpenTypeWeight(700)
        });

        if (documentation.ValueTypes.Any(t => t.Type != AmlDataType.Unknown))
        {
          block.Inlines.Add(new Run(": "));
          block.Inlines.Add(new Run(string.Join("|", documentation.ValueTypes
            .Where(t => t.Type != AmlDataType.Unknown)
            .Select(v =>
            {
              var typeString = v.Type.ToString();
              typeString = typeString.Substring(0, 1).ToLowerInvariant() + typeString.Substring(1);
              if (!string.IsNullOrEmpty(v.Source))
                typeString += "[" + v.Source + "]";
              else if (v.Values.Any())
                typeString += "[" + string.Join("|", v.Values) + "]";
              return typeString;
            })))
          {
            Foreground = Brushes.Blue
          });
        }

        var visitor = new TooltipElementVisitor();
        var content = new List<DependencyObject>() { block };
        content.AddRange(documentation.Summary
          .Select(e => e.Visit(visitor))
          .ToList());
        return visitor.GetBlock(content);
      });
    }
  }
}
