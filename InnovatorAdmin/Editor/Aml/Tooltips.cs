using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
          FontWeight = System.Windows.FontWeight.FromOpenTypeWeight(700)
        });

        if (!string.IsNullOrEmpty(property.Label))
          block.Inlines.Add(new Run(" (" + property.Label + ")"));
        block.Inlines.Add(new Run(": "));

        var builder = new StringBuilder(property.TypeName)
          .Append('[');
        if (property.Restrictions.Any())
        {
          builder.Append(property.Restrictions.First());
        }
        else if (property.StoredLength > 0)
        {
          builder.Append(property.StoredLength);
        }
        else if (property.Precision > 0 || property.Scale > 0)
        {
          builder.Append(property.Precision).Append(',').Append(property.Scale);
        }
        else if (!string.IsNullOrEmpty(property.ForeignLinkPropName))
        {
          builder.Append(property.ForeignLinkPropName).Append('.').Append(property.ForeignPropName);
        }

        if (property.IsRequired)
          builder.Append(", not null");
        builder.Append(']');

        block.Inlines.Add(new Run(builder.ToString())
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
          FontWeight = System.Windows.FontWeight.FromOpenTypeWeight(700)
        });

        if (!string.IsNullOrEmpty(itemType.Label ?? itemType.TabLabel))
          block.Inlines.Add(new Run(" (" + (itemType.Label ?? itemType.TabLabel) + ")"));

        if (itemType.IsRelationship)
        {
          block.Inlines.Add(new Run(": "));
          block.Inlines.Add(new Run((itemType.Source?.Name ?? "null") + " -> " + (itemType.Related?.Name ?? "null"))
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
  }
}
