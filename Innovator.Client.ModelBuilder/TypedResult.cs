using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Innovator.Client.ModelBuilder
{
  public class TypedResult : IReadOnlyResult
  {
    private IEnumerable<IReadOnlyItem> _items;

    public ServerException Exception { get; set; }

    public string Value { get; set; }

    public TypedResult() { }
    public TypedResult(IEnumerable<IReadOnlyItem> items)
    {
      _items = items;
    }

    public IReadOnlyItem AssertItem(string type = null)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IReadOnlyItem> AssertItems()
    {
      throw new NotImplementedException();
    }

    public IReadOnlyResult AssertNoError()
    {
      if (this.Exception != null)
        throw Exception;
      return this;
    }

    public IEnumerable<IReadOnlyItem> Items()
    {
      return _items;
    }

    public void ToAml(XmlWriter writer, AmlWriterSettings settings)
    {
      throw new NotImplementedException();
    }

  }
}
