using Innovator.Telemetry;
using OpenTelemetry;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InnovatorAdmin.Cmd
{
  internal class ActivityProcessor : BaseProcessor<Activity>
  {
    private readonly IActivityProcessor _processor;

    public ActivityProcessor(IActivityProcessor processor)
    {
      _processor = processor;
    }

    public override void OnStart(Activity data)
    {
      var tags = data.TagObjects
        .Concat(data.Tags.Select(k => new KeyValuePair<string, object>(k.Key, k.Value)));
      _processor.ProcessActivity(true, data.StartTimeUtc, data.OperationName, data.DisplayName, tags, data.SpanId.ToString(), data.TraceId.ToString());
    }

    public override void OnEnd(Activity data)
    {
      var tags = data.TagObjects
        .Concat(data.Tags.Select(k => new KeyValuePair<string, object>(k.Key, k.Value)));
      _processor.ProcessActivity(false, data.StartTimeUtc + data.Duration, data.OperationName, data.DisplayName, tags, data.SpanId.ToString(), data.TraceId.ToString());
    }
  }
}
