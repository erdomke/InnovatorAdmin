using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Aras
{
  public interface IDataItem : IItem, Data.IDataRecord, Xml.IAttributeContainer
  {
    IEnumerable<IProperty> Properties { get; }
    IProperty Property(string name);
  }
}
