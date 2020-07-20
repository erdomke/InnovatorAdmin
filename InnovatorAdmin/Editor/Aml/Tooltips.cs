using System;
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
        {
          if (builder[builder.Length - 1] != '[')
            builder.Append(", ");
          builder.Append("not null");
        }
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
        else if (_propertyHelp.TryGetValue(property.Name, out var descrip))
        {
          block.Inlines.Add(new LineBreak());
          block.Inlines.Add(new Run(descrip));
        }

        return block;
      });
    }

    private static Dictionary<string, string> _propertyHelp = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
      { "classification", "Describes the type of the item with a tree-like structure (Inherited from Item)" },
      { "config_id", "Server assigned. Defines a common ID linking all the generations of an item (Inherited from Item)" },
      { "created_by_id", "Server assigned. ID of the user that created the item (Inherited from Item)" },
      { "created_on", "Server assigned. Date the item was created (Inherited from Item)" },
      { "css", "Defines styling within the web client (Inherited from Item)" },
      { "current_state", "Server assigned. ID of the current life cycle state (Inherited from Item)" },
      { "generation", "Server assigned. Sequential number identifying the version/snapshot of the item (Inherited from Item)" },
      { "id", "Server assigned. A 32 character globally unique identifier (GUID) for the item (Inherited from Item)" },
      { "is_current", "Server assigned. Boolean returning true if this is the most current generation (Inherited from Item)" },
      { "is_released", "Boolean returning true if this item is in a released life cycle state (Inherited from Item)" },
      { "keyed_name", "Server assigned. Human friendly identifier for the item (Inherited from Item)" },
      { "locked_by_id", "ID of the user that has locked the item (Inherited from Item)" },
      { "major_rev", "Revision label of the versioned item (Inherited from Item)" },
      { "managed_by_id", "ID of the identity considered the 'Manager' for permissions and workflow assignments (Inherited from Item)" },
      { "minor_rev", "Not used (Inherited from Item)" },
      { "modified_by_id", "Server assigned. ID of the user that last modified the item (Inherited from Item)" },
      { "modified_on", "Server assigned. Date the item was last modified (Inherited from Item)" },
      { "new_version", "Boolean returning true if a new version of this automatically versioned item has been created for the current lock-save-unlock cycle (Inherited from Item)" },
      { "not_lockable", "Boolean returning true if this item cannot be changed regardless of permissions (Inherited from Item)" },
      { "owned_by_id", "ID of the identity considered the 'Owner' for permissions and workflow assignments (Inherited from Item)" },
      { "permission_id", "ID of the current permission item (Inherited from Item)" },
      { "state", "Server assigned. Name of the current life cycle state (Inherited from Item)" },
      { "team_id", "ID of the current team (Inherited from Item)" },
      { "effective_date", "Date that this version is considered effective and can first be used (Inherited from Versioned Item)" },
      { "release_date", "Server assigned. Date that this version was first promoted to a released life cycle state (Inherited from Versioned Item)" },
      { "superseded_date", "Date that this version was superseded by the release of a more recent version (Inherited from Versioned Item)" },
      { "behavior", "Server assigned. Describes whether the related item will always point to the specified version or the most current version (Inherited from Relationship)" },
      { "source_id", "ID of the parent (source) item for the relationship (Inherited from Relationship)" },
      { "related_id", "ID of the child (related) item for the relationship (Inherited from Relationship)" },
      { "sort_order", "Number describing the order items should be sorted (Inherited from Relationship)" },
      { "indexed_on", "Server assigned. Date the item was last indexed by the Enterprise Search indexer (Inherited from Indexed Item)" },
      { "itemtype", "ID of the item type this item belongs to (Inherited from Poly Item)" }
    };

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

    public static object Documentation(AmlDocumentation documentation, string type)
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

        if (!string.IsNullOrEmpty(documentation.Summary))
        {
          block.Inlines.Add(new LineBreak());
          block.Inlines.Add(new Run(documentation.Summary));
        }

        return block;
      });
    }
  }
}
