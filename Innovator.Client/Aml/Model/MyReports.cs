using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type MyReports </summary>
  public class MyReports : Item
  {
    protected MyReports() { }
    public MyReports(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static MyReports() { Innovator.Client.Item.AddNullItem<MyReports>(new MyReports { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

  }
}