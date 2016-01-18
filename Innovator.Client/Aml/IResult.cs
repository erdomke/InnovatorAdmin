using System;
using System.Collections.Generic;

namespace Innovator.Client
{
  public interface IResult : IReadOnlyResult, IErrorBuilder
  {
    new IItem AssertItem(string type = null);
    IResult Add(IItem content);
    new string Value { get; set; }
  }

  public interface IReadOnlyResult
  {
    IReadOnlyItem AssertItem(string type = null);
    IEnumerable<IReadOnlyItem> AssertItems();
    IReadOnlyResult AssertNoError();
    ServerException Exception { get; }
    IEnumerable<IReadOnlyItem> Items();
    string Value { get; }
    string ToAml();
  }
}
