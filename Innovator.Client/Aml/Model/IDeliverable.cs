using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Interface for polymorphic item type Deliverable </summary>
  public interface IDeliverable : IItem
  {
    /// <summary>Retrieve the <c>effective_date</c> property of the item</summary>
    IProperty_Date EffectiveDate();
    /// <summary>Retrieve the <c>release_date</c> property of the item</summary>
    IProperty_Date ReleaseDate();
    /// <summary>Retrieve the <c>superseded_date</c> property of the item</summary>
    IProperty_Date SupersededDate();
  }
}