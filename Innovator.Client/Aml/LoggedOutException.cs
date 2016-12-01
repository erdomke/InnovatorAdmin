using System;
#if SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace Innovator.Client
{
#if SERIALIZATION
  [Serializable]
#endif
  public class LoggedOutException : Exception
  {
    public LoggedOutException() : base() { }
    public LoggedOutException(string message) : base(message) { }
    public LoggedOutException(string message, Exception innerException) : base(message, innerException) { }
#if SERIALIZATION
    public LoggedOutException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
  }
}
