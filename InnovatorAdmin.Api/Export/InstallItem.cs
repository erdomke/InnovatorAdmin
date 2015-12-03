using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace InnovatorAdmin
{
  public class InstallItem
  {
    private IEnumerable<ItemReference> _dependencies;
    private XmlElement _elem;
    private ItemReference _itemRef;
    private string _name;
    private InstallType _type;

    internal IEnumerable<ItemReference> CoreDependencies { get { return _dependencies; } }
    public InstallType Type { 
      get { return _type; }
      set { _type = value; }
    }
    public ItemReference Reference { get { return _itemRef; } }
    public string InstalledId { get; set; }
    public string Name 
    {
      get 
      { 
        if (_name == null)
        {
          switch (_type)
          {
            case InstallType.Create:
              return "Install of " + this.Reference.ToString();
            case InstallType.DependencyCheck:
              return "Check of Dependency " + this.Reference.ToString();
            case InstallType.Script:
              return "Script: " + this.Reference.KeyedName;
          }
        }
        return _name; 
      }
      set { _name = value; }
    }
    public XmlElement Script { get { return _elem; } }

    public InstallItem() { }

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

    public static InstallItem FromScript(XmlElement elem)
    {
      var result = new InstallItem();
      result._elem = elem;
      result._itemRef = ItemReference.FromFullItem(elem, true);
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
        result._type = InstallType.DependencyCheck;
      }
      else if (elem.HasAttribute("action"))
      {
        switch (elem.Attributes["action"].Value)
        {
          case "add":
          case "merge":
          case "create":
            result._type = InstallType.Create;
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
            result._dependencies = Enumerable.Repeat(result._itemRef, 1);
            result._itemRef = new ItemReference("*Script", result._itemRef.ToString() + " " + Utils.GetChecksum(Encoding.UTF8.GetBytes(elem.OuterXml)))
            {
              KeyedName = RenderAttributes(elem)
            };
            result._type = InstallType.Script;
            break;
          default:
            result._dependencies = Enumerable.Repeat(new ItemReference("Method", "[Method].[name] = '" + elem.Attributes["action"].Value + "'")
            {
              KeyedName = elem.Attributes["action"].Value
            }, 1);
            result._itemRef = new ItemReference("*Script", result._itemRef.ToString() + " " + Utils.GetChecksum(Encoding.UTF8.GetBytes(elem.OuterXml)))
            {
              KeyedName = RenderAttributes(elem)
            };
            result._type = InstallType.Script;
            break;
        }
      }

      if (elem.Attribute(XmlFlags.Attr_IsScript) == "1")
      {
        if (string.IsNullOrEmpty(result._itemRef.KeyedName))
        {
          result._itemRef.KeyedName = RenderAttributes(elem);
        }
        result._type = InstallType.Script;
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
      result._type = InstallType.DependencyCheck;
      return result;
    }
    public static InstallItem FromWarning(ItemReference itemRef, string warning)
    {
      var result = new InstallItem();
      result._itemRef = itemRef;
      result._name = warning;
      result._type = InstallType.Warning;
      return result;
    }

    private static string RenderAttributes(XmlElement elem)
    {
      var builder = new StringBuilder();
      foreach (var attr in elem.Attributes.OfType<XmlAttribute>())
      {
        if (builder.Length > 0) builder.Append(' ');
        builder.Append(attr.LocalName).Append("=\"").Append(attr.Value).Append('\"');
      }
      return builder.ToString();
    }
  }
}
