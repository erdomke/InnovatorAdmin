//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = string.Format("{0:yy}.{1:d3}.{2}"
    , DateTime.UtcNow
    , DateTime.UtcNow.DayOfYear
    , (int)((DateTime.UtcNow - DateTime.UtcNow.Date).TotalSeconds / 2));

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
  .Does(() =>
{
  CleanDirectory(Directory("./InnovatorAdmin/bin/"));
  CleanDirectory(Directory("./InnovatorAdmin.Api/bin/"));
  CleanDirectory(Directory("./InnovatorAdmin.Cmd/bin/"));
  CleanDirectory(Directory("./artifacts/"));
});

Task("Publish")
  .IsDependentOn("Clean")
  .Does(() =>
{
  var msBuild = new DotNetMSBuildSettings();
  msBuild.Properties["Version"] = new string[] { version };
  var deleteSettings = new DeleteDirectorySettings {
    Recursive = true,
    Force = true
  };

  DotNetPublish("./InnovatorAdmin/InnovatorAdmin.csproj", new DotNetPublishSettings
  {
    Configuration = configuration,
    OutputDirectory = "./publish/",
    SelfContained = true,
    PublishSingleFile = false,
    PublishReadyToRun = false,
    PublishTrimmed = false,
    MSBuildSettings = msBuild,
    Verbosity = DotNetVerbosity.Detailed
  });
  Zip("./publish/", $"./artifacts/InnovatorAdmin.{version}.zip");
  DeleteDirectory("./publish", deleteSettings);

  DotNetPublish("./InnovatorAdmin.Cmd/InnovatorAdmin.Cmd.csproj", new DotNetPublishSettings
  {
    Configuration = configuration,
    OutputDirectory = "./publish/",
    SelfContained = true,
    PublishSingleFile = true,
    PublishReadyToRun = false,
    PublishTrimmed = false,
    MSBuildSettings = msBuild
  });
  Zip("./publish/", $"./artifacts/InnovatorAdmin.Cmd.{version}.zip");
  DeleteDirectory("./publish", deleteSettings);

  DotNetPack("./InnovatorAdmin.Api/InnovatorAdmin.Api.csproj", new DotNetPackSettings
  {
    Configuration = configuration,
    OutputDirectory = "./artifacts/",
    MSBuildSettings = msBuild
  });

  Information("Version: " + version);
});

Task("Test")
  .IsDependentOn("Publish")
  .Does(() =>
{
  DotNetTest("./InnovatorAdmin.ApiTests/InnovatorAdmin.ApiTests.csproj", new DotNetTestSettings
  {
      Configuration = configuration
  });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
  .IsDependentOn("Test");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
