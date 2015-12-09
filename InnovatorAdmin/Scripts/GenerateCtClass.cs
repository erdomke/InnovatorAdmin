using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using System.ComponentModel;
using Microsoft.VisualBasic;

namespace InnovatorAdmin.Scripts
{
  [DisplayName("Generate CT+ Class"), Description("Generate a CT+ class file for the given Item Type"), DefaultProperty("ItemType")]
  public class GenerateCtClass : IAsyncScript
  {
    [ DisplayName("Item Type")
    , Category("Main")
    , Description("The name of the item type to generate a class for")]
    public string ItemType { get; set; }
    [ DisplayName("Merge File Path")
    , Category("Main")
    , Description("The path to the file to merge the results into. {0} will be replaced with the ItemType name (spaces removed).")]
    public string MergePath { get; set; }
    [ DisplayName("Is Base?")
    , Category("Optional")
    , DefaultValue(false)
    , Description("If the class is core to Aras and will go in Gentex.Data.Aras")]
    public bool IsBase { get; set; }
    [ DisplayName("Get Properties")
    , Category("Optional")
    , DefaultValue(false)
    , Description("Whether to hard code the property access (which migh help with performance) such as is done with the Part model")]
    public bool GetProperties { get; set; }
    [ DisplayName("Include Sort Order")
    , Category("Optional")
    , DefaultValue(false)
    , Description("Whether or not to include the sort_order property in the class")]
    public bool IncludeSortOrder { get; set; }

    public GenerateCtClass()
    {
      this.MergePath = @"C:\path\to\file\{0}.vb";
    }


    public IPromise<string> Execute(IAsyncConnection conn)
    {
      return ExecuteAsync(conn).ToPromise();
    }

