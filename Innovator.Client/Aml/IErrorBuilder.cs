using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Represents an interface for building an error message
  /// </summary>
  public interface IErrorBuilder
  {
    /// <summary>
    /// Specify the context of the error message
    /// </summary>
    IErrorBuilder ErrorContext(IReadOnlyItem item);
    /// <summary>
    /// Add an error message
    /// </summary>
    IErrorBuilder ErrorMsg(string message);
    /// <summary>
    /// Add an error message specifying the properties the message pertains to
    /// </summary>
    IErrorBuilder ErrorMsg(string message, params string[] properties);
    /// <summary>
    /// Add an error message specifying the properties the message pertains to
    /// </summary>
    IErrorBuilder ErrorMsg(string message, IEnumerable<string> properties);
  }
}
