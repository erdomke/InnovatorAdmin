using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using System.Security.Cryptography;
using System.IO;

namespace InnovatorAdmin.Testing
{
  public class Login : ICommand
  {
    private SecureToken _password;

    public string Url { get; set; }
    public string Database { get; set; }
    public string UserName { get; set; }
    public bool HasPassword { get { return !_password.IsNullOrEmpty(); } }
    //public SecureToken Password { get; set; }
    public CredentialType Type { get; set; }
    /// <summary>
    /// Comment preceding the command in the script
    /// </summary>
    public string Comment { get; set; }

    public async Task Run(TestContext context)
    {
      var url = this.Url;
      var db = this.Database;

      var remoteConn = context.Connection as IRemoteConnection;
      if (string.IsNullOrEmpty(url) && remoteConn != null)
        url = remoteConn.Url.ToString();
      if (string.IsNullOrEmpty(db))
        db = context.Connection.Database;

      var prefs = new ConnectionPreferences() { UserAgent = "InnovatorAdmin UnitTest" };
      var conn = await Factory.GetConnection(url, prefs, true).ToTask();
      ICredentials cred;
      switch (this.Type)
      {
        case CredentialType.Anonymous:
          cred = new AnonymousCredentials(db);
          break;
        case CredentialType.Windows:
          cred = new WindowsCredentials(db);
          break;
        default:
          if (_password.IsNullOrEmpty())
          {
            cred = context.CredentialStore.OfType<ExplicitCredentials>()
              .FirstOrDefault(c => string.Equals(c.Database, db) && string.Equals(c.Username, this.UserName));
          }
          else
          {
            cred = new ExplicitCredentials(db, this.UserName, _password);
          }
          break;
      }

      if (cred == null)
        throw new InvalidOperationException("Could not create credentials for this login type");
      await conn.Login(cred, true).ToTask();
      context.PushConnection(conn);
    }

    /// <summary>
    /// Set the type property from a <see cref="string"/>
    /// </summary>
    public Login SetType(string value)
    {
      CredentialType type;
      if (string.IsNullOrEmpty(value))
      {
        this.Type = CredentialType.Explicit;
      }
      else if (Enum.TryParse(value, out type))
      {
        this.Type = type;
      }
      else
      {
        throw new ArgumentException(value + " is not a valid value for the Type property of a Login element");
      }
      return this;
    }
    public Login SetPassword(string value, string sessionId)
    {
      if (!string.IsNullOrEmpty(value))
      {
        if (!value.StartsWith("pw:"))
        {
          _password = value;
        }
        else
        {
          try
          {
            var data = Convert.FromBase64String(value.Substring(3));
            using (var enc = new Encryption(sessionId))
            using (var output = new MemoryStream(data))
            using (var cryptStream = new CryptoStream(output, enc.CreateDecryptor(), CryptoStreamMode.Read))
            {
              _password = new SecureToken(cryptStream);
            }
          }
          catch (FormatException)
          {
            _password = value;
          }
        }
      }
      return this;
    }

    public string GetEncryptedPassword(string sessionId)
    {
      using (var enc = new Encryption(sessionId))
      using (var output = new MemoryStream())
      using (var cryptStream = new CryptoStream(output, enc.CreateEncryptor(), CryptoStreamMode.Write))
      {
        _password.UseBytes((ref byte[] b) => { cryptStream.Write(b, 0, b.Length); return true; });
        cryptStream.FlushFinalBlock();

        return "pw:" + Convert.ToBase64String(output.ToArray());
      }
    }

    private class Encryption : IDisposable
    {
      private static byte[] _salt = new byte[] { 83, 155, 38, 152, 173, 36, 103, 205, 180, 140, 173, 124, 205, 106, 188, 123, 9, 76, 199, 206, 205, 32, 210, 165 };
      private AesCryptoServiceProvider _crypto = new AesCryptoServiceProvider();
      private Rfc2898DeriveBytes _pbkdf2;
      private byte[] _key;
      private byte[] _IV;

      public Encryption(string sessionId)
      {
        var domain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain();
        var encryptionPassword = (sessionId ?? "") + "|";
        if (domain == null)
        {
          encryptionPassword += Environment.UserDomainName ?? "";
        }
        else if (domain.GetDirectoryEntry() == null)
        {
          encryptionPassword += domain.Name ?? "";
        }
        else
        {
          encryptionPassword += Convert.ToBase64String(domain.GetDirectoryEntry().Guid.ToByteArray());
        }
        _crypto.KeySize = 256;
        _crypto.BlockSize = 128;
        _pbkdf2 = new Rfc2898DeriveBytes(encryptionPassword, _salt, 1000);
        _key = _pbkdf2.GetBytes(_crypto.KeySize / 8);
        _IV = _pbkdf2.GetBytes(_crypto.BlockSize / 8);
      }

      public ICryptoTransform CreateEncryptor()
      {
        return _crypto.CreateEncryptor(_key, _IV);
      }

      public ICryptoTransform CreateDecryptor()
      {
        return _crypto.CreateDecryptor(_key, _IV);
      }

      public void Dispose()
      {
        _pbkdf2.Dispose();
        _crypto.Dispose();
      }
    }

    /// <summary>
    /// Visit this object for the purposes of rendering it to an output
    /// </summary>
    public void Visit(ITestVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
