using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  class InitializeSessionToken : TokenCredentials
  {
    private AsymmetricEncryptor _encryptor;
    private string _nonce;

    public AsymmetricEncryptor Encryptor
    {
      get { return _encryptor; }
    }
    public string Nonce
    {
      get { return _nonce; }
    }
    public string SsoUrl { get; set; }

    public InitializeSessionToken(string value, string nonce, string publicKey)
      : base(value)
    {
      _nonce = nonce;
      _encryptor = new AsymmetricEncryptor(publicKey);
    }
  }
}
