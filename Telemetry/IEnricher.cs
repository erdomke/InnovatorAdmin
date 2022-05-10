using System;
using System.Collections.Generic;

namespace Innovator.Telemetry
{
  public interface IEnricher
  {
    /// <summary>
    /// Enrich the tags for a log entry by gather data from the state object, exception, scope, and other sources
    /// </summary>
    /// <param name="attributes">The tags to update</param>
    /// <param name="formattedMessage">The formatted message string</param>
    /// <param name="state">The state</param>
    /// <param name="exception">The exception</param>
    /// <param name="enrichScope">Callback used to get scope data (if available)</param>
    /// <returns>The new message string</returns>
    string Enrich(IDictionary<string, object> attributes, string formattedMessage, object state, Exception exception, Action<IDictionary<string, object>> enrichScope);
  }
}
