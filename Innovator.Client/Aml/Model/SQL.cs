using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SQL </summary>
  public class SQL : Item
  {
    protected SQL() { }
    public SQL(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>execution_count</c> property of the item</summary>
    public IProperty_Number ExecutionCount()
    {
      return this.Property("execution_count");
    }
    /// <summary>Retrieve the <c>execution_flag</c> property of the item</summary>
    public IProperty_Text ExecutionFlag()
    {
      return this.Property("execution_flag");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>old_name</c> property of the item</summary>
    public IProperty_Text OldName()
    {
      return this.Property("old_name");
    }
    /// <summary>Retrieve the <c>oracle_body</c> property of the item</summary>
    public IProperty_Text OracleBody()
    {
      return this.Property("oracle_body");
    }
    /// <summary>Retrieve the <c>sqlserver_body</c> property of the item</summary>
    public IProperty_Text SqlserverBody()
    {
      return this.Property("sqlserver_body");
    }
    /// <summary>Retrieve the <c>stale</c> property of the item</summary>
    public IProperty_Boolean Stale()
    {
      return this.Property("stale");
    }
    /// <summary>Retrieve the <c>transform_first</c> property of the item</summary>
    public IProperty_Boolean TransformFirst()
    {
      return this.Property("transform_first");
    }
    /// <summary>Retrieve the <c>type</c> property of the item</summary>
    public IProperty_Text Type()
    {
      return this.Property("type");
    }
  }
}