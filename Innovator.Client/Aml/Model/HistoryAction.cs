using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type History Action </summary>
  public class HistoryAction : Item
  {
    protected HistoryAction() { }
    public HistoryAction(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>comment_text</c> property of the item</summary>
    public IProperty_Text CommentText()
    {
      return this.Property("comment_text");
    }
    /// <summary>Retrieve the <c>is_internal</c> property of the item</summary>
    public IProperty_Boolean IsInternal()
    {
      return this.Property("is_internal");
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
  }
}