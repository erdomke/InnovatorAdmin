using Aras.AutoComplete.AmlSchema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.AutoComplete
{
  public class CompletionHelper
  {
    private Func<string, XmlNode, XmlNode> _applyAction;
    private ItemTypeCollection _itemTypes = new ItemTypeCollection();
    private List<string> _methods;

    public IEnumerable<string> GetCompletions(string xml, string soapAction, out int overlap)
    {
      overlap = 0;
      if (string.IsNullOrEmpty(xml)) return Enumerable.Empty<string>();

      var path = new List<AmlNode>();
      string attr = null;
      string attrValue = null;
      
      var state = Utils.ProcessFragment(xml, (r, o) =>
      {
        switch (r.NodeType)
        {
          case XmlNodeType.Element:
            if (!r.IsEmptyElement) 
              path.Add(new AmlNode() { LocalName = r.LocalName, Type = r.GetAttribute("type"), Action = r.GetAttribute("action") });
            break;
          case XmlNodeType.EndElement:
            path.RemoveAt(path.Count - 1);
            break;
          case XmlNodeType.Attribute:
            attr = r.LocalName;
            attrValue = r.Value;
            break;
        }
        return true;
      });
      if (state == Utils.XmlState.Tag && xml.EndsWith("\"")) return Enumerable.Empty<string>();

      var items = new List<string>();

      if (path.Count < 1)
      {
        switch (soapAction)
        {
          case "ApplySQL":
            items.Add("sql");
            break;
          case "ApplyAML":
            items.Add("AML");
            break;
          default:
            items.Add("Item");
            break;
        }
      }
      else
      {
        switch (state)
        {
          case Utils.XmlState.Attribute:
          case Utils.XmlState.AttributeStart:
            switch (path.Last().LocalName)
            {
              case "and":
              case "or":
              case "not":
              case "Relationships":
              case "AML":
              case "sql":
              case "SQL":
                break;
              case "Item":
                switch (soapAction)
                {
                  case "GenerateNewGUIDEx":
                    items.Add("quantity");
                    break;
                  case "":
                    break;
                  default:
                    items.Add("action");
                    if (path.Last().Action == "getPermissions") items.Add("access_type");
                    items.Add("config_path");
                    items.Add("doGetItem");
                    items.Add("id");
                    items.Add("idlist");
                    items.Add("isCriteria");
                    items.Add("language");
                    items.Add("levels");
                    items.Add("maxRecords");
                    items.Add("page");
                    items.Add("pagesize");
                    items.Add("orderBy");
                    items.Add("queryDate");
                    items.Add("queryType");
                    items.Add("related_expand");
                    items.Add("select");
                    items.Add("serverEvents");
                    items.Add("type");
                    items.Add("typeID");
                    items.Add("version");
                    items.Add("where");
                    break;
                }
                break;
              default:
                items.Add("condition");
                items.Add("is_null");
                break;
            }
            if (!string.IsNullOrEmpty(attr))
            {
              items = FilterAndSort(items, attr);
              overlap = attr.Length;
            }
            break;
          case Utils.XmlState.AttributeValue:
            if (path.Last().LocalName == "Item")
            {
              ItemType itemType;
              switch (attr)
              {
                case "action":
                  items.Add("ActivateActivity");
                  items.Add("add");
                  items.Add("AddItem");
                  items.Add("AddHistory");
                  items.Add("ApplyUpdate");
                  items.Add("BuildProcessReport");
                  items.Add("CancelWorkflow");
                  items.Add("checkImportedItemType");
                  items.Add("closeWorkflow");
                  items.Add("copy");
                  items.Add("copyAsIs");
                  items.Add("copyAsNew");
                  items.Add("create");
                  items.Add("delete");
                  items.Add("edit");
                  items.Add("EmailItem");
                  items.Add("EvaluateActivity");
                  items.Add("exportItemType");
                  items.Add("get");
                  items.Add("getItemAllVersions");
                  items.Add("getAffectedItems");
                  items.Add("GetItemConfig");
                  items.Add("getItemLastVersion");
                  items.Add("getItemNextStates");
                  items.Add("getItemRelationships");
                  items.Add("GetItemRepeatConfig");
                  items.Add("getItemWhereUsed");
                  items.Add("GetMappedPath");
                  items.Add("getPermissions");
                  items.Add("getRelatedItem");
                  items.Add("GetUpdateInfo");
                  items.Add("instantiateWorkflow");
                  items.Add("lock");
                  items.Add("merge");
                  items.Add("New Workflow Map");
                  items.Add("PromoteItem");
                  items.Add("purge");
                  items.Add("recache");
                  items.Add("replicate");
                  items.Add("resetAllItemsAccess");
                  items.Add("resetItemAccess");
                  items.Add("resetLifecycle");
                  items.Add("setDefaultLifecycle");
                  items.Add("skip");
                  items.Add("startWorkflow");
                  items.Add("unlock");
                  items.Add("update");
                  items.Add("ValidateWorkflowMap");
                  items.Add("version");

                  if (_methods == null)
                  {
                    var methods = GetItems("ApplyItem", "<AML><Item action=\"get\" type=\"Method\" select=\"name\"></Item></AML>");
                    _methods = methods.Select(m => m.Element("name", "")).ToList();
                  }

                  items.AddRange(_methods);
                  break;
                case "access_type":
                  items.Add("can_add");
                  items.Add("can_delete");
                  items.Add("can_get");
                  items.Add("can_update");
                  break;
                case "doGetItem":
                case "version":
                case "isCriteria":
                case "related_expand":
                case "serverEvents":
                  items.Add("0");
                  items.Add("1");
                  break;
                case "queryType":
                  items.Add("Effective");
                  items.Add("Latest");
                  items.Add("Released");
                  break;
                case "orderBy":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _itemTypes.TryGetValue(path.Last().Type, out itemType))
                  {
                    var lastComma = attrValue.LastIndexOf(",");
                    if (lastComma >= 0) attrValue = attrValue.Substring(lastComma + 1).Trim();
                    
                    items.AddRange(GetProperties(itemType).Select(p => p.Name));
                    items.AddRange(GetProperties(itemType).Select(p => p.Name + " DESC"));
                  }
                  break;
                case "select":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _itemTypes.TryGetValue(path.Last().Type, out itemType))
                  {
                    string partial;
                    var selectPath = SelectPath(attrValue, out partial);
                    attrValue = partial;

                    Property currProp;
                    foreach (var part in selectPath)
                    {
                      if (TryGetProperty(itemType, part, out currProp) 
                        && currProp.Type == PropertyType.item 
                        && currProp.Restrictions.Any() 
                        && _itemTypes.TryGetValue(currProp.Restrictions.First(), out itemType))
                      {
                        // continue
                      }
                      else
                      {
                        break;
                      }
                    }

                    items.AddRange(GetProperties(itemType).Select(p => p.Name));
                  }
                  break;
                case "type":
                  if (path.Count > 2 
                    && path[path.Count - 3].LocalName == "Item" 
                    && path[path.Count - 2].LocalName == "Relationships") 
                  {
                    if (!string.IsNullOrEmpty(path[path.Count - 3].Type)
                      && _itemTypes.TryGetValue(path[path.Count - 3].Type, out itemType))
                    {
                      items.AddRange(itemType.Relationships.Select(r => r.Name));
                    }
                  }
                  else
                  {
                    items.AddRange(_itemTypes.Select(i => i.Value.Name));
                  }
                  break;
                case "where":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _itemTypes.TryGetValue(path.Last().Type, out itemType))
                  {
                    items.AddRange(GetProperties(itemType).Select(p => "[" + itemType.Name + "].[" + p.Name + "]"));
                  }
                  break;
              }
            }
            else
            {
              switch (attr)
              {
                case "condition":
                  items.Add("between");
                  items.Add("eq");
                  items.Add("ge");
                  items.Add("gt");
                  items.Add("in");
                  items.Add("is not null");
                  items.Add("is null");
                  items.Add("is");
                  items.Add("le");
                  items.Add("like");
                  items.Add("lt");
                  items.Add("ne");
                  items.Add("not between");
                  items.Add("not in");
                  items.Add("not like");
                  break;
                case "is_null":
                  items.Add("0");
                  items.Add("1");
                  break;
              }
            }

            if (string.IsNullOrEmpty(attrValue))
            {
              items.Sort();
            }
            else
            {
              items = FilterAndSort(items, attrValue);
              if (overlap == 0) overlap = attrValue.Length;
            }
            break;
          default:
            if (path.Count == 1 && path.First().LocalName == "AML")
            {
              items.Add("Item");
            }
            else
            {
              var j = path.Count - 1;
              while (path[j].LocalName == "and" || path[j].LocalName == "not" || path[j].LocalName == "or") j--;
              var last = path[j];
              if (last.LocalName == "Item")
              {
                items.Add("Relationships");
                if (last.Action == "get")
                {
                  items.Add("and");
                  items.Add("not");
                  items.Add("or");
                }
                ItemType itemType;
                if (!string.IsNullOrEmpty(last.Type) 
                  && _itemTypes.TryGetValue(last.Type, out itemType))
                {
                  items.AddRange(GetProperties(itemType).Select(p => p.Name));
                }
              }
              else if (path.Count > 1)
              {
                var lastItem = path.LastOrDefault(n => n.LocalName == "Item");
                if (lastItem != null)
                {
                  if (path.Last().LocalName == "Relationships")
                  {
                    items.Add("Item");
                  }
                  else
                  {
                    ItemType itemType;
                    if (!string.IsNullOrEmpty(lastItem.Type)
                      && _itemTypes.TryGetValue(lastItem.Type, out itemType))
                    {
                      Property prop;
                      if (TryGetProperty(itemType, path.Last().LocalName, out prop))
                      {
                        if (prop.Type == PropertyType.item && prop.Restrictions.Any())
                        {
                          foreach (var type in prop.Restrictions)
                          {
                            items.Add("Item type=\"" + type + "\"");
                          }
                        }
                      }
                    }
                  }
                }
              }
            }

            items.Sort();
            break;
        }

      }
      return items;
    }

    public string GetQuery(string xml, int offset)
    {
      var start = -1;
      var end = -1;
      var depth = 0;
      string result = null;

      Utils.ProcessFragment(xml, (r, o) =>
      {
        switch (r.NodeType)
        {
          case XmlNodeType.Element:

            if (depth == 0)
            {
              start = o;
            }
            if (r.IsEmptyElement)
            {
              end = xml.IndexOf("/>", o) + 2;
              if (offset >= start && offset < end)
              {
                result = xml.Substring(start, end - start);
                return false;
              }
            }
            else
            {
              depth++;
            }
            break;
          case XmlNodeType.EndElement:
            depth--;
            if (depth == 0)
            {
              end = xml.IndexOf('>', o) + 1;
              if (offset >= start && offset < end)
              {
                result = xml.Substring(start, end - start);
                return false;
              }
            }
            break;
        }
        return true;
      });

      return result;
    }

    public virtual void InitializeConnection(Func<string, XmlNode, XmlNode> applyAction)
    {
      _itemTypes.Clear();
      _applyAction = applyAction;
      LoadItemTypes(GetItems("ApplyAML", "<AML><Item action=\"get\" type=\"ItemType\" select=\"name\" /><Item action=\"get\" type=\"RelationshipType\" related_expand=\"0\" select=\"related_id,source_id,relationship_id,name\" /></AML>"));
    }

    public string LastOpenTag(string xml)
    {
      bool isOpenTag = false;
      var path = new List<AmlNode>();

      var state = Utils.ProcessFragment(xml, (r, o) =>
      {
        switch (r.NodeType)
        {
          case XmlNodeType.Element:
            if (!r.IsEmptyElement)
            {
              path.Add(new AmlNode() { LocalName = r.LocalName });
              isOpenTag = true;
            }
            break;
          case XmlNodeType.EndElement:
            path.RemoveAt(path.Count - 1);
            isOpenTag = false;
            break;
          default:
            isOpenTag = false;
            break;
        }
        return true;
      });

      if (isOpenTag && path.Any()) return path.Last().LocalName;
      return null;
    }

    private List<string> FilterAndSort(IEnumerable<string> values, string substring)
    {
      var result = values.Where(i => i.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
      result.Sort((x, y) =>
      {
        var xStart = x.StartsWith(substring, StringComparison.OrdinalIgnoreCase);
        var yStart = y.StartsWith(substring, StringComparison.OrdinalIgnoreCase);
        if (xStart && !yStart)
        {
          return -1;
        }
        else if (yStart && !xStart)
        {
          return 1;
        }
        else
        {
          return x.CompareTo(y);
        }
      });
      return result;
    }

    private IEnumerable<XmlElement> GetItems(string soapAction, string input)
    {
      var inputDoc = new XmlDocument();
      inputDoc.LoadXml(input);
      return this.GetItems(soapAction, inputDoc);
    }
    private IEnumerable<XmlElement> GetItems(string soapAction, XmlNode input)
    {
      var result = _applyAction.Invoke(soapAction, input);
      var node = result;
      while (node != null && node.LocalName != "Item") node = node.ChildNodes.OfType<XmlElement>().FirstOrDefault();
      if (node == null) return Enumerable.Empty<XmlElement>();
      return node.ParentNode.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == "Item");
    }
    private IEnumerable<Property> GetProperties(ItemType itemType)
    {
      if (itemType.Properties.Count < 1)
        LoadProperties(itemType, GetItems("ApplyItem", string.Format("<AML><Item action=\"get\" type=\"ItemType\" select=\"name\"><name>{0}</name><Relationships><Item action=\"get\" type=\"Property\" select=\"name,label,data_type,data_source\" /></Relationships></Item></AML>", itemType.Name)));
      return itemType.Properties.Values;
    }
    /// <summary>
    /// Loads the metadata pertaining to the item types in Aras.
    /// </summary>
    /// <param name="items">The item type data.</param>
    private void LoadItemTypes(IEnumerable<XmlElement> items)
    {
      try
      {
        ItemType currType = null;
        ItemType sourceType = null;
        
        foreach (var item in items)
        {
          Debug.Print(item.Attribute("type"));
          switch (item.Attribute("type"))
          {
            case "RelationshipType":
              if (_itemTypes.TryGetValue(item.Element("relationship_id").Attribute("name"), out currType))
              {
                if (item.Element("source_id").Attribute("name") != null &&
                    _itemTypes.TryGetValue(item.Element("source_id").Attribute("name"), out sourceType))
                {
                  currType.Source = sourceType;
                  sourceType.Relationships.Add(currType);
                }
              }
              break;
            default: //"ItemType"
              _itemTypes.Add(new ItemType(item.Element("name").InnerText));
              break;
          }
        }
      }
      catch (Exception ex)
      {
        Debug.Print(ex.ToString());
      }
    }

    /// <summary>
    /// Loads the property metadata for the current type into the schema.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="properties">The properties.</param>
    private void LoadProperties(ItemType type, IEnumerable<XmlElement> properties)
    {
      if (properties.Any())
      {
        var props = properties.Single().ElementsByXPath("Relationships/Item[@type='Property']");
        Property newProp = null;
        foreach (var prop in props)
        {
          newProp = new Property(prop.Element("name").InnerText);
          newProp.SetType(prop.Element("data_type").InnerText);
          if (newProp.Type == PropertyType.item && prop.Element("data_source").Attribute("name") != null)
          {
            newProp.Restrictions.Add(prop.Element("data_source").Attribute("name"));
          }
          type.Properties.Add(newProp);
        }
      }
    }

    private List<string> SelectPath(string selectStr, out string partial)
    {
      partial = null;
      var lastOperator = -1;
      var path = new List<string>();

      for (var i = 0; i < selectStr.Length; i++)
      {
        if (selectStr[i] == '(' || selectStr[i] == ')' || selectStr[i] == ',')
        {
          switch (selectStr[i])
          {
            case '(':
              path.Add(selectStr.Substring(lastOperator + 1, i - lastOperator - 1).Trim());
              break;
            case ')':
              path.RemoveAt(path.Count - 1);
              break;
          }
          lastOperator = i;
        }
      }
      if (lastOperator < selectStr.Length - 1)
      {
        partial = selectStr.Substring(lastOperator + 1).Trim();
      }
      return path;
    }

    private bool TryGetProperty(ItemType itemType, string name, out Property prop)
    {
      if (itemType.Properties.Count < 1)
        LoadProperties(itemType, GetItems("ApplyItem", string.Format("<AML><Item action=\"get\" type=\"ItemType\" select=\"name\"><name>{0}</name><Relationships><Item action=\"get\" type=\"Property\" select=\"name,label,data_type,data_source\" /></Relationships></Item></AML>", itemType.Name)));
      return itemType.Properties.TryGetValue(name, out prop);
    }

    private class AmlNode
    {
      public string LocalName { get; set; }
      public string Type { get; set; }
      public string Action { get; set; }
    }
  }
}
