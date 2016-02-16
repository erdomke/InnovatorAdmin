using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Aras
{
  public class Result : IResult
  {
    private string _result;

    public string Value
    {
      get { return _result; }
    }

    public Result(string result)
    {
      _result = result;
    }
  }
}
