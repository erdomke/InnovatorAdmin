using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SSVC_Preferences </summary>
  public class SSVC_Preferences : Item, INullRelationship<Preference>
  {
    protected SSVC_Preferences() { }
    public SSVC_Preferences(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SSVC_Preferences() { Innovator.Client.Item.AddNullItem<SSVC_Preferences>(new SSVC_Preferences { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>default_bookmark</c> property of the item</summary>
    public IProperty_Text DefaultBookmark()
    {
      return this.Property("default_bookmark");
    }
    /// <summary>Retrieve the <c>default_flagged_by_number</c> property of the item</summary>
    public IProperty_Number DefaultFlaggedByNumber()
    {
      return this.Property("default_flagged_by_number");
    }
    /// <summary>Retrieve the <c>default_replies_number</c> property of the item</summary>
    public IProperty_Number DefaultRepliesNumber()
    {
      return this.Property("default_replies_number");
    }
    /// <summary>Retrieve the <c>messages_max_lines</c> property of the item</summary>
    public IProperty_Number MessagesMaxLines()
    {
      return this.Property("messages_max_lines");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}