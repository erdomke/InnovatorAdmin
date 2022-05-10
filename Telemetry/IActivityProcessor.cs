using System;
using System.Collections.Generic;

namespace Innovator.Telemetry
{
  public interface IActivityProcessor
  {
    void ProcessActivity(bool start, DateTime timestamp, string operationName, string displayName, IEnumerable<KeyValuePair<string, object>> tags, string spanId, string traceId);
  }
}
