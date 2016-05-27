using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  /// <summary>
  /// Class for generated mutable AML objects
  /// </summary>
  /// <example>
  /// <code lang="C#">
  /// var aml = conn.AmlContext;
  /// // --- OR ---
  /// var aml = ElementFactory.Local;
  /// 
  /// IItem myItem = aml.Item(aml.Type(myType), aml.Action(myAction));
  /// IResult myResult = aml.Result(resultText);
  /// ServerException = aml.ServerException(errorMessage); 
  /// </code>
  /// </example>
  public class ElementFactory
  {
    private IServerContext _context;

    /// <summary>
    /// Context for serializing/deserializing native types (e.g. <c>DateTime</c>, <c>double</c>, <c>boolean</c>, etc.)
    /// </summary>
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

    /// <summary>Return a result from an AML string</summary>
    public IResult FromXml(string xml)
    {
      return new Result(this, xml);
    }
    /// <summary>Return a result from an AML node</summary>
    public IResult FromXml(XmlNode xml)
    {
      return new Result(this, xml);
    }
    /// <summary>Return a result from an AML string indicating that it is the result of a query performed on a specific connection</summary>
    public IReadOnlyResult FromXml(string xml, string query, IConnection conn)
    {
      return new Result(this, xml, query, conn);
    }
    /// <summary>Return a result from an AML stream indicating that it is the result of a query performed on a specific connection</summary>
    public IReadOnlyResult FromXml(Stream xml, string query, IConnection conn)
    {
      var doc = new XmlDocument();
      doc.Load(xml);
      return new Result(this, doc.DocumentElement, query, conn);
    }
    /// <summary>Return a result from an AML node indicating that it is the result of a query performed on a specific connection</summary>
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
    /// <summary>Create a new <c>AML</c> tag (for use with the ApplyAML method)</summary>
    public IElement Aml(params object[] content)
    {
      return new GenericElement(this, "AML", content);
    }
    /// <summary>Create a logical <c>or</c> AML tag used with 'get' queries</summary>
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
    /// <summary>Create a new <c>classification</c> property</summary>
    public IProperty Classification(params object[] content)
    {
      return new Property(this, "classification", content);
    }
    /// <summary>Create a new <c>condition</c> attribute</summary>
    public IAttribute Condition(Condition value)
    {
      return new Attribute(this, "condition", value);
    }
    /// <summary>Create a new <c>config_id</c> property</summary>
    public IProperty ConfigId(params object[] content)
    {
      return new Property(this, "config_id", content);
    }
    /// <summary>Create a new <c>created_by_id</c> property</summary>
    public IProperty CreatedById(params object[] content)
    {
      return new Property(this, "created_by_id", content);
    }
    /// <summary>Create a new <c>created_on</c> property</summary>
    public IProperty CreatedOn(params object[] content)
    {
      return new Property(this, "created_on", content);
    }
    /// <summary>Create a new <c>css</c> property</summary>
    public IProperty Css(params object[] content)
    {
      return new Property(this, "css", content);
    }
    /// <summary>Create a new <c>current_state</c> property</summary>
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
    /// <summary>Create a generic AML tag given a name and the content</summary>
    public IElement Element(string name, params object[] content)
    {
      return new GenericElement(this, name, content);
    }
    /// <summary>Create a new <c>generation</c> property</summary>
    public IProperty Generation(params object[] content)
    {
      return new Property(this, "generation", content);
    }
    /// <summary>Create a new where <c>id</c> attribute</summary>
    public IAttribute Id(string value)
    {
      return new Attribute(this, "id", value);
    }
    /// <summary>Create a new <c>idlist</c> attribute</summary>
    public IAttribute IdList(string value)
    {
      return new Attribute(this, "idlist", value);
    }
    /// <summary>Create a new <c>id</c> property</summary>
    public IProperty IdProp(params object[] content)
    {
      return new Property(this, "id", content);
    }
    /// <summary>Create a new <c>is_current</c> property</summary>
    public IProperty IsCurrent(params object[] content)
    {
      return new Property(this, "is_current", content);
    }
    /// <summary>Create a new <c>is_null</c> attribute</summary>
    public IAttribute IsNull(bool value)
    {
      return new Attribute(this, "is_null", value);
    }
    /// <summary>Create a new <c>is_released</c> property</summary>
    public IProperty IsReleased(params object[] content)
    {
      return new Property(this, "is_released", content);
    }
    /// <summary>Create a new <c>Item</c> AML tag</summary>
    public IItem Item(params object[] content)
    {
      return new Item(this, content);
    }
    /// <summary>Create a new <c>keyed_name</c> property</summary>
    public IProperty KeyedName(params object[] content)
    {
      return new Property(this, "keyed_name", content);
    }
    /// <summary>Create a new <c>locked_by_id</c> property</summary>
    public IProperty LockedById(params object[] content)
    {
      return new Property(this, "locked_by_id", content);
    }
    /// <summary>Create a new <c>major_rev</c> property</summary>
    public IProperty MajorRev(params object[] content)
    {
      return new Property(this, "major_rev", content);
    }
    /// <summary>Create a new <c>managed_by_id</c> property</summary>
    public IProperty ManagedById(params object[] content)
    {
      return new Property(this, "managed_by_id", content);
    }
    /// <summary>Create a new <c>maxRecords</c> attribute</summary>
    public IAttribute MaxRecords(int value)
    {
      return new Attribute(this, "maxRecords", value);
    }
    /// <summary>Create a new <c>minor_rev</c> property</summary>
    public IProperty MinorRev(params object[] content)
    {
      return new Property(this, "minor_rev", content);
    }
    /// <summary>Create a new <c>modified_by_id</c> property</summary>
    public IProperty ModifiedById(params object[] content)
    {
      return new Property(this, "modified_by_id", content);
    }
    /// <summary>Create a new <c>modified_on</c> property</summary>
    public IProperty ModifiedOn(params object[] content)
    {
      return new Property(this, "modified_on", content);
    }
    /// <summary>Create a new <c>new_version</c> property</summary>
    public IProperty NewVersion(params object[] content)
    {
      return new Property(this, "new_version", content);
    }
    /// <summary>Creates a 'No items found' exception</summary>
    /// <param name="type">The ItemType name for the item which couldn't be found</param>
    /// <param name="query">The AML query which didn't return any results</param>
    public NoItemsFoundException NoItemsFoundException(string type, string query)
    {
      return new NoItemsFoundException(this, type, query);
    }
    /// <summary>Creates a 'No items found' exception</summary>
    public NoItemsFoundException NoItemsFoundException(string message)
    {
      return new NoItemsFoundException(this, message);
    }
    /// <summary>Creates a 'No items found' exception</summary>
    public NoItemsFoundException NoItemsFoundException(string message, Exception innerException)
    {
      return new NoItemsFoundException(this, message, innerException);
    }
    /// <summary>Create a logical <c>not</c> AML tag used with 'get' queries</summary>
    public ILogical Not(params object[] content)
    {
      return new Logical(this, "not", content);
    }
    /// <summary>Create a new <c>not_lockable</c> property</summary>
    public IProperty NotLockable(params object[] content)
    {
      return new Property(this, "not_lockable", content);
    }
    /// <summary>Create a logical <c>or</c> AML tag used with 'get' queries</summary>
    public ILogical Or(params object[] content)
    {
      return new Logical(this, "or", content);
    }
    /// <summary>Create a new <c>orderBy</c> attribute</summary>
    public IAttribute OrderBy(string value)
    {
      return new Attribute(this, "orderBy", value);
    }
    /// <summary>Create a new <c>owned_by_id</c> property</summary>
    public IProperty OwnedById(params object[] content)
    {
      return new Property(this, "owned_by_id", content);
    }
    /// <summary>Create a new <c>page</c> attribute</summary>
    public IAttribute Page(int value)
    {
      return new Attribute(this, "page", value);
    }
    /// <summary>Create a new <c>pagesize</c> attribute</summary>
    public IAttribute PageSize(int value)
    {
      return new Attribute(this, "pagesize", value);
    }
    /// <summary>Create a new <c>permission_id</c> property</summary>
    public IProperty PermissionId(params object[] content)
    {
      return new Property(this, "permission_id", content);
    }
    /// <summary>Create a new property tag with the specified name</summary>
    public IProperty Property(string name, params object[] content)
    {
      return new Property(this, name, content);
    }
    /// <summary>Create a new <c>queryDate</c> attribute</summary>
    public IAttribute QueryDate(DateTime value)
    {
      return new Attribute(this, "queryDate", value);
    }
    /// <summary>Create a new <c>queryType</c> attribute</summary>
    public IAttribute QueryType(QueryType value)
    {
      return new Attribute(this, "queryType", value);
    }
    /// <summary>Create a new <c>related_expand</c> attribute</summary>
    public IAttribute RelatedExpand(bool value)
    {
      return new Attribute(this, "related_expand", value);
    }
    /// <summary>Create a new <c>related_id</c> property</summary>
    public IProperty RelatedId(params object[] content)
    {
      return new Property(this, "related_id", content);
    }
    /// <summary>Create a new <c>Relationships</c> tag</summary>
    public IRelationships Relationships(params object[] content)
    {
      return new Relationships(this, content);
    }
    /// <summary>Create a new <c>Result</c> AML tag</summary>
    public IResult Result(params object[] content)
    {
      var result = new Result(this);
      result.Add(content);
      return result;
    }
    /// <summary>Create a new <c>select</c> attribute</summary>
    public IAttribute Select(string value)
    {
      return new Attribute(this, "select", value);
    }
    /// <summary>Create a new <c>select</c> attribute</summary>
    public IAttribute Select(params SubSelect[] properties)
    {
      return new Attribute(this, "select", SubSelect.ToString(properties));
    }
    /// <summary>Create a new <c>serverEvents</c> attribute</summary>
    public IAttribute ServerEvents(bool value)
    {
      return new Attribute(this, "serverEvents", value);
    }
    /// <summary>Create a new server exception</summary>
    public ServerException ServerException(string message)
    {
      return new ServerException(this, message);
    }
    /// <summary>Create a new server exception</summary>
    public ServerException ServerException(string message, Exception innerException)
    {
      return new ServerException(this, message, innerException);
    }
    /// <summary>Create a new <c>state</c> property</summary>
    public IProperty State(params object[] content)
    {
      return new Property(this, "state", content);
    }
    /// <summary>Create a new <c>source_id</c> property</summary>
    public IProperty SourceId(params object[] content)
    {
      return new Property(this, "source_id", content);
    }
    /// <summary>Create a new <c>type</c> attribute</summary>
    /// <remarks>type [String] The ItemType name for which the Item is an instance.</remarks>
    public IAttribute Type(string value)
    {
      return new Attribute(this, "type", value);
    }
    /// <summary>Create a new <c>typeID</c> attribute</summary>
    public IAttribute TypeId(string value)
    {
      return new Attribute(this, "typeID", value);
    }
    /// <summary>Create a new validation exception</summary>
    public ValidationException ValidationException(string message, IReadOnlyItem item, params string[] properties)
    {
      return new ValidationException(this, message, item, properties);
    }
    /// <summary>Create a new validation exception</summary>
    public ValidationException ValidationException(string message, Exception innerException, IReadOnlyItem item, params string[] properties)
    {
      return new ValidationException(this, message, innerException, item, properties);
    }
    /// <summary>Create a new validation exception</summary>
    public ValidationReportException ValidationException(string message, IReadOnlyItem item, string report)
    {
      return new ValidationReportException(this, message, item, report);
    }
    /// <summary>Create a new validation exception</summary>
    public ValidationReportException ValidationException(string message, Exception innerException, IReadOnlyItem item, string report)
    {
      return new ValidationReportException(this, message, innerException, item, report);
    }
    /// <summary>Create a new <c>where</c> attribute</summary>
    /// <remarks>where [String] Used instead of the id attribute to specify the WHERE clause for the search criteria. Include the table name with the column name using the dot notation: where="[user].first_name like 'Tom%'"</remarks>
    public IAttribute Where(string value)
    {
      return new Attribute(this, "where", value);
    }

    /// <summary>Generate a new GUID id</summary>
    public string NewId()
    {
      return Guid.NewGuid().ToString("N").ToUpperInvariant();
    }

    /// <summary>Generate a new factory assuming the local time zone and culture</summary>
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
