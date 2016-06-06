using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.ApplicationInsights;
using RoslynPad.Host;
using RoslynPad.Roslyn;
using RoslynPad.Utilities;
using InnovatorAdmin;

namespace RoslynPad
{
  internal sealed class WindowViewModel : NotificationObject, IParentViewModel
  {
    private static readonly Version _currentVersion = new Version(0, 8);
    private static readonly string _currentVersionVariant = "";

    private const string ApplicationInsightsInstrumentationKey = "86551688-26d9-4124-8376-3f7ddcf84b8e";
    public const string NuGetPathVariableName = "$NuGet";

    private MethodViewModel _currentOpenDocument;
    private Exception _lastError;
    private bool _hasUpdate;
    private double _editorFontSize;

    public DocumentViewModel DocumentRoot { get; }
    public NuGetConfiguration NuGetConfiguration { get; }
    public RoslynHost RoslynHost { get; }
    public InnovatorAdmin.Connections.ConnectionData ConnData { get; private set; }

    public WindowViewModel()
    {
      var library = InnovatorAdmin.Connections.ConnectionLibrary.FromFile(Utils.GetConnectionFilePath());
      var args = Environment.GetCommandLineArgs();
      ConnData = library.Connections
        .Where(c => c.Type == InnovatorAdmin.Connections.ConnectionType.Innovator
          && (args.Length <= 1 || args[1] == c.ConnectionName))
        .FirstOrDefault();

      NuGet = new NuGetViewModel();
      NuGetConfiguration = new NuGetConfiguration(NuGet.GlobalPackageFolder, NuGetPathVariableName);
      RoslynHost = new RoslynHost(NuGetConfiguration, new[] { Assembly.Load("RoslynPad.RoslynEditor") });
      ChildProcessManager = new ChildProcessManager();

      CloseCurrentDocumentCommand = new DelegateCommand(CloseCurrentDocument);

      _editorFontSize = Properties.Settings.Default.EditorFontSize;

      if (HasCachedUpdate())
      {
        HasUpdate = true;
      }
      else
      {
        Task.Run(CheckForUpdates);
      }
    }

    public string WindowTitle
    {
      get
      {
        var title = "Method " + _currentVersion;
        if (!string.IsNullOrEmpty(_currentVersionVariant))
        {
          title += "-" + _currentVersionVariant;
        }
        return title;
      }
    }

    public bool HasUpdate
    {
      get { return _hasUpdate; }
      private set { SetProperty(ref _hasUpdate, value); }
    }

    private static bool HasCachedUpdate()
    {
      Version latestVersion;
      return Version.TryParse(Properties.Settings.Default.LatestVersion, out latestVersion) &&
             latestVersion > _currentVersion;
    }

    private async Task CheckForUpdates()
    {
      string latestVersionString;
      using (var client = new HttpClient())
      {
        try
        {
          latestVersionString = await client.GetStringAsync("https://roslynpad.net/latest").ConfigureAwait(false);
        }
        catch
        {
          return;
        }
      }
      Version latestVersion;
      if (Version.TryParse(latestVersionString, out latestVersion))
      {
        if (latestVersion > _currentVersion)
        {
          HasUpdate = true;
        }
        Properties.Settings.Default.LatestVersion = latestVersionString;
        Properties.Settings.Default.Save();
      }
    }

    public NuGetViewModel NuGet { get; }

    public MethodViewModel CurrentOpenDocument
    {
      get { return _currentOpenDocument; }
      set { SetProperty(ref _currentOpenDocument, value); }
    }

    public DelegateCommand CloseCurrentDocumentCommand { get; }

    public async Task CloseDocument(MethodViewModel document)
    {
      var result = await document.Save(promptSave: true).ConfigureAwait(true);
      if (result == SaveResult.Cancel)
      {
        return;
      }
      if (document.Document?.IsAutoSave == true)
      {
        File.Delete(document.Document.Path);
      }
      RoslynHost.CloseDocument(document.DocumentId);
      document.Close();
    }

    private async Task CloseCurrentDocument()
    {
      if (CurrentOpenDocument != null)
      {
        await CloseDocument(CurrentOpenDocument).ConfigureAwait(false);
      }
    }

    public bool SendTelemetry
    {
      get { return Properties.Settings.Default.SendErrors; }
      set
      {
        Properties.Settings.Default.SendErrors = value;
        Properties.Settings.Default.Save();
        OnPropertyChanged(nameof(SendTelemetry));
      }
    }

    public ChildProcessManager ChildProcessManager { get; }

    public double MinimumEditorFontSize => 8;
    public double MaximumEditorFontSize => 72;

    public double EditorFontSize
    {
      get { return _editorFontSize; }
      set
      {
        if (value < MinimumEditorFontSize || value > MaximumEditorFontSize) return;

        if (SetProperty(ref _editorFontSize, value))
        {
          Properties.Settings.Default.EditorFontSize = value;
          Properties.Settings.Default.Save();
          EditorFontSizeChanged?.Invoke(value);
        }
      }
    }

    public event Action<double> EditorFontSizeChanged;

    public DocumentViewModel AddDocument(string documentName)
    {
      return DocumentRoot.CreateNew(documentName);
    }

    public Task AutoSaveOpenDocuments()
    {
      return Task.Delay(1);
    }
    public void OpenDocument(DocumentViewModel document)
    {
      // Do nothing
    }
  }
}
