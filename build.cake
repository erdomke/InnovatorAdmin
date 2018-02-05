#addin "Cake.FileHelpers"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configs = new string[] { "Release" };
string version;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
  .Does(() =>
{
  var buildDir = Directory("./public/InnovatorAdmin/lib/");
  CleanDirectory(buildDir);
});

Task("Patch-Version")
  .IsDependentOn("Clean")
  .Does(() =>
{
  version = DateTime.Now.ToString("yyyy.MM.dd.HHmm");
  var content = string.Format(@"using System.Reflection;

[assembly: AssemblyVersion(""{0}"")]
[assembly: AssemblyFileVersion(""{0}"")]", version);
  FileWriteText("./InnovatorAdmin/AssemblyInfo.Version.cs", content);
});

Task("Restore-NuGet-Packages")
  .IsDependentOn("Patch-Version")
  .Does(() =>
{
  NuGetRestore("./InnovatorAdmin.sln");
});

Task("Build")
  .IsDependentOn("Restore-NuGet-Packages")
  .Does(() =>
{
  foreach (var config in configs)
  {
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./InnovatorAdmin.sln", settings =>
        settings.SetConfiguration(config));
    }
    else
    {
      // Use XBuild
      XBuild("./InnovatorAdmin.sln", settings =>
        settings.SetConfiguration(config));
    }
  }
});

Task("NuGet-Pack")
  .IsDependentOn("Build")
  .Does(() =>
{
  DeleteFiles("./artifacts/*.nupkg");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/Squirrel*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/Splat*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/ObjectListView*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/NuGet*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/Nancy*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/Mvp.Xml*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/Mono.Cecil*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/Microsoft.*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/LibGit*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/DeltaCompression*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/ICSharpCode.SharpZipLib*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/Innovator.Client*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/Innovator.Client*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/SharpCompress*");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/*.xml");
  DeleteDirectory("./publish/InnovatorAdmin/lib/net45/lib",true);
  var nuGetPackSettings = new NuGetPackSettings {
    Id = "InnovatorAdmin",
    Version = version,
    Authors = new [] {"eric.domke"},
    Description = "A tool for managing Aras Innovator installations focusing on improving the import/export experience.",
    RequireLicenseAcceptance = false,
    OutputDirectory = "./artifacts"
  };
  NuGetPack("./publish/InnovatorAdmin/InnovatorAdmin.nuspec", nuGetPackSettings);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("NuGet-Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
