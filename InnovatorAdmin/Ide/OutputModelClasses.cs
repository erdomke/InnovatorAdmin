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
      var itemTypes = (await _conn.ApplyAsync(@"<Item type='ItemType' action='get' select='id,implementation_type'>
  <Relationships>
    <Item type='Property' action='get' select='name,data_source,data_type,foreign_property(data_type)'>
      <name condition='not in'>'classification','config_id','created_by_id','created_on','css','current_state','generation','id','is_current','is_released','itemtype','keyed_name','locked_by_id','major_rev','managed_by_id','minor_rev','modified_by_id','modified_on','new_version','not_lockable','owned_by_id','permission_id','related_id','source_id','state','team_id'</name>
    </Item>
    <Item type='Morphae' action='get' select='related_id' related_expand='0' />
  </Relationships>
</Item>", true, false).ToTask()).Items();
      var links = new NameValueCollection();

      foreach (var itemType in itemTypes.Where(i => i.Property("implementation_type").Value == "polymorphic"))
      {
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
          foreach (var prop in itemType.Relationships("Property"))
          {
            await writer.WriteAsync("    /// <summary>Retrieve the <c>");
            await writer.WriteAsync(prop.Property("name").Value);
            await writer.WriteAsync("</c> property of the item</summary>\r\n");
            await writer.WriteAsync("    IProperty_");
            await writer.WriteAsync(PropType(prop));
            await writer.WriteAsync(" ");
            await writer.WriteAsync(GetPropName(prop.Property("name").Value, itemTypeLabel.Substring(1)));
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
          await writer.WriteAsync(@"
  {
    protected ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(@"() { }
    public ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(@"(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
");
          foreach (var prop in itemType.Relationships("Property"))
          {
            await writer.WriteAsync("    /// <summary>Retrieve the <c>");
            await writer.WriteAsync(prop.Property("name").Value);
            await writer.WriteAsync("</c> property of the item</summary>\r\n");
            await writer.WriteAsync("    public IProperty_");
            await writer.WriteAsync(PropType(prop));
            await writer.WriteAsync(" ");
            await writer.WriteAsync(GetPropName(prop.Property("name").Value, itemTypeLabel));
            await writer.WriteAsync(@"()
    {
      return this.Property(""");
            await writer.WriteAsync(prop.Property("name").Value);
            await writer.WriteAsync(@""");
    }
");
          }
          await writer.WriteAsync(@"  }
}");
        }
      }
    }

    private string PropType(IReadOnlyItem prop)
    {
      var dataType = prop.Property("data_type").Value;
      if (dataType == "foreign" && prop.Property("foreign_property").HasValue())
        dataType = prop.Property("foreign_property").AsItem().Property("data_type").Value;
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
          return "Item";
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
