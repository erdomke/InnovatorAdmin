using System;
using InnovatorAdmin.Connections;
using RoslynPad.Host;
using RoslynPad.Roslyn;
using RoslynPad.Utilities;
using System.Threading.Tasks;

namespace RoslynPad
{
  internal interface IParentViewModel
  {
    DocumentViewModel AddDocument(string path);
    Task AutoSaveOpenDocuments();
    ChildProcessManager ChildProcessManager { get; }
    ConnectionData ConnData { get; }
    MethodViewModel CurrentOpenDocument { get; set; }
    double EditorFontSize { get; set; }
    event Action<double> EditorFontSizeChanged;
    bool HasUpdate { get; }
    double MaximumEditorFontSize { get; }
    double MinimumEditorFontSize { get; }
    NuGetViewModel NuGet { get; }
    NuGetConfiguration NuGetConfiguration { get; }
    void OpenDocument(DocumentViewModel document);
    RoslynHost RoslynHost { get; }
    string WindowTitle { get; }
  }
}
