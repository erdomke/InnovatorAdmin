using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  class AmlElement : Element
  {
    private ElementFactory _amlContext;
    private string _name;
    private IElement _parent;

    public override ElementFactory AmlContext { get { return _amlContext; } }
    public override string Name { get { return _name; } }
    public override IElement Parent
    {
      get { return _parent ?? NullElem; }
      set { _parent = value; }
    }

    public AmlElement(ElementFactory amlContext, string name, params object[] content)
    {
      _amlContext = amlContext;
      _name = name;
      Add(content);
    }
    public AmlElement(IElement parent, string name)
    {
      _amlContext = parent.AmlContext;
      _name = name;
      _parent = parent;
    }
    public AmlElement(IElement parent, IReadOnlyElement elem) : base()
    {
      _amlContext = parent.AmlContext;
      _name = elem.Name;
      _parent = parent;
      CopyData(elem);
    }

    private static AmlElement _nullElem = new AmlElement((ElementFactory)null, null);
    public static AmlElement NullElem { get { return _nullElem; } }

  }
}
