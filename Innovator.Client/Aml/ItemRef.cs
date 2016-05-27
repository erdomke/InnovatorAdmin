using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class ItemRef : IItemRef
  {
    private string _id;
    private string _type;

    /// <inheritdoc/>
    public string Id()
    {
      return _id;
    }

    /// <inheritdoc/>
    public string TypeName()
    {
      return _type;
    }

#if NET4
    public ItemRef(dynamic value)
    {
      _id = value.Id;
      _type = value.Type;
    }
#endif

    public ItemRef(string type, string id)
    {
      _id = id;
      _type = type;
    }
  }
}
