using System;
using System.Runtime.Serialization;

namespace Innovator.Client
{
  [Serializable]
  public class LoggedOutException : Exception
  {
    public LoggedOutException() : base() { }
    public LoggedOutException(string message) : base(message) { }
    public LoggedOutException(string message, Exception innerException) : base(message, innerException) { }
    public LoggedOutException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}
