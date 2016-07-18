using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type History Container </summary>
  public class HistoryContainer : Item
  {
    protected HistoryContainer() { }
    public HistoryContainer(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>item_config_id</c> property of the item</summary>
    public IProperty_Text ItemConfigId()
    {
      return this.Property("item_config_id");
    }
    /// <summary>Retrieve the <c>item_keyed_name</c> property of the item</summary>
    public IProperty_Text ItemKeyedName()
    {
      return this.Property("item_keyed_name");
    }
    /// <summary>Retrieve the <c>itemtype_id</c> property of the item</summary>
    public IProperty_Item ItemtypeId()
    {
      return this.Property("itemtype_id");
    }
  }
}