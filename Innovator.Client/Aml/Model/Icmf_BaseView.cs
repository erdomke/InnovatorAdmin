using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Interface for polymorphic item type cmf_BaseView </summary>
  public interface Icmf_BaseView : IItem
  {
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    IProperty_Text NameProp();
  }
}