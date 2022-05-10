using System;
using System.Collections.Generic;
using System.Linq;

namespace Innovator.Telemetry
{
  public class DefaultEnricher : IEnricher
  {
    private const string OriginalFormatPropertyName = "{OriginalFormat}";

    private Dictionary<Type, Action<IDictionary<string, object>, Exception>> _exceptionEnrichers = new Dictionary<Type, Action<IDictionary<string, object>, Exception>>();

    public DefaultEnricher WithExceptionEnricher(Type type, Action<IDictionary<string, object>, Exception> enricher)
    {
      _exceptionEnrichers[type] = enricher;
      return this;
    }

    public string Enrich(IDictionary<string, object> attributes, string formattedMessage, object state, Exception exception, Action<IDictionary<string, object>> enrichScope)
    {
      var message = formattedMessage;
      WithException(attributes, exception);
      enrichScope?.Invoke(attributes);

      if (state is IEnumerable<KeyValuePair<string, object>> structure)
      {
        foreach (var kvp in structure)
        {
          if (kvp.Key == OriginalFormatPropertyName)
          {
            if (string.IsNullOrEmpty(message) && !(kvp.Value is string msg && msg == "[null]"))
              message = state.ToString();
          }
          else
          {
            attributes[kvp.Key] = kvp.Value;
          }

        }
      }
      else if (state != null)
      {
        attributes["state"] = state;
      }

      if (string.IsNullOrEmpty(message) && exception != null)
        message = exception.Message;
      return message;
    }

    private IDictionary<string, object> WithException(IDictionary<string, object> attributes, Exception exception)
    {
      if (exception == null)
        return attributes;

      attributes["exception.type"] = exception.GetType().FullName;
      attributes["exception.message"] = exception.Message;
      if (exception.StackTrace != null)
        attributes["exception.stacktrace"] = exception.StackTrace.ToString();

      foreach (var key in exception.Data)
        attributes[key.ToString()] = exception.Data[key];

      if (_exceptionEnrichers.TryGetValue(exception.GetType(), out var enricher))
        enricher?.Invoke(attributes, exception);

      var innerExceptions = Enumerable.Empty<Exception>();
      if (exception is AggregateException aggregateException)
        innerExceptions = aggregateException.InnerExceptions;
      else if (exception.InnerException != null)
        innerExceptions = new[] { exception.InnerException };

      if (innerExceptions.Any())
      {
        attributes["exception.inner"] = innerExceptions
          .Select(e => WithException(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), e))
          .ToList();
      }
      return attributes;
    }
  }
}
