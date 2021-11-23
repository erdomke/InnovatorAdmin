#tool "Squirrel.Windows" 
#addin Cake.Squirrel
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
  CleanDirectory("./publish/InnovatorAdmin/lib/");
  CleanDirectory("./publish/InnovatorAdmin.Cmd/tools/");
});

Task("Patch-Version")
  .IsDependentOn("Clean")
  .Does(() =>
{
  version = DateTime.Now.ToString("yyyy.MM.dd.HHmm");
  
  if (!string.Equals(target, "Default", StringComparison.OrdinalIgnoreCase))
  {
    var now = DateTime.UtcNow;
    version = string.Format("{0:yy}.{1:d3}.{2}"
, now
, now.DayOfYear
, (int)((DateTime.UtcNow - DateTime.UtcNow.Date).TotalSeconds / 2));  
  }
  Information("Version: " + version);
  
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
    DotNetCoreBuild("./InnovatorAdmin.sln", new DotNetCoreBuildSettings
    {
      Configuration = config,
    });
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
  DeleteDirectory("./publish/InnovatorAdmin/lib/net45/lib", new DeleteDirectorySettings {
    Recursive = true,
    Force = true
  });
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

Task("Release-NuGet-Pack")
  .IsDependentOn("Build")
  .Does(() =>
{
  DeleteFiles("./artifacts/*.nupkg");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/*.xml");
  DeleteFiles("./publish/InnovatorAdmin/lib/net45/*.pdb");
  DeleteDirectory("./publish/InnovatorAdmin/lib/net45/lib", new DeleteDirectorySettings {
    Recursive = true,
    Force = true
  });
  var nuGetPackSettings = new NuGetPackSettings {
    Id = "InnovatorAdmin",
    Version = version,
    Authors = new [] {"eric.domke"},
    Description = "A tool for managing Aras Innovator installations focusing on improving the import/export experience.",
    RequireLicenseAcceptance = false,
    OutputDirectory = "./artifacts"
  };
  NuGetPack("./publish/InnovatorAdmin/InnovatorAdmin.Squirrel.nuspec", nuGetPackSettings);
});

Task("Squirrel-Release")
.IsDependentOn("Release-NuGet-Pack")
  .Does(() =>
{
  Information("Version: " + version);
  var file = GetFiles("./artifacts/InnovatorAdmin*.nupkg").First();
  Information(file);
  Squirrel(file, new SquirrelSettings() {
    NoMsi = true,
    PackagesDirectory = "./artifacts/",
    ReleaseDirectory = "./Releases/"
  });
});

Task("Cmd-Pack")
  .IsDependentOn("Build")
  .Does(() =>
{
  DeleteFiles("./artifacts/*.nupkg");
  DeleteFiles("./publish/InnovatorAdmin.Cmd/tools/*.pdb");
  DeleteFiles("./publish/InnovatorAdmin.Cmd/tools/*.xml");
  var nuGetPackSettings = new NuGetPackSettings {
    Id = "InnovatorAdmin.Cmd",
    Version = version,
    Authors = new [] {"eric.domke"},
    Description = "Command-line application for administrating an Aras Innovator instance.",
    RequireLicenseAcceptance = false,
    OutputDirectory = "./artifacts"
  };
  NuGetPack("./publish/InnovatorAdmin.Cmd/InnovatorAdmin.Cmd.nuspec", nuGetPackSettings);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("NuGet-Pack");
    
Task("Release")
    .IsDependentOn("Squirrel-Release");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
