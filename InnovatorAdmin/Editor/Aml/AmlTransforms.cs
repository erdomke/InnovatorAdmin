using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InnovatorAdmin.Editor
{
  public static class AmlTransforms
  {
    public static void CriteriaToWhereClause(XElement elem)
    {
      var item = elem.DescendantsAndSelf("Item").First();
      if (item.Attribute("type") == null)
        return;

      var builder = new CriteriaBuilder()
      {
        Operator = "and",
        Type = "[" + item.Attribute("type").Value.Replace(' ', '_') + "]"
      };
      ProcessCriteria(elem, builder);
      var where = item.Attribute("where");
      var whereClause = builder.ToString();
      if (where != null)
        whereClause += " and " + where.Value;
      item.SetAttributeValue("where", whereClause);
    }

    private static void ProcessCriteria(XElement parent, CriteriaBuilder builder)
    {
      CriteriaBuilder newBuilder;
      foreach (var elem in parent.Elements())
      {
        switch (elem.Name.LocalName)
        {
          case "Relationships":
            break;
          case "not":
            newBuilder = new CriteriaBuilder()
            {
              Not = true,
              Operator = "and",
              Type = builder.Type
            };
            ProcessCriteria(elem, newBuilder);
            builder.Builders.Add(newBuilder);
            elem.Remove();
            break;
          case "and":
            newBuilder = new CriteriaBuilder()
            {
              Not = true,
              Operator = "and",
              Type = builder.Type
            };
            ProcessCriteria(elem, newBuilder);
            builder.Builders.Add(newBuilder);
            elem.Remove();
            break;
          case "or":
            newBuilder = new CriteriaBuilder()
            {
              Not = true,
              Operator = "or",
              Type = builder.Type
            };
            ProcessCriteria(elem, newBuilder);
            builder.Builders.Add(newBuilder);
            elem.Remove();
            break;
          default:
            builder.Add(elem);
            elem.Remove();
            break;
        }
      }
    }

    private class CriteriaBuilder
    {
      private List<string> _expressions = new List<string>();
      private List<CriteriaBuilder> _builders = new List<CriteriaBuilder>();

      public bool Not { get; set; }
      public string Operator { get; set; }
      public string Type { get; set; }
      public IList<CriteriaBuilder> Builders { get { return _builders; } }

      public void Add(XElement elem)
      {
        var conditionAttr = elem.Attribute("condition");
        Add(elem.Name.LocalName, conditionAttr == null ? "eq" : conditionAttr.Value, elem.Value);
      }
      public void Add(string field, string op, string expression)
      {
        switch (op)
        {
          case "between":
            _expressions.Add(Type + "." + field + " between " + expression);
            break;
          case "eq":
            _expressions.Add(Type + "." + field + " = " + RenderValue(expression));
            break;
          case "ge":
            _expressions.Add(Type + "." + field + " >= " + RenderValue(expression));
            break;
          case "gt":
            _expressions.Add(Type + "." + field + " > " + RenderValue(expression));
            break;
          case "in":
            if (!string.IsNullOrWhiteSpace(expression) && expression.TrimStart()[0] != '(')
              expression = "(" + expression + ")";
            _expressions.Add(Type + "." + field + " in " + expression);
            break;
          case "is not null":
          case "is null":
            _expressions.Add(Type + "." + field + " " + op);
            break;
          case "is":
            _expressions.Add(Type + "." + field + " is " + expression);
            break;
          case "le":
            _expressions.Add(Type + "." + field + " <= " + RenderValue(expression));
            break;
          case "like":
            _expressions.Add(Type + "." + field + " like " + RenderValue(expression).Replace('*', '%'));
            break;
          case "lt":
            _expressions.Add(Type + "." + field + " < " + RenderValue(expression));
            break;
          case "ne":
            _expressions.Add(Type + "." + field + " <> " + RenderValue(expression));
            break;
          case "not between":
            _expressions.Add(Type + "." + field + " not between " + expression);
            break;
          case "not in":
            if (!string.IsNullOrWhiteSpace(expression) && expression.TrimStart()[0] != '(')
              expression = "(" + expression + ")";
            _expressions.Add("not " + Type + "." + field + " in " + expression);
            break;
          case "not like":
            _expressions.Add("not " + Type + "." + field + " like " + RenderValue(expression).Replace('*', '%'));
            break;
        }
      }

      public override string ToString()
      {
        var expr = _expressions
          .Concat(_builders.Select(b => "(" + b.ToString() + ")"))
          .GroupConcat(" " + Operator.Trim() + " ", e => e);
        if (Not)
          return "not (" + expr + ")";
        return expr;
      }

      private string RenderValue(string value)
      {
        double number;
        if (double.TryParse(value, out number))
          return value;
        return "'" + value.Replace("'", "''") + "'";
      }
    }

  }
}
