using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_TabularViewTree </summary>
  public class cmf_TabularViewTree : Item, INullRelationship<cmf_TabularView>
  {
    protected cmf_TabularViewTree() { }
    public cmf_TabularViewTree(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_TabularViewTree() { Innovator.Client.Item.AddNullItem<cmf_TabularViewTree>(new cmf_TabularViewTree { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>document_element_icon</c> property of the item</summary>
    public IProperty_Text DocumentElementIcon()
    {
      return this.Property("document_element_icon");
    }
    /// <summary>Retrieve the <c>element_type</c> property of the item</summary>
    public IProperty_Item<cmf_ElementType> ElementType()
    {
      return this.Property("element_type");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}