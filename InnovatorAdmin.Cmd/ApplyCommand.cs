using CommandLine;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin.Cmd
{
  [Verb("apply", HelpText = "Apply AML")]
  internal class ApplyCommand : SharedOptions
  {
    [Option('a', "action", HelpText = "SOAP action to use")]
    public string SoapAction { get; set; }

    [Option('o', "outputfile", HelpText = "Path to the output XML file")]
    public string Output { get; set; }

    [Option('g', "log", HelpText = "Path to the log file")]
    public string LogFile { get; set; }

    [Option('t', "timeout", HelpText = "Timeout for the HTTP request in milliseconds")]
    public int Timeout { get; set; }

    public Task<int> Execute()
    {
      return ConsoleTask.ExecuteAsync(this, async (console) =>
      {
        console.WriteLine("Connecting to innovator...");
        var conn = await GetConnection().ConfigureAwait(false);

        console.WriteLine("Reading input...");
        var stream = string.IsNullOrEmpty(InputFile) ? console.ReadInput() : new FileStream(InputFile, FileMode.Open, FileAccess.Read);
        var cmd = default(Command);
        if (string.IsNullOrEmpty(SoapAction))
        {
          var input = conn.AmlContext.FromXml(stream);
          cmd = new Command(input);
          if (input.Items().Count() > 1)
            cmd.WithAction(CommandAction.ApplyAML);
        }
        else
        {
          cmd = new Command(stream.AsString());
        }

        console.WriteLine("Calling action...");
        var result = (await conn.ApplyAsync(cmd, true, false).ConfigureAwait(false)).AssertNoError();

        console.WriteLine("Writing output...");
        using (var output = string.IsNullOrEmpty(Output) ? Console.Out : new StreamWriter(Output))
        using (var xml = XmlWriter.Create(output))
        {
          result.ToAml(xml);
        }
      }, console =>
      {
        if (string.IsNullOrEmpty(Output))
          console.LogWriter = string.IsNullOrEmpty(LogFile) ? null : new StreamWriter(LogFile);
      });
    }
  }
}
