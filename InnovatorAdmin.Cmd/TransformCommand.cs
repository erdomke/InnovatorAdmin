using CommandLine;
using Microsoft.Web.XmlTransform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  [Verb("transform", HelpText = "Transform an XML file using the XDT format")]
  internal class TransformCommand
  {
    [Option('f', "inputfile", HelpText = "Path to input XML file")]
    public string InputFile { get; set; }

    [Option('t', "transform", HelpText = "Path to transformation file")]
    public string Transform { get; set; }

    [Option('o', "outputfile", HelpText = "Path to the output XML file", Required = true)]
    public string Output { get; set; }

    public int Execute()
    {
      return ConsoleTask.Execute(this, console =>
      {
        if (string.IsNullOrWhiteSpace(InputFile))
          throw new ArgumentNullException(nameof(InputFile));
        if (string.IsNullOrWhiteSpace(Transform))
          throw new ArgumentNullException(nameof(Transform));
        if (string.IsNullOrWhiteSpace(Output))
          throw new ArgumentNullException(nameof(Output));

        if (!File.Exists(InputFile))
          throw new FileNotFoundException("Input file was not found.", InputFile);
        if (!File.Exists(Transform))
          throw new FileNotFoundException("Transform file was not found.", Transform);

        var logger = new Logger(console);
        using (var document = new XmlTransformableDocument())
        using (var transformation = new XmlTransformation(Transform, logger))
        {
          document.PreserveWhitespace = true;
          document.Load(InputFile);

          var success = transformation.Apply(document);

          if (success)
            document.Save(Output);
          else
            throw new AggregateException(logger.Exceptions);
        }
      });
    }

    private class Logger : IXmlTransformationLogger
    {
      private ConsoleTask _console;
      private List<Exception> _exceptions = new List<Exception>();

      public IList<Exception> Exceptions { get { return _exceptions; } }

      public Logger(ConsoleTask console)
      {
        _console = console;
      }

      public void EndSection(string message, params object[] messageArgs)
      {
        EndSection(MessageType.Normal, message, messageArgs);
      }

      public void EndSection(MessageType type, string message, params object[] messageArgs)
      {
        LogMessage(type, message, messageArgs);
      }

      public void LogError(string message, params object[] messageArgs)
      {
        LogError(null, -1, -1, message, messageArgs);
      }

      public void LogError(string file, string message, params object[] messageArgs)
      {
        LogError(file, -1, -1, message, messageArgs);
      }

      public void LogError(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
      {
        LogErrorFromException(new Exception(string.Format(message, messageArgs)), file, lineNumber, linePosition);
      }

      public void LogErrorFromException(Exception ex)
      {
        LogErrorFromException(ex, null, -1, -1);
      }

      public void LogErrorFromException(Exception ex, string file)
      {
        LogErrorFromException(ex, file, -1, -1);
      }

      public void LogErrorFromException(Exception ex, string file, int lineNumber, int linePosition)
      {
        _exceptions.Add(ex);
        _console.Write("ERROR ");
        WriteFile(file, lineNumber, linePosition);
        _console.WriteLine(ex.Message);
      }

      public void LogMessage(string message, params object[] messageArgs)
      {
        LogMessage(MessageType.Normal, message, messageArgs);
      }

      public void LogMessage(MessageType type, string message, params object[] messageArgs)
      {
        _console.Write("INFO ");
        _console.WriteLine(message, messageArgs);
      }

      public void LogWarning(string message, params object[] messageArgs)
      {
        LogWarning(null, -1, -1, message, messageArgs);
      }

      public void LogWarning(string file, string message, params object[] messageArgs)
      {
        LogWarning(file, -1, -1, message, messageArgs);
      }

      public void LogWarning(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
      {
        _console.Write("WARN ");
        WriteFile(file, lineNumber, linePosition);
        _console.WriteLine(message, messageArgs);
      }

      public void StartSection(string message, params object[] messageArgs)
      {
        StartSection(MessageType.Normal, message, messageArgs);
      }

      public void StartSection(MessageType type, string message, params object[] messageArgs)
      {
        LogMessage(type, message, messageArgs);
      }

      private void WriteFile(string file, int lineNumber, int linePosition)
      {
        if (!string.IsNullOrEmpty(file))
        {
          _console.Write(Path.GetFileName(file));
          if (lineNumber >= 0)
          {
            _console.Write("(" + lineNumber);
            if (linePosition >= 0)
              _console.Write("," + linePosition);
            _console.Write(")");
          }
          _console.Write(" ");
        }
      }
    }
  }
}
