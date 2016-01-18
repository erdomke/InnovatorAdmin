using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class PromiseResult<T1, T2>
  {
    public T1 Result1 { get; internal set; }
    public T2 Result2 { get; internal set; }

    internal PromiseResult() { }
    public PromiseResult(T1 result1, T2 result2)
    {
      this.Result1 = result1;
      this.Result2 = result2;
    }
  }

  public class PromiseResult<T1, T2, T3>
  {
    public T1 Result1 { get; internal set; }
    public T2 Result2 { get; internal set; }
    public T3 Result3 { get; internal set; }

    internal PromiseResult() { }
    public PromiseResult(T1 result1, T2 result2, T3 result3)
    {
      this.Result1 = result1;
      this.Result2 = result2;
      this.Result3 = result3;
    }
  }

  public class PromiseResult<T1, T2, T3, T4>
  {
    public T1 Result1 { get; internal set; }
    public T2 Result2 { get; internal set; }
    public T3 Result3 { get; internal set; }
    public T4 Result4 { get; internal set; }

    internal PromiseResult() { }
    public PromiseResult(T1 result1, T2 result2, T3 result3, T4 result4)
    {
      this.Result1 = result1;
      this.Result2 = result2;
      this.Result3 = result3;
      this.Result4 = result4;
    }
  }
}
