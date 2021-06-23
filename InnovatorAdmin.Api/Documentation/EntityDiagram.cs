using Innovator.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public class EntityDiagram : IDiagram
  {
    public List<EntityAssociation> Associations { get; } = new List<EntityAssociation>();
    public List<Entity> Entities { get; } = new List<Entity>();

    public static EntityDiagram FromTypes(IEnumerable<ItemType> itemTypes, string packageName)
    {
      var result = new EntityDiagram();
      var allNames = new HashSet<string>();
      itemTypes = itemTypes.Where(i => !i.IsUiOnly);
      var entities = itemTypes.ToDictionary(i => i.Name, i =>
      {
        var entity = new Entity()
        {
          Id = i.Name.Replace(' ', '_'),
          Label = i.Name
        };
        var label = i.Label ?? i.TabLabel;
        if (!string.IsNullOrEmpty(label))
          entity.Label += $"\n({label})";
        entity.Attributes.AddRange(i.Properties.Values
          .Where(p => p.Applicable && !p.Core)
          .OrderBy(p => p.Name)
          .Select(p => new EntityAttribute()
          {
            Name = p.Name,
            DataType = p.TypeDisplay()
          }));

        entity.Package = packageName;
        if (i.IsPolymorphic)
        {
          entity.Stereotype = "polymorphic";
          entity.Type = EntityType.Abstract;
        }
        else if (i.IsFederated)
        {
          entity.Stereotype = "federated";
        }
        else if (i.IsRelationship)
        {
          entity.Stereotype = "relationship";
        }

        if (i.ClassPaths.Any())
        {
          var builder = new StringBuilder("Classes\r\n");
          BuildClassificationNote(i.ClassStructure, 0, builder);
          entity.Note = builder.ToString().Trim();
        }

        allNames.Add(i.SourceTypeName ?? "");
        allNames.Add(i.RelatedTypeName ?? "");
        allNames.UnionWith(i.Morphae);

        return entity;
      });
      foreach (var name in allNames
        .Where(n => !string.IsNullOrEmpty(n) && !entities.ContainsKey(n)))
      {
        entities[name] = new Entity() { Id = name.Replace(' ', '_'), Label = name };
      }
      result.Entities.AddRange(entities.Values);

      foreach (var itemType in itemTypes)
      {
        var source = entities[itemType.Name];
        foreach (var prop in itemType.Properties.Values
          .Where(p => p.Type == PropertyType.item
            && p.Restrictions.Count > 0
            && (p.Name == "related_id" || p.Name == "source_id")))
        {
          if (entities.TryGetValue(prop.Restrictions[0], out var target))
          {
            var relEnd = new AssociationEnd()
            {
              Entity = source,
              Multiplicity = int.MaxValue,
              Type = AssociationType.Composition
            };
            var nonRelEnd = new AssociationEnd()
            {
              Entity = target,
              Multiplicity = 1,
            };
            result.Associations.Add(prop.Name == "source_id"
              ? new EntityAssociation()
              {
                Source = nonRelEnd,
                Related = relEnd,
                Label = prop.Name
              }
              : new EntityAssociation()
              {
                Source = relEnd,
                Related = nonRelEnd,
                Label = prop.Name
              });
          }
        }

        foreach (var morphae in itemType.Morphae)
        {
          if (entities.TryGetValue(morphae, out var target))
          {
            result.Associations.Add(new EntityAssociation()
            {
              Source = new AssociationEnd()
              {
                Entity = source,
                Type = AssociationType.Inheritance
              },
              Related = new AssociationEnd()
              {
                Entity = target,
              },
              Label = "Implements"
            });
          }
        }
      }

      return result;
    }

    private static void BuildClassificationNote(ClassNode node, int indent, StringBuilder builder)
    {
      foreach (var child in node.Children.OrderBy(n => n.Name, StringComparer.OrdinalIgnoreCase))
      {
        builder.Append(' ', indent);
        builder.Append('└');
        builder.AppendLine(child.Name);
        BuildClassificationNote(child, indent + 1, builder);
      }
    }

    public Task WriteAsync<T>(IDiagramWriter<T> writer, T target)
    {
      return writer.WriteAsync(this, target);
    }
  }
}
