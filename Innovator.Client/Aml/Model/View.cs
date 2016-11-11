using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type View </summary>
  public class View : Item, INullRelationship<ItemType>, IRelationship<Form>
  {
    protected View() { }
    public View(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static View() { Innovator.Client.Item.AddNullItem<View>(new View { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>client</c> property of the item</summary>
    public IProperty_Text Client()
    {
      return this.Property("client");
    }
    /// <summary>Retrieve the <c>display_priority</c> property of the item</summary>
    public IProperty_Number DisplayPriority()
    {
      return this.Property("display_priority");
    }
    /// <summary>Retrieve the <c>form_classification</c> property of the item</summary>
    public IProperty_Text FormClassification()
    {
      return this.Property("form_classification");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>method</c> property of the item</summary>
    public IProperty_Item<Method> Method()
    {
      return this.Property("method");
    }
    /// <summary>Retrieve the <c>role</c> property of the item</summary>
    public IProperty_Item<Identity> Role()
    {
      return this.Property("role");
    }
    /// <summary>Retrieve the <c>show_ssvc</c> property of the item</summary>
    public IProperty_Boolean ShowSsvc()
    {
      return this.Property("show_ssvc");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>type</c> property of the item</summary>
    public IProperty_Text Type()
    {
      return this.Property("type");
    }
    /// <summary>Retrieve the <c>url</c> property of the item</summary>
    public IProperty_Text Url()
    {
      return this.Property("url");
    }
  }
}