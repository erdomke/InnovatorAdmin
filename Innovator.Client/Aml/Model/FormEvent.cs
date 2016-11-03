using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Form Event </summary>
  public class FormEvent : Item, INullRelationship<Form>, IRelationship<Method>
  {
    protected FormEvent() { }
    public FormEvent(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static FormEvent() { Innovator.Client.Item.AddNullItem<FormEvent>(new FormEvent { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>form_event</c> property of the item</summary>
    public IProperty_Text FormEventProp()
    {
      return this.Property("form_event");
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