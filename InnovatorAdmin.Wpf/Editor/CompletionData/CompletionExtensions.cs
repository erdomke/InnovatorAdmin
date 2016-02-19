using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;

namespace InnovatorAdmin.Editor
{
  public static class CompletionExtensions
  {
    public static IEnumerable<ICompletionData> GetCompletions<T>(
      this IEnumerable<string> values, Action<T> configure = null) where T : IMutableCompletionData, new()
    {
      return values.Select(v => {
        var result = new T() { Text = v };
        if (configure != null)
          configure(result);
        return (ICompletionData)result;
      });
    }
    public static IPromise<IEnumerable<ICompletionData>> GetPromise<T>(
      this IEnumerable<string> values, Action<T> configure = null) where T : IMutableCompletionData, new()
    {
      return Promises.Resolved(values.GetCompletions<T>(configure));
    }

    public static IPromise<IEnumerable<ICompletionData>> GetPromise<T>
      (params string[] values) where T : IMutableCompletionData, new()
    {
      return Promises.Resolved(values.GetCompletions<T>());
    }
    public static IPromise<IEnumerable<ICompletionData>> GetPromise<T>
      (Action<T> configure, params string[] values) where T : IMutableCompletionData, new()
    {
      return Promises.Resolved(values.GetCompletions<T>(configure));
    }
  }
}
