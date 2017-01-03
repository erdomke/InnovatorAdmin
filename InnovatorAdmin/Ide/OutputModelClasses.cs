using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class OutputModelClasses
  {
    private IAsyncConnection _conn;
    private string _baseDir = @"C:\Users\eric.domke\Documents\Models\";
    private string _companyName = "Gentex";

    public OutputModelClasses(IAsyncConnection conn)
    {
      _conn = conn;
    }

    public async Task Run()
    {
      var itemTypes = (await _conn.ApplyAsync(@"<Item type='ItemType' action='get' select='id,implementation_type,is_relationship,is_versionable'>
  <Relationships>
    <Item type='Property' action='get' select='name,data_source,data_type,foreign_property(data_type)'>
      <name condition='not in'>'classification','config_id','created_by_id','created_on','css','current_state','generation','is_current','is_released','itemtype','keyed_name','locked_by_id','major_rev','managed_by_id','minor_rev','modified_by_id','modified_on','new_version','not_lockable','owned_by_id','permission_id','state','team_id'</name>
    </Item>
    <Item type='Morphae' action='get' select='related_id' related_expand='0' />
  </Relationships>
</Item>", true, false).ToTask()).Items().OfType<Innovator.Client.Model.ItemType>();

      var defaultFactory = new Innovator.Client.DefaultItemFactory();
      itemTypes = itemTypes.Where(i => defaultFactory.NewItem(_conn.AmlContext, i.IdProp().KeyedName().Value) == null).ToArray();

      var dict = itemTypes.ToDictionary(i => i.Id());
      var polymorphicIds = new HashSet<string>();
      var links = new NameValueCollection();

      var files = new List<string>() { "AssemblyInfo.Version.cs" };
      var directories = new string[]
      {
        Path.Combine(_baseDir, "Innovator.Client." + _companyName),
        Path.Combine(_baseDir, "Innovator.Client." + _companyName + "/Properties")
      };

      foreach (var dir in directories)
      {
        if (!Directory.Exists(dir))
          Directory.CreateDirectory(dir);
      }


      foreach (var itemType in itemTypes.Where(i => i.ImplementationType().Value == "polymorphic"))
      {
        polymorphicIds.Add(itemType.Id());
        var itemTypeLabel = "I" + itemType.IdProp().KeyedName().Value.Replace(" ", "");
        files.Add(itemTypeLabel + ".cs");
        using (var writer = new StreamWriter(Path.Combine(_baseDir, "Innovator.Client." + _companyName + "/" + itemTypeLabel + ".cs")))
        {
          await writer.WriteAsync(@"using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>");
          await writer.WriteAsync("Interface for polymorphic item type " + itemType.IdProp().KeyedName().Value);
          await writer.WriteAsync(@" </summary>
  public interface ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(@" : IItem
  {
");
          var versionable = itemType.Relationships("Morphae").All(m => dict[m.RelatedId().Value].IsVersionable().AsBoolean(false));

          foreach (var prop in itemType
            .Relationships()
            .OfType<Innovator.Client.Model.Property>()
            .Where(p => p.NameProp().Value != "source_id" && p.NameProp().Value != "related_id" && p.NameProp().Value != "id"))
          {
            if (!versionable
              && (prop.NameProp().Value == "effective_date" || prop.NameProp().Value == "release_date" || prop.NameProp().Value == "superseded_date"))
              continue;

            await writer.WriteAsync("    /// <summary>Retrieve the <c>");
            await writer.WriteAsync(prop.NameProp().Value);
            await writer.WriteAsync("</c> property of the item</summary>\r\n");
            await writer.WriteAsync("    IProperty_");
            await writer.WriteAsync(PropType(prop, polymorphicIds));
            await writer.WriteAsync(" ");
            await writer.WriteAsync(GetPropName(prop.NameProp().Value, itemTypeLabel.Substring(1)));
            await writer.WriteLineAsync(@"();");
          }
          await writer.WriteAsync(@"  }
}");
        }
        links.Add(itemType.Id(), itemTypeLabel);
        foreach (var poly in itemType.Relationships("Morphae"))
        {
          links.Add(poly.RelatedId().Value, itemTypeLabel);
        }
      }

      foreach (var itemType in itemTypes)
      {
        var itemTypeLabel = itemType.IdProp().KeyedName().Value.Replace(" ", "");
        files.Add(itemTypeLabel + ".cs");
        using (var writer = new StreamWriter(Path.Combine(_baseDir, "Innovator.Client." + _companyName + "/" + itemTypeLabel + ".cs")))
        {
          await writer.WriteAsync(@"using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>");
          await writer.WriteAsync("Class for the item type " + itemType.IdProp().KeyedName().Value);
          await writer.WriteAsync(@" </summary>
  public class ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(@" : Item");
          if (links[itemType.Id()] != null)
          {
            await writer.WriteAsync(@", ");
            await writer.WriteAsync(links.GetValues(itemType.Id()).GroupConcat(", "));
          }
          if (itemType.IsRelationship().AsBoolean(false))
          {
            var source = itemType.Relationships().OfType<Innovator.Client.Model.Property>().Single(p => p.NameProp().Value == "source_id");
            if (source.DataSource().KeyedName().HasValue())
            {
              await writer.WriteAsync(@", INullRelationship<");
              await writer.WriteAsync((polymorphicIds.Contains(source.DataSource().Value) ? "I" : "") + source.DataSource().KeyedName().Value.Replace(" ", ""));
              await writer.WriteAsync(@">");
            }

            var related = itemType.Relationships().OfType<Innovator.Client.Model.Property>().Single(p => p.NameProp().Value == "related_id");
            if (related.DataSource().KeyedName().HasValue())
            {
              await writer.WriteAsync(@", IRelationship<");
              await writer.WriteAsync((polymorphicIds.Contains(source.DataSource().Value) ? "I" : "") + related.DataSource().KeyedName().Value.Replace(" ", ""));
              await writer.WriteAsync(@">");
            }
          }
          await writer.WriteAsync(@"
  {
    protected ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(@"() { }
    public ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(@"(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync("() { Innovator.Client.Item.AddNullItem<");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(">(new ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync(@" { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

");

          foreach (var prop in itemType
            .Relationships()
            .OfType<Innovator.Client.Model.Property>()
            .Where(p => p.NameProp().Value != "source_id" && p.NameProp().Value != "related_id" && p.NameProp().Value != "id"))
          {
            await writer.WriteAsync("    /// <summary>Retrieve the <c>");
            await writer.WriteAsync(prop.NameProp().Value);
            await writer.WriteAsync("</c> property of the item</summary>\r\n");
            await writer.WriteAsync("    public IProperty_");
            await writer.WriteAsync(PropType(prop, polymorphicIds));
            await writer.WriteAsync(" ");
            await writer.WriteAsync(GetPropName(prop.NameProp().Value, itemTypeLabel));
            await writer.WriteAsync(@"()
    {
      return this.Property(""");
            await writer.WriteAsync(prop.NameProp().Value);
            await writer.WriteAsync(@""");
    }
");
          }
          await writer.WriteAsync(@"  }
}");
        }
      }

      files.Add("CorporateItemFactory.cs");
      using (var writer = new StreamWriter(Path.Combine(_baseDir, "Innovator.Client." + _companyName + "/" + "CorporateItemFactory.cs")))
      {
        await writer.WriteAsync(@"using Innovator.Client;
using Innovator.Client.Model;

namespace Innovator.Client.Model
{
  public class CorporateItemFactory : IItemFactory
  {
    private static ElementFactory _local = new ElementFactory(new ServerContext(false), new CorporateItemFactory());
    public static ElementFactory Local { get { return _local; } }

    private static IReadOnlyItem _nullItem = new IReadOnlyItem[] { }.FirstOrNullItem();
    public static IReadOnlyItem NullItem { get { return _nullItem; } }

    private IItemFactory _default = new DefaultItemFactory();

    public Item NewItem(ElementFactory factory, string type)
    {
      switch (type)
      {
");
        foreach (var itemType in itemTypes)
        {
          var itemTypeLabel = itemType.IdProp().KeyedName().Value.Replace(" ", "");
          await writer.WriteAsync("        case \"");
          await writer.WriteAsync(itemType.IdProp().KeyedName().Value);
          await writer.WriteAsync("\": return new ");
          await writer.WriteAsync(itemTypeLabel);
          await writer.WriteAsync("(factory);");
          await writer.WriteLineAsync();
        }
        await writer.WriteAsync(@"      }

      return _default.NewItem(factory, type);
    }
  }
}");
      }

      using (var writer = new StreamWriter(Path.Combine(_baseDir, "Innovator.Client." + _companyName + "/Innovator.Client." + _companyName + ".csproj")))
      {
        await writer.WriteAsync(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""14.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <MinimumVisualStudioVersion>14.0</MinimumVisualStudioVersion>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{779753D2-6514-4A32-B180-D13B4FA61CB3}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>Innovator.Client." + _companyName + @"</RootNamespace>
    <AssemblyName>Innovator.Client." + _companyName + @"</AssemblyName>
    <DefaultLanguage>en-US</DefaultLanguage>
    <FileAlignment>512</FileAlignment>
    <ProjectTypeGuids>{786C830F-07A1-408B-BD7F-6EE04809D6DB};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
    <TargetFrameworkProfile>
    </TargetFrameworkProfile>
    <TargetFrameworkVersion>v5.0</TargetFrameworkVersion>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <!-- A reference to the entire .NET Framework is automatically included -->
    <None Include=""project.json"" />
  </ItemGroup>
  <ItemGroup>
");
        files.Sort();
        foreach (var file in files)
        {
          await writer.WriteAsync("    <Compile Include=\"");
          await writer.WriteAsync(file);
          await writer.WriteAsync("\" />");
          await writer.WriteLineAsync();
        }
        await writer.WriteAsync(@"  </ItemGroup>
  <Import Project=""$(MSBuildExtensionsPath32)\Microsoft\Portable\$(TargetFrameworkVersion)\Microsoft.Portable.CSharp.targets"" />
  <!-- To modify your build process, add your task inside one of the targets below and uncomment it.
       Other similar extension points exist, see Microsoft.Common.targets.
  <Target Name=""BeforeBuild"">
  </Target>
  <Target Name=""AfterBuild"">
  </Target>
  -->
</Project>");
      }

      using (var writer = new StreamWriter(Path.Combine(_baseDir, "Innovator.Client." + _companyName + "/Properties/AssemblyInfo.cs")))
      {
        await writer.WriteAsync(@"using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle(""Innovator.Client.");
        await writer.WriteAsync(_companyName);
        await writer.WriteAsync(@""")]
[assembly: AssemblyDescription(""Client models for an Aras Innovator installation"")]
[assembly: AssemblyConfiguration("""")]
[assembly: AssemblyCompany("""")]
[assembly: AssemblyProduct(""Innovator.Client.");
        await writer.WriteAsync(_companyName);
        await writer.WriteAsync(@""")]
[assembly: AssemblyCopyright(""Copyright © 2017"")]
[assembly: AssemblyTrademark("""")]
[assembly: AssemblyCulture("""")]
[assembly: NeutralResourcesLanguage(""en"")]");
      }

      using (var writer = new StreamWriter(Path.Combine(_baseDir, "Innovator.Client." + _companyName + "/AssemblyInfo.Version.cs")))
      {
        await writer.WriteAsync(@"using System.Reflection;

[assembly: AssemblyVersion(""1.0.6213.30600"")]
[assembly: AssemblyFileVersion(""1.0.6213.30600"")]
");
      }

      var projJson = @"{
  ""version"": ""1.0.6213.30600"",
  ""title"": ""Innovator.Client.Corporate"",
  ""supports"": {},
  ""frameworks"": {
    ""net35"": {
      ""dependencies"": {
        ""Innovator.Client"": ""2017.1.3.420""
      }
    },
    ""net40"": {
      ""dependencies"": {
        ""Innovator.Client"": ""2017.1.3.420""
      }
    },
    ""net45"": {
      ""dependencies"": {
        ""Innovator.Client"": ""2017.1.3.420""
      }
    },
    ""netstandard1.1"":
    {
      ""dependencies"": {
        ""Microsoft.NETCore.Portable.Compatibility"": ""1.0.1"",
        ""NETStandard.Library"": ""1.6.0"",
        ""Innovator.Client"": ""2017.1.3.420""
      }
    },
    ""netstandard1.3"":
    {
      ""dependencies"": {
        ""Microsoft.NETCore.Portable.Compatibility"": ""1.0.1"",
        ""NETStandard.Library"": ""1.6.0"",
        ""Innovator.Client"": ""2017.1.3.420""
      }
    }
  }
}".Replace("Innovator.Client.Corporate", "Innovator.Client." + _companyName);
      using (var writer = new StreamWriter(Path.Combine(_baseDir, "Innovator.Client." + _companyName + "/project.json")))
      {
        await writer.WriteAsync(projJson);
      }

      var build = @"<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } ""Error executing SVN. Please verify SVN command-line client is installed""
#>

$epoch = Get-Date -Date ""2000-01-01 00:00:00Z""
$epoch = $epoch.ToUniversalTime()
$now = [System.DateTime]::UtcNow
$span = NEW-TIMESPAN -Start $epoch -End $now
$days = [int]$span.TotalDays
$span = NEW-TIMESPAN -Start $now.Date -End $now
$seconds = [int]($span.TotalSeconds / 2)

$version = ""1.0.$days.$seconds""

$assyInfo = ""using System.Reflection;`r`n`r`n[assembly: AssemblyVersion(""""$version"""")]`r`n[assembly: AssemblyFileVersion(""""$version"""")]""

$assyInfo | Out-File Innovator.Client.Corporate\AssemblyInfo.Version.cs

(Get-Content Innovator.Client.Corporate/project.json) `
    -replace '""version"": ""\d+\.\d+\.\d+\.\d+"",', """"""version"""": """"$version"""","" |
  Out-File Innovator.Client.Corporate/project.json

function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw (""Exec: "" + $errorMessage)
    }
}

exec { & dotnet restore }
exec { & dotnet pack Innovator.Client.Corporate/project.json -c Release -o .\artifacts --version-suffix=$version }  ".Replace("Innovator.Client.Corporate", "Innovator.Client." + _companyName);
      using (var writer = new StreamWriter(Path.Combine(_baseDir, "build.ps1")))
      {
        await writer.WriteAsync(build);
      }

      using (var writer = new StreamWriter(Path.Combine(_baseDir, "Innovator.Client." + _companyName + ".sln")))
      {
        await writer.WriteAsync(@"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 14
VisualStudioVersion = 14.0.25420.1
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""Innovator.Client.");
        await writer.WriteAsync(_companyName);
        await writer.WriteAsync(@""", ""Innovator.Client.");
        await writer.WriteAsync(_companyName);
        await writer.WriteAsync(@"\Innovator.Client.");
        await writer.WriteAsync(_companyName);
        await writer.WriteAsync(@".csproj"", ""{779753D2-6514-4A32-B180-D13B4FA61CB3}""
EndProject
Global
  GlobalSection(SolutionConfigurationPlatforms) = preSolution
    Debug|Any CPU = Debug|Any CPU
    Release|Any CPU = Release|Any CPU
  EndGlobalSection
  GlobalSection(ProjectConfigurationPlatforms) = postSolution
    {779753D2-6514-4A32-B180-D13B4FA61CB3}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {779753D2-6514-4A32-B180-D13B4FA61CB3}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {779753D2-6514-4A32-B180-D13B4FA61CB3}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {779753D2-6514-4A32-B180-D13B4FA61CB3}.Release|Any CPU.Build.0 = Release|Any CPU
  EndGlobalSection
  GlobalSection(SolutionProperties) = preSolution
    HideSolutionNode = FALSE
  EndGlobalSection
EndGlobal
");
      }
    }

    private string PropType(Innovator.Client.Model.Property prop, HashSet<string> polymorphicIds)
    {
      var dataType = prop.DataType().Value;
      if (dataType == "foreign" && prop.ForeignProperty().HasValue())
      {
        prop = prop.ForeignProperty().AsModel();
        dataType = prop.DataType().Value;
      }
      switch (dataType)
      {
        case "boolean":
          return "Boolean";
        case "date":
          return "Date";
        case "integer":
        case "float":
        case "decimal":
          return "Number";
        case "item":
          if (prop.DataSource().KeyedName().Exists)
            return "Item<"
              + (polymorphicIds.Contains(prop.DataSource().Value) ? "I" : "")
              + prop.DataSource().KeyedName().Value.Replace(" ", "") + ">";
          return "Item<IReadOnlyItem>";
        default:
          return "Text";
      }
    }

    private string GetPropName(string name, string itemTypeLabel)
    {
      var output = new char[name.Length];
      var o = 0;
      bool lastCharBoundary = true;
      for (var i = 0; i < name.Length; i++)
      {
        if (name[i] == '_')
        {
          lastCharBoundary = true;
        }
        else
        {
          if (lastCharBoundary)
            output[o] = char.ToUpper(name[i]);
          else
            output[o] = char.ToLower(name[i]);
          o++;
          lastCharBoundary = false;
        }
      }
      var result = new string(output, 0, o);
      switch (result)
      {
        case "Add":
        case "AmlContext":
        case "Attribute":
        case "Attributes":
        case "Clone":
        case "Elements":
        case "Equals":
        case "Exists":
        case "GetHashCode":
        case "GetType":
        case "Id":
        case "Name":
        case "Next":
        case "Parent":
        case "Property":
        case "ReadOnly":
        case "Relationships":
        case "Remove":
        case "RemoveAttributes":
        case "RemoveNodes":
        case "ToAml":
        case "ToString":
        case "TypeName":
        case "Value":
          return result + "Prop";
      }
      if (result == itemTypeLabel)
        return result + "Prop";
      return result;
    }
  }
}
