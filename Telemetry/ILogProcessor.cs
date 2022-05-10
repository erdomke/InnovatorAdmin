using System;
using System.Collections.Generic;

namespace Innovator.Telemetry
{
  public interface ILogProcessor
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="level">The logging level. The number should be in one of the ranges for <a href="https://opentelemetry.io/docs/reference/specification/logs/data-model/#field-severitynumber">SeverityNumber</a> in the OpenTelemetry docs</param>
    /// <param name="timestamp">When the event was logged</param>
    /// <param name="formattedMessage">The formatted log message (if captured)</param>
    /// <param name="attributes">Structured data to include with the log message captured from any exceptions, the scope, etc.</param>
    /// <param name="spanId">Span ID from the current trace/activity</param>
    /// <param name="traceId">Trace ID from the current trace/activity</param>
    void ProcessLog(int level, DateTime timestamp, string formattedMessage, IEnumerable<KeyValuePair<string, object>> attributes, string spanId, string traceId);
  }
}
