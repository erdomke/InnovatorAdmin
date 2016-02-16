//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using Pipes.Sgml;

//namespace Pipes.IO
//{
//  public static class WriterExtensions
//  {
//    /// <summary>
//    /// Get a writer which writes the input to a <see cref="Stream"/>
//    /// </summary>
//    public static IProcessorSettings<Tin, Stream, Tsettings> GetStreamWriter<Tin, Tsettings>(this IProcessorSettings<Tin, ISgmlWriter, Tsettings> writer)
//    {
//      return new SgmlStreamWriter<Tin, Tsettings>(writer);
//    }
//    /// <summary>
//    /// Get a writer which writes the input to a <see cref="Stream"/>
//    /// </summary>
//    public static IProcessor<Tin, Stream> GetStreamWriter<Tin>(this IProcessor<Tin, ISgmlWriter> writer)
//    {
//      return new SgmlStreamWriter<Tin, object>(writer);
//    }

//    private class SgmlStreamWriter<Tin, Tsettings> : IProcessorSettings<Tin, Stream, Tsettings>
//    {
//      private IProcessor<Tin, ISgmlWriter> _sgmlWriter;

//      public SgmlStreamWriter(IProcessor<Tin, ISgmlWriter> sgmlWriter)
//      {
//        _sgmlWriter = sgmlWriter;
//      }

//      public void Initialize(Tin data, Stream writer, Tsettings settings)
//      {
//        using (var streamWriter = new System.IO.StreamWriter(writer, System.Text.Encoding.UTF8))
//        {
//          using (var htmlWriter = Pipes.Sgml.HtmlTextWriter.Create(streamWriter))
//          {
//            var withSettings = _sgmlWriter as IProcessorSettings<Tin, ISgmlWriter, Tsettings>;
//            if (settings == null || settings.Equals(default(Tsettings)) || withSettings == null)
//            {
//              _sgmlWriter.Initialize(data, htmlWriter);
//            }
//            else
//            {
//              withSettings.Initialize(data, htmlWriter, settings);
//            }
//          }
//        }
//      }

//      public void Initialize(Tin data, Stream writer)
//      {
//        Initialize(data, writer, default(Tsettings));
//      }
//    }

//    /// <summary>
//    /// Get a writer which writes the input to a <see cref="TextWriter"/>
//    /// </summary>
//    public static IProcessorSettings<Tin, TextWriter, Tsettings> GetTextWriter<Tin, Tsettings>(IProcessorSettings<Tin, ISgmlWriter, Tsettings> writer)
//    {
//      return new SgmlTextWriter<Tin, Tsettings>(writer);
//    }
//    /// <summary>
//    /// Get a writer which writes the input to a <see cref="TextWriter"/>
//    /// </summary>
//    public static IProcessor<Tin, TextWriter> GetTextWriter<Tin>(IProcessor<Tin, ISgmlWriter> writer)
//    {
//      return new SgmlTextWriter<Tin, object>(writer);
//    }

//    private class SgmlTextWriter<Tin, Tsettings> : IProcessorSettings<Tin, TextWriter, Tsettings>
//    {
//      private IProcessor<Tin, ISgmlWriter> _sgmlWriter;

//      public SgmlTextWriter(IProcessor<Tin, ISgmlWriter> sgmlWriter)
//      {
//        _sgmlWriter = sgmlWriter;
//      }

//      public void Initialize(Tin data, TextWriter writer, Tsettings settings)
//      {
//        using (var htmlWriter = Pipes.Sgml.HtmlTextWriter.Create(writer))
//        {
//          var withSettings = _sgmlWriter as IProcessorSettings<Tin, ISgmlWriter, Tsettings>;
//          if (settings == null || settings.Equals(default(Tsettings)) || withSettings == null)
//          {
//            _sgmlWriter.Initialize(data, htmlWriter);
//          }
//          else
//          {
//            withSettings.Initialize(data, htmlWriter, settings);
//          }
//        }
//      }

//      public void Initialize(Tin data, TextWriter writer)
//      {
//        Initialize(data, writer, default(Tsettings));
//      }
//    }

//    /// <summary>
//    /// Get a writer which writes the input to a <see cref="Stream"/>
//    /// </summary>
//    public static IProcessorSettings<Tin, Stream, Tsettings> GetStreamWriter<Tin, Tsettings>(this IProcessorSettings<Tin, TextWriter, Tsettings> writer)
//    {
//      return new TextStreamWriter<Tin, Tsettings>(writer);
//    }
//    /// <summary>
//    /// Get a writer which writes the input to a <see cref="Stream"/>
//    /// </summary>
//    public static IProcessor<Tin, Stream> GetStreamWriter<Tin>(this IProcessor<Tin, TextWriter> writer)
//    {
//      return new TextStreamWriter<Tin, object>(writer);
//    }

//    private class TextStreamWriter<Tin, Tsettings> : IProcessorSettings<Tin, Stream, Tsettings>
//    {
//      private IProcessor<Tin, TextWriter> _sgmlWriter;

//      public TextStreamWriter(IProcessor<Tin, TextWriter> sgmlWriter)
//      {
//        _sgmlWriter = sgmlWriter;
//      }

//      public void Initialize(Tin data, Stream writer, Tsettings settings)
//      {
//        using (var streamWriter = new System.IO.StreamWriter(writer, System.Text.Encoding.UTF8))
//        {
//          var withSettings = _sgmlWriter as IProcessorSettings<Tin, TextWriter, Tsettings>;
//          if (settings == null || settings.Equals(default(Tsettings)) || withSettings == null)
//          {
//            _sgmlWriter.Initialize(data, streamWriter);
//          }
//          else
//          {
//            withSettings.Initialize(data, streamWriter, settings);
//          }
//        }
//      }

//      public void Initialize(Tin data, Stream writer)
//      {
//        Initialize(data, writer, default(Tsettings));
//      }
//    }

//    /// <summary>
//    /// Writes the input data to a file
//    /// </summary>
//    public static void WriteToFile<Tin, Tsettings>(this IProcessorSettings<Tin, Stream, Tsettings> writer, Tin data, string path, Tsettings settings)
//    {
//      using (var file = new FileStream(path, FileMode.Create, FileAccess.Write))
//      {
//        writer.Initialize(data, file, settings);
//      }
//    }
//    /// <summary>
//    /// Writes the input data to a file
//    /// </summary>
//    public static void WriteToFile<Tin>(this IProcessor<Tin, Stream> writer, Tin data, string path)
//    {
//      using (var file = new FileStream(path, FileMode.Create, FileAccess.Write))
//      {
//        writer.Initialize(data, file);
//      }
//    }
//    /// <summary>
//    /// Writes the input data to a string
//    /// </summary>
//    public static string WriteToString<Tin, Tsettings>(this IProcessorSettings<Tin, TextWriter, Tsettings> writer, Tin data, Tsettings settings)
//    {
//      using (var stringWriter = new System.IO.StringWriter())
//      {
//        writer.Initialize(data, stringWriter, settings);
//        stringWriter.Flush();
//        return stringWriter.ToString();
//      }
//    }
//    /// <summary>
//    /// Writes the input data to a string
//    /// </summary>
//    public static string WriteToString<Tin>(this IProcessor<Tin, TextWriter> writer, Tin data)
//    {
//      using (var stringWriter = new System.IO.StringWriter())
//      {
//        writer.Initialize(data, stringWriter);
//        stringWriter.Flush();
//        return stringWriter.ToString();
//      }
//    }
//  }
//}
