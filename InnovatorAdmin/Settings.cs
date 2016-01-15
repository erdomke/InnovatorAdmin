using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class Settings
  {
    [DisplayName("Diff Tool"), ParamControl(typeof(Editor.FilePathControl), "Base|Compare")]
    public string DiffToolCommand { get; set; }
    [DisplayName("Merge Tool"), ParamControl(typeof(Editor.FilePathControl), "Base|Local|Remote")]
    public string MergeToolCommand { get; set; }

    public async Task PerformDiff(string baseName, ITextSource baseSource
      , string compareName, ITextSource compareSource)
    {
      while (this.DiffToolCommand.IndexOf("$(base)", StringComparison.OrdinalIgnoreCase) < 0
        || this.DiffToolCommand.IndexOf("$(compare)", StringComparison.OrdinalIgnoreCase) < 0)
      {
        using (var dialog = new Dialog.SettingsDialog())
        {
          dialog.Filter.Add(s => s.DiffToolCommand);
          dialog.Settings = this;
          dialog.Message = string.IsNullOrWhiteSpace(this.DiffToolCommand)
            ? "Please specify a diff command. Use the macros '$(base)' and '$(compare)' to specify where these paths should be placed."
            : "The macros '$(base)' and/or '$(compare)' are missing from the diff command; please add them";
          if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            return;
          this.DiffToolCommand = Properties.Settings.Default.DiffToolCommand;
        }
      }

      var basePath = await WriteTempFile(baseName, baseSource);
      var comparePath = await WriteTempFile(compareName, compareSource);

      var cmd = GetFilePathAndArguments(this.DiffToolCommand
        .Replace("$(base)", "\"" + basePath + "\"", StringComparison.OrdinalIgnoreCase)
        .Replace("$(compare)", "\"" + comparePath + "\"", StringComparison.OrdinalIgnoreCase));
      Process.Start(cmd.Item1, cmd.Item2);
    }

    private Tuple<string, string> GetFilePathAndArguments(string path)
    {
      path = path.Trim();
      if (path.StartsWith("\""))
      {
        var idx = path.IndexOf('"', 1);
        if (idx < 1)
          return Tuple.Create(path, string.Empty);
        else
          return Tuple.Create(path.Substring(1, idx - 1), path.Substring(idx + 1).Trim());
      }
      else
      {
        var idx = path.IndexOf(' ');
        while (idx >= 0 && !File.Exists(path.Substring(0, idx)))
          idx = path.IndexOf(' ', idx + 1);

        if (idx < 1)
          return Tuple.Create(path, string.Empty);
        else
          return Tuple.Create(path.Substring(0, idx), path.Substring(idx + 1).Trim());
      }
    }

    private async Task<string> WriteTempFile(string name, ITextSource data)
    {
      var path = Path.Combine(Path.GetTempPath(), SnippetManager.CleanFileName(name) + ".txt");
      using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
      using (var writer = new StreamWriter(stream))
      using (var reader = data.CreateReader())
      {
        await reader.CopyToAsync(writer);
      }
      return path;
    }

    public void Save()
    {
      Properties.Settings.Default.DiffToolCommand = this.DiffToolCommand;
      Properties.Settings.Default.MergeToolCommand = this.MergeToolCommand;
      Properties.Settings.Default.Save();
    }

    public static Settings Current
    {
      get
      {
        return new Settings()
        {
          DiffToolCommand = Properties.Settings.Default.DiffToolCommand,
          MergeToolCommand = Properties.Settings.Default.MergeToolCommand
        };
      }
    }
  }
}
