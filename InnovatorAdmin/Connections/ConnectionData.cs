using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Aras.Tools.InnovatorAdmin;

namespace Aras.Tools.InnovatorAdmin.Connections
{
  public class ConnectionData : ICloneable, IXmlSerializable
  {
    [DisplayName("Connection Name")]
    public string ConnectionName { get; set; }
    [DisplayName("Database"), TypeConverter(typeof(DatabaseConverter))]
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
      this.ConnectionName = reader.ReadElementString("ConnectionName");
      this.Database = reader.ReadElementString("Database");
      try
      {
        this.Password = RijndaelSimple.Decrypt(_encryptKey, reader.ReadElementString("Password"));
      }
      catch (System.Security.Cryptography.CryptographicException)
      {
        this.Password = string.Empty;
      }
      catch (FormatException)
      {
        this.Password = string.Empty;
      }
      this.Url = reader.ReadElementString("Url");
      this.UserName = reader.ReadElementString("UserName");
      reader.ReadEndElement();
    }

    public void WriteXml(System.Xml.XmlWriter writer)
    {
      writer.WriteElementString("ConnectionName", this.ConnectionName);
      writer.WriteElementString("Database", this.Database);
      writer.WriteElementString("Password", EncryptPassword(this.Password));
      writer.WriteElementString("Url", this.Url);
      writer.WriteElementString("UserName", this.UserName);
    }

    private const string _encryptKey = "{00b3424f-ee37-435b-8207-52d37d5241e9}";

    private string EncryptPassword(string password)
    {
      if (!password.IsGuid())
      {
        password = Aras.IOM.Innovator.ScalcMD5(password);
      }
      return RijndaelSimple.Encrypt(_encryptKey, password);
    }
  }
}
