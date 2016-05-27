namespace Innovator.Client
{
  /// <summary>
  /// Credentials for authenticating to an Aras Innovator instance
  /// </summary>
  public interface ICredentials
  {
    /// <summary>
    /// The database to connect to
    /// </summary>
    string Database { get; }
  }
}
