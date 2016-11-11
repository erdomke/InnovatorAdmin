using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ContentItems </summary>
  public class cmf_ContentItems : Item, Icmf_ContentItems
  {
    protected cmf_ContentItems() { }
    public cmf_ContentItems(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ContentItems() { Innovator.Client.Item.AddNullItem<cmf_ContentItems>(new cmf_ContentItems { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>effective_date</c> property of the item</summary>
    public IProperty_Date EffectiveDate()
    {
      return this.Property("effective_date");
    }
    /// <summary>Retrieve the <c>release_date</c> property of the item</summary>
    public IProperty_Date ReleaseDate()
    {
      return this.Property("release_date");
    }
    /// <summary>Retrieve the <c>superseded_date</c> property of the item</summary>
    public IProperty_Date SupersededDate()
    {
      return this.Property("superseded_date");
    }
  }
}