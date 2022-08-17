using Innovator.Telemetry;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Collections.Generic;

namespace InnovatorAdmin
{
  internal class LogProcessor : BaseProcessor<LogRecord>
  {
    private readonly ILogProcessor _processor;
    private readonly IEnricher _enricher;

    public LogProcessor(ILogProcessor processor, IEnricher enricher)
    {
      _processor = processor;
      _enricher = enricher;
    }

    public override void OnEnd(LogRecord data)
    {
      var level = 0;
      switch (data.LogLevel)
      {
        case LogLevel.Critical:
          level = 21;
          break;
        case LogLevel.Debug:
          level = 5;
          break;
        case LogLevel.Error:
          level = 17;
          break;
        case LogLevel.Information:
          level = 9;
          break;
        case LogLevel.Trace:
          level = 1;
          break;
        case LogLevel.Warning:
          level = 13;
          break;
      }

      var attributes = new Dictionary<string, object>();
      var message = _enricher.Enrich(attributes, data.FormattedMessage, data.State ?? data.StateValues, data.Exception, attr =>
      {
        data.ForEachScope((scope, a) =>
        {
          foreach (var kvp in scope)
            a[kvp.Key] = kvp.Value;
        }, attr);
      });
      _processor.ProcessLog(level, data.Timestamp, message, attributes, data.SpanId.ToString(), data.TraceId.ToString());
    }
  }
}
