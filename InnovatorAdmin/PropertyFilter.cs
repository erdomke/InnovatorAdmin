using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public interface IPropertyFilter
  {
    bool Contains(PropertyDescriptor descrip);
  }

  public class PropertyFilter<T> : IPropertyFilter
  {
    private HashSet<string> _names = new HashSet<string>();

    public void Add(Expression<Func<T, object>> propertyGetter)
    {
      var body = propertyGetter.Body;
      // Convert
      if (body.NodeType == ExpressionType.Convert
        || body.NodeType == ExpressionType.ConvertChecked)
      {
        body = ((UnaryExpression)propertyGetter.Body).Operand;
      }

      // PropertyExpression
      var propExpr = body as MemberExpression;
      if (propExpr == null)
        throw new ArgumentException("Expression should be a property access (e.g. i => i.PropertyName )");
      _names.Add(propExpr.Member.Name);
    }

    public void Clear()
    {
      _names.Clear();
    }

    public bool Contains(PropertyDescriptor descrip)
    {
      return typeof(T).IsAssignableFrom(descrip.ComponentType)
        && (!_names.Any() || _names.Contains(descrip.Name));
    }

    public static void Test()
    {
      var filter = new PropertyFilter<Settings>();
      filter.Add(s => s.DiffToolCommand);
    }
  }
}
