using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type CommandBarSection </summary>
  public class CommandBarSection : Item
  {
    protected CommandBarSection() { }
    public CommandBarSection(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static CommandBarSection() { Innovator.Client.Item.AddNullItem<CommandBarSection>(new CommandBarSection { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>additional_data</c> property of the item</summary>
    public IProperty_Text AdditionalData()
    {
      return this.Property("additional_data");
    }
    /// <summary>Retrieve the <c>builder_method</c> property of the item</summary>
    public IProperty_Item<Method> BuilderMethod()
    {
      return this.Property("builder_method");
    }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>location_name</c> property of the item</summary>
    public IProperty_Text LocationName()
    {
      return this.Property("location_name");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
  }
}