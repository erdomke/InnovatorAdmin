namespace Innovator.Client
{
  public interface ITokenStore : ICredentials
  {
    string RefreshToken { get; set; }
    string AccessToken { get; set; }
  }
}