    public async Task<string> ExecuteAsync(IAsyncConnection conn)
    {
      var defaultProps = new List<string>() {
        "classification",
        "config_id",
        "created_by_id",
        "created_on",
        "css",
        "current_state",
        "effective_date",
        "generation",
        "id",
        "is_current",
        "is_released",
        "keyed_name",
        "locked_by_id",
        "major_rev",
        "managed_by_id",
        "minor_rev",
        "modified_by_id",
        "modified_on",
        "new_version",
        "not_lockable",
        "owned_by_id",
        "permission_id",
        "release_date",
        "state",
        "superseded_date",
        "team_id",
        "related_id",
        "source_id",
        "behavior",
        "itemtype"
      };
      if (!this.IncludeSortOrder)
        defaultProps.Add("sort_order");

      var info = await conn.ApplyAsync(@"<AML>
                                          <Item type='ItemType' action='get'>
                                            <name>@0</name>
                                            <Relationships>
                                              <Item action='get' type='Property' select='label,name,is_required,is_class_required,data_type,data_source,readonly,pattern,stored_length,foreign_property(label,name,is_required,is_class_required,data_type,data_source,readonly,pattern,stored_length)'></Item>
                                            </Relationships>
                                          </Item>
                                        </AML>", true, true, this.ItemType).ToTask();
      var itemTypeInfo = info.AssertItem();
      var itemProps = new List<ArasProperty>();
      var classBuilder = new StringBuilder();
      var polyItem = itemTypeInfo.Property("implementation_type").AsString("") == "polymorphic";

      if (!this.IsBase) classBuilder.AppendLine("Imports Gentex.ComponentTracker.Model");
      classBuilder.AppendLine("Imports Gentex.Data.Aras.Model");
      classBuilder.AppendLine("Imports Gentex.Data.Base.Model");
      classBuilder.AppendLine();
      if (this.IsBase)
        classBuilder.AppendLine("Namespace Aras.Model");
      else
        classBuilder.AppendLine("Namespace Model");
      classBuilder.AppendFormat(@"  <SourceName(""{0}"")> _", this.ItemType).AppendLine();
      if (itemTypeInfo.Property("is_relationship").AsBoolean(false))
      {
        classBuilder.AppendFormat("  Public Class {0}", Strings.StrConv(this.ItemType, VbStrConv.ProperCase).Replace(" ", "")).AppendLine();
        var rel = await conn.ApplyAsync(@"<AML>
                                            <Item type='RelationshipType' action='get' select='source_id,related_id' related_expand='0'>
                                              <relationship_id>@0</relationship_id>
                                            </Item>
                                          </AML>", true, true, itemTypeInfo.Id()).ToTask();
        var relTypeInfo = rel.AssertItem();
        if (!relTypeInfo.RelatedId().KeyedName().HasValue())
          classBuilder.AppendFormat("    Inherits NullRelationship(Of {0})",
                                    Strings.StrConv(relTypeInfo.SourceId().Attribute("name").Value, VbStrConv.ProperCase).Replace(" ", ""))
                      .AppendLine();
        else
          classBuilder.AppendFormat("    Inherits Relationship(Of {0}, {1})",
                                    Strings.StrConv(relTypeInfo.SourceId().Attribute("name").Value, VbStrConv.ProperCase).Replace(" ", ""),
                                    Strings.StrConv(relTypeInfo.RelatedId().Attribute("name").Value, VbStrConv.ProperCase).Replace(" ", ""))
                      .AppendLine();
      }
      else if (polyItem)
      {
        classBuilder.AppendFormat("  Public Interface I{0}", Strings.StrConv(this.ItemType, VbStrConv.ProperCase).Replace(" ", "")).AppendLine();
        if (itemTypeInfo.Property("is_versionable").AsBoolean(false))
          classBuilder.AppendLine("    Inherits IVersionableItem");
        else
          classBuilder.AppendLine("    Inherits IItem");
      }
      else
      {
        classBuilder.AppendFormat("  Public Class {0}", Strings.StrConv(this.ItemType, VbStrConv.ProperCase).Replace(" ", "")).AppendLine();
        if (itemTypeInfo.Property("is_versionable").AsBoolean(false))
          classBuilder.AppendLine("    Inherits VersionableItem");
        else
          classBuilder.AppendLine("    Inherits Item");
      }
      classBuilder.AppendLine();

      ArasProperty arasProp;
      foreach (var prop in itemTypeInfo.Relationships("Property"))
      {
        if (!defaultProps.Contains(prop.Property("name").AsString("")))
        {
          arasProp = await ArasProperty.NewProp(prop, conn);
          itemProps.Add(arasProp);
        }
      }

      if (!polyItem)
      {
        itemProps.Sort(SortVariable);
        foreach (var prop in itemProps)
        {
          if (prop.PropType == ArasProperty.PropTypes.ReadOnly)
          {
            classBuilder.AppendFormat(@"    Private {0} As New ReadOnlyPropertyValue(Of {1})(""{2}"", Me)", prop.VarName, prop.DataType, prop.Name).AppendLine();
          }
          else if (prop.PropType == ArasProperty.PropTypes.Normal)
          {
            classBuilder.AppendFormat(@"    Private {0} As New PropertyValue(Of {1})(""{2}"", Me, {3})", prop.VarName, prop.DataType, prop.Name, prop.Required).AppendLine();
          }
        }
        classBuilder.AppendLine();

        var foreignProps = itemProps.Where(p => p.PropType == ArasProperty.PropTypes.Foreign);
        if (foreignProps.Any())
        {
          foreach (var prop in foreignProps)
          {
            classBuilder.AppendFormat(@"    Private {0} As New ForeignPropertyValue(Of {1}, {2})(""{3}"", Me, {4}, Function(item) item.{5})"
              , prop.VarName, prop.ForeignLinkProp.DataType, prop.DataType, prop.Name, prop.ForeignLinkProp.VarName, prop.ForeignProp.PropName)
              .AppendLine();
          }
          classBuilder.AppendLine();
        }
      }

      itemProps.Sort(SortProperty);
      foreach (var prop in itemProps)
      {
        classBuilder.AppendLine("    ''' <summary>");
        classBuilder.AppendFormat("    ''' Gets the {0}.", prop.Label.ToLower().Replace("&", "&amp;")).AppendLine();
        classBuilder.AppendLine("    ''' </summary>");

        classBuilder.AppendFormat(@"    <DisplayName(""{0}""), SourceName(""{1}"")", prop.Label, prop.Name);
        if (!String.IsNullOrEmpty(prop.List))
        {
          classBuilder.AppendFormat(@", List(""{0}""", prop.List);
          if (!String.IsNullOrEmpty(prop.ListFilter))
          {
            classBuilder.AppendFormat(@", ""{0}""", prop.ListFilter);
          }
          if (prop.EbsList)
          {
            if (String.IsNullOrEmpty(prop.ListFilter)) classBuilder.Append(", ");
            classBuilder.Append(", True");
          }
          classBuilder.Append(")");
        }
        if (prop.StringLength > 0)
          classBuilder.AppendFormat(", StringField({0})", prop.StringLength);
        classBuilder.Append(">");
        classBuilder.AppendLine();

        classBuilder.Append("    ");
        if (!polyItem) classBuilder.Append("Public ");

        switch (prop.PropType)
        {
          case ArasProperty.PropTypes.ReadOnly:
            classBuilder.AppendFormat("ReadOnly Property {0} As ReadOnlyPropertyValue(Of {1})", prop.PropName, prop.DataType).AppendLine();
            break;
          case ArasProperty.PropTypes.Foreign:
            classBuilder.AppendFormat("ReadOnly Property {0} As IPropertyValue(Of {1})", prop.PropName, prop.DataType).AppendLine();
            break;
          default:
            classBuilder.AppendFormat("ReadOnly Property {0} As PropertyValue(Of {1})", prop.PropName, prop.DataType).AppendLine();
            break;
        }

        if (!polyItem)
        {
          classBuilder.AppendLine("      Get");
          classBuilder.AppendFormat("        Return {0}", prop.VarName).AppendLine();
          classBuilder.AppendLine("      End Get");
          classBuilder.AppendLine("    End Property");
        }
      }

      classBuilder.AppendLine();
      if (polyItem)
      {
        classBuilder.AppendLine("  End Interface");
      }
      else
      {
        if (this.IsBase)
          classBuilder.AppendLine("    Public Sub New(ByVal builder As Base.Model.IModelBuilder)");
        else
          classBuilder.AppendLine("    Public Sub New(ByVal builder As IModelBuilder)");

        classBuilder.AppendLine("      MyBase.New(builder)");
        classBuilder.AppendLine("    End Sub");

        if (this.GetProperties)
        {
          classBuilder.AppendLine();
          classBuilder.AppendLine("    Private _getter As New PropGetter(Me)");
          classBuilder.AppendLine("    Protected Overrides Function GetPropertyGetter() As Gentex.Data.Base.Model.IModelPropertyGetter");
          classBuilder.AppendLine("      Return _getter");
          classBuilder.AppendLine("    End Function");
          classBuilder.AppendLine();
          classBuilder.AppendLine("    Private Class PropGetter");
          classBuilder.AppendLine("      Implements IModelPropertyGetter");
          classBuilder.AppendLine();
          classBuilder.AppendFormat("      Private _parent As {0}", Strings.StrConv(this.ItemType, VbStrConv.ProperCase).Replace(" ", "")).AppendLine();
          classBuilder.AppendLine();
          classBuilder.AppendLine("      Public ReadOnly Property SupportsByName As Boolean Implements IModelPropertyGetter.SupportsByName");
          classBuilder.AppendLine("        Get");
          classBuilder.AppendLine("          Return True");
          classBuilder.AppendLine("        End Get");
          classBuilder.AppendLine("      End Property");
          classBuilder.AppendLine();
          classBuilder.AppendFormat("      Public Sub New(parent as {0})", Strings.StrConv(this.ItemType, VbStrConv.ProperCase).Replace(" ", "")).AppendLine();
          classBuilder.AppendLine("        _parent = parent");
          classBuilder.AppendLine("      End Sub");
          classBuilder.AppendLine();
          classBuilder.AppendLine("      Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of PropertyValueInfo) Implements System.Collections.Generic.IEnumerable(Of Gentex.Data.Base.Model.PropertyValueInfo).GetEnumerator");
          classBuilder.AppendLine("        Dim props As New List(Of Data.Base.Model.PropertyValueInfo)");
          itemProps.Sort((x, y) => x.VarName.CompareTo(y.VarName));
          foreach (var prop in itemProps)
          {
            classBuilder.AppendLine(String.Format("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent.{0}, Nothing))", prop.VarName));
          }
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._configId, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._generation, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._isCurrent, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._isReleased, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._majorRev, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._minorRev, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._releaseDate, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._supersededDate, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._classification, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._createdBy, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._createdOn, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._currentState, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._effectiveDate, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._lockedBy, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._managedBy, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._modifiedBy, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._modifiedOn, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._notLockable, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._ownedBy, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._permission, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._state, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._team, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._id, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._keyedName, Nothing))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._canDelete, New LazyLoadAttribute(True, False)))");
          classBuilder.AppendLine("        props.Add(New Data.Base.Model.PropertyValueInfo(_parent._canUpdate, New LazyLoadAttribute(True, False)))");
          classBuilder.AppendLine("        Return props");
          classBuilder.AppendLine("      End Function");
          classBuilder.AppendLine();
          classBuilder.AppendLine("      Public Function ByName(name As String) As Base.Model.PropertyValueInfo Implements Gentex.Data.Base.Model.IModelPropertyGetter.ByName");
          classBuilder.AppendLine("        Select Case name");
          foreach (var prop in itemProps)
          {
            classBuilder.AppendLine(String.Format(@"          Case ""{0}""", prop.Name));
            classBuilder.AppendLine(String.Format("            Return New Data.Base.Model.PropertyValueInfo(_parent.{0}, Nothing)", prop.VarName));
          }
          classBuilder.AppendLine("          Case Field_ConfigId");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._configId, Nothing)");
          classBuilder.AppendLine("          Case Field_Generation");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._generation, Nothing)");
          classBuilder.AppendLine("          Case \"is_current\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._isCurrent, Nothing)");
          classBuilder.AppendLine("          Case \"is_released\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._isReleased, Nothing)");
          classBuilder.AppendLine("          Case Field_MajorRev");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._majorRev, Nothing)");
          classBuilder.AppendLine("          Case Field_MinorRev");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._minorRev, Nothing)");
          classBuilder.AppendLine("          Case \"release_date\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._releaseDate, Nothing)");
          classBuilder.AppendLine("          Case \"superseded_date\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._supersededDate, Nothing)");
          classBuilder.AppendLine("          Case Field_Classification");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._classification, Nothing)");
          classBuilder.AppendLine("          Case \"created_by_id\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._createdBy, Nothing)");
          classBuilder.AppendLine("          Case \"created_on\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._createdOn, Nothing)");
          classBuilder.AppendLine("          Case \"current_state\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._currentState, Nothing)");
          classBuilder.AppendLine("          Case \"effective_date\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._effectiveDate, Nothing)");
          classBuilder.AppendLine("          Case \"locked_by_id\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._lockedBy, Nothing)");
          classBuilder.AppendLine("          Case \"managed_by_id\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._managedBy, Nothing)");
          classBuilder.AppendLine("          Case \"modified_by_id\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._modifiedBy, Nothing)");
          classBuilder.AppendLine("          Case \"modified_on\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._modifiedOn, Nothing)");
          classBuilder.AppendLine("          Case \"not_lockable\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._notLockable, Nothing)");
          classBuilder.AppendLine("          Case \"owned_by_id\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._ownedBy, Nothing)");
          classBuilder.AppendLine("          Case \"permission_id\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._permission, Nothing)");
          classBuilder.AppendLine("          Case Field_State");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._state, Nothing)");
          classBuilder.AppendLine("          Case \"team_id\"");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._team, Nothing)");
          classBuilder.AppendLine("          Case Field_PermissionCanDelete");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._canDelete, New LazyLoadAttribute(True, False))");
          classBuilder.AppendLine("          Case Field_PermissionCanUpdate");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._canUpdate, New LazyLoadAttribute(True, False))");
          classBuilder.AppendLine("          Case Field_Id");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._id, Nothing)");
          classBuilder.AppendLine("          Case Field_KeyedName");
          classBuilder.AppendLine("            Return New Data.Base.Model.PropertyValueInfo(_parent._keyedName, Nothing)");
          classBuilder.AppendLine("          Case Else");
          classBuilder.AppendLine("            Return Nothing");
          classBuilder.AppendLine("        End Select");
          classBuilder.AppendLine("      End Function");
          classBuilder.AppendLine();
          classBuilder.AppendLine("      Private Function GetEnumeratorCore() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator");
          classBuilder.AppendLine("        Return GetEnumerator()");
          classBuilder.AppendLine("      End Function");
          classBuilder.AppendLine("    End Class");
        }

        classBuilder.AppendLine("  End Class");
      }
      classBuilder.AppendLine("End Namespace");
      return classBuilder.ToString();
    }

    private int SortVariable(ArasProperty x, ArasProperty y)
    {
      return x.VarName.CompareTo(y.VarName);
    }
    private int SortProperty(ArasProperty x, ArasProperty y)
    {
      return x.PropName.CompareTo(y.PropName);
    }

    private class ArasProperty
    {
      public enum PropTypes
      {
        Foreign,
        ReadOnly,
        Normal
      }

      public string DataType { get; set; }
      public bool EbsList { get; set; }
      public ArasProperty ForeignProp { get; set; }
      public ArasProperty ForeignLinkProp { get; set; }
      public string Label { get; set; }
      public string List { get; set; }
      public string ListFilter { get; set; }
      public string Name { get; set; }
      public string PropName { get; set; }
      public PropTypes PropType { get; set; }
      public bool Required { get; set; }
      public int StringLength { get; set; }
      public string VarName { get; set; }

      private ArasProperty() { }

      private static Dictionary<string, ArasProperty> _cache = new Dictionary<string, ArasProperty>();

      private static ArasProperty NewProp(string id)
      {
        ArasProperty prop = null;
        if (!_cache.TryGetValue(id, out prop))
        {
          prop = new ArasProperty();
          _cache[id] = prop;
        }
        return prop;
      }
      public static async Task<ArasProperty> NewProp(IReadOnlyItem item, IAsyncConnection conn)
      {
        var prop = NewProp(item.Id());
        await prop.Initialize(item, conn);
        return prop;
      }

      public async Task Initialize(IReadOnlyItem item, IAsyncConnection conn)
      {
        this.Label = item.Property("label").AsString("");
        this.Name = item.Property("name").AsString("");
        this.Required = item.Property("is_required").AsBoolean(false)
          || item.Property("is_class_required").AsBoolean(false);
        this.PropName = LabelToProp(this.Label);
        this.VarName = NameToVar(this.Name);

        if (string.IsNullOrEmpty(this.PropName))
          this.PropName = LabelToProp(this.Name);
        if (string.IsNullOrEmpty(this.Label))
          this.Label = Strings.StrConv(this.Name.Replace('_', ' '), VbStrConv.ProperCase);

        if (item.Property("data_type").Value == "foreign")
        {
          this.PropType = PropTypes.Foreign;
          if (!item.Property("foreign_property").HasValue())
          {
            var result = await conn.ApplyAsync(@"<AML>
                                            <Item type='Property' action='get' id='@0' select='label,name,is_required,is_class_required,data_type,data_source,readonly,pattern,stored_length,foreign_property(label,name,is_required,is_class_required,data_type,data_source,readonly,pattern,stored_length)'>
                                            </Item>
                                          </AML>", true, true, item.Id()).ToTask();
            item = result.AssertItem();
          }
          ForeignProp = await NewProp(item.Property("foreign_property").AsItem(), conn);
          ForeignLinkProp = NewProp(item.Property("data_source").Value);
        }
        else if (item.Property("readonly").AsBoolean(false))
        {
          PropType = PropTypes.ReadOnly;
        }
        else
        {
          PropType = PropTypes.Normal;
        }

        if (this.ForeignProp != null)
        {
          this.DataType = ForeignProp.DataType;
        }
        else
        {
          switch (item.Property("data_type").AsString("").ToLowerInvariant())
          {
            case "decimal":
            case "float":
              DataType = "Double";
              break;
            case "date":
              DataType = "Date";
              break;
            case "item":
              DataType = Strings.StrConv(item.Property("data_source").KeyedName().Value, VbStrConv.ProperCase)
                .Replace(" ", "");
              if (item.Property("data_source").Value == "0C8A70AE86AF49AD873F817593F893D4")
              {
                this.List = item.Property("pattern").AsString("");
                this.EbsList = true;
              }
              break;
            case "integer":
              DataType = "Integer";
              break;
            case "boolean":
              DataType = "Boolean";
              break;
            case "list":
            case "filter list":
            case "mv_list":
              DataType = "String";
              this.List = item.Property("data_source").AsString("");
              this.ListFilter = item.Property("pattern").AsString("");
              break;
            case "image":
              DataType = "Gentex.Drawing.WebLazyImage";
              break;
            default: //"list", "string", "text"
              this.StringLength = item.Property("stored_length").AsInt(-1);
              this.DataType = "String";
              break;
          }
        }
      }

      private string LabelToProp(string label)
      {
        if (string.IsNullOrEmpty(label))
          return null;

        var result = new StringBuilder(label.Length);
        var chars = label.ToCharArray();

        if (!char.IsLetter(chars[0]))
          result.Append("P_");

        for (var i = 0; i < chars.Length; i++)
        {
          if (char.IsLetter(chars[i]))
          {
            if (i == 0 || !char.IsLetter(chars[i-1]))
            {
              result.Append(char.ToUpperInvariant(chars[i]));
            }
            else
            {
              result.Append(char.ToLowerInvariant(chars[i]));
            }
          }
          else if (char.IsDigit(chars[i]))
          {
            result.Append(chars[i]);
          }
        }

        if (result.Length > 64)
          return result.ToString(0, 64);
        return result.ToString();
      }

      private string NameToVar(string label)
      {
        if (string.IsNullOrEmpty(label))
          return null;

        var result = new StringBuilder(label.Length);
        var chars = label.ToCharArray();

        result.Append('_');
        for (var i = 0; i < chars.Length; i++)
        {
          if (char.IsLetter(chars[i]))
          {
            if (i == 0 || !char.IsLetter(chars[i - 1]))
            {
              result.Append(char.ToUpperInvariant(chars[i]));
            }
            else
            {
              result.Append(char.ToLowerInvariant(chars[i]));
            }
          }
          else if (char.IsDigit(chars[i]))
          {
            result.Append(chars[i]);
          }
        }

        return result.ToString();
      }

      public override string ToString()
      {
        return string.Format("{0} ({1})", this.Name, this.DataType);
      }
    }
  }
}
