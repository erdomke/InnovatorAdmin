using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Innovator.Client
{
  public class AsymmetricEncryptor : IDisposable
  {
    private const bool UseOaepPadding = false;
    private RSACryptoServiceProvider _rsa;
    private string _publicKey;

    public string PublicKey
    {
      get { return _publicKey; }
    }

    public AsymmetricEncryptor()
    {
      _rsa = new RSACryptoServiceProvider(1024, GetParameters());
      var p = _rsa.ExportParameters(false);
      _publicKey = Convert.ToBase64String(p.Modulus) + ";" + Convert.ToBase64String(p.Exponent);
    }
    public AsymmetricEncryptor(string publicKey)
    {
      var rsaParam = new RSAParameters();
      var parts = publicKey.Split(';');
      if (parts.Length != 2) throw new ArgumentException("Invalid public key", "publicKey");
      rsaParam.Modulus = Convert.FromBase64String(parts[0]);
      rsaParam.Exponent = Convert.FromBase64String(parts[1]);

      _rsa = new RSACryptoServiceProvider(GetParameters());
      _rsa.PersistKeyInCsp = false;
      _rsa.ImportParameters(rsaParam);
    }

    private CspParameters GetParameters()
    {
      var csp = new CspParameters();
      csp.KeyContainerName = "Innovator.Client";
      csp.Flags = CspProviderFlags.UseMachineKeyStore;
      csp.ProviderType = 1;
      csp.KeyNumber = 1;
      return csp;
    }

    public byte[] Encrypt(byte[] data)
    {
      return _rsa.Encrypt(data, UseOaepPadding);
    }
    public byte[] Encrypt(byte[] data, int start, int count)
    {
      if (start == 0 && count == data.Length)
      {
        return _rsa.Encrypt(data, UseOaepPadding);
      }
      else
      {
        var buffer = new byte[count];
        try
        {
          for (var i = 0; i < count; i++)
          {
            buffer[i] = data[i + start];
          }
          return _rsa.Encrypt(buffer, UseOaepPadding);
        }
        finally
        {
          for (var i = 0; i < count; i++)
          {
            buffer[i] = 0;
          }
        }
      }
    }
    public byte[] Decrypt(byte[] data)
    {
      return _rsa.Decrypt(data, UseOaepPadding);
    }

    public void Dispose()
    {
      if (_rsa != null)
      {
        _rsa.Clear();
        _rsa = null;
        _publicKey = null;
      }
    }
  }
}
