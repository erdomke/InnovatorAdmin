using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
  internal class BaseSqlCriteriaWriter
  {
    private ComparisonType _lastComparison = ComparisonType.Equal;
    private Stack<int> _ops = new Stack<int>();
    private bool _started = false;
    private Code.BaseCodeTextWriter _writer;

    public BaseSqlCriteriaWriter(Code.BaseCodeTextWriter writer)
    {
      _writer = writer;
      _ops.Push((int)LogicalOperator.And);
    }

    public void Logical(LogicalOperator op)
    {
      if (_ops.Count < 1 || (int)op != _ops.Peek())
      {
        if (op == LogicalOperator.Not)
        {
          _ops.Push(_ops.Peek() | (int)op);
          _writer.Write(" NOT (");
        }
        else
        {
          _ops.Push((int)op);
          _writer.Write(" (");
        }
      }
    }

    public void LogicalEnd()
    {
      _writer.Write(")");
      _ops.Pop();
    }

    public void Comparison(ComparisonType comp)
    {
      switch (comp)
      {
        case ComparisonType.Between:
          _writer.Write(" BETWEEN ");
          break;
        case ComparisonType.Contains:
        case ComparisonType.EndsWith:
        case ComparisonType.Like:
        case ComparisonType.StartsWith:
          _writer.Write(" LIKE ");
          break;
        case ComparisonType.Equal:
          _writer.Write(" = ");
          break;
        case ComparisonType.GreaterThan:
          _writer.Write(" > ");
          break;
        case ComparisonType.GreaterThanEqual:
          _writer.Write(" >= ");
          break;
        case ComparisonType.In:
          _writer.Write(" IN ");
          break;
        case ComparisonType.IsNotNull:
          _writer.Write(" IS NOT ");
          _writer.Null();
          _writer.Write(" ");
          break;
        case ComparisonType.IsNull:
          _writer.Write(" IS ");
          _writer.Null();
          _writer.Write(" ");
          break;
        case ComparisonType.LessThan:
          _writer.Write(" < ");
          break;
        case ComparisonType.LessThanEqual:
          _writer.Write(" <= ");
          break;
        case ComparisonType.NotEqual:
          _writer.Write(" <> ");
          break;
        case ComparisonType.NotLike:
          throw new NotSupportedException();
      }
      _lastComparison = comp;
    }

    public void Identifier(object value)
    {
      if (_started)
      {
        if ((_ops.Peek() & (int)LogicalOperator.And) == (int)LogicalOperator.And)
        {
          _writer.Write(" AND ");
        }
        else if ((_ops.Peek() & (int)LogicalOperator.Or) == (int)LogicalOperator.Or)
        {
          _writer.Write(" OR ");
        }
      }
      _started = true;
    }

    public void StringValue()
    {
      switch (_lastComparison)
      {
        case ComparisonType.Contains:
        case ComparisonType.EndsWith:
          _writer.Write('%');
          break;
      }
    }
    public void StringValueEnd()
    {
      switch (_lastComparison)
      {
        case ComparisonType.Contains:
        case ComparisonType.StartsWith:
          _writer.Write('%');
          break;
      }
    }
  }
}
