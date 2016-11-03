using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type PreferenceTypes </summary>
  public class PreferenceTypes : Item
  {
    protected PreferenceTypes() { }
    public PreferenceTypes(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static PreferenceTypes() { Innovator.Client.Item.AddNullItem<PreferenceTypes>(new PreferenceTypes { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

  }
}