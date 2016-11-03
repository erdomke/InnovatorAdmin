using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_TabularView </summary>
  public class cmf_TabularView : Item, Icmf_BaseView
  {
    protected cmf_TabularView() { }
    public cmf_TabularView(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_TabularView() { Innovator.Client.Item.AddNullItem<cmf_TabularView>(new cmf_TabularView { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>default_header_style</c> property of the item</summary>
    public IProperty_Item<cmf_Style> DefaultHeaderStyle()
    {
      return this.Property("default_header_style");
    }
    /// <summary>Retrieve the <c>grid_border_color</c> property of the item</summary>
    public IProperty_Text GridBorderColor()
    {
      return this.Property("grid_border_color");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>tree_label_method</c> property of the item</summary>
    public IProperty_Item<Method> TreeLabelMethod()
    {
      return this.Property("tree_label_method");
    }
  }
}