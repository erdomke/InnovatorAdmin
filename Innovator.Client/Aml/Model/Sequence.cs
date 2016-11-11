using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Sequence </summary>
  public class Sequence : Item
  {
    protected Sequence() { }
    public Sequence(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Sequence() { Innovator.Client.Item.AddNullItem<Sequence>(new Sequence { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>current_value</c> property of the item</summary>
    public IProperty_Number CurrentValue()
    {
      return this.Property("current_value");
    }
    /// <summary>Retrieve the <c>initial_value</c> property of the item</summary>
    public IProperty_Number InitialValue()
    {
      return this.Property("initial_value");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>pad_to</c> property of the item</summary>
    public IProperty_Number PadTo()
    {
      return this.Property("pad_to");
    }
    /// <summary>Retrieve the <c>pad_with</c> property of the item</summary>
    public IProperty_Text PadWith()
    {
      return this.Property("pad_with");
    }
    /// <summary>Retrieve the <c>prefix</c> property of the item</summary>
    public IProperty_Text Prefix()
    {
      return this.Property("prefix");
    }
    /// <summary>Retrieve the <c>step</c> property of the item</summary>
    public IProperty_Number Step()
    {
      return this.Property("step");
    }
    /// <summary>Retrieve the <c>suffix</c> property of the item</summary>
    public IProperty_Text Suffix()
    {
      return this.Property("suffix");
    }
  }
}