using System.Xml;

namespace InnovatorAdmin
{
  public interface IDependencyContext
  {
    XmlElement Element { get; }
    ItemReference ItemReference { get; }
    void AddDependency(ItemReference itemRef, XmlNode context, XmlNode reference);
  }
}
