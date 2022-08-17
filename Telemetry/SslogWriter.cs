using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Innovator.Telemetry
{
  public class SslogWriter : IActivityProcessor, ILogProcessor
  {
    private TextWrapper _writer;
    private JsonSerializer _serializer = JsonSerializer.Create();
    private object _lock = new object();
    private List<StartActivity> _starts = new List<StartActivity>();
    private bool _dateWritten = false;

    public bool UseConsoleColors { get; set; }

    public SslogWriter(TextWriter writer)
    {
      _writer = new TextWrapper(writer)
      {
        MaxWidth = 120,
        BreakInWord = false
      };
    }

    public void ProcessActivity(bool start, DateTime timestamp, string operationName, string displayName, IEnumerable<KeyValuePair<string, object>> tags, string spanId, string traceId)
    {
      var full = true;
      var message = displayName ?? operationName;
      if (start)
      {
        if (!string.IsNullOrEmpty(spanId))
          _starts.Add(new StartActivity(spanId, tags));
      }
      else
      {
        var idx = _starts.FindIndex(s => s.SpanId == spanId);
        if (idx == _starts.Count - 1)
        {
          full = false;
          message = null;
        }
        tags = tags.Where(k => !_starts[idx].Tags.Contains(k.Key)).ToList();
        if (idx >= 0)
          _starts.RemoveAt(idx);
      }

      var command = start ? "START" : "END";
      var allTags = tags
        .GroupBy(t => t.Key)
        .ToDictionary(g => g.Key, g => (object)g.First().Value);
      if (full && spanId?.Trim('0')?.Length > 0)
        allTags["span_id"] = spanId;
      if (full && traceId?.Trim('0')?.Length > 0)
        allTags["trace_id"] = traceId;
      WriteStructuredLog(command, timestamp, message, allTags);
    }

    public void ProcessLog(int level, DateTime timestamp, string formattedMessage, IEnumerable<KeyValuePair<string, object>> tags, string spanId, string traceId)
    {
      var command = "INFO";
      if (level >= 1 && level <= 4)
        command = "TRACE";
      else if (level >= 5 && level <= 8)
        command = "DEBUG";
      else if (level >= 9 && level <= 12)
        command = "INFO";
      else if (level >= 13 && level <= 16)
        command = "WARN";
      else if (level >= 17 && level <= 20)
        command = "ERROR";
      else if (level > 20)
        command = "FATAL";

      var allTags = tags
            .GroupBy(t => t.Key)
            .ToDictionary(g => g.Key, g => g.First().Value);
      if (spanId?.Trim('0')?.Length > 0)
      {
        if (_starts.Count < 1 || _starts.Last().SpanId != spanId)
        {
          allTags["span_id"] = spanId;
          if (!string.IsNullOrEmpty(traceId))
            allTags["trace_id"] = traceId;
        }
      }

      WriteStructuredLog(command, timestamp, formattedMessage, allTags);
    }

    private void WriteStructuredLog(string command, DateTime timestamp, string message, IEnumerable<KeyValuePair<string, object>> tags)
    {
      lock (_lock)
      {
        var messageWritten = false;
        if (UseConsoleColors)
          Console.ForegroundColor = ConsoleColor.DarkGray;
        _writer.Write("##");
        
        _writer.OnlyAllowBreakAfter = ',';
        using (var json = new JsonTextWriter(_writer)
        {
          CloseOutput = false
        })
        {
          json.WriteStartArray();

          command = command.ToUpperInvariant();
          if (UseConsoleColors)
          {
            json.Flush();
            switch (command)
            {
              case "INFO":
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
              case "WARN":
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
              case "ERROR":
              case "FATAL":
                Console.ForegroundColor = ConsoleColor.Red;
                break;
              case "START":
              case "END":
                Console.ForegroundColor = ConsoleColor.Green;
                break;
              default:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
            }
          }
          json.WriteValue(command);

          if (UseConsoleColors)
          {
            json.Flush();
            Console.ForegroundColor = ConsoleColor.DarkGray;
          }
          var dateFormat = _dateWritten ? "HH:mm:ss.ffffff" : "yyyy-MM-ddTHH:mm:ss.ffffff";
          _dateWritten = true;
          json.WriteValue(timestamp.ToString(dateFormat));

          if (tags.Any())
          {
            if (!string.IsNullOrEmpty(message))
            {
              messageWritten = true;
              if (UseConsoleColors)
              {
                json.Flush();
                Console.ResetColor();
              }
              json.WriteValue(message);
              if (UseConsoleColors)
              {
                json.Flush();
                Console.ForegroundColor = ConsoleColor.DarkGray;
              }
            }
            json.WriteStartObject();
            foreach (var tag in tags)
            {
              json.WritePropertyName(tag.Key);
              _serializer.Serialize(json, tag.Value);
            }
            json.WriteEndObject();
          }
        }
        _writer.OnlyAllowBreakAfter = '\0';

        if (!messageWritten && !string.IsNullOrEmpty(message))
        {
          if (UseConsoleColors)
            Console.ResetColor();
          _writer.WriteSpace(1);
          _writer.Write(message);
        }

        if (UseConsoleColors)
          Console.ForegroundColor = ConsoleColor.DarkGray;
        _writer.WriteLine("##");
        if (UseConsoleColors)
          Console.ResetColor();
        _writer.Flush();
      }
    }

    private class StartActivity
    {
      public string SpanId { get; }

      public HashSet<string> Tags { get; }

      public StartActivity(string spanId, IEnumerable<KeyValuePair<string, object>> tags)
      {
        SpanId = spanId;
        Tags = new HashSet<string>(tags.Select(k => k.Key));
      }
    }
  }
}
