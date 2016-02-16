using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
  public class OracleSqlCriteriaWriter : OracleSqlTextWriter, ICriteriaWriter
  {
    private BaseSqlCriteriaWriter criteria;

    public OracleSqlCriteriaWriter(System.IO.TextWriter writer)
      : base(writer) 
    {
      criteria = new BaseSqlCriteriaWriter(this);
    }

    public ICriteriaWriter Logical(LogicalOperator op)
    {
      criteria.Logical(op);
      return this;
    }

    public ICriteriaWriter LogicalEnd()
    {
      criteria.LogicalEnd();
      return this;
    }

    public ICriteriaWriter Comparison(ComparisonType comp)
    {
      criteria.Comparison(comp);
      return this;
    }
    
    public override Code.IBaseCodeWriter Identifier(object value)
    {
      criteria.Identifier(value);
      return base.Identifier(value);
    }
    public override System.IO.TextWriter StringValue()
    {
      base.StringValue();
      criteria.StringValue();
      return this;
    }
    public override Code.ICodeTextWriter StringValueEnd()
    {
      criteria.StringValueEnd();
      return base.StringValueEnd();
    }
  }
}
