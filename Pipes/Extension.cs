using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pipes
{
  public static class Extension
  {

    public static void Exec(this IWriter writer)
    {
      var processor = writer.Parent as IProcessor;
      if (processor == null) throw new InvalidOperationException();
      processor.Execute();
    }
    public static void ExecToFile(this IWriter<TextWriter> writer, string path)
    {
      using (var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
      {
        using (var w = new System.IO.StreamWriter(stream))
        {
          var processor = writer.Parent as IProcessor;
          if (processor == null) throw new InvalidOperationException();
          writer.Initialize(w);
          processor.Execute();
        }
      }
    }
    public static void ExecToFile(this IWriter<Stream> writer, string path)
    {
      using (var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
      {
        var processor = writer.Parent as IProcessor;
        if (processor == null) throw new InvalidOperationException();
        writer.Initialize(stream);
        processor.Execute();
      }
    }
    public static string ExecToString(this IWriter<TextWriter> writer)
    {
      using (var w = new StringWriter())
      {
        var processor = writer.Parent as IProcessor;
        if (processor == null) throw new InvalidOperationException();
        writer.Initialize(w);
        processor.Execute();
        return w.ToString();
      }
    }
    public static void ExecToStream(this IWriter<TextWriter> writer, Stream stream)
    {
      using (var w = new System.IO.StreamWriter(stream))
      {
        var processor = writer.Parent as IProcessor;
        if (processor == null) throw new InvalidOperationException();
        writer.Initialize(w);
        processor.Execute();
      }
    }
    public static void ExecToStream(this IWriter<Stream> writer, Stream stream)
    {
      var processor = writer.Parent as IProcessor;
      if (processor == null) throw new InvalidOperationException();
      writer.Initialize(stream);
      processor.Execute();
    }

    public static S Pipe<S>(this string input, S output) where S : IPipeInput<System.IO.TextReader>
    {
      output.Initialize(new IO.StringTextSource(input));
      return output;
    }
    public static S FileInput<S>(this string path, S output) where S : IPipeInput<System.IO.TextReader>
    {
      output.Initialize(new IO.FileTextSource(path));
      return output;
    }
    public static S Pipe<T, S>(this IEnumerable<T> input, S output) where S : IPipeInput<T>
    {
      output.Initialize(input);
      return output;
    }
    public static S Pipe<T, S, C>(this IEnumerable<T> input, S output, C settings) where S : IPipeInput<T>, IConfigurable<C>
    {
      output.InitializeSettings(settings);
      output.Initialize(input);
      return output;
    }
    public static S Pipe<T, S>(this IWriter<T> input, S output) where S : T
    {
      var writer = output as IWriter;
      if (writer != null) writer.Parent = input.Parent ?? input;
      return input.Initialize(output);
    }
    public static S Pipe<T, S, C>(this IWriter<T> input, S output, C settings) where S : T, IWriter, IConfigurable<C>
    {
      output.InitializeSettings(settings);
      output.Parent = input.Parent ?? input;
      return input.Initialize(output);
    }
    public static S PipeExec<T, S>(this IWriter<T> input, S output) where S : T
    {
      input.Initialize(output);
      var proc = (input.Parent ?? input) as IProcessor;
      if (proc == null) throw new InvalidOperationException("No processor found");
      proc.Execute();
      return output;
    }
    public static S PipeExec<T, S, C>(this IWriter<T> input, S output, C settings) where S : T, IConfigurable<C>
    {
      output.InitializeSettings(settings);
      input.Initialize(output);
      var proc = (input.Parent ?? input) as IProcessor;
      if (proc == null) throw new InvalidOperationException("No processor found");
      proc.Execute();
      return output;
    }
    public static S ProcessAny<T, S>(this T data, S processor) where S : IProcessor<T>
    {
      processor.InitializeData(data);
      return processor;
    }
    public static S ProcessAny<T, S, C>(this T data, S processor, C settings) where S : IProcessor<T>, IConfigurable<C>
    {
      processor.InitializeData(data);
      processor.InitializeSettings(settings);
      return processor;
    }
    public static S ProcessInput<T, S>(this IPipeOutput<T> input, S processor) where S : IProcessor<IEnumerable<T>>
    {
      processor.InitializeData(input);
      return processor;
    }
    public static S ProcessInput<T, S, C>(this IPipeOutput<T> input, S processor, C settings) where S : IProcessor<IEnumerable<T>>, IConfigurable<C>
    {
      processor.InitializeData(input);
      processor.InitializeSettings(settings);
      return processor;
    }
    

    public static Data.Table.IReport ToReport(this DataGridView grid, string name)
    {
      return new Data.Reports.DataGridViewReport(grid, name);
    }
    public static Data.Table.IReport ToReport(this ListView grid, string name)
    {
      return new Data.Reports.ListViewReport(grid, name);
    }
    public static Data.Table.IReport ToReport(this DataTable grid, string name)
    {
      return new Data.Reports.DataTableReport(grid, name);
    }
    public static IWriter<Stream> ToStreamWriter(this IWriter<Stream> input)
    {
      return input;
    }
    public static IWriter<Stream> ToStreamWriter(this IWriter<TextWriter> input)
    {
      return new StreamWriter(input);
    }
    public static Data.Table.IFormattedTable ToTable(this DataTable grid)
    {
      return new Data.Reports.DataTableTable(grid);
    }

    private class StreamWriter : IWriter<Stream>
    {
      private IWriter<TextWriter> _writer;

      public StreamWriter(IWriter<TextWriter> writer)
      {
        _writer = writer;
        this.Parent = writer.Parent;
      }

      public T Initialize<T>(T coreWriter) where T : Stream
      {
        _writer.Initialize(new System.IO.StreamWriter(coreWriter));
        return coreWriter;
      }

      public object Parent { get; set; }
    }

    internal static bool IsEqual(object x, object y)
    {
      if (x == null && y == null)
      {
        return true;
      }
      else if (x == null || y == null)
      {
        return false;
      }
      else
      {
        return x.Equals(y);
      }
    }

    internal static bool IsEqualIgnoreCase(this string x, string y)
    {
      if (x == null && y == null)
      {
        return true;
      }
      else if (x == null || y == null)
      {
        return false;
      }
      else
      {
        return string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
      }
    }
    
    internal static string NullToString(this object obj)
    {
      if (obj == null)
      {
        return null;
      }
      else
      {
        return obj.ToString();
      }
    }
  }
}
