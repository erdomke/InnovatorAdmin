using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public interface IContext
  {
    IServerConnection Conn { get; }
  }
  public interface ISingleItemContext : IContext
  {
    IReadOnlyItem Item { get; }
  }
  public interface IMultipleItemContext : IContext
  {
    IEnumerable<IReadOnlyItem> Items { get; }
  }
  public interface IValidationContext : IContext
  {
    /// <summary>
    /// Indicates if the argument is new (not in the database)
    /// </summary>
    bool IsNew { get; }

    /// <summary>
    /// Error builder which captures any errors which are encountered
    /// </summary>
    IErrorBuilder ErrorBuilder { get; }

    /// <summary>
    /// Get the exception object created for any errors that have happened so far.
    /// </summary>
    Exception Exception { get; }

    /// <summary>
    /// Get the existing item in the database
    /// </summary>
    IReadOnlyItem Existing { get; }

    /// <summary>
    /// The changes given to the database.  This object should be modified to make any additional
    /// changes
    /// </summary>
    IItem Item { get; }

    /// <summary>
    /// Gets an item which represents the new item after the changes are applied
    /// </summary>
    IReadOnlyItem Merged { get; }

    /// <summary>
    /// Indicates if a property is being set null.  Note that this does not detect if the property
    /// already is null.
    /// </summary>
    bool IsBeingSetNull(string name);

    /// <summary>
    /// Indicates if one or more properties in the list are changing
    /// </summary>
    /// <param name="propertyNames">Property name(s)</param>
    bool IsChanging(params string[] names);

    /// <summary>
    /// Gets a property from the <see cref="Item"/> item (if it exists).  Otherwise, the property
    /// from <see cref="Existing"/> is returned
    /// </summary>
    IReadOnlyProperty NewOrExisting(string name);

    /// <summary>
    /// Method for modifying the query to get existing items
    /// </summary>
    Action<IItem> QueryDefaults { get; set; }
  }
  public interface IVersionContext : IContext
  {
    IReadOnlyItem OldVersion { get; }
    IReadOnlyItem NewVersion { get; }

    /// <summary>
    /// Method for modifying the query to get the new revision
    /// </summary>
    Action<IItem> QueryDefaults { get; set; }
  }
  public interface IPromotionContext : ISingleItemContext
  {
    /// <summary>
    /// The Life Cycle transition which is taking place
    /// </summary>
    LifeCycleTransition Transition { get; }
  }
  public interface IWorkflowContext : IContext
  {
    /// <summary>
    /// The activity during which the method is being called
    /// </summary>
    Activity Activity { get; }
    /// <summary>
    /// The item which is the context of the workflow
    /// </summary>
    IReadOnlyItem Context { get; }
    /// <summary>
    /// The event for which the server method is being called
    /// </summary>
    WorkflowEvent WorkflowEvent { get; }
    /// <summary>
    /// Error builder which captures any errors which are encountered
    /// </summary>
    IErrorBuilder ErrorBuilder { get; }
    /// <summary>
    /// Get the exception object created for any errors that have happened so far.
    /// </summary>
    Exception Exception { get; }
    /// <summary>
    /// Method for modifying the query to get the context item
    /// </summary>
    Action<IItem> QueryDefaults { get; set; }
  }
  public interface IVoteContext : IWorkflowContext
  {
    /// <summary>
    /// Which assignment the event pertains to
    /// </summary>
    IReadOnlyItem Assignment { get; }
    /// <summary>
    /// The workflow path being voted for
    /// </summary>
    string Path { get; }
  }
  public interface IDelegateContext : IWorkflowContext
  {
    /// <summary>
    /// Which assignment the event pertains to
    /// </summary>
    IReadOnlyItem Assignment { get; }
    /// <summary>
    /// The identity which is being delegated to
    /// </summary>
    IReadOnlyItem DelegateTo { get; }
  }

  public interface IMethod
  {
    IReadOnlyResult Execute(ISingleItemContext arg);
  }
  public interface IBeforeGet
  {
    IReadOnlyItem Execute(ISingleItemContext arg);
  }
  public interface IAfterGet
  {
    IEnumerable<IReadOnlyItem> Execute(IMultipleItemContext arg);
  }
  public interface IOnAction
  {
    IEnumerable<IReadOnlyItem> Execute(ISingleItemContext arg);
  }
  public interface IOnGet : IOnAction { }
  public interface IBeforeAddUpdate
  {
    void Execute(IValidationContext arg);
  }
  public interface ISingleItemSubroutine
  {
    void Execute(ISingleItemContext arg);
  }
  public interface IAfterAddUpdate : ISingleItemSubroutine { }
  public interface IBeforeVersion : ISingleItemSubroutine { }
  public interface IAfterVersion
  {
    void Execute(IVersionContext arg);
  }
  public interface IGetKeyedName
  {
    string Execute(ISingleItemContext arg);
  }
  public interface IPromotion
  {
    void Execute(IPromotionContext arg);
  }
  public interface IWorkflow
  {
    void Execute(IWorkflowContext arg);
  }
  public interface IVote
  {
    void Execute(IVoteContext arg);
  }
  public interface IDelegate
  {
    void Execute(IDelegateContext arg);
  }
  public interface IRefuse : IDelegate { }
}
