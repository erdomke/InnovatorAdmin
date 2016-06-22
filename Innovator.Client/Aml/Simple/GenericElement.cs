using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client.Aml.Simple
{
  class GenericElement : Element
  {
    private string _name;
    private IElement _parent;

    public override string Name { get { return _name; } }
    public override IElement Parent { get { return _parent; } }

    public GenericElement(string name, IElement parent)
    {
      _name = name;
      _parent = parent;
    }
    public GenericElement(IReadOnlyElement elem, IElement parent) : base(elem)
    {
      _name = elem.Name;
      _parent = parent;
    }
  }
}
