using CommandLine;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  public class ConsoleTask
  {
    private Stopwatch _st = Stopwatch.StartNew();
    private bool _lineStart = true;

    public TimeSpan Elapsed { get { return _st.Elapsed; } }
    public TextWriter LogWriter { get; set; } = Console.Out;

    private ConsoleTask() { }

    public Stream ReadInput()
    {
      var result = new MemoryStream();
      if (Console.IsInputRedirected)
      {
        using (Stream stream = Console.OpenStandardInput())
        {
          var buffer = new byte[4096];  // Use whatever size you want
          var read = -1;
          while (true)
          {
            AutoResetEvent gotInput = new AutoResetEvent(false);
            Thread inputThread = new Thread(() =>
            {
              try
              {
                read = stream.Read(buffer, 0, buffer.Length);
                gotInput.Set();
              }
              catch (ThreadAbortException)
              {
                Thread.ResetAbort();
              }
            })
            {
              IsBackground = true
            };

            inputThread.Start();

            // Timeout expired?
            if (!gotInput.WaitOne(100))
            {
              inputThread.Abort();
              break;
            }

            // End of stream?
            if (read == 0)
              break;

            // Got data
            result.Write(buffer, 0, read);
          }
        }
      }
      else
      {
        var builder = new StringBuilder();
        while (true)
        {
          var key = Console.ReadKey();
          if (key.KeyChar == '\u001a' || key.KeyChar == '\u001c')
            break;
          if (key.KeyChar == '\b')
            builder.Length--;
          else
            builder.Append(key.KeyChar);
        }
        var bytes = Encoding.UTF8.GetBytes(builder.ToString());
        result.Write(bytes, 0, bytes.Length);
      }

      result.Position = 0;
      return result;
    }

    public ProgressBar Progress()
    {
      return new ProgressBar();
    }

    public ConsoleTask Write(string value)
    {
      if (!string.IsNullOrEmpty(value))
      {
        WritePrefix().Write(value);
        _lineStart = value.EndsWith("\r") || value.EndsWith("\n");
      }
      return this;
    }

    public ConsoleTask Write(string format, params object[] args)
    {
      if (!string.IsNullOrEmpty(format))
      {
        WritePrefix().Write(format, args);
        _lineStart = format.EndsWith("\r") || format.EndsWith("\n");
      }
      return this;
    }

    public ConsoleTask WriteLine()
    {
      LogWriter?.WriteLine();
      _lineStart = true;
      return this;
    }

    public ConsoleTask WriteLine(string value)
    {
      WritePrefix()?.WriteLine(value);
      _lineStart = true;
      return this;
    }

    public ConsoleTask WriteLine(string format, params object[] args)
    {
      WritePrefix()?.WriteLine(format, args);
      _lineStart = true;
      return this;
    }

    private TextWriter WritePrefix()
    {
      if (_lineStart)
      {
        LogWriter?.Write(_st.Elapsed.ToString(@"hh\:mm\:ss"));
        LogWriter?.Write(" ");
        _lineStart = false;
      }
      return LogWriter;
    }

    public static int Execute<T>(T options, Action<ConsoleTask> func)
    {
      return ExecuteAsync(options, c =>
      {
        func(c);
        return Task.FromResult(true);
      }).Result;
    }

    public async static Task<int> ExecuteAsync<T>(T options, Func<ConsoleTask, Task> func, Action<ConsoleTask> config = null)
    {
      var task = new ConsoleTask();
      config?.Invoke(task);
      var verb = options.GetType().GetCustomAttributes(typeof(VerbAttribute), true)
          .OfType<VerbAttribute>()
          .FirstOrDefault()
          ?.Name ?? "Task";
      verb = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(verb);
      try
      {
        task.LogWriter?.WriteLine(Parser.Default.FormatCommandLine(options));
        task.LogWriter?.WriteLine();

        await func.Invoke(task);

        task.WriteLine();
        task.WriteLine(verb + " succeeded");
        return 0;
      }
      catch (Exception ex)
      {
        var flat = ex;
        if (ex is AggregateException agg)
          flat = agg.Flatten();

        task.LogWriter?.WriteLine();
        task.LogWriter?.WriteLine();

        var errorOutput = task.LogWriter;
        if (errorOutput == Console.Out)
          errorOutput = Console.Error;
        errorOutput?.WriteLine(@"{0:hh\:mm\:ss} {1} failed.", task.Elapsed, verb);
        errorOutput?.WriteLine();
        errorOutput?.WriteLine(flat.ToString());

        var serverEx = ex as ServerException;
        if (serverEx != null)
        {
          if (!string.IsNullOrEmpty(serverEx.Database))
          {
            errorOutput?.WriteLine();
            errorOutput?.WriteLine("DATABASE: " + serverEx.Database);
          }
          if (!string.IsNullOrEmpty(serverEx.Query))
          {
            errorOutput?.WriteLine();
            errorOutput?.WriteLine("QUERY: " + serverEx.Query);
          }
        }
        return -1;
      }
      finally
      {
        if (task.LogWriter != null && task.LogWriter != Console.Out)
          task.LogWriter.Dispose();
      }
    }
  }
}
