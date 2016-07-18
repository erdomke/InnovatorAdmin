using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Life Cycle Transition </summary>
  public class LifeCycleTransition : Item
  {
    protected LifeCycleTransition() { }
    public LifeCycleTransition(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>from_state</c> property of the item</summary>
    public IProperty_Item FromState()
    {
      return this.Property("from_state");
    }
    /// <summary>Retrieve the <c>get_comment</c> property of the item</summary>
    public IProperty_Boolean GetComment()
    {
      return this.Property("get_comment");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>post_action</c> property of the item</summary>
    public IProperty_Item PostAction()
    {
      return this.Property("post_action");
    }
    /// <summary>Retrieve the <c>pre_action</c> property of the item</summary>
    public IProperty_Item PreAction()
    {
      return this.Property("pre_action");
    }
    /// <summary>Retrieve the <c>role</c> property of the item</summary>
    public IProperty_Item Role()
    {
      return this.Property("role");
    }
    /// <summary>Retrieve the <c>segments</c> property of the item</summary>
    public IProperty_Text Segments()
    {
      return this.Property("segments");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>to_state</c> property of the item</summary>
    public IProperty_Item ToState()
    {
      return this.Property("to_state");
    }
    /// <summary>Retrieve the <c>x</c> property of the item</summary>
    public IProperty_Number X()
    {
      return this.Property("x");
    }
    /// <summary>Retrieve the <c>y</c> property of the item</summary>
    public IProperty_Number Y()
    {
      return this.Property("y");
    }
  }
}