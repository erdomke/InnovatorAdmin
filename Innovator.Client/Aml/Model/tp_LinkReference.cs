using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type tp_LinkReference </summary>
  public class tp_LinkReference : Item
  {
    protected tp_LinkReference() { }
    public tp_LinkReference(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>path</c> property of the item</summary>
    public IProperty_Text Path()
    {
      return this.Property("path");
    }
    /// <summary>Retrieve the <c>reference_id</c> property of the item</summary>
    public IProperty_Text ReferenceId()
    {
      return this.Property("reference_id");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>targetelement</c> property of the item</summary>
    public IProperty_Text Targetelement()
    {
      return this.Property("targetelement");
    }
  }
}