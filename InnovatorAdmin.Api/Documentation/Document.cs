using Innovator.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace InnovatorAdmin.Documentation
{
  public class Document
  {
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public List<IElement> Content { get; } = new List<IElement>();

    public static Document FromItemType(ItemType itemType, DocumentOptions options, IArasMetadataProvider metadata = null)
    {
      var document = new Document()
      {
        Title = itemType.Name,
        SubTitle = "ItemType"
      };
      if (itemType.IsPolymorphic)
        document.SubTitle = "Poly ItemType";
      else if (itemType.IsFederated)
        document.SubTitle = "Federated ItemType";
      else if (itemType.IsRelationship)
        document.SubTitle = "RelationshipType";
      var label = itemType.Label ?? itemType.TabLabel;
      if (!string.IsNullOrEmpty(label) && !string.Equals(label, itemType.Name, StringComparison.OrdinalIgnoreCase))
        document.Title += " (" + label + ")";

      var typeDescripPara = new Paragraph();
      if (!string.IsNullOrEmpty(itemType.Description))
        typeDescripPara.Children.Add(new TextRun(itemType.Description));
      if (itemType.IsVersionable)
        typeDescripPara.Children.Add(new TextRun((typeDescripPara.Children.Count > 0 ? " " : "") + "Versionable.", RunStyle.Italic));

      if (typeDescripPara.Children.Count > 0)
        document.Content.Add(typeDescripPara);

      var inheritsFrom = new List<ItemType>();
      if (metadata != null)
      {
        inheritsFrom = metadata.ItemTypes
          .Where(i => i.Morphae.Contains(itemType.Name))
          .ToList();
      }

      if (inheritsFrom.Count > 0)
      {
        if (options.IncludeCrossReferenceLinks)
        {
          var paragraph = new Paragraph(new TextRun("Inherits: "));
          document.Content.Add(paragraph);
          for (var i = 0; i < inheritsFrom.Count; i++)
          {
            if (i > 0)
              paragraph.Children.Add(new TextRun(", "));
            paragraph.Children.Add(new DocLink() { Type = "ItemType", Name = inheritsFrom[i].Name, Id = inheritsFrom[i].Id });
          }
        }
        else
        {
          document.Content.Add(new Paragraph(new TextRun("Inherits: " + string.Join(", ", inheritsFrom.Select(i => i.Name)))));
        }
      }

      if (itemType.Morphae.Count > 0)
      {
        if (options.IncludeCrossReferenceLinks)
        {
          var paragraph = new Paragraph(new TextRun("Derived: "));
          document.Content.Add(paragraph);
          var derived = itemType.Morphae.ToList();
          for (var i = 0; i < derived.Count; i++)
          {
            if (i > 0)
              paragraph.Children.Add(new TextRun(", "));
            paragraph.Children.Add(new DocLink() { Type = "ItemType", Name = derived[i] });
          }
        }
        else
        {
          document.Content.Add(new Paragraph(new TextRun("Derived: " + string.Join(", ", itemType.Morphae))));
        }
      }

      if (itemType.ClassPaths.Any())
        document.Content.Add(BuildClassList(itemType.ClassStructure));

      var examples = new Section("Examples");
      document.Content.Add(examples);

      var aml = ElementFactory.Local;
      var amlGet = aml.Item(aml.Type(itemType.Name), aml.Action("get"));
      var orderBy = string.Join(",", itemType.Properties.Values
        .Where(p => p.OrderBy.HasValue)
        .OrderBy(p => p.OrderBy.Value)
        .Select(p => p.Name));
      if (!string.IsNullOrEmpty(orderBy))
        amlGet.Add(aml.Attribute("orderBy", orderBy));
      if (itemType.DefaultPageSize.HasValue)
        amlGet.Add(aml.Attribute("page", 1), aml.Attribute("pagesize", itemType.DefaultPageSize));
      if (itemType.MaxRecords.HasValue)
        amlGet.Add(aml.Attribute("maxRecords", itemType.MaxRecords));
      foreach (var prop in itemType.Properties.Values.Where(p => !string.IsNullOrEmpty(p.DefaultSearch)))
        amlGet.Add(aml.Property(prop.Name, prop.DefaultSearch));
      examples.Children.Add(new CodeBlock()
      {
        Code = ToAmlString(amlGet),
        Language = "xml"
      });

      if (!itemType.IsPolymorphic)
      {
        var amlAdd = aml.Item(aml.Type(itemType.Name), aml.Action("add"));
        foreach (var prop in itemType.Properties.Values.Where(p => IncludePropertyInAdd(itemType, p)))
          amlAdd.Add(aml.Property(prop.Name, prop.DefaultValue ?? ""));
        examples.Children.Add(new CodeBlock()
        {
          Code = ToAmlString(amlAdd),
          Language = "xml"
        });
      }

      document.Content.Add(new Section("Properties", itemType.Properties.Values
        .Where(p => IncludePropertyDocumentation(itemType, p, options))
        .GroupBy(p =>
        {
          if (!string.IsNullOrEmpty(p.ClassPath))
            return "1:Defined for " + p.ClassPath;
          else if (p.Name.StartsWith("xp-"))
            return "2:Extended";
          else if (p.Core)
            return "3:Core";
          else
            return "0:Defined for All";
        })
        .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
        .Select(g =>
        {
          var section = new Section(g.Key.Substring(2));
          var table = new List() { Type = ListType.Table };
          table.Header = new ListItem("Property", "Description");
          foreach (var property in g.OrderBy(p => p.Name))
          {
            var row = new ListItem();
            if (options.IncludeCrossReferenceLinks)
              row.Term.Add(new DocLink()
              {
                Id = property.Id,
                Name = property.Name,
                Type = "Property"
              });
            else
              row.Term.Add(new TextRun(property.Name, RunStyle.Bold));

            if (!string.IsNullOrEmpty(property.Label))
              row.Term.Add(new TextRun($" ({property.Label})"));
            row.Term.Add(new TextRun(": "));

            if (options.IncludeCrossReferenceLinks && property.Restrictions.Any()
              && (property.Type == PropertyType.item || property.Type == PropertyType.list))
            {
              row.Term.Add(new TextRun(property.TypeName + "[", RunStyle.Code));
              var links = property.Restrictions.Select(r => new DocLink()
              {
                Name = r,
                Type = property.Type == PropertyType.list ? "List" : "ItemType"
              }).ToList();
              row.Term.Add(links.First());
              foreach (var link in links.Skip(1))
              {
                row.Term.Add(new TextRun(", "));
                row.Term.Add(link);
              }
              row.Term.Add(new TextRun("]", RunStyle.Code));
            }
            else
            {
              row.Term.Add(new TextRun(property.TypeDisplay(), RunStyle.Code));
            }

            if (property.Name == "keyed_name" && ShowKeyedNameCalc(itemType))
            {
              var code = "= " + string.Join(" + ' ' + ", itemType.Properties.Values.Where(p => p.KeyedNameOrder.HasValue).Select(p => p.Name));
              row.Description.Add(new TextRun(code, RunStyle.Code));
            }

            if (property.ReadOnly)
              row.Description.Add(new TextRun((row.Description.Count > 0 ? " " : "") + "Read only.", RunStyle.Italic));
            if (!string.IsNullOrEmpty(property.Description))
              row.Description.Add(new TextRun((row.Description.Count > 0 ? " " : "") + property.Description));
            if (!property.Core && inheritsFrom.Any(i => i.Properties.ContainsKey(property.Name)))
              row.Description.Add(new TextRun($"{(row.Description.Count > 0 ? " " : "")}(Inherited from {string.Join(", ", inheritsFrom.Where(i => i.Properties.ContainsKey(property.Name)).Select(i => i.Name))})"));
            if (property.Type != PropertyType.date && !string.IsNullOrEmpty(property.Pattern))
              row.Description.Add(new TextRun($"{(row.Description.Count > 0 ? " " : "")}Pattern: {property.Pattern}", RunStyle.Italic));
            if (property.DefaultValue != null)
              row.Description.Add(new TextRun($"{(row.Description.Count > 0 ? " " : "")}Default: {property.DefaultValue}", RunStyle.Italic));
            else if (property.Type == PropertyType.boolean && !property.Core)
              row.Description.Add(new TextRun((row.Description.Count > 0 ? " " : "") + "Default: 0", RunStyle.Italic));

            table.Children.Add(row);
          }
          section.Children.Add(table);
          return section;
        })));

      if (itemType.Relationships.Any(i => !i.IsUiOnly))
      {
        var relList = new List()
        {
          Type = ListType.Bullet
        };
        relList.Children.AddRange(itemType.Relationships.Where(i => !i.IsUiOnly).Select(r => ItemTypeListItem(r, options)));
        document.Content.Add(new Section("Relationships", new[] { relList }));
      }

      var methodTable = new List()
      {
        Type = ListType.Bullet
      };
      methodTable.Children.AddRange(inheritsFrom
        .SelectMany(i => i.ServerEvents)
        .Concat(itemType.ServerEvents)
        .Where(e => e.Method.KeyedName?.StartsWith("polymorphic_item_") != true)
        .GroupBy(e =>
        {
          if (string.IsNullOrEmpty(e.Event))
            return "";
          if (e.Event.StartsWith("onBefore"))
            return e.Event.Substring(8);
          if (e.Event.StartsWith("onAfter"))
            return e.Event.Substring(7);
          if (e.Event.StartsWith("on"))
            return e.Event.Substring(2);
          return e.Event;
        })
        .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
        .Select(g =>
        {
          var result = new ListItem();
          result.Description.Add(new TextRun(g.Key, RunStyle.Bold));

          var list = new List()
          {
            Type = ListType.Number
          };
          list.Children.AddRange(g.OrderBy(e =>
            {
              if (e.Event?.StartsWith("onBefore") == true)
                return 0;
              if (e.Event?.StartsWith("onAfter") == true)
                return 2;
              return 1;
            })
            .ThenBy(e => e.SortOrder)
            .Select(e =>
            {
              var eventItem = new ListItem();
              if (options.IncludeCrossReferenceLinks)
              {
                eventItem.Description.Add(new DocLink()
                {
                  Name = e.Method.KeyedName,
                  Type = "Method"
                });
              }
              else
              {
                eventItem.Description.Add(new TextRun(e.Method.KeyedName, RunStyle.Italic));
              }
              eventItem.Description.Add(new TextRun($" ({e.Event})"));
              var method = metadata?.Methods.FirstOrDefault(m => m.KeyedName == e.Method.KeyedName);

              var separator = ": ";
              if (method?.Documentation.Summary.Count > 0)
              {
                eventItem.Description.Add(new TextRun(separator));
                eventItem.Description.AddRange(method.Documentation.Summary);
                separator = ". ";
              }

              var inherit = inheritsFrom.FirstOrDefault(i => i.ServerEvents.Contains(e));
              if (inherit != null)
                eventItem.Description.Add(new TextRun($"{separator}(Inherited from {inherit.Name})"));
              return eventItem;
            }));
          result.Description.Add(list);
          return result;
        }));
      if (methodTable.Children.Count > 0)
        document.Content.Add(new Section("Methods", new[] { methodTable }));
      
      return document;
    }

    private static bool ShowKeyedNameCalc(ItemType itemType)
    {
      return itemType.Properties.Values.Any(p => p.KeyedNameOrder.HasValue)
        && !itemType.ServerEvents.Any(e => e.Event == "GetKeyedName");
    }

    private static HashSet<string> _alwaysRequiredProperties = new HashSet<string>() { "created_by_id", "created_on", "config_id", "id", "permission_id" };

    private static ListItem ItemTypeListItem(ItemType itemType, DocumentOptions options)
    {
      var result = new ListItem();
      if (options.IncludeCrossReferenceLinks)
        result.Description.Add(new DocLink()
        {
          Id = itemType.Id,
          Name = itemType.Name,
          Type = "ItemType"
        });
      else
        result.Description.Add(new TextRun(itemType.Name, RunStyle.Bold));

      var relLabel = itemType.TabLabel ?? itemType.Label;
      if (!string.IsNullOrEmpty(relLabel))
        result.Description.Add(new TextRun($" ({relLabel})"));

      var separator = ": ";
      if (!string.IsNullOrEmpty(itemType.Description))
      {
        result.Description.Add(new TextRun(separator + itemType.Description));
        separator = ". ";
      }

      if (!string.IsNullOrEmpty(itemType.RelatedTypeName))
      {
        result.Description.Add(new TextRun(separator + "Relationship to "));
        if (options.IncludeCrossReferenceLinks)
          result.Description.Add(new DocLink()
          {
            Name = itemType.RelatedTypeName,
            Type = "ItemType"
          });
        else
          result.Description.Add(new TextRun(itemType.RelatedTypeName, RunStyle.Bold));
      }

      if (itemType.Relationships.Any(i => !i.IsUiOnly))
      {
        var relList = new List()
        {
          Type = ListType.Bullet
        };
        relList.Children.AddRange(itemType.Relationships.Where(i => !i.IsUiOnly).Select(r => ItemTypeListItem(r, options)));
        result.Description.Add(relList);
      }

      return result;
    }

    private static bool IncludePropertyInAdd(ItemType itemType, Property property)
    {
      if (property.DefaultValue != null && !(property.DefaultValue is string str && str == ""))
        return true;

      if (property.Name == "source_id" && itemType.IsRelationship)
        return true;

      if (property.IsRequired && !_alwaysRequiredProperties.Contains(property.Name) && property.TypeName != "sequence")
        return true;

      return false;
    }

    private static bool IncludePropertyDocumentation(ItemType itemType, Property property, DocumentOptions options)
    {
      if (!property.Applicable)
        return false;
      if (!property.Core || options.IncludeCoreProperties)
        return true;

      switch (property.Name)
      {
        case "source_id":
        case "related_id":
          return property.Restrictions.Count > 0 && itemType.IsRelationship;
        case "keyed_name":
          if (ShowKeyedNameCalc(itemType))
            return true;
          break;
      }

      if (!string.IsNullOrEmpty(property.Label))
      {
        switch (property.Name)
        {
          case "classification":
            return property.Label != "Classification";
          case "is_released":
            return property.Label != "Released";
          case "not_lockable":
            return property.Label != "Not Lockable";
          case "team_id":
            return property.Label != "Team";
          default:
            return true;
        }
      }

      if (property.IsRequired && !_alwaysRequiredProperties.Contains(property.Name))
        return true;
      if (property.DefaultValue != null && !(property.DefaultValue is string str && str == ""))
        return true;
      if (property.Type != PropertyType.date && !string.IsNullOrEmpty(property.Pattern))
        return true;

      return false;
    }

    private static string ToAmlString(IAmlNode node)
    {
      using (var writer = new StringWriter())
      using (var xml = XmlWriter.Create(writer, new XmlWriterSettings()
      {
        OmitXmlDeclaration = true,
        Indent = true,
        IndentChars = "  "
      }))
      {
        node.ToAml(xml);
        xml.Flush();
        return writer.ToString();
      }
    }

    private static List BuildClassList(ClassNode classNode)
    {
      var result = new List() { Type = ListType.Bullet };
      foreach (var child in classNode.Children.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase))
      {
        var item = new ListItem(child.Name);
        if (child.Children.Any())
          item.Description.Add(BuildClassList(child));
        result.Children.Add(item);
      }
      return result;
    }

    public static Document FromMethod(Method method, DocumentOptions options)
    {
      var document = new Document()
      {
        Title = method.KeyedName,
        SubTitle = "Method"
      };

      if (method.Documentation != null)
      {
        document.Content.AddRange(method.Documentation.Summary ?? Enumerable.Empty<IElement>());
        document.Content.Add(new Paragraph(new TextRun("POST Server/InnovatorServer.aspx", RunStyle.Code)));
        var aml = ElementFactory.Local;
        var sampleCall = aml.Item(aml.Action(method.KeyedName));
        BuildMethodCall(aml, sampleCall, method.Documentation);
        if (!sampleCall.Type().HasValue())
          sampleCall.Attribute("type").Set("Method");
        document.Content.Add(new CodeBlock()
        {
          Code = ToAmlString(sampleCall),
          Language = "xml"
        });
        document.Content.Add(new Paragraph(new TextRun("POST /odata/method." + method.KeyedName, RunStyle.Code)));

        var parameters = method.Documentation.DescendantDoc()
          .Where(e => !string.IsNullOrEmpty(e.OriginalXPath))
          .ToList();
        if (parameters.Count > 0)
        {
          var paramSection = new Section("Parameters");
          foreach (var parameter in parameters)
          {
            paramSection.Children.Add(new Paragraph(new TextRun(parameter.OriginalXPath, RunStyle.Code)));
            paramSection.Children.AddRange(parameter.Summary);
          }
          document.Content.Add(paramSection);
        }

        document.Content.AddRange(method.Documentation.Documentation);
      }

      return document;
    }

    private static void BuildMethodCall(ElementFactory aml, Innovator.Client.IElement element, OperationElement operation)
    {
      foreach (var attribute in operation.Attributes)
        element.Add(aml.Attribute(attribute.Name, TryGetConstant(attribute, out var constValue) ? constValue : ""));

      foreach (var child in operation.Elements)
      {
        var newElement = default(Innovator.Client.IElement);
        switch (child.Name)
        {
          case "Relationships":
            newElement = aml.Relationships();
            break;
          case "Item":
            newElement = aml.Item();
            break;
          default:
            newElement = aml.Property(child.Name);
            if (TryGetConstant(child, out var constValue))
              newElement.Add(constValue);
            break;
        }
        BuildMethodCall(aml, newElement, child);
        element.Add(newElement);
      }
    }

    private static bool TryGetConstant(OperationElement operation, out string value)
    {
      value = null;
      if (!operation.ValueTypes.Any() || operation.ValueTypes.Skip(1).Any())
        return false;
      if (operation.ValueTypes.Single().Type != AmlDataType.Enum)
        return false;
      if (!operation.ValueTypes.Single().Values.Any() || operation.ValueTypes.Single().Values.Skip(1).Any())
        return false;
      value = operation.ValueTypes.Single().Values.Single();
      return true;
    }
  }
}
