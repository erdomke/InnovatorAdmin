using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Time to Manufacturing </summary>
  public class TimetoManufacturing : Item
  {
    protected TimetoManufacturing() { }
    public TimetoManufacturing(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static TimetoManufacturing() { Innovator.Client.Item.AddNullItem<TimetoManufacturing>(new TimetoManufacturing { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

  }
}