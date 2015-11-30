using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Aras.Tools.InnovatorAdmin;
using System.Security.Cryptography;
using Innovator.Client;

namespace Aras.Tools.InnovatorAdmin.Connections
{
  public class ConnectionData : ICloneable, IXmlSerializable
  {
    [DisplayName("Connection Name")]
    public string ConnectionName { get; set; }
    [DisplayName("Database")]
    public string Database { get; set; }
    [DisplayName("Password"), PasswordPropertyText(true)]
    public string Password { get; set; }
    [DisplayName("Url")]
    public string Url { get; set; }
    [DisplayName("User Name")]
    public string UserName { get; set; }

    public ConnectionData Clone()
    {
      return new ConnectionData()
      {
        ConnectionName = this.ConnectionName,
        Database = this.Database,
        Password = this.Password,
        Url = this.Url,
        UserName = this.UserName
      };
    }

    object ICloneable.Clone()
    {
      return Clone();
    }

    public override string ToString()
    {
      return this.ConnectionName;
    }

    public System.Xml.Schema.XmlSchema GetSchema()
    {
      return null;
    }

    public void ReadXml(System.Xml.XmlReader reader)
    {
      reader.ReadStartElement("ConnectionData");
      reader.MoveToContent();

      while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
      {
        switch (reader.LocalName)
        {
          case "Color":

            break;
          case "ConnectionName":
            this.ConnectionName = reader.ReadElementString(reader.LocalName);
            break;
          case "Database":
            this.Database = reader.ReadElementString(reader.LocalName);
            break;
          case "Password":
            var encrypted = reader.ReadElementString(reader.LocalName);
            string password;
            var success = TryDecryptWindows(encrypted, out password)
              || TryDecryptStatic(encrypted, out password);
            this.Password = password;
            break;
          case "Url":
            this.Url = reader.ReadElementString(reader.LocalName);
            break;
          case "UserName":
            this.UserName = reader.ReadElementString(reader.LocalName);
            break;
          default:
            reader.ReadOuterXml();
            reader.MoveToContent();
            break;
        }
      }
      reader.ReadEndElement();
    }

    public void WriteXml(System.Xml.XmlWriter writer)
    {
      writer.WriteElementString("ConnectionName", this.ConnectionName);
      writer.WriteElementString("Database", this.Database);
      writer.WriteElementString("Password", EncryptWindows(this.Password));
      writer.WriteElementString("Url", this.Url);
      writer.WriteElementString("UserName", this.UserName);
    }


    #region New Encryption
    private readonly byte[] _salt = new byte[] { 0x4f, 0xbe, 0x6e, 0x2e, 0x27, 0x5e, 0xdf, 0x7a, 0xec, 0x62, 0x76, 0xfa, 0xa4, 0xee, 0xd8, 0xd3, 0xdf, 0x12, 0x33, 0xb7, 0xfb, 0xf4, 0x81, 0xe6 };
    private bool TryDecryptWindows(string encrypted, out string decrypted)
    {
      decrypted = string.Empty;
      try
      {
        var data = Convert.FromBase64String(encrypted);
        var decryptedData = ProtectedData.Unprotect(data, _salt, DataProtectionScope.CurrentUser);
        decrypted = Encoding.ASCII.GetString(decryptedData);
        return true;
      }
      catch (System.Security.Cryptography.CryptographicException)
      {
        return false;
      }
      catch (FormatException)
      {
        return false;
      }
    }
    private string EncryptWindows(string data)
    {
      return Convert.ToBase64String(ProtectedData.Protect(Encoding.ASCII.GetBytes(data), _salt, DataProtectionScope.CurrentUser));
    }
    #endregion
    #region Old Encryption
    private const string _encryptKey = "{00b3424f-ee37-435b-8207-52d37d5241e9}";

    private bool TryDecryptStatic(string encrypted, out string decrypted)
    {
      decrypted = string.Empty;
      try
      {
        decrypted = RijndaelSimple.Decrypt(_encryptKey, encrypted);
        return true;
      }
      catch (System.Security.Cryptography.CryptographicException)
      {
        return false;
      }
      catch (FormatException)
      {
        return false;
      }
    }

    private string EncryptStatic(string password)
    {
      if (!password.IsGuid())
      {
        password = ScalcMD5(password);
      }
      return RijndaelSimple.Encrypt(_encryptKey, password);
    }
    #endregion

    public static string ScalcMD5(string val)
    {
      string result;
      using (var mD5CryptoServiceProvider = new MD5CryptoServiceProvider())
      {
        var aSCIIEncoding = new ASCIIEncoding();
        var bytes = aSCIIEncoding.GetBytes(val);
        string text = "";
        var array = mD5CryptoServiceProvider.ComputeHash(bytes);
        short num = 0;
        while ((int)num < array.GetLength(0))
        {
          string text2 = Convert.ToString(array[(int)num], 16).ToLowerInvariant();
          if (text2.Length == 1)
          {
            text2 = "0" + text2;
          }
          text += text2;
          num += 1;
        }
        result = text;
      }
      return result;
    }
  }
}
