using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
  public interface ICriteriaWriter : Pipes.Code.IBaseCodeWriter
  {
    ICriteriaWriter Logical(LogicalOperator op);
    ICriteriaWriter LogicalEnd();
    ICriteriaWriter Comparison(ComparisonType comp);
  }
}
