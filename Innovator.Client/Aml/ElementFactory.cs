using System;
using System.Collections.Generic;
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
    private IItemFactory _itemFactory;

    /// <summary>
    /// Context for serializing/deserializing native types (e.g. <c>DateTime</c>, <c>double</c>, <c>boolean</c>, etc.)
    /// </summary>
    public IServerContext LocalizationContext
    {
      get { return _context; }
    }

    public IItemFactory ItemFactory
    {
      get { return _itemFactory; }
    }

    public ElementFactory(IServerContext context, IItemFactory itemFactory = null)
    {
      _context = context;
      _itemFactory = itemFactory ?? new DefaultItemFactory();
    }

    /// <summary>
    /// Formats an AML string by substituting the @0 style parameters with the
    /// arguments specified.
    /// </summary>
    /// <param name="format">Query to format</param>
    /// <param name="args">Arguments to substitute into the query</param>
    public string FormatAml(string format, params object[] args)
    {
      var sub = new ParameterSubstitution();
      sub.AddIndexedParameters(args);
      return sub.Substitute(format, _context);
    }

    /// <summary>Return a result from an AML string</summary>
    public IResult FromXml(string xml)
    {
      using (var strReader = new StringReader(xml))
      using (var xmlReader = XmlReader.Create(strReader))
      {
        return FromXml(xmlReader);
      }
    }
    /// <summary>Return a result from a stream</summary>
    public IResult FromXml(Stream xml)
    {
      using (var xmlReader = XmlReader.Create(xml))
      {
        return FromXml(xmlReader);
      }
    }
    /// <summary>Return a result from an AML text reader</summary>
    public IResult FromXml(TextReader xml)
    {
      using (var xmlReader = XmlReader.Create(xml))
      {
        return FromXml(xmlReader);
      }
    }
    /// <summary>Return a result from an XML reader</summary>
    public IResult FromXml(XmlReader xml)
    {
      return (IResult)FromXml(xml, null, null);
    }
    /// <summary>Return a result from an AML string indicating that it is the result of a query performed on a specific connection</summary>
    public IReadOnlyResult FromXml(string xml, string query, IConnection conn)
    {
      using (var strReader = new StringReader(xml))
      using (var xmlReader = XmlReader.Create(strReader))
      {
        return FromXml(xmlReader, query, conn == null ? null : conn.Database);
      }
    }
    /// <summary>Return a result from an AML stream indicating that it is the result of a query performed on a specific connection</summary>
    public IReadOnlyResult FromXml(Stream xml, string query, IConnection conn)
    {
      using (var xmlReader = XmlReader.Create(xml))
      {
        return FromXml(xmlReader, query, conn == null ? null : conn.Database);
      }
    }
    /// <summary>Return a result from an XmlReader indicating that it is the result of a query performed on a specific connection</summary>
    public IReadOnlyResult FromXml(XmlReader xml, string query, string database)
    {
      var writer = new ResultWriter(this, database, query);

      var num = (xml.NodeType == XmlNodeType.None) ? -1 : xml.Depth;
      do
      {
        switch (xml.NodeType)
        {
          case XmlNodeType.Element:
            writer.WriteStartElement(xml.Prefix, xml.LocalName, xml.NamespaceURI);
            var empty = xml.IsEmptyElement;
            if (xml.MoveToFirstAttribute())
            {
              do
              {
                writer.WriteStartAttribute(xml.Prefix, xml.LocalName, xml.NamespaceURI);
                while (xml.ReadAttributeValue())
                {
                  if (xml.NodeType == XmlNodeType.EntityReference)
                  {
                    writer.WriteEntityRef(xml.Name);
                  }
                  else
                  {
                    writer.WriteString(xml.Value);
                  }
                }
                writer.WriteEndAttribute();
              }
              while (xml.MoveToNextAttribute());
            }
            if (empty)
            {
              writer.WriteEndElement();
            }
            break;
          case XmlNodeType.Text:
            writer.WriteString(xml.Value);
            break;
          case XmlNodeType.CDATA:
            writer.WriteCData(xml.Value);
            break;
          case XmlNodeType.EntityReference:
            writer.WriteEntityRef(xml.Name);
            break;
          case XmlNodeType.SignificantWhitespace:
            writer.WriteWhitespace(xml.Value);
            break;
          case XmlNodeType.EndElement:
            writer.WriteFullEndElement();
            break;

            //Just ignore the following
            //case XmlNodeType.Whitespace:
            //case XmlNodeType.ProcessingInstruction:
            //case XmlNodeType.XmlDeclaration:
            //case XmlNodeType.Comment:
            //case XmlNodeType.DocumentType:
        }
      }
      while (xml.Read() && (num < xml.Depth || (num == xml.Depth && xml.NodeType == XmlNodeType.EndElement)));

      return writer.Result;
    }

    /// <summary>Create a new action attribute tag</summary>
    /// <remarks>action [String] The name of the Method (or Built in Action Method) to apply to the Item.</remarks>
    public IAttribute Action(string value)
    {
      return new Attribute("action", value);
    }
    /// <summary>Create a new <c>AML</c> tag (for use with the ApplyAML method)</summary>
    public IElement Aml(params object[] content)
    {
      return new AmlElement(this, "AML", content);
    }
    /// <summary>Create a logical <c>or</c> AML tag used with 'get' queries</summary>
    public ILogical And(params object[] content)
    {
      return new Logical(this, "and", content);
    }
    /// <summary>Create a new attribute tag with the specified name</summary>
    public IAttribute Attribute(string name)
    {
      return new Attribute(name);
    }
    /// <summary>Create a new attribute tag with the specified name and value</summary>
    public IAttribute Attribute(string name, object value)
    {
      return new Attribute(name, value);
    }
    /// <summary>Create a new <c>classification</c> property</summary>
    public IProperty Classification(params object[] content)
    {
      return new Property("classification", content);
    }
    /// <summary>Create a new <c>condition</c> attribute</summary>
    public IAttribute Condition(Condition value)
    {
      return new Attribute("condition", value);
    }
    /// <summary>Create a new <c>config_id</c> property</summary>
    public IProperty ConfigId(params object[] content)
    {
      return new Property("config_id", content);
    }
    /// <summary>Create a new <c>created_by_id</c> property</summary>
    public IProperty CreatedById(params object[] content)
    {
      return new Property("created_by_id", content);
    }
    /// <summary>Create a new <c>created_on</c> property</summary>
    public IProperty CreatedOn(params object[] content)
    {
      return new Property("created_on", content);
    }
    /// <summary>Create a new <c>css</c> property</summary>
    public IProperty Css(params object[] content)
    {
      return new Property("css", content);
    }
    /// <summary>Create a new <c>current_state</c> property</summary>
    public IProperty CurrentState(params object[] content)
    {
      return new Property("current_state", content);
    }
    /// <summary>Create a new doGetItem attribute tag</summary>
    /// <remarks>doGetItem [Boolean] If 0 then do not perform a final get action on the Item after the server performed that action as defined by the action attribute. Default is 1.</remarks>
    public IAttribute DoGetItem(bool value)
    {
      return new Attribute("doGetItem", value);
    }
    /// <summary>Create a generic AML tag given a name and the content</summary>
    public IElement Element(string name, params object[] content)
    {
      return new AmlElement(this, name, content);
    }
    /// <summary>Create a new <c>generation</c> property</summary>
    public IProperty Generation(params object[] content)
    {
      return new Property("generation", content);
    }
    /// <summary>Create a new where <c>id</c> attribute</summary>
    public IAttribute Id(string value)
    {
      return new Attribute("id", value);
    }
    /// <summary>Create a new where <c>id</c> attribute</summary>
    public IAttribute Id(Guid? value)
    {
      return new Attribute("id", value);
    }
    /// <summary>Create a new <c>idlist</c> attribute</summary>
    public IAttribute IdList(string value)
    {
      return new Attribute("idlist", value);
    }
    /// <summary>Create a new <c>id</c> property</summary>
    public IProperty IdProp(params object[] content)
    {
      return new Property("id", content);
    }
    /// <summary>Create a new <c>is_current</c> property</summary>
    public IProperty IsCurrent(params object[] content)
    {
      return new Property("is_current", content);
    }
    /// <summary>Create a new <c>is_null</c> attribute</summary>
    public IAttribute IsNull(bool value)
    {
      return new Attribute("is_null", value);
    }
    /// <summary>Create a new <c>is_released</c> property</summary>
    public IProperty IsReleased(params object[] content)
    {
      return new Property("is_released", content);
    }
    /// <summary>Create a new <c>Item</c> AML tag</summary>
    public IItem Item(params object[] content)
    {
      var type = content
        .OfType<IReadOnlyAttribute>()
        .FirstOrDefault(a => a.Name == "type");
      if (type != null)
      {
        var result = _itemFactory.NewItem(this, type.Value);
        if (result != null)
        {
          result.Add(content);
          return result;
        }
      }

      return new Item(this, content);
    }
    /// <summary>Create a new <c>keyed_name</c> property</summary>
    public IProperty KeyedName(params object[] content)
    {
      return new Property("keyed_name", this, content);
    }
    /// <summary>Create a new <c>locked_by_id</c> property</summary>
    public IProperty LockedById(params object[] content)
    {
      return new Property("locked_by_id", this, content);
    }
    /// <summary>Create a new <c>major_rev</c> property</summary>
    public IProperty MajorRev(params object[] content)
    {
      return new Property("major_rev", content);
    }
    /// <summary>Create a new <c>managed_by_id</c> property</summary>
    public IProperty ManagedById(params object[] content)
    {
      return new Property("managed_by_id", content);
    }
    /// <summary>Create a new <c>maxRecords</c> attribute</summary>
    public IAttribute MaxRecords(int value)
    {
      return new Attribute("maxRecords", value);
    }
    /// <summary>Create a new <c>minor_rev</c> property</summary>
    public IProperty MinorRev(params object[] content)
    {
      return new Property("minor_rev", content);
    }
    /// <summary>Create a new <c>modified_by_id</c> property</summary>
    public IProperty ModifiedById(params object[] content)
    {
      return new Property("modified_by_id", content);
    }
    /// <summary>Create a new <c>modified_on</c> property</summary>
    public IProperty ModifiedOn(params object[] content)
    {
      return new Property("modified_on", content);
    }
    /// <summary>Create a new <c>new_version</c> property</summary>
    public IProperty NewVersion(params object[] content)
    {
      return new Property("new_version", content);
    }
    /// <summary>Creates a 'No items found' exception</summary>
    /// <param name="type">The ItemType name for the item which couldn't be found</param>
    /// <param name="query">The AML query which didn't return any results</param>
    public NoItemsFoundException NoItemsFoundException(string type, string query)
    {
      return new NoItemsFoundException(type, query);
    }
    /// <summary>Creates a 'No items found' exception</summary>
    public NoItemsFoundException NoItemsFoundException(string message)
    {
      return new NoItemsFoundException(message);
    }
    /// <summary>Creates a 'No items found' exception</summary>
    public NoItemsFoundException NoItemsFoundException(string message, Exception innerException)
    {
      return new NoItemsFoundException(message, innerException);
    }
    /// <summary>Create a logical <c>not</c> AML tag used with 'get' queries</summary>
    public ILogical Not(params object[] content)
    {
      return new Logical(this, "not", content);
    }
    /// <summary>Create a new <c>not_lockable</c> property</summary>
    public IProperty NotLockable(params object[] content)
    {
      return new Property("not_lockable", content);
    }
    /// <summary>Create a logical <c>or</c> AML tag used with 'get' queries</summary>
    public ILogical Or(params object[] content)
    {
      return new Logical(this, "or", content);
    }
    /// <summary>Create a new <c>orderBy</c> attribute</summary>
    public IAttribute OrderBy(string value)
    {
      return new Attribute("orderBy", value);
    }
    /// <summary>Create a new <c>owned_by_id</c> property</summary>
    public IProperty OwnedById(params object[] content)
    {
      return new Property("owned_by_id", content);
    }
    /// <summary>Create a new <c>page</c> attribute</summary>
    public IAttribute Page(int value)
    {
      return new Attribute("page", value);
    }
    /// <summary>Create a new <c>pagesize</c> attribute</summary>
    public IAttribute PageSize(int value)
    {
      return new Attribute("pagesize", value);
    }
    /// <summary>Create a new <c>permission_id</c> property</summary>
    public IProperty PermissionId(params object[] content)
    {
      return new Property("permission_id", content);
    }
    /// <summary>Create a new property tag with the specified name</summary>
    public IProperty Property(string name, params object[] content)
    {
      return new Property(name, content);
    }
    /// <summary>Create a new <c>queryDate</c> attribute</summary>
    public IAttribute QueryDate(DateTime value)
    {
      return new Attribute("queryDate", value);
    }
    /// <summary>Create a new <c>queryType</c> attribute</summary>
    public IAttribute QueryType(QueryType value)
    {
      return new Attribute("queryType", value);
    }
    /// <summary>Create a new <c>related_expand</c> attribute</summary>
    public IAttribute RelatedExpand(bool value)
    {
      return new Attribute("related_expand", value);
    }
    /// <summary>Create a new <c>related_id</c> property</summary>
    public IProperty RelatedId(params object[] content)
    {
      return new Property("related_id", content);
    }
    /// <summary>Create a new <c>Relationships</c> tag</summary>
    public IRelationships Relationships(params object[] content)
    {
      return new Relationships(content);
    }
    /// <summary>Create a new <c>Result</c> AML tag</summary>
    public IResult Result(params object[] content)
    {
      var result = new Result(this);
      if (!content.Any())
      {
        // do nothing
      }
      else if (content.Length == 1 && content[0] is ServerException)
      {
        result.Exception = (ServerException)content[0];
      }
      else if (content.OfType<IItem>().Any())
      {
        foreach (var item in content.OfType<IItem>())
        {
          result.Add(item);
        }
      }
      else if (content.Length == 1)
      {
        result.Value = _context.Format(content[0]);
      }
      else
      {
        throw new NotSupportedException();
      }
      return result;
    }
    /// <summary>Create a new <c>select</c> attribute</summary>
    public IAttribute Select(string value)
    {
      return new Attribute("select", value);
    }
    /// <summary>Create a new <c>select</c> attribute</summary>
    public IAttribute Select(params SubSelect[] properties)
    {
      return new Attribute("select", SubSelect.ToString(properties));
    }
    /// <summary>Create a new <c>serverEvents</c> attribute</summary>
    public IAttribute ServerEvents(bool value)
    {
      return new Attribute("serverEvents", value);
    }
    /// <summary>Create a new server exception</summary>
    public ServerException ServerException(string message)
    {
      return new ServerException(message);
    }
    /// <summary>Create a new server exception</summary>
    public ServerException ServerException(string message, Exception innerException)
    {
      return new ServerException(message, innerException);
    }
    /// <summary>Create a new <c>state</c> property</summary>
    public IProperty State(params object[] content)
    {
      return new Property("state", content);
    }
    /// <summary>Create a new <c>source_id</c> property</summary>
    public IProperty SourceId(params object[] content)
    {
      return new Property("source_id", content);
    }
    /// <summary>Create a new <c>type</c> attribute</summary>
    /// <remarks>type [String] The ItemType name for which the Item is an instance.</remarks>
    public IAttribute Type(string value)
    {
      return new Attribute("type", value);
    }
    /// <summary>Create a new <c>typeID</c> attribute</summary>
    public IAttribute TypeId(string value)
    {
      return new Attribute("typeID", value);
    }
    /// <summary>Create a new validation exception</summary>
    public ValidationException ValidationException(string message, IReadOnlyItem item, params string[] properties)
    {
      return new ValidationException(message, item, properties);
    }
    /// <summary>Create a new validation exception</summary>
    public ValidationException ValidationException(string message, Exception innerException, IReadOnlyItem item, params string[] properties)
    {
      return new ValidationException(message, innerException, item, properties);
    }
    /// <summary>Create a new validation exception</summary>
    public ValidationReportException ValidationException(string message, IReadOnlyItem item, string report)
    {
      return new ValidationReportException(message, item, report);
    }
    /// <summary>Create a new validation exception</summary>
    public ValidationReportException ValidationException(string message, Exception innerException, IReadOnlyItem item, string report)
    {
      return new ValidationReportException(message, innerException, item, report);
    }
    /// <summary>Create a new <c>where</c> attribute</summary>
    /// <remarks>where [String] Used instead of the id attribute to specify the WHERE clause for the search criteria. Include the table name with the column name using the dot notation: where="[user].first_name like 'Tom%'"</remarks>
    public IAttribute Where(string value)
    {
      return new Attribute("where", value);
    }

    /// <summary>Generate a new GUID id</summary>
    public string NewId()
    {
      return Guid.NewGuid().ToString("N").ToUpperInvariant();
    }

    /// <summary>Generate a new factory assuming the local time zone and culture</summary>
    public static readonly ElementFactory Local = new ElementFactory(new ServerContext());
  }
}
