using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace InnovatorAdmin
{
  public class InstallItem : IPackageFile
  {
    public const string ScriptType = "*Script";

    private IEnumerable<ItemReference> _dependencies;
    private XmlElement _elem;
    private ItemReference _itemRef;
    private string _name;

    internal IEnumerable<ItemReference> CoreDependencies => _dependencies;
    public InstallType Type { get; set; }
    public ItemReference Reference => _itemRef;
    public string InstalledId { get; set; }
    public string Name
    {
      get
      {
        if (_name == null)
        {
          switch (Type)
          {
            case InstallType.Create:
              return "Install of " + this.Reference.ToString();
            case InstallType.DependencyCheck:
              return "Check of Dependency " + this.Reference.ToString();
            case InstallType.Script:
              return this.Reference.KeyedName;
          }
        }
        return _name;
      }
      set { _name = value; }
    }
    public string Path { get; set; }
    public XmlElement Script => _elem;

    string IPackageFile.Path
    {
      get
      {
        if (!string.IsNullOrWhiteSpace(this.Path))
          return this.Path;
        var folder = Type == InstallType.Script ? "_Scripts" : Reference.Type;
        return folder + "\\" + Utils.CleanFileName((Reference.KeyedName ?? "").Trim() + "_" + Reference.Unique) + ".xml";
      }
    }

    private InstallItem() { }

    internal bool IsDelete()
    {
      return Type == InstallType.Script && Name.Split(' ').Contains("Delete");
    }

    public void SetScript(string script)
    {
      var aml = _elem.OwnerDocument.CreateElement("AML");
      aml.InnerXml = script;
      if (aml.Elements().Count() == 1)
      {
        _elem = aml.Elements().Single();
      }
      else
      {
        _elem = aml;
      }
    }

    public override string ToString()
    {
      return this.Name;
    }

    public static InstallItem FromScript(XmlElement elem, string path)
    {
      var result = FromScript(elem);
      result.Path = path;
      return result;
    }

    public static InstallItem FromScript(XmlElement elem
      , Func<XmlElement, string> keyedNameGetter = null)
    {

      var result = new InstallItem
      {
        _elem = elem,
        _itemRef = ItemReference.FromFullItem(elem, true)
      };
      if (result._itemRef.Type.IsGuid())
      {
        result.InstalledId = result._itemRef.Type;
      }
      else
      {
        result.InstalledId = elem.Attribute("id", "");
      }

      if (elem.HasAttribute("_dependency_check"))
      {
        result.Type = InstallType.DependencyCheck;
      }
      else if (elem.HasAttribute("action"))
      {
        switch (elem.Attributes["action"].Value)
        {
          case "add":
          case "merge":
          case "create":
            result.Type = InstallType.Create;
            break;
          case "ActivateActivity":
          case "AddItem":
          case "AddHistory":
          case "ApplyUpdate":
          case "BuildProcessReport":
          case "CancelWorkflow":
          case "checkImportedItemType":
          case "closeWorkflow":
          case "copy":
          case "copyAsIs":
          case "copyAsNew":
          case "delete":
          case "edit":
          case "EmailItem":
          case "EvaluateActivity":
          case "exportItemType":
          case "get":
          case "getItemAllVersions":
          case "getAffectedItems":
          case "getItemConfig":
          case "getItemLastVersion":
          case "getItemNextStates":
          case "getItemRelationships":
          case "GetItemRepeatConfig":
          case "getItemWhereUsed":
          case "GetMappedPath":
          case "getPermissions":
          case "getRelatedItem":
          case "GetUpdateInfo":
          case "instantiateWorkflow":
          case "lock":
          case "New Workflow Map":
          case "PromoteItem":
          case "purge":
          case "recache":
          case "replicate":
          case "resetAllItemsAccess":
          case "resetItemAccess":
          case "resetLifecycle":
          case "setDefaultLifecycle":
          case "skip":
          case "startWorkflow":
          case "unlock":
          case "update":
          case "ValidateWorkflowMap":
          case "version":
            if ((elem.Attributes["type"].Value != "Form" && elem.Attributes["type"].Value != "View")
              || elem.Attributes["action"].Value != "delete")
            {
              result._dependencies = Enumerable.Repeat(result._itemRef, 1);
            }

            result._itemRef = new ItemReference(ScriptType, result._itemRef + " " + Utils.GetChecksum(Encoding.UTF8.GetBytes(elem.OuterXml)))
            {
              KeyedName = RenderAttributes(elem)
            };
            result.Type = InstallType.Script;
            break;
          default:
            result._dependencies = Enumerable.Repeat(new ItemReference("Method", "[Method].[name] = '" + elem.Attributes["action"].Value + "'")
            {
              KeyedName = elem.Attributes["action"].Value
            }, 1);
            result._itemRef = new ItemReference(ScriptType, result._itemRef + " " + Utils.GetChecksum(Encoding.UTF8.GetBytes(elem.OuterXml)))
            {
              KeyedName = RenderAttributes(elem)
            };
            result.Type = InstallType.Script;
            break;
        }
      }

      if (elem.Attribute(XmlFlags.Attr_IsScript) == "1")
      {
        if (string.IsNullOrEmpty(result._itemRef.KeyedName))
        {
          result._itemRef.KeyedName = RenderAttributes(elem);
        }
        result.Type = InstallType.Script;
      }
      return result;
    }

    public static InstallItem FromDependency(ItemReference itemRef)
    {
      var result = new InstallItem();
      result._itemRef = itemRef;
      result._elem = new XmlDocument().CreateElement("Item");
      result._elem.SetAttribute("type", result._itemRef.Type);
      if (result._itemRef.Unique.IsGuid())
      {
        result._elem.SetAttribute("id", result._itemRef.Unique);
      }
      else
      {
        result._elem.SetAttribute("where", result._itemRef.Unique);
      }
      result._elem.SetAttribute("action", "get");
      result._elem.SetAttribute("_dependency_check", "1");
      result._elem.SetAttribute("_keyed_name", result._itemRef.KeyedName);
      result.Type = InstallType.DependencyCheck;
      return result;
    }

    public static InstallItem FromWarning(ItemReference itemRef, string warning)
    {
      var result = new InstallItem();
      result._itemRef = itemRef;
      result._name = warning;
      result.Type = InstallType.Warning;
      return result;
    }

    internal static string RenderAttributes(XmlElement elem, string keyedName = null)
    {
      var builder = new StringBuilder();

      if (elem.HasAttribute("action"))
      {
        var action = elem.Attribute("action");
        builder.Append(char.ToUpper(action[0]))
          .Append(action.Substring(1))
          .Append(" of");
      }
      if (elem.HasAttribute("where"))
      {
        builder.Append(" ").Append(elem.Attribute("where"));
      }
      else
      {
        if (elem.HasAttribute("type"))
        {
          builder.Append(" ").Append(elem.Attribute("type")).Append(":");
        }
        if (!string.IsNullOrEmpty(keyedName))
        {
          builder.Append(" ").Append(keyedName);
        }
        else if (elem.HasAttribute("id"))
        {
          builder.Append(" ").Append(elem.Attribute("id"));
        }
      }

      if (elem.HasAttribute(XmlFlags.Attr_ScriptType))
      {
        builder.Append(" (").Append(elem.Attribute(XmlFlags.Attr_ScriptType)).Append(")");
      }

      return builder.ToString();
    }

    Stream IPackageFile.Open()
    {
      var settings = new XmlWriterSettings
      {
        OmitXmlDeclaration = true,
        Indent = true,
        IndentChars = "  ",
        CloseOutput = false
      };

      var result = new MemoryStream();
      using (var writer = XmlWriter.Create(result, settings))
      {
        writer.WriteStartElement("AML");
        this.Script.WriteTo(writer);
        writer.WriteEndElement();
        writer.Flush();
      }
      result.Position = 0;
      return result;
    }
  }

  public static class InstallItemExtensions
  {
    public static string FilePath(this InstallItem line, HashSet<string> existingPaths, string extension = ".xml")
    {
      var folder = line.Type == InstallType.Script ? "_Scripts" : line.Reference.Type;
      var newPath = folder + "\\" + Utils.CleanFileName(line.Reference.KeyedName ?? line.Reference.Unique).Trim() + extension;
      if (existingPaths?.Contains(newPath) == true)
        newPath = folder + "\\" + Utils.CleanFileName((line.Reference.KeyedName ?? "").Trim() + "_" + line.Reference.Unique) + extension;
      return newPath;
    }
  }
}
