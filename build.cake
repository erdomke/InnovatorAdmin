#tool "Squirrel.Windows" 
#addin Cake.Squirrel
#addin nuget:?package=Cake.FileHelpers&version=4.0.1

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
  // TODO: Set version?
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
