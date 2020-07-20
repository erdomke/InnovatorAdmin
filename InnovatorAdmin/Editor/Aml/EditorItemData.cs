using Innovator.Client;
using System.Collections.Generic;

namespace InnovatorAdmin.Editor
{
  public class EditorItemData : IItemData
  {
    private Dictionary<string, string> _propertyData =
      new Dictionary<string, string>();

    public string Type { get; set; }
    public string Id { get; set; }
    public string Action { get; set; }

    public object Property(string name)
    {
      string result;
      if (_propertyData.TryGetValue(name, out result))
        return result;
      return null;
    }
    internal void Property(string name, string value)
    {
      _propertyData[name] = value;
    }

    public IReadOnlyItem ToItem(ElementFactory aml)
    {
      var item = aml.Item(aml.Type(Type), aml.Id(Id));
      foreach (var prop in _propertyData)
        item.Property(prop.Key).Set(prop.Value);
      return item;
    }
  }
}
