using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin
{
  public static class SharedUtils
  {
    private static ActivitySource _activitySource = new ActivitySource("InnovatorAdmin");

    public static Activity StartActivity(string name, string displayName = null, IEnumerable<KeyValuePair<string, object>> tags = null)
    {
      var activity = _activitySource.CreateActivity(name, ActivityKind.Internal, default(ActivityContext), tags: tags);
      if (activity != null && !string.IsNullOrEmpty(displayName))
        activity.DisplayName = displayName;
      return activity?.Start();
    }

    public static void EnrichServerException(IDictionary<string, object> attributes, ServerException exception)
    {
      attributes["fault"] = TidyXml(exception.Fault.ToAml, false);
      attributes["query"] = TidyXml(exception.Query, false);
    }

    public static string TidyXml(string xml, bool indent)
    {
      if (string.IsNullOrEmpty(xml))
        return xml;

      using (var reader = new StringReader(xml))
      using (var writer = new StringWriter())
      {
        TidyXml(reader, writer, indent);
        return writer.ToString();
      }
    }

    public static string TidyXml(Action<XmlWriter> copyTo, bool indent)
    {
      using (var writer = new StringWriter())
      {
        TidyXml(copyTo, writer, indent);
        return writer.ToString();
      }
    }

    public static void TidyXml(TextReader reader, TextWriter writer, bool indent)
    {
      using (var xmlReader = XmlReader.Create(reader, new XmlReaderSettings
      {
        IgnoreWhitespace = true,
        ConformanceLevel = ConformanceLevel.Fragment
      }))
      {
        TidyXml(xmlWriter =>
        {
          while (!xmlReader.EOF)
          {
            xmlWriter.WriteNode(xmlReader, true);
          }
        }, writer, indent);
      }
    }

    private static void TidyXml(Action<XmlWriter> copyTo, TextWriter writer, bool indent)
    {
      using (var xmlWriter = new XmlTextWriter(writer)
      {
        QuoteChar = '\''
      })
      {
        if (indent)
        {
          xmlWriter.Indentation = 2;
          xmlWriter.IndentChar = ' ';
          xmlWriter.Formatting = Formatting.Indented;
        }
        else
        {
          xmlWriter.Formatting = Formatting.None;
        }
        copyTo(xmlWriter);
        xmlWriter.Flush();
      }
    }

    public static Task<IList<T>> TaskPool<T>(int maxConcurrent, params Func<Task<T>>[] taskFactory)
    {
      return TaskPool(maxConcurrent, null, taskFactory);
    }

    public static async Task<IList<T>> TaskPool<T>(int maxConcurrent, Action<int, string> progress, params Func<Task<T>>[] taskFactory)
    {
      var queue = new Queue<Func<Task<T>>>(Math.Max(taskFactory.Length - maxConcurrent, 0));
      var runningTasks = new List<Task<T>>(maxConcurrent);
      var results = new List<T>(taskFactory.Length);
      for (var i = 0; i < taskFactory.Length; i++)
      {
        if (i < maxConcurrent)
          runningTasks.Add(taskFactory[i].Invoke());
        else
          queue.Enqueue(taskFactory[i]);
      }

      while (runningTasks.Count > 0)
      {
        var task = await Task.WhenAny(runningTasks);
        runningTasks.Remove(task);
        results.Add(task.Result);
        progress?.Invoke(results.Count * 100 / taskFactory.Length, "");
        if (queue.Count > 0)
          runningTasks.Add(queue.Dequeue().Invoke());
      }

      return results;
    }

    public static void WriteTo(this Stream stream, string path)
    {
      using (var write = new FileStream(path, FileMode.Create, FileAccess.Write))
      {
        stream.CopyTo(write);
      }
    }
  }
}
