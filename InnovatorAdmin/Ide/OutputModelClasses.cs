using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class OutputModelClasses
  {
    private IAsyncConnection _conn;

    public OutputModelClasses(IAsyncConnection conn)
    {
      _conn = conn;
    }

    public async Task Run()
    {
      var itemTypes = (await _conn.ApplyAsync(@"<Item type='ItemType' action='get' select='id,implementation_type,is_relationship,is_versionable'>
  <Relationships>
    <Item type='Property' action='get' select='name,data_source,data_type,foreign_property(data_type)'>
      <name condition='not in'>'classification','config_id','created_by_id','created_on','css','current_state','generation','is_current','is_released','itemtype','keyed_name','locked_by_id','major_rev','managed_by_id','minor_rev','modified_by_id','modified_on','new_version','not_lockable','owned_by_id','permission_id','state','team_id'</name>
    </Item>
    <Item type='Morphae' action='get' select='related_id' related_expand='0' />
  </Relationships>
</Item>", true, false).ToTask()).Items().OfType<Innovator.Client.Model.ItemType>();
      var dict = itemTypes.ToDictionary(i => i.Id());
      var polymorphicIds = new HashSet<string>();
      var links = new NameValueCollection();

      foreach (var itemType in itemTypes.Where(i => i.ImplementationType().Value == "polymorphic"))
      {
        polymorphicIds.Add(itemType.Id());
        var itemTypeLabel = "I" + itemType.IdProp().KeyedName().Value.Replace(" ", "");
        using (var writer = new StreamWriter(@"C:\Users\eric.domke\Documents\Models\" + itemTypeLabel + ".cs"))
        {
          await writer.WriteAsync(@"using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>");
          await writer.WriteAsync("Interface for polymorphic item type " + itemType.IdProp().KeyedName().Value);
          await writer.WriteAsync(@" </summary>
  public interface ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(@" : IItem
  {
");
          var versionable = itemType.Relationships("Morphae").All(m => dict[m.RelatedId().Value].IsVersionable().AsBoolean(false));

          foreach (var prop in itemType
            .Relationships()
            .OfType<Innovator.Client.Model.Property>()
            .Where(p => p.NameProp().Value != "source_id" && p.NameProp().Value != "related_id" && p.NameProp().Value != "id"))
          {
            if (!versionable
              && (prop.NameProp().Value == "effective_date" || prop.NameProp().Value == "release_date" || prop.NameProp().Value == "superseded_date"))
              continue;

            await writer.WriteAsync("    /// <summary>Retrieve the <c>");
            await writer.WriteAsync(prop.NameProp().Value);
            await writer.WriteAsync("</c> property of the item</summary>\r\n");
            await writer.WriteAsync("    IProperty_");
            await writer.WriteAsync(PropType(prop, polymorphicIds));
            await writer.WriteAsync(" ");
            await writer.WriteAsync(GetPropName(prop.NameProp().Value, itemTypeLabel.Substring(1)));
            await writer.WriteLineAsync(@"();");
          }
          await writer.WriteAsync(@"  }
}");
        }
        links.Add(itemType.Id(), itemTypeLabel);
        foreach (var poly in itemType.Relationships("Morphae"))
        {
          links.Add(poly.RelatedId().Value, itemTypeLabel);
        }
      }

      foreach (var itemType in itemTypes)
      {
        var itemTypeLabel = itemType.IdProp().KeyedName().Value.Replace(" ", "");
        using (var writer = new StreamWriter(@"C:\Users\eric.domke\Documents\Models\" + itemTypeLabel + ".cs"))
        {
          await writer.WriteAsync(@"using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>");
          await writer.WriteAsync("Class for the item type " + itemType.IdProp().KeyedName().Value);
          await writer.WriteAsync(@" </summary>
  public class ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(@" : Item");
          if (links[itemType.Id()] != null)
          {
            await writer.WriteAsync(@", ");
            await writer.WriteAsync(links.GetValues(itemType.Id()).GroupConcat(", "));
          }
          if (itemType.IsRelationship().AsBoolean(false))
          {
            var source = itemType.Relationships().OfType<Innovator.Client.Model.Property>().Single(p => p.NameProp().Value == "source_id");
            if (source.DataSource().KeyedName().HasValue())
            {
              await writer.WriteAsync(@", INullRelationship<");
              await writer.WriteAsync((polymorphicIds.Contains(source.DataSource().Value) ? "I" : "") + source.DataSource().KeyedName().Value.Replace(" ", ""));
              await writer.WriteAsync(@">");
            }

            var related = itemType.Relationships().OfType<Innovator.Client.Model.Property>().Single(p => p.NameProp().Value == "related_id");
            if (related.DataSource().KeyedName().HasValue())
            {
              await writer.WriteAsync(@", IRelationship<");
              await writer.WriteAsync((polymorphicIds.Contains(source.DataSource().Value) ? "I" : "") + related.DataSource().KeyedName().Value.Replace(" ", ""));
              await writer.WriteAsync(@">");
            }
          }
          await writer.WriteAsync(@"
  {
    protected ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(@"() { }
    public ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(@"(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync("() { Innovator.Client.Item.AddNullItem<");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(">(new ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(@" { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

");

          foreach (var prop in itemType
            .Relationships()
            .OfType<Innovator.Client.Model.Property>()
            .Where(p => p.NameProp().Value != "source_id" && p.NameProp().Value != "related_id" && p.NameProp().Value != "id"))
          {
            await writer.WriteAsync("    /// <summary>Retrieve the <c>");
            await writer.WriteAsync(prop.NameProp().Value);
            await writer.WriteAsync("</c> property of the item</summary>\r\n");
            await writer.WriteAsync("    public IProperty_");
            await writer.WriteAsync(PropType(prop, polymorphicIds));
            await writer.WriteAsync(" ");
            await writer.WriteAsync(GetPropName(prop.NameProp().Value, itemTypeLabel));
            await writer.WriteAsync(@"()
    {
      return this.Property(""");
            await writer.WriteAsync(prop.NameProp().Value);
            await writer.WriteAsync(@""");
    }
");
          }
          await writer.WriteAsync(@"  }
}");
        }
      }
    }

    private string PropType(Innovator.Client.Model.Property prop, HashSet<string> polymorphicIds)
    {
      var dataType = prop.DataType().Value;
      if (dataType == "foreign" && prop.ForeignProperty().HasValue())
      {
        prop = prop.ForeignProperty().AsModel();
        dataType = prop.DataType().Value;
      }
      switch (dataType)
      {
        case "boolean":
          return "Boolean";
        case "date":
          return "Date";
        case "integer":
        case "float":
        case "decimal":
          return "Number";
        case "item":
          if (prop.DataSource().KeyedName().Exists)
            return "Item<"
              + (polymorphicIds.Contains(prop.DataSource().Value) ? "I" : "")
              + prop.DataSource().KeyedName().Value.Replace(" ", "") + ">";
          return "Item<IReadOnlyItem>";
        default:
          return "Text";
      }
    }

    private string GetPropName(string name, string itemTypeLabel)
    {
      var output = new char[name.Length];
      var o = 0;
      bool lastCharBoundary = true;
      for (var i = 0; i < name.Length; i++)
      {
        if (name[i] == '_')
        {
          lastCharBoundary = true;
        }
        else
        {
          if (lastCharBoundary)
            output[o] = char.ToUpper(name[i]);
          else
            output[o] = char.ToLower(name[i]);
          o++;
          lastCharBoundary = false;
        }
      }
      var result = new string(output, 0, o);
      switch (result)
      {
        case "Add":
        case "AmlContext":
        case "Attribute":
        case "Attributes":
        case "Clone":
        case "Elements":
        case "Equals":
        case "Exists":
        case "GetHashCode":
        case "GetType":
        case "Id":
        case "Name":
        case "Next":
        case "Parent":
        case "Property":
        case "ReadOnly":
        case "Relationships":
        case "Remove":
        case "RemoveAttributes":
        case "RemoveNodes":
        case "ToAml":
        case "ToString":
        case "TypeName":
        case "Value":
          return result + "Prop";
      }
      if (result == itemTypeLabel)
        return result + "Prop";
      return result;
    }
  }
}
