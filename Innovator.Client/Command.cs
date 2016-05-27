using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Innovator.Client
{
  /// <summary>
  /// An AML request to be submitted to the server
  /// </summary>
  public class Command
  {
    private List<string> _queries = new List<string>(1);
    private ParameterSubstitution _sub = new ParameterSubstitution();
    private CommandAction _action;
    private string _actionString;

    /// <summary>
    /// What MIME type the client should accept for the request
    /// </summary>
    public string AcceptMimeType { get; set; }
    /// <summary>
    /// SOAP action to use with the AML
    /// </summary>
    public CommandAction Action
    {
      get { return _action; }
      set { _action = value; _actionString = null; }
    }
    /// <summary>
    /// SOAP action to use with the AML (represented as a string)
    /// </summary>
    public string ActionString
    {
      get { return _actionString ?? Action.ToString(); }
      set { _actionString = value; }
    }

    /// <summary>
    /// The AML query
    /// </summary>
    public virtual string Aml
    {
      get
      {
        if (_queries.Count < 1) return null;
        if (_queries.Count == 1) return _queries[0];
        return "<AML>" + _queries.GroupConcat("") + "</AML>";
      }
      set
      {
        if (value == null)
        {
          _queries.Clear();
        }
        else
        {
          if (_queries.Count == 1)
          {
            _queries[0] = value;
          }
          else
          {
            _queries.Clear();
            _queries.Add(value);
          }
        }
      }
    }

    /// <summary>
    /// Instantiate a new command
    /// </summary>
    public Command()
    {
      this.AcceptMimeType = "text/xml";
      this.Action = CommandAction.ApplyItem;
    }

    internal string Substitute(string query, IServerContext context)
    {
      return _sub.RenderParameter(query, context);
    }

    /// <summary>
    /// Replaces SQL Server style numbered AML parameters (used as attribute and element values) with the corresponding arguments.
    /// Property type conversion and XML formatting is performed
    /// </summary>
    /// <param name="format">Format string containing the parameters</param>
    /// <param name="args">Replacement values for the numbered parameters</param>
    /// <returns>A valid AML string</returns>
    public Command(string query, params object[] args) : this()
    {
      this.WithAml(query, args);
    }

    /// <summary>
    /// Replaces SQL Server style style AML parameters (used as attribute and element values) with the corresponding arguments.
    /// Property type conversion and XML formatting is performed
    /// </summary>
    /// <param name="format">Format string containing the parameters</param>
    /// <param name="paramaters">Replacement values for the named parameters</param>
    /// <returns>A valid AML string</returns>
    public Command(string query, Connection.DbParams parameters) : this()
    {
      this.Aml = query;
      foreach (var param in parameters)
      {
        _sub.AddParameter(param.ParameterName, param.Value);
      }
    }
    public Command(IElement aml) : this(aml.ToAml())
    {
      if (aml.Name == "AML" && aml.Elements().Count() > 1)
        this.Action = CommandAction.ApplyAML;
    }
    public Command(IEnumerable<IElement> aml) : this()
    {
      this.Aml = "<AML>" + aml.GroupConcat("", i => i.ToAml()) + "</AML>";
      this.Action = CommandAction.ApplyAML;
    }
    /// <summary>
    /// Specify the SOAP action to use with the AML
    /// </summary>
    public Command WithAction(CommandAction action)
    {
      this.Action = action;
      return this;
    }
    /// <summary>
    /// Specify the SOAP action to use with the AML (as a string)
    /// </summary>
    public Command WithAction(string action)
    {
      CommandAction parsed;
      if (Utils.EnumTryParse<CommandAction>(action, true, out parsed))
      {
        this.Action = parsed;
      }
      else
      {
        _actionString = action;
      }
      return this;
    }
    /// <summary>
    /// Replaces SQL Server style numbered AML parameters (used as attribute and element values) with the corresponding arguments.
    /// Property type conversion and XML formatting is performed
    /// </summary>
    /// <param name="format">Format string containing the parameters</param>
    /// <param name="args">Replacement values for the numbered parameters</param>
    /// <returns>A valid AML string</returns>
    public Command WithAml(string query, params object[] args)
    {
      this.Aml = query;
      _sub.AddIndexedParameters(args);
      return this;
    }
    /// <summary>
    /// Specify the MIME type to accept
    /// </summary>
    public Command WithMimeType(string mimeType)
    {
      this.AcceptMimeType = mimeType;
      return this;
    }
    /// <summary>
    /// Specify a named parameter and its value
    /// </summary>
    public Command WithParam(string name, object value)
    {
      _sub.AddParameter(name, value);
      return this;
    }

    /// <summary>
    /// Append a query to this request
    /// </summary>
    /// <param name="query">Query to append</param>
    protected virtual void AddAml(string query)
    {
      _queries.Add(query);
    }

    /// <summary>
    /// Specify a method to configure each outgoing HTTP request associated specifically with this AML request
    /// </summary>
    public Action<IHttpRequest> Settings { get; set; }

    /// <summary>
    /// Implicitly convert strings to commands as needed
    /// </summary>
    public static implicit operator Command(string aml)
    {
      return new Command() { Aml = aml };
    }
    /// <summary>
    /// Implicitly convert XML elements to commands as needed
    /// </summary>
    public static implicit operator Command(XElement aml)
    {
      return new Command() { Aml = aml.ToString() };
    }
    /// <summary>
    /// Implicitly convert XML elements to commands as needed
    /// </summary>
    public static implicit operator Command(XmlNode aml)
    {
      return new Command() { Aml = aml.OuterXml };
    }

    /// <summary>
    /// Perform parameter substitutions and return the resulting AML
    /// </summary>
    public string ToNormalizedAml(IServerContext context)
    {
      if (_sub.ParamCount > 0)
        return _sub.Substitute(this.Aml, context);
      return this.Aml;
    }
    /// <summary>
    /// Return the AML string
    /// </summary>
    public override string ToString()
    {
      return this.Aml;
    }
  }
}
