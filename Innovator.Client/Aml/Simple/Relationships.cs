using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  class Relationships : Element, IRelationships
  {
    public override string Name { get { return "Relationships"; } }
    public override ILinkedElement Next { get; set; }
    public override IElement Parent { get; set; }

    public Relationships() { }
    public Relationships(IElement parent)
    {
      this.Parent = parent;
    }
    public Relationships(params object[] content) : this()
    {
      for (var i = 0; i < content.Length; i++)
      {
        Add(content[i]);
      }
    }

    public IEnumerable<IReadOnlyItem> ByType(string type)
    {
      return Elements().OfType<IReadOnlyItem>().Where(i => i.TypeName() == type);
    }
  }
}
