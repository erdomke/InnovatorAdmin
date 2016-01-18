using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  public class ElementFactory
  {
    private IServerContext _context;

    public IServerContext LocalizationContext
    {
      get { return _context; }
    }

    public ElementFactory(IServerContext context)
    {
      _context = context;
    }

    public string FormatAmlValue(object value)
    {
      return _context.Format(value);
    }
    
    public IResult FromXml(string xml)
    {
      return new Result(this, xml);
    }
    public IResult FromXml(XmlNode xml)
    {
      return new Result(this, xml);
    }
    public IReadOnlyResult FromXml(string xml, string query, IConnection conn)
    {
      return new Result(this, xml, query, conn);
    }
    public IReadOnlyResult FromXml(Stream xml, string query, IConnection conn)
    {
      var doc = new XmlDocument();
      doc.Load(xml);
      return new Result(this, doc.DocumentElement, query, conn);
    }
    public IReadOnlyResult FromXml(XmlNode xml, string query, IConnection conn)
    {
      return new Result(this, xml, query, conn);
    }

    internal IElement ElementFromXml(string xml)
    {
      var doc = new XmlDocument();
      doc.LoadXml(xml);
      return ElementFromXml(doc.DocumentElement);
    }
    internal IElement ElementFromXml(XmlElement xml)
    {
      if (xml == null) return null;
      if (xml.LocalName == "Envelope" && (xml.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || string.IsNullOrEmpty(xml.NamespaceURI)))
        return null;
      if (xml.LocalName == "Item") return new Item(this, xml);
      if (xml.LocalName == "and") return new Logical(this, xml);
      if (xml.LocalName == "not") return new Logical(this, xml);
      if (xml.LocalName == "or") return new Logical(this, xml);
      if (xml.LocalName == "Relationships") return new Relationships(this, xml);
      if (xml.ParentNode != null && xml.ParentNode.LocalName == "Item")
        return new Property(this, xml);
      return new GenericElement(this, xml);
    }

    /// <summary>Create a new action attribute tag</summary>
    /// <remarks>action [String] The name of the Method (or Built in Action Method) to apply to the Item.</remarks>
    public IAttribute Action(string value)
    {
      return new Attribute(this, "action", value);
    }
    public ILogical And(params object[] content)
    {
      return new Logical(this, "and", content);
    }
    /// <summary>Create a new attribute tag with the specified name</summary>
    public IAttribute Attribute(string name)
    {
      return new Attribute(this, name);
    }
    /// <summary>Create a new attribute tag with the specified name and value</summary>
    public IAttribute Attribute(string name, object value)
    {
      return new Attribute(this, name, value);
    }
    /// <summary>Create a new classification property tag</summary>
    public IProperty Classification(params object[] content)
    {
      return new Property(this, "classification", content);
    }
    /// <summary>Create a new condition attribute tag</summary>
    public IAttribute Condition(Condition value)
    {
      return new Attribute(this, "condition", value);
    }
    /// <summary>Create a new config_id property tag</summary>
    public IProperty ConfigId(params object[] content)
    {
      return new Property(this, "config_id", content);
    }
    /// <summary>Create a new created_by_id property tag</summary>
    public IProperty CreatedById(params object[] content)
    {
      return new Property(this, "created_by_id", content);
    }
    /// <summary>Create a new created_on property tag</summary>
    public IProperty CreatedOn(params object[] content)
    {
      return new Property(this, "created_on", content);
    }
    /// <summary>Create a new css property tag</summary>
    public IProperty Css(params object[] content)
    {
      return new Property(this, "css", content);
    }
    /// <summary>Create a new current_state property tag</summary>
    public IProperty CurrentState(params object[] content)
    {
      return new Property(this, "current_state", content);
    }
    /// <summary>Create a new doGetItem attribute tag</summary>
    /// <remarks>doGetItem [Boolean] If 0 then do not perform a final get action on the Item after the server performed that action as defined by the action attribute. Default is 1.</remarks>
    public IAttribute DoGetItem(bool value)
    {
      return new Attribute(this, "doGetItem", value);
    }
    public IElement Element(string name, params object[] content)
    {
      return new GenericElement(this, name, content);
    }
    /// <summary>Create a new generation property tag</summary>
    public IProperty Generation(params object[] content)
    {
      return new Property(this, "generation", content);
    }
    /// <summary>Create a new where id attribute tag</summary>
    public IAttribute Id(string value)
    {
      return new Attribute(this, "id", value);
    }
    /// <summary>Create a new idlist attribute tag</summary>
    public IAttribute IdList(string value)
    {
      return new Attribute(this, "idlist", value);
    }
    /// <summary>Create a new generation id tag</summary>
    public IProperty IdProp(params object[] content)
    {
      return new Property(this, "id", content);
    }
    public IProperty IsCurrent(params object[] content)
    {
      return new Property(this, "is_current", content);
    }
    public IAttribute IsNull(bool value)
    {
      return new Attribute(this, "is_null", value);
    }
    /// <summary>Create a new is_released property tag</summary>
    public IProperty IsReleased(params object[] content)
    {
      return new Property(this, "is_released", content);
    }
    /// <summary>Create a new Item AML tag</summary>
    public IItem Item(params object[] content)
    {
      return new Item(this, content);
    }
    /// <summary>Create a new keyed_name property tag</summary>
    public IProperty KeyedName(params object[] content)
    {
      return new Property(this, "keyed_name", content);
    }
    /// <summary>Create a new locked_by_id property tag</summary>
    public IProperty LockedById(params object[] content)
    {
      return new Property(this, "locked_by_id", content);
    }
    /// <summary>Create a new major_rev property tag</summary>
    public IProperty MajorRev(params object[] content)
    {
      return new Property(this, "major_rev", content);
    }
    /// <summary>Create a new managed_by_id property tag</summary>
    public IProperty ManagedById(params object[] content)
    {
      return new Property(this, "managed_by_id", content);
    }
    /// <summary>Create a new maxRecords attribute tag</summary>
    public IAttribute MaxRecords(int value)
    {
      return new Attribute(this, "maxRecords", value);
    }
    /// <summary>Create a new minor_rev property tag</summary>
    public IProperty MinorRev(params object[] content)
    {
      return new Property(this, "minor_rev", content);
    }
    /// <summary>Create a new modified_by_id property tag</summary>
    public IProperty ModifiedById(params object[] content)
    {
      return new Property(this, "modified_by_id", content);
    }
    /// <summary>Create a new modified_on property tag</summary>
    public IProperty ModifiedOn(params object[] content)
    {
      return new Property(this, "modified_on", content);
    }
    /// <summary>Create a new new_version property tag</summary>
    public IProperty NewVersion(params object[] content)
    {
      return new Property(this, "new_version", content);
    }
    public NoItemsFoundException NoItemsFoundException(string type, string query)
    {
      return new NoItemsFoundException(this, type, query);
    }
    public NoItemsFoundException NoItemsFoundException(string message)
    {
      return new NoItemsFoundException(this, message);
    }
    public NoItemsFoundException NoItemsFoundException(string message, Exception innerException)
    {
      return new NoItemsFoundException(this, message, innerException);
    }
    public ILogical Not(params object[] content)
    {
      return new Logical(this, "not", content);
    }
    /// <summary>Create a new not_lockable property tag</summary>
    public IProperty NotLockable(params object[] content)
    {
      return new Property(this, "not_lockable", content);
    }
    public ILogical Or(params object[] content)
    {
      return new Logical(this, "or", content);
    }
    /// <summary>Create a new orderBy attribute tag</summary>
    public IAttribute OrderBy(string value)
    {
      return new Attribute(this, "orderBy", value);
    }
    /// <summary>Create a new owned_by_id property tag</summary>
    public IProperty OwnedById(params object[] content)
    {
      return new Property(this, "owned_by_id", content);
    }
    /// <summary>Create a new page attribute tag</summary>
    public IAttribute Page(int value)
    {
      return new Attribute(this, "page", value);
    }
    /// <summary>Create a new pagesize attribute tag</summary>
    public IAttribute PageSize(int value)
    {
      return new Attribute(this, "pagesize", value);
    }
    /// <summary>Create a new permission_id property tag</summary>
    public IProperty PermissionId(params object[] content)
    {
      return new Property(this, "permission_id", content);
    }
    /// <summary>Create a new property tag with the specified name</summary>
    public IProperty Property(string name, params object[] content)
    {
      return new Property(this, name, content);
    }
    /// <summary>Create a new queryDate attribute tag</summary>
    public IAttribute QueryDate(DateTime value)
    {
      return new Attribute(this, "queryDate", value);
    }
    /// <summary>Create a new queryType attribute tag</summary>
    public IAttribute QueryType(QueryType value)
    {
      return new Attribute(this, "queryType", value);
    }
    /// <summary>Create a new related_expand attribute tag</summary>
    public IAttribute RelatedExpand(bool value)
    {
      return new Attribute(this, "related_expand", value);
    }
    /// <summary>Create a new related_id property tag</summary>
    public IProperty RelatedId(params object[] content)
    {
      return new Property(this, "related_id", content);
    }
    /// <summary>Create a new Relationships tag</summary>
    public IRelationships Relationships(params object[] content)
    {
      return new Relationships(this, content);
    }
    public IResult Result()
    {
      return new Result(this);
    }
    public IResult Result(string value)
    {
      return new Result(this) { Value = value };
    }
    /// <summary>Create a new select attribute tag</summary>
    public IAttribute Select(string value)
    {
      return new Attribute(this, "select", value);
    }
    /// <summary>Create a new select attribute tag</summary>
    public IAttribute Select(params SubSelect[] properties)
    {
      return new Attribute(this, "select", SubSelect.ToString(properties));
    }
    /// <summary>Create a new serverEvents attribute tag</summary>
    public IAttribute ServerEvents(bool value)
    {
      return new Attribute(this, "serverEvents", value);
    }
    public ServerException ServerException(string message)
    {
      return new ServerException(this, message);
    }
    public ServerException ServerException(string message, Exception innerException)
    {
      return new ServerException(this, message, innerException);
    }
    /// <summary>Create a new state property tag</summary>
    public IProperty State(params object[] content)
    {
      return new Property(this, "state", content);
    }
    /// <summary>Create a new source_id property tag</summary>
    public IProperty SourceId(params object[] content)
    {
      return new Property(this, "source_id", content);
    }
    /// <summary>Create a new type attribute tag</summary>
    /// <remarks>type [String] The ItemType name for which the Item is an instance.</remarks>
    public IAttribute Type(string value)
    {
      return new Attribute(this, "type", value);
    }
    /// <summary>Create a new typeID attribute tag</summary>
    public IAttribute TypeId(string value)
    {
      return new Attribute(this, "typeID", value);
    }
    public ValidationException ValidationException(string message, IReadOnlyItem item, params string[] properties)
    {
      return new ValidationException(this, message, item, properties);
    }
    public ValidationException ValidationException(string message, Exception innerException, IReadOnlyItem item, params string[] properties)
    {
      return new ValidationException(this, message, innerException, item, properties);
    }
    public ValidationReportException ValidationException(string message, IReadOnlyItem item, string report)
    {
      return new ValidationReportException(this, message, item, report);
    }
    public ValidationReportException ValidationException(string message, Exception innerException, IReadOnlyItem item, string report)
    {
      return new ValidationReportException(this, message, innerException, item, report);
    }
    /// <summary>Create a new where attribute tag</summary>
    /// <remarks>where [String] Used instead of the id attribute to specify the WHERE clause for the search criteria. Include the table name with the column name using the dot notation: where="[user].first_name like 'Tom%'"</remarks>
    public IAttribute Where(string value)
    {
      return new Attribute(this, "where", value);
    }

    public string NewId()
    {
      return Guid.NewGuid().ToString("N").ToUpperInvariant();
    }

    public static readonly ElementFactory Local = new ElementFactory(new ServerContext());

    internal IAttribute Attribute(XmlAttribute attribute)
    {
      return new Attribute(this, attribute);
    }
    internal IAttribute AttributeTemplate(string name, XmlElement parent)
    {
      return new Attribute(this, parent, name);
    }
    internal IProperty PropertyTemplate(string name, string lang, XmlElement parent)
    {
      return new Property(this, parent, name, lang);
    }
    internal ServerException ServerException(XmlElement node)
    {
      var fault = Innovator.Client.ServerException.GetFaultNode(node);
      switch (fault.ChildNodes.OfType<XmlElement>().Single(e => e.LocalName == "faultcode").InnerText)
      {
        case "0":
          return new NoItemsFoundException(this, fault);
        default:
          return new ServerException(this, fault);
      }
    }
  }
}
