using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using InnovatorAdmin;
using System.Security.Cryptography;
using Innovator.Client;
using System.Drawing;

namespace InnovatorAdmin.Connections
{
  public class ConnectionData : ICloneable, IXmlSerializable
  {
    private const int DefaultTimeout = 100000;
    private BindingList<ConnectionParameter> _params = new BindingList<ConnectionParameter>();

    [DisplayName("Type")]
    public ConnectionType Type { get; set; }
    [DisplayName("Authentication")]
    public Authentication Authentication { get; set; }
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
    [DisplayName("Color")]
    public Color Color { get; set; }
    [DisplayName("Confirm")]
    public Boolean Confirm { get; set; }
    [DisplayName("Timeout")]
    public int Timeout { get; set; }

    [Browsable(false)]
    public IList<ConnectionParameter> Params { get { return _params; } }

    public ConnectionData()
    {
      this.Type = ConnectionType.Innovator;
      this.Timeout = DefaultTimeout;
    }

    public ConnectionData Clone()
    {
      var result = new ConnectionData()
      {
        ConnectionName = this.ConnectionName,
        Database = this.Database,
        Password = this.Password,
        Url = this.Url,
        UserName = this.UserName,
        Color = this.Color,
        Type = this.Type,
        Authentication = this.Authentication,
        Confirm = this.Confirm,
        Timeout = this.Timeout
      };

      foreach (var param in _params)
      {
        result._params.Add(param);
      }
      return result;
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
          case "Authentication":
            Authentication newAuth;
            if (Enum.TryParse<Authentication>(reader.ReadElementString(reader.LocalName), out newAuth))
              this.Authentication = newAuth;
            break;
          case "Color":
            this.Color = FromHex(reader.ReadElementString(reader.LocalName));
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
          case "Type":
            ConnectionType newType;
            if (Enum.TryParse<ConnectionType>(reader.ReadElementString(reader.LocalName), out newType))
              this.Type = newType;
            break;
          case "Confirm":
            Boolean newConfirm;
            if (Boolean.TryParse(reader.ReadElementString(reader.LocalName), out newConfirm))
              this.Confirm = newConfirm;
            break;
          case "Timeout":
            int timeout;
            this.Timeout = int.TryParse(reader.ReadElementString(reader.LocalName), out timeout)
              ? timeout : DefaultTimeout;
            break;
          case "Params":
            if (reader.IsEmptyElement)
            {
              reader.Read();
            }
            else
            {
              reader.Read();
              while (reader.NodeType != System.Xml.XmlNodeType.EndElement || reader.LocalName != "Params")
              {
                if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == "Param")
                {
                  _params.Add(new ConnectionParameter()
                  {
                    Name = reader.GetAttribute("name"),
                    Value = reader.ReadElementString(reader.LocalName)
                  });
                }
                else
                {
                  reader.Read();
                }
              }
              reader.Read();
            }
            break;
          default:
            reader.ReadOuterXml();
            reader.MoveToContent();
            break;
        }
      }

      if (this.Color == System.Drawing.Color.Empty)
      {
        var idx = (int)(((long)this.ConnectionName.GetHashCode() - int.MinValue) % _tabColors.Length);
        this.Color = _tabColors[idx];
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
      writer.WriteElementString("Color", ToHex(this.Color));
      writer.WriteElementString("Type", this.Type.ToString());
      writer.WriteElementString("Authentication", this.Authentication.ToString());
      writer.WriteElementString("Confirm", this.Confirm.ToString());
      writer.WriteElementString("Timeout", this.Timeout.ToString());
      writer.WriteStartElement("Params");
      foreach (var param in _params)
      {
        writer.WriteStartElement("Param");
        writer.WriteAttributeString("name", param.Name);
        writer.WriteValue(param.Value);
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
      writer.Flush();
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
      return Convert.ToBase64String(ProtectedData.Protect(Encoding.ASCII.GetBytes(data ?? ""), _salt, DataProtectionScope.CurrentUser));
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
    #endregion

    private Color FromHex(string hex)
    {
      if (hex.StartsWith("#"))
        hex = hex.Substring(1);

      if (hex.Length != 6) throw new Exception("Color not valid");

      return System.Drawing.Color.FromArgb(
          int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
          int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
          int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
    }
    private string ToHex(Color color)
    {
      return "#"
        + color.R.ToString("X2")
        + color.G.ToString("X2")
        + color.B.ToString("X2");
    }

    private static Color[] _tabColors;

    static ConnectionData()
    {
      _tabColors = new Color[]
      {
        System.Drawing.Color.FromArgb(153, 92, 92),
        System.Drawing.Color.FromArgb(166, 99, 99),
        System.Drawing.Color.FromArgb(191, 115, 115),
        System.Drawing.Color.FromArgb(229, 138, 138),
        System.Drawing.Color.FromArgb(255, 153, 153),
        System.Drawing.Color.FromArgb(153, 61, 61),
        System.Drawing.Color.FromArgb(166, 66, 66),
        System.Drawing.Color.FromArgb(191, 77, 77),
        System.Drawing.Color.FromArgb(217, 87, 87),
        System.Drawing.Color.FromArgb(242, 97, 97),
        System.Drawing.Color.FromArgb(153, 31, 31),
        System.Drawing.Color.FromArgb(166, 33, 33),
        System.Drawing.Color.FromArgb(191, 38, 38),
        System.Drawing.Color.FromArgb(229, 46, 46),
        System.Drawing.Color.FromArgb(255, 51, 51),
        System.Drawing.Color.FromArgb(153, 0, 0),
        System.Drawing.Color.FromArgb(166, 0, 0),
        System.Drawing.Color.FromArgb(178, 0, 0),
        System.Drawing.Color.FromArgb(191, 0, 0),
        System.Drawing.Color.FromArgb(217, 0, 0),
        System.Drawing.Color.FromArgb(229, 0, 0),
        System.Drawing.Color.FromArgb(255, 0, 0),
        System.Drawing.Color.FromArgb(166, 109, 99),
        System.Drawing.Color.FromArgb(204, 135, 122),
        System.Drawing.Color.FromArgb(229, 151, 138),
        System.Drawing.Color.FromArgb(255, 168, 153),
        System.Drawing.Color.FromArgb(153, 75, 61),
        System.Drawing.Color.FromArgb(166, 81, 66),
        System.Drawing.Color.FromArgb(204, 100, 82),
        System.Drawing.Color.FromArgb(229, 112, 92),
        System.Drawing.Color.FromArgb(255, 125, 102),
        System.Drawing.Color.FromArgb(153, 49, 31),
        System.Drawing.Color.FromArgb(166, 53, 33),
        System.Drawing.Color.FromArgb(191, 61, 38),
        System.Drawing.Color.FromArgb(217, 69, 43),
        System.Drawing.Color.FromArgb(242, 78, 48),
        System.Drawing.Color.FromArgb(153, 23, 0),
        System.Drawing.Color.FromArgb(166, 25, 0),
        System.Drawing.Color.FromArgb(178, 27, 0),
        System.Drawing.Color.FromArgb(204, 31, 0),
        System.Drawing.Color.FromArgb(229, 34, 0),
        System.Drawing.Color.FromArgb(255, 38, 0),
        System.Drawing.Color.FromArgb(153, 110, 92),
        System.Drawing.Color.FromArgb(178, 129, 107),
        System.Drawing.Color.FromArgb(204, 147, 122),
        System.Drawing.Color.FromArgb(229, 165, 138),
        System.Drawing.Color.FromArgb(255, 184, 153),
        System.Drawing.Color.FromArgb(166, 96, 66),
        System.Drawing.Color.FromArgb(204, 118, 82),
        System.Drawing.Color.FromArgb(229, 133, 92),
        System.Drawing.Color.FromArgb(255, 148, 102),
        System.Drawing.Color.FromArgb(153, 67, 31),
        System.Drawing.Color.FromArgb(166, 73, 33),
        System.Drawing.Color.FromArgb(191, 84, 38),
        System.Drawing.Color.FromArgb(229, 101, 46),
        System.Drawing.Color.FromArgb(255, 112, 51),
        System.Drawing.Color.FromArgb(153, 46, 0),
        System.Drawing.Color.FromArgb(166, 50, 0),
        System.Drawing.Color.FromArgb(178, 54, 0),
        System.Drawing.Color.FromArgb(217, 65, 0),
        System.Drawing.Color.FromArgb(242, 73, 0),
        System.Drawing.Color.FromArgb(166, 129, 99),
        System.Drawing.Color.FromArgb(191, 149, 115),
        System.Drawing.Color.FromArgb(217, 169, 130),
        System.Drawing.Color.FromArgb(242, 189, 145),
        System.Drawing.Color.FromArgb(153, 103, 61),
        System.Drawing.Color.FromArgb(166, 111, 66),
        System.Drawing.Color.FromArgb(191, 128, 77),
        System.Drawing.Color.FromArgb(217, 145, 87),
        System.Drawing.Color.FromArgb(255, 171, 102),
        System.Drawing.Color.FromArgb(153, 86, 31),
        System.Drawing.Color.FromArgb(178, 100, 36),
        System.Drawing.Color.FromArgb(204, 114, 41),
        System.Drawing.Color.FromArgb(229, 129, 46),
        System.Drawing.Color.FromArgb(255, 143, 51),
        System.Drawing.Color.FromArgb(153, 69, 0),
        System.Drawing.Color.FromArgb(166, 75, 0),
        System.Drawing.Color.FromArgb(191, 86, 0),
        System.Drawing.Color.FromArgb(229, 103, 0),
        System.Drawing.Color.FromArgb(255, 115, 0),
        System.Drawing.Color.FromArgb(153, 129, 92),
        System.Drawing.Color.FromArgb(166, 139, 99),
        System.Drawing.Color.FromArgb(204, 171, 122),
        System.Drawing.Color.FromArgb(229, 193, 138),
        System.Drawing.Color.FromArgb(255, 214, 153),
        System.Drawing.Color.FromArgb(153, 116, 61),
        System.Drawing.Color.FromArgb(166, 126, 66),
        System.Drawing.Color.FromArgb(178, 136, 71),
        System.Drawing.Color.FromArgb(217, 165, 87),
        System.Drawing.Color.FromArgb(242, 184, 97),
        System.Drawing.Color.FromArgb(153, 104, 31),
        System.Drawing.Color.FromArgb(166, 113, 33),
        System.Drawing.Color.FromArgb(178, 121, 36),
        System.Drawing.Color.FromArgb(204, 139, 41),
        System.Drawing.Color.FromArgb(229, 156, 46),
        System.Drawing.Color.FromArgb(255, 173, 51),
        System.Drawing.Color.FromArgb(153, 92, 0),
        System.Drawing.Color.FromArgb(178, 107, 0),
        System.Drawing.Color.FromArgb(217, 130, 0),
        System.Drawing.Color.FromArgb(242, 145, 0),
        System.Drawing.Color.FromArgb(153, 138, 92),
        System.Drawing.Color.FromArgb(166, 149, 99),
        System.Drawing.Color.FromArgb(191, 172, 115),
        System.Drawing.Color.FromArgb(229, 207, 138),
        System.Drawing.Color.FromArgb(255, 230, 153),
        System.Drawing.Color.FromArgb(153, 130, 61),
        System.Drawing.Color.FromArgb(166, 141, 66),
        System.Drawing.Color.FromArgb(178, 152, 71),
        System.Drawing.Color.FromArgb(204, 173, 82),
        System.Drawing.Color.FromArgb(229, 195, 92),
        System.Drawing.Color.FromArgb(255, 217, 102),
        System.Drawing.Color.FromArgb(153, 122, 31),
        System.Drawing.Color.FromArgb(166, 133, 33),
        System.Drawing.Color.FromArgb(191, 153, 38),
        System.Drawing.Color.FromArgb(229, 184, 46),
        System.Drawing.Color.FromArgb(255, 204, 51),
        System.Drawing.Color.FromArgb(153, 115, 0),
        System.Drawing.Color.FromArgb(166, 124, 0),
        System.Drawing.Color.FromArgb(178, 134, 0),
        System.Drawing.Color.FromArgb(204, 153, 0),
        System.Drawing.Color.FromArgb(229, 172, 0),
        System.Drawing.Color.FromArgb(255, 191, 0),
        System.Drawing.Color.FromArgb(153, 147, 92),
        System.Drawing.Color.FromArgb(166, 159, 99),
        System.Drawing.Color.FromArgb(191, 184, 115),
        System.Drawing.Color.FromArgb(229, 220, 138),
        System.Drawing.Color.FromArgb(255, 245, 153),
        System.Drawing.Color.FromArgb(153, 144, 61),
        System.Drawing.Color.FromArgb(166, 156, 66),
        System.Drawing.Color.FromArgb(178, 168, 71),
        System.Drawing.Color.FromArgb(191, 180, 77),
        System.Drawing.Color.FromArgb(229, 216, 92),
        System.Drawing.Color.FromArgb(255, 240, 102),
        System.Drawing.Color.FromArgb(153, 141, 31),
        System.Drawing.Color.FromArgb(166, 152, 33),
        System.Drawing.Color.FromArgb(178, 164, 36),
        System.Drawing.Color.FromArgb(204, 188, 41),
        System.Drawing.Color.FromArgb(242, 223, 48),
        System.Drawing.Color.FromArgb(153, 138, 0),
        System.Drawing.Color.FromArgb(166, 149, 0),
        System.Drawing.Color.FromArgb(178, 161, 0),
        System.Drawing.Color.FromArgb(191, 172, 0),
        System.Drawing.Color.FromArgb(217, 195, 0),
        System.Drawing.Color.FromArgb(255, 230, 0),
        System.Drawing.Color.FromArgb(150, 153, 92),
        System.Drawing.Color.FromArgb(162, 166, 99),
        System.Drawing.Color.FromArgb(175, 179, 107),
        System.Drawing.Color.FromArgb(200, 204, 122),
        System.Drawing.Color.FromArgb(237, 242, 145),
        System.Drawing.Color.FromArgb(148, 153, 61),
        System.Drawing.Color.FromArgb(161, 166, 66),
        System.Drawing.Color.FromArgb(173, 179, 71),
        System.Drawing.Color.FromArgb(198, 204, 82),
        System.Drawing.Color.FromArgb(223, 230, 92),
        System.Drawing.Color.FromArgb(247, 255, 102),
        System.Drawing.Color.FromArgb(147, 153, 31),
        System.Drawing.Color.FromArgb(159, 166, 33),
        System.Drawing.Color.FromArgb(171, 179, 36),
        System.Drawing.Color.FromArgb(184, 191, 38),
        System.Drawing.Color.FromArgb(208, 217, 43),
        System.Drawing.Color.FromArgb(245, 255, 51),
        System.Drawing.Color.FromArgb(145, 153, 0),
        System.Drawing.Color.FromArgb(157, 166, 0),
        System.Drawing.Color.FromArgb(170, 179, 0),
        System.Drawing.Color.FromArgb(182, 191, 0),
        System.Drawing.Color.FromArgb(206, 217, 0),
        System.Drawing.Color.FromArgb(242, 255, 0),
        System.Drawing.Color.FromArgb(141, 153, 92),
        System.Drawing.Color.FromArgb(152, 166, 99),
        System.Drawing.Color.FromArgb(176, 191, 115),
        System.Drawing.Color.FromArgb(211, 230, 138),
        System.Drawing.Color.FromArgb(235, 255, 153),
        System.Drawing.Color.FromArgb(135, 153, 61),
        System.Drawing.Color.FromArgb(146, 166, 66),
        System.Drawing.Color.FromArgb(157, 179, 71),
        System.Drawing.Color.FromArgb(168, 191, 77),
        System.Drawing.Color.FromArgb(191, 217, 87),
        System.Drawing.Color.FromArgb(213, 242, 97),
        System.Drawing.Color.FromArgb(129, 153, 31),
        System.Drawing.Color.FromArgb(139, 166, 33),
        System.Drawing.Color.FromArgb(150, 179, 36),
        System.Drawing.Color.FromArgb(161, 191, 38),
        System.Drawing.Color.FromArgb(171, 204, 41),
        System.Drawing.Color.FromArgb(193, 230, 46),
        System.Drawing.Color.FromArgb(214, 255, 51),
        System.Drawing.Color.FromArgb(122, 153, 0),
        System.Drawing.Color.FromArgb(133, 166, 0),
        System.Drawing.Color.FromArgb(143, 179, 0),
        System.Drawing.Color.FromArgb(153, 191, 0),
        System.Drawing.Color.FromArgb(163, 204, 0),
        System.Drawing.Color.FromArgb(184, 230, 0),
        System.Drawing.Color.FromArgb(204, 255, 0),
        System.Drawing.Color.FromArgb(132, 153, 92),
        System.Drawing.Color.FromArgb(143, 166, 99),
        System.Drawing.Color.FromArgb(164, 191, 115),
        System.Drawing.Color.FromArgb(197, 230, 138),
        System.Drawing.Color.FromArgb(219, 255, 153),
        System.Drawing.Color.FromArgb(121, 153, 61),
        System.Drawing.Color.FromArgb(131, 166, 66),
        System.Drawing.Color.FromArgb(141, 179, 71),
        System.Drawing.Color.FromArgb(161, 204, 82),
        System.Drawing.Color.FromArgb(181, 230, 92),
        System.Drawing.Color.FromArgb(201, 255, 102),
        System.Drawing.Color.FromArgb(110, 153, 31),
        System.Drawing.Color.FromArgb(119, 166, 33),
        System.Drawing.Color.FromArgb(129, 179, 36),
        System.Drawing.Color.FromArgb(138, 191, 38),
        System.Drawing.Color.FromArgb(147, 204, 41),
        System.Drawing.Color.FromArgb(156, 217, 43),
        System.Drawing.Color.FromArgb(174, 242, 48),
        System.Drawing.Color.FromArgb(99, 153, 0),
        System.Drawing.Color.FromArgb(108, 166, 0),
        System.Drawing.Color.FromArgb(116, 179, 0),
        System.Drawing.Color.FromArgb(124, 191, 0),
        System.Drawing.Color.FromArgb(133, 204, 0),
        System.Drawing.Color.FromArgb(149, 230, 0),
        System.Drawing.Color.FromArgb(166, 255, 0),
        System.Drawing.Color.FromArgb(122, 153, 92),
        System.Drawing.Color.FromArgb(133, 166, 99),
        System.Drawing.Color.FromArgb(143, 179, 107),
        System.Drawing.Color.FromArgb(153, 191, 115),
        System.Drawing.Color.FromArgb(173, 217, 130),
        System.Drawing.Color.FromArgb(194, 242, 145),
        System.Drawing.Color.FromArgb(107, 153, 61),
        System.Drawing.Color.FromArgb(116, 166, 66),
        System.Drawing.Color.FromArgb(125, 179, 71),
        System.Drawing.Color.FromArgb(143, 204, 82),
        System.Drawing.Color.FromArgb(161, 230, 92),
        System.Drawing.Color.FromArgb(178, 255, 102),
        System.Drawing.Color.FromArgb(92, 153, 31),
        System.Drawing.Color.FromArgb(99, 166, 33),
        System.Drawing.Color.FromArgb(107, 179, 36),
        System.Drawing.Color.FromArgb(115, 191, 38),
        System.Drawing.Color.FromArgb(122, 204, 41),
        System.Drawing.Color.FromArgb(130, 217, 43),
        System.Drawing.Color.FromArgb(145, 242, 48),
        System.Drawing.Color.FromArgb(76, 153, 0),
        System.Drawing.Color.FromArgb(83, 166, 0),
        System.Drawing.Color.FromArgb(89, 179, 0),
        System.Drawing.Color.FromArgb(96, 191, 0),
        System.Drawing.Color.FromArgb(102, 204, 0),
        System.Drawing.Color.FromArgb(108, 217, 0),
        System.Drawing.Color.FromArgb(121, 242, 0),
        System.Drawing.Color.FromArgb(113, 153, 92),
        System.Drawing.Color.FromArgb(123, 166, 99),
        System.Drawing.Color.FromArgb(142, 191, 115),
        System.Drawing.Color.FromArgb(170, 230, 138),
        System.Drawing.Color.FromArgb(189, 255, 153),
        System.Drawing.Color.FromArgb(93, 153, 61),
        System.Drawing.Color.FromArgb(101, 166, 66),
        System.Drawing.Color.FromArgb(109, 179, 71),
        System.Drawing.Color.FromArgb(117, 191, 77),
        System.Drawing.Color.FromArgb(132, 217, 87),
        System.Drawing.Color.FromArgb(148, 242, 97),
        System.Drawing.Color.FromArgb(73, 153, 31),
        System.Drawing.Color.FromArgb(80, 166, 33),
        System.Drawing.Color.FromArgb(86, 179, 36),
        System.Drawing.Color.FromArgb(92, 191, 38),
        System.Drawing.Color.FromArgb(98, 204, 41),
        System.Drawing.Color.FromArgb(104, 217, 43),
        System.Drawing.Color.FromArgb(122, 255, 51),
        System.Drawing.Color.FromArgb(54, 153, 0),
        System.Drawing.Color.FromArgb(58, 166, 0),
        System.Drawing.Color.FromArgb(62, 179, 0),
        System.Drawing.Color.FromArgb(67, 191, 0),
        System.Drawing.Color.FromArgb(76, 217, 0),
        System.Drawing.Color.FromArgb(85, 242, 0),
        System.Drawing.Color.FromArgb(121, 179, 107),
        System.Drawing.Color.FromArgb(130, 191, 115),
        System.Drawing.Color.FromArgb(147, 217, 130),
        System.Drawing.Color.FromArgb(173, 255, 153),
        System.Drawing.Color.FromArgb(106, 204, 82),
        System.Drawing.Color.FromArgb(119, 230, 92),
        System.Drawing.Color.FromArgb(133, 255, 102),
        System.Drawing.Color.FromArgb(55, 153, 31),
        System.Drawing.Color.FromArgb(60, 166, 33),
        System.Drawing.Color.FromArgb(64, 179, 36),
        System.Drawing.Color.FromArgb(69, 191, 38),
        System.Drawing.Color.FromArgb(73, 204, 41),
        System.Drawing.Color.FromArgb(78, 217, 43),
        System.Drawing.Color.FromArgb(87, 242, 48),
        System.Drawing.Color.FromArgb(41, 204, 0),
        System.Drawing.Color.FromArgb(46, 230, 0),
        System.Drawing.Color.FromArgb(51, 255, 0),
        System.Drawing.Color.FromArgb(126, 204, 122),
        System.Drawing.Color.FromArgb(142, 230, 138),
        System.Drawing.Color.FromArgb(158, 255, 153),
        System.Drawing.Color.FromArgb(66, 153, 61),
        System.Drawing.Color.FromArgb(71, 166, 66),
        System.Drawing.Color.FromArgb(77, 179, 71),
        System.Drawing.Color.FromArgb(82, 191, 77),
        System.Drawing.Color.FromArgb(93, 217, 87),
        System.Drawing.Color.FromArgb(110, 255, 102),
        System.Drawing.Color.FromArgb(61, 255, 51),
        System.Drawing.Color.FromArgb(8, 153, 0),
        System.Drawing.Color.FromArgb(92, 153, 98),
        System.Drawing.Color.FromArgb(99, 166, 106),
        System.Drawing.Color.FromArgb(115, 191, 122),
        System.Drawing.Color.FromArgb(130, 217, 139),
        System.Drawing.Color.FromArgb(153, 255, 163),
        System.Drawing.Color.FromArgb(66, 166, 76),
        System.Drawing.Color.FromArgb(71, 179, 82),
        System.Drawing.Color.FromArgb(77, 191, 88),
        System.Drawing.Color.FromArgb(82, 204, 94),
        System.Drawing.Color.FromArgb(97, 242, 111),
        System.Drawing.Color.FromArgb(33, 166, 46),
        System.Drawing.Color.FromArgb(36, 179, 50),
        System.Drawing.Color.FromArgb(38, 191, 54),
        System.Drawing.Color.FromArgb(41, 204, 57),
        System.Drawing.Color.FromArgb(43, 217, 61),
        System.Drawing.Color.FromArgb(0, 166, 17),
        System.Drawing.Color.FromArgb(0, 179, 18),
        System.Drawing.Color.FromArgb(0, 191, 19),
        System.Drawing.Color.FromArgb(0, 217, 22),
        System.Drawing.Color.FromArgb(92, 153, 107),
        System.Drawing.Color.FromArgb(99, 166, 116),
        System.Drawing.Color.FromArgb(115, 191, 134),
        System.Drawing.Color.FromArgb(130, 217, 152),
        System.Drawing.Color.FromArgb(153, 255, 178),
        System.Drawing.Color.FromArgb(61, 153, 84),
        System.Drawing.Color.FromArgb(66, 166, 91),
        System.Drawing.Color.FromArgb(71, 179, 98),
        System.Drawing.Color.FromArgb(82, 204, 112),
        System.Drawing.Color.FromArgb(92, 230, 126),
        System.Drawing.Color.FromArgb(102, 255, 140),
        System.Drawing.Color.FromArgb(33, 166, 66),
        System.Drawing.Color.FromArgb(43, 217, 87),
        System.Drawing.Color.FromArgb(51, 255, 102),
        System.Drawing.Color.FromArgb(0, 153, 38),
        System.Drawing.Color.FromArgb(0, 230, 57),
        System.Drawing.Color.FromArgb(0, 255, 64),
        System.Drawing.Color.FromArgb(92, 153, 116),
        System.Drawing.Color.FromArgb(99, 166, 126),
        System.Drawing.Color.FromArgb(107, 179, 136),
        System.Drawing.Color.FromArgb(122, 204, 155),
        System.Drawing.Color.FromArgb(138, 230, 174),
        System.Drawing.Color.FromArgb(153, 255, 194),
        System.Drawing.Color.FromArgb(61, 153, 98),
        System.Drawing.Color.FromArgb(66, 166, 106),
        System.Drawing.Color.FromArgb(71, 179, 114),
        System.Drawing.Color.FromArgb(77, 191, 122),
        System.Drawing.Color.FromArgb(92, 230, 147),
        System.Drawing.Color.FromArgb(102, 255, 163),
        System.Drawing.Color.FromArgb(31, 153, 80),
        System.Drawing.Color.FromArgb(33, 166, 86),
        System.Drawing.Color.FromArgb(36, 179, 93),
        System.Drawing.Color.FromArgb(38, 191, 99),
        System.Drawing.Color.FromArgb(43, 217, 113),
        System.Drawing.Color.FromArgb(48, 242, 126),
        System.Drawing.Color.FromArgb(0, 153, 61),
        System.Drawing.Color.FromArgb(0, 179, 71),
        System.Drawing.Color.FromArgb(0, 191, 77),
        System.Drawing.Color.FromArgb(0, 204, 82),
        System.Drawing.Color.FromArgb(0, 230, 92),
        System.Drawing.Color.FromArgb(92, 153, 125),
        System.Drawing.Color.FromArgb(107, 179, 146),
        System.Drawing.Color.FromArgb(122, 204, 167),
        System.Drawing.Color.FromArgb(138, 230, 188),
        System.Drawing.Color.FromArgb(153, 255, 209),
        System.Drawing.Color.FromArgb(61, 153, 112),
        System.Drawing.Color.FromArgb(66, 166, 121),
        System.Drawing.Color.FromArgb(71, 179, 130),
        System.Drawing.Color.FromArgb(82, 204, 149),
        System.Drawing.Color.FromArgb(92, 230, 168),
        System.Drawing.Color.FromArgb(102, 255, 186),
        System.Drawing.Color.FromArgb(31, 153, 98),
        System.Drawing.Color.FromArgb(33, 166, 106),
        System.Drawing.Color.FromArgb(36, 179, 114),
        System.Drawing.Color.FromArgb(41, 204, 131),
        System.Drawing.Color.FromArgb(48, 242, 155),
        System.Drawing.Color.FromArgb(0, 204, 112),
        System.Drawing.Color.FromArgb(0, 230, 126),
        System.Drawing.Color.FromArgb(0, 255, 140),
        System.Drawing.Color.FromArgb(92, 153, 135),
        System.Drawing.Color.FromArgb(107, 179, 157),
        System.Drawing.Color.FromArgb(122, 204, 180),
        System.Drawing.Color.FromArgb(138, 230, 202),
        System.Drawing.Color.FromArgb(153, 255, 224),
        System.Drawing.Color.FromArgb(61, 153, 125),
        System.Drawing.Color.FromArgb(66, 166, 136),
        System.Drawing.Color.FromArgb(71, 179, 146),
        System.Drawing.Color.FromArgb(82, 204, 167),
        System.Drawing.Color.FromArgb(97, 242, 199),
        System.Drawing.Color.FromArgb(31, 153, 116),
        System.Drawing.Color.FromArgb(33, 166, 126),
        System.Drawing.Color.FromArgb(36, 179, 136),
        System.Drawing.Color.FromArgb(41, 204, 155),
        System.Drawing.Color.FromArgb(46, 230, 174),
        System.Drawing.Color.FromArgb(51, 255, 194),
        System.Drawing.Color.FromArgb(0, 153, 107),
        System.Drawing.Color.FromArgb(0, 166, 116),
        System.Drawing.Color.FromArgb(0, 179, 125),
        System.Drawing.Color.FromArgb(0, 204, 143),
        System.Drawing.Color.FromArgb(0, 230, 161),
        System.Drawing.Color.FromArgb(0, 255, 178),
        System.Drawing.Color.FromArgb(92, 153, 144),
        System.Drawing.Color.FromArgb(107, 179, 168),
        System.Drawing.Color.FromArgb(122, 204, 192),
        System.Drawing.Color.FromArgb(138, 230, 216),
        System.Drawing.Color.FromArgb(153, 255, 240),
        System.Drawing.Color.FromArgb(61, 153, 139),
        System.Drawing.Color.FromArgb(66, 166, 151),
        System.Drawing.Color.FromArgb(71, 179, 162),
        System.Drawing.Color.FromArgb(82, 204, 186),
        System.Drawing.Color.FromArgb(97, 242, 220),
        System.Drawing.Color.FromArgb(31, 153, 135),
        System.Drawing.Color.FromArgb(36, 179, 157),
        System.Drawing.Color.FromArgb(43, 217, 191),
        System.Drawing.Color.FromArgb(48, 242, 213),
        System.Drawing.Color.FromArgb(0, 166, 141),
        System.Drawing.Color.FromArgb(0, 191, 163),
        System.Drawing.Color.FromArgb(0, 217, 184),
        System.Drawing.Color.FromArgb(0, 255, 217),
        System.Drawing.Color.FromArgb(92, 153, 153),
        System.Drawing.Color.FromArgb(99, 166, 166),
        System.Drawing.Color.FromArgb(115, 191, 191),
        System.Drawing.Color.FromArgb(130, 217, 217),
        System.Drawing.Color.FromArgb(145, 242, 242),
        System.Drawing.Color.FromArgb(61, 153, 153),
        System.Drawing.Color.FromArgb(66, 166, 166),
        System.Drawing.Color.FromArgb(71, 179, 179),
        System.Drawing.Color.FromArgb(82, 204, 204),
        System.Drawing.Color.FromArgb(97, 242, 242),
        System.Drawing.Color.FromArgb(41, 204, 204),
        System.Drawing.Color.FromArgb(0, 153, 153),
        System.Drawing.Color.FromArgb(0, 166, 166),
        System.Drawing.Color.FromArgb(0, 179, 179),
        System.Drawing.Color.FromArgb(0, 217, 217),
        System.Drawing.Color.FromArgb(0, 242, 242),
        System.Drawing.Color.FromArgb(92, 144, 153),
        System.Drawing.Color.FromArgb(99, 156, 166),
        System.Drawing.Color.FromArgb(122, 192, 204),
        System.Drawing.Color.FromArgb(138, 216, 230),
        System.Drawing.Color.FromArgb(153, 240, 255),
        System.Drawing.Color.FromArgb(66, 151, 166),
        System.Drawing.Color.FromArgb(77, 174, 191),
        System.Drawing.Color.FromArgb(87, 197, 217),
        System.Drawing.Color.FromArgb(97, 220, 242),
        System.Drawing.Color.FromArgb(36, 157, 179),
        System.Drawing.Color.FromArgb(43, 191, 217),
        System.Drawing.Color.FromArgb(48, 213, 242),
        System.Drawing.Color.FromArgb(0, 130, 153),
        System.Drawing.Color.FromArgb(0, 152, 179),
        System.Drawing.Color.FromArgb(0, 173, 204),
        System.Drawing.Color.FromArgb(0, 195, 230),
        System.Drawing.Color.FromArgb(0, 217, 255),
        System.Drawing.Color.FromArgb(92, 135, 153),
        System.Drawing.Color.FromArgb(115, 168, 191),
        System.Drawing.Color.FromArgb(138, 202, 230),
        System.Drawing.Color.FromArgb(153, 224, 255),
        System.Drawing.Color.FromArgb(66, 136, 166),
        System.Drawing.Color.FromArgb(77, 157, 191),
        System.Drawing.Color.FromArgb(92, 188, 230),
        System.Drawing.Color.FromArgb(102, 209, 255),
        System.Drawing.Color.FromArgb(33, 126, 166),
        System.Drawing.Color.FromArgb(41, 155, 204),
        System.Drawing.Color.FromArgb(46, 174, 230),
        System.Drawing.Color.FromArgb(51, 194, 255),
        System.Drawing.Color.FromArgb(0, 107, 153),
        System.Drawing.Color.FromArgb(0, 116, 166),
        System.Drawing.Color.FromArgb(0, 134, 191),
        System.Drawing.Color.FromArgb(0, 161, 230),
        System.Drawing.Color.FromArgb(0, 179, 255),
        System.Drawing.Color.FromArgb(99, 136, 166),
        System.Drawing.Color.FromArgb(122, 167, 204),
        System.Drawing.Color.FromArgb(138, 188, 230),
        System.Drawing.Color.FromArgb(153, 209, 255),
        System.Drawing.Color.FromArgb(61, 112, 153),
        System.Drawing.Color.FromArgb(71, 130, 179),
        System.Drawing.Color.FromArgb(87, 158, 217),
        System.Drawing.Color.FromArgb(102, 186, 255),
        System.Drawing.Color.FromArgb(31, 98, 153),
        System.Drawing.Color.FromArgb(36, 114, 179),
        System.Drawing.Color.FromArgb(43, 139, 217),
        System.Drawing.Color.FromArgb(51, 163, 255),
        System.Drawing.Color.FromArgb(0, 91, 166),
        System.Drawing.Color.FromArgb(0, 105, 191),
        System.Drawing.Color.FromArgb(0, 126, 230),
        System.Drawing.Color.FromArgb(0, 140, 255),
        System.Drawing.Color.FromArgb(92, 116, 153),
        System.Drawing.Color.FromArgb(107, 136, 179),
        System.Drawing.Color.FromArgb(122, 155, 204),
        System.Drawing.Color.FromArgb(138, 174, 230),
        System.Drawing.Color.FromArgb(153, 194, 255),
        System.Drawing.Color.FromArgb(66, 106, 166),
        System.Drawing.Color.FromArgb(77, 122, 191),
        System.Drawing.Color.FromArgb(87, 139, 217),
        System.Drawing.Color.FromArgb(97, 155, 242),
        System.Drawing.Color.FromArgb(31, 80, 153),
        System.Drawing.Color.FromArgb(36, 93, 179),
        System.Drawing.Color.FromArgb(41, 106, 204),
        System.Drawing.Color.FromArgb(46, 119, 230),
        System.Drawing.Color.FromArgb(51, 133, 255),
        System.Drawing.Color.FromArgb(0, 61, 153),
        System.Drawing.Color.FromArgb(0, 71, 179),
        System.Drawing.Color.FromArgb(0, 87, 217),
        System.Drawing.Color.FromArgb(0, 102, 255),
        System.Drawing.Color.FromArgb(99, 116, 166),
        System.Drawing.Color.FromArgb(115, 134, 191),
        System.Drawing.Color.FromArgb(130, 152, 217),
        System.Drawing.Color.FromArgb(145, 170, 242),
        System.Drawing.Color.FromArgb(66, 91, 166),
        System.Drawing.Color.FromArgb(77, 105, 191),
        System.Drawing.Color.FromArgb(87, 119, 217),
        System.Drawing.Color.FromArgb(102, 140, 255),
        System.Drawing.Color.FromArgb(33, 66, 166),
        System.Drawing.Color.FromArgb(38, 76, 191),
        System.Drawing.Color.FromArgb(46, 92, 230),
        System.Drawing.Color.FromArgb(0, 41, 166),
        System.Drawing.Color.FromArgb(0, 45, 179),
        System.Drawing.Color.FromArgb(0, 51, 204),
        System.Drawing.Color.FromArgb(0, 57, 230),
        System.Drawing.Color.FromArgb(0, 64, 255),
        System.Drawing.Color.FromArgb(92, 98, 153),
        System.Drawing.Color.FromArgb(107, 114, 179),
        System.Drawing.Color.FromArgb(122, 131, 204),
        System.Drawing.Color.FromArgb(138, 147, 230),
        System.Drawing.Color.FromArgb(153, 163, 255),
        System.Drawing.Color.FromArgb(61, 70, 153),
        System.Drawing.Color.FromArgb(71, 82, 179),
        System.Drawing.Color.FromArgb(87, 100, 217),
        System.Drawing.Color.FromArgb(97, 111, 242),
        System.Drawing.Color.FromArgb(31, 43, 153),
        System.Drawing.Color.FromArgb(38, 54, 191),
        System.Drawing.Color.FromArgb(43, 61, 217),
        System.Drawing.Color.FromArgb(48, 68, 242),
        System.Drawing.Color.FromArgb(0, 15, 153),
        System.Drawing.Color.FromArgb(0, 17, 166),
        System.Drawing.Color.FromArgb(0, 18, 179),
        System.Drawing.Color.FromArgb(0, 19, 191),
        System.Drawing.Color.FromArgb(0, 22, 217),
        System.Drawing.Color.FromArgb(0, 25, 255),
        System.Drawing.Color.FromArgb(95, 92, 153),
        System.Drawing.Color.FromArgb(103, 99, 166),
        System.Drawing.Color.FromArgb(119, 115, 191),
        System.Drawing.Color.FromArgb(134, 130, 217),
        System.Drawing.Color.FromArgb(158, 153, 255),
        System.Drawing.Color.FromArgb(66, 61, 153),
        System.Drawing.Color.FromArgb(71, 66, 166),
        System.Drawing.Color.FromArgb(77, 71, 179),
        System.Drawing.Color.FromArgb(88, 82, 204),
        System.Drawing.Color.FromArgb(99, 92, 230),
        System.Drawing.Color.FromArgb(110, 102, 255),
        System.Drawing.Color.FromArgb(37, 31, 153),
        System.Drawing.Color.FromArgb(43, 36, 179),
        System.Drawing.Color.FromArgb(49, 41, 204),
        System.Drawing.Color.FromArgb(55, 46, 230),
        System.Drawing.Color.FromArgb(61, 51, 255),
        System.Drawing.Color.FromArgb(8, 0, 166),
        System.Drawing.Color.FromArgb(9, 0, 179),
        System.Drawing.Color.FromArgb(10, 0, 204),
        System.Drawing.Color.FromArgb(12, 0, 230),
        System.Drawing.Color.FromArgb(113, 99, 166),
        System.Drawing.Color.FromArgb(130, 115, 191),
        System.Drawing.Color.FromArgb(147, 130, 217),
        System.Drawing.Color.FromArgb(165, 145, 242),
        System.Drawing.Color.FromArgb(80, 61, 153),
        System.Drawing.Color.FromArgb(86, 66, 166),
        System.Drawing.Color.FromArgb(93, 71, 179),
        System.Drawing.Color.FromArgb(106, 82, 204),
        System.Drawing.Color.FromArgb(126, 97, 242),
        System.Drawing.Color.FromArgb(60, 33, 166),
        System.Drawing.Color.FromArgb(69, 38, 191),
        System.Drawing.Color.FromArgb(78, 43, 217),
        System.Drawing.Color.FromArgb(92, 51, 255),
        System.Drawing.Color.FromArgb(31, 0, 153),
        System.Drawing.Color.FromArgb(38, 0, 191),
        System.Drawing.Color.FromArgb(43, 0, 217),
        System.Drawing.Color.FromArgb(48, 0, 242),
        System.Drawing.Color.FromArgb(113, 92, 153),
        System.Drawing.Color.FromArgb(123, 99, 166),
        System.Drawing.Color.FromArgb(142, 115, 191),
        System.Drawing.Color.FromArgb(170, 138, 230),
        System.Drawing.Color.FromArgb(189, 153, 255),
        System.Drawing.Color.FromArgb(93, 61, 153),
        System.Drawing.Color.FromArgb(101, 66, 166),
        System.Drawing.Color.FromArgb(117, 77, 191),
        System.Drawing.Color.FromArgb(132, 87, 217),
        System.Drawing.Color.FromArgb(148, 97, 242),
        System.Drawing.Color.FromArgb(73, 31, 153),
        System.Drawing.Color.FromArgb(80, 33, 166),
        System.Drawing.Color.FromArgb(86, 36, 179),
        System.Drawing.Color.FromArgb(98, 41, 204),
        System.Drawing.Color.FromArgb(116, 48, 242),
        System.Drawing.Color.FromArgb(54, 0, 153),
        System.Drawing.Color.FromArgb(58, 0, 166),
        System.Drawing.Color.FromArgb(62, 0, 179),
        System.Drawing.Color.FromArgb(67, 0, 191),
        System.Drawing.Color.FromArgb(71, 0, 204),
        System.Drawing.Color.FromArgb(80, 0, 230),
        System.Drawing.Color.FromArgb(89, 0, 255),
        System.Drawing.Color.FromArgb(122, 92, 153),
        System.Drawing.Color.FromArgb(143, 107, 179),
        System.Drawing.Color.FromArgb(163, 122, 204),
        System.Drawing.Color.FromArgb(184, 138, 230),
        System.Drawing.Color.FromArgb(204, 153, 255),
        System.Drawing.Color.FromArgb(107, 61, 153),
        System.Drawing.Color.FromArgb(116, 66, 166),
        System.Drawing.Color.FromArgb(134, 77, 191),
        System.Drawing.Color.FromArgb(161, 92, 230),
        System.Drawing.Color.FromArgb(179, 102, 255),
        System.Drawing.Color.FromArgb(92, 31, 153),
        System.Drawing.Color.FromArgb(99, 33, 166),
        System.Drawing.Color.FromArgb(107, 36, 179),
        System.Drawing.Color.FromArgb(122, 41, 204),
        System.Drawing.Color.FromArgb(145, 48, 242),
        System.Drawing.Color.FromArgb(77, 0, 153),
        System.Drawing.Color.FromArgb(83, 0, 166),
        System.Drawing.Color.FromArgb(89, 0, 179),
        System.Drawing.Color.FromArgb(96, 0, 191),
        System.Drawing.Color.FromArgb(108, 0, 217),
        System.Drawing.Color.FromArgb(128, 0, 255),
        System.Drawing.Color.FromArgb(132, 92, 153),
        System.Drawing.Color.FromArgb(154, 107, 179),
        System.Drawing.Color.FromArgb(175, 122, 204),
        System.Drawing.Color.FromArgb(208, 145, 242),
        System.Drawing.Color.FromArgb(121, 61, 153),
        System.Drawing.Color.FromArgb(131, 66, 166),
        System.Drawing.Color.FromArgb(141, 71, 179),
        System.Drawing.Color.FromArgb(161, 82, 204),
        System.Drawing.Color.FromArgb(181, 92, 230),
        System.Drawing.Color.FromArgb(201, 102, 255),
        System.Drawing.Color.FromArgb(110, 31, 153),
        System.Drawing.Color.FromArgb(119, 33, 166),
        System.Drawing.Color.FromArgb(129, 36, 179),
        System.Drawing.Color.FromArgb(147, 41, 204),
        System.Drawing.Color.FromArgb(165, 46, 230),
        System.Drawing.Color.FromArgb(184, 51, 255),
        System.Drawing.Color.FromArgb(99, 0, 153),
        System.Drawing.Color.FromArgb(108, 0, 166),
        System.Drawing.Color.FromArgb(116, 0, 179),
        System.Drawing.Color.FromArgb(124, 0, 191),
        System.Drawing.Color.FromArgb(133, 0, 204),
        System.Drawing.Color.FromArgb(149, 0, 230),
        System.Drawing.Color.FromArgb(166, 0, 255),
        System.Drawing.Color.FromArgb(141, 92, 153),
        System.Drawing.Color.FromArgb(152, 99, 166),
        System.Drawing.Color.FromArgb(176, 115, 191),
        System.Drawing.Color.FromArgb(211, 138, 230),
        System.Drawing.Color.FromArgb(235, 153, 255),
        System.Drawing.Color.FromArgb(135, 61, 153),
        System.Drawing.Color.FromArgb(146, 66, 166),
        System.Drawing.Color.FromArgb(168, 77, 191),
        System.Drawing.Color.FromArgb(191, 87, 217),
        System.Drawing.Color.FromArgb(213, 97, 242),
        System.Drawing.Color.FromArgb(129, 31, 153),
        System.Drawing.Color.FromArgb(139, 33, 166),
        System.Drawing.Color.FromArgb(150, 36, 179),
        System.Drawing.Color.FromArgb(161, 38, 191),
        System.Drawing.Color.FromArgb(182, 43, 217),
        System.Drawing.Color.FromArgb(203, 48, 242),
        System.Drawing.Color.FromArgb(122, 0, 153),
        System.Drawing.Color.FromArgb(133, 0, 166),
        System.Drawing.Color.FromArgb(143, 0, 179),
        System.Drawing.Color.FromArgb(153, 0, 191),
        System.Drawing.Color.FromArgb(173, 0, 217),
        System.Drawing.Color.FromArgb(204, 0, 255),
        System.Drawing.Color.FromArgb(150, 92, 153),
        System.Drawing.Color.FromArgb(162, 99, 166),
        System.Drawing.Color.FromArgb(187, 115, 191),
        System.Drawing.Color.FromArgb(225, 138, 230),
        System.Drawing.Color.FromArgb(250, 153, 255),
        System.Drawing.Color.FromArgb(148, 61, 153),
        System.Drawing.Color.FromArgb(161, 66, 166),
        System.Drawing.Color.FromArgb(173, 71, 179),
        System.Drawing.Color.FromArgb(186, 77, 191),
        System.Drawing.Color.FromArgb(210, 87, 217),
        System.Drawing.Color.FromArgb(247, 102, 255),
        System.Drawing.Color.FromArgb(147, 31, 153),
        System.Drawing.Color.FromArgb(159, 33, 166),
        System.Drawing.Color.FromArgb(171, 36, 179),
        System.Drawing.Color.FromArgb(196, 41, 204),
        System.Drawing.Color.FromArgb(220, 46, 230),
        System.Drawing.Color.FromArgb(245, 51, 255),
        System.Drawing.Color.FromArgb(145, 0, 153),
        System.Drawing.Color.FromArgb(157, 0, 166),
        System.Drawing.Color.FromArgb(170, 0, 179),
        System.Drawing.Color.FromArgb(194, 0, 204),
        System.Drawing.Color.FromArgb(218, 0, 230),
        System.Drawing.Color.FromArgb(242, 0, 255),
        System.Drawing.Color.FromArgb(153, 92, 147),
        System.Drawing.Color.FromArgb(179, 107, 171),
        System.Drawing.Color.FromArgb(204, 122, 196),
        System.Drawing.Color.FromArgb(230, 138, 220),
        System.Drawing.Color.FromArgb(255, 153, 245),
        System.Drawing.Color.FromArgb(153, 61, 144),
        System.Drawing.Color.FromArgb(166, 66, 156),
        System.Drawing.Color.FromArgb(179, 71, 168),
        System.Drawing.Color.FromArgb(217, 87, 204),
        System.Drawing.Color.FromArgb(255, 102, 240),
        System.Drawing.Color.FromArgb(153, 31, 141),
        System.Drawing.Color.FromArgb(166, 33, 152),
        System.Drawing.Color.FromArgb(179, 36, 164),
        System.Drawing.Color.FromArgb(217, 43, 199),
        System.Drawing.Color.FromArgb(255, 51, 235),
        System.Drawing.Color.FromArgb(153, 0, 138),
        System.Drawing.Color.FromArgb(166, 0, 149),
        System.Drawing.Color.FromArgb(191, 0, 172),
        System.Drawing.Color.FromArgb(230, 0, 207),
        System.Drawing.Color.FromArgb(153, 92, 138),
        System.Drawing.Color.FromArgb(166, 99, 149),
        System.Drawing.Color.FromArgb(204, 122, 184),
        System.Drawing.Color.FromArgb(242, 145, 218),
        System.Drawing.Color.FromArgb(153, 61, 130),
        System.Drawing.Color.FromArgb(179, 71, 152),
        System.Drawing.Color.FromArgb(204, 82, 173),
        System.Drawing.Color.FromArgb(230, 92, 195),
        System.Drawing.Color.FromArgb(255, 102, 217),
        System.Drawing.Color.FromArgb(153, 31, 122),
        System.Drawing.Color.FromArgb(166, 33, 133),
        System.Drawing.Color.FromArgb(179, 36, 143),
        System.Drawing.Color.FromArgb(204, 41, 163),
        System.Drawing.Color.FromArgb(230, 46, 184),
        System.Drawing.Color.FromArgb(255, 51, 204),
        System.Drawing.Color.FromArgb(153, 0, 115),
        System.Drawing.Color.FromArgb(166, 0, 124),
        System.Drawing.Color.FromArgb(191, 0, 143),
        System.Drawing.Color.FromArgb(217, 0, 163),
        System.Drawing.Color.FromArgb(242, 0, 182),
        System.Drawing.Color.FromArgb(166, 99, 139),
        System.Drawing.Color.FromArgb(204, 122, 171),
        System.Drawing.Color.FromArgb(230, 138, 193),
        System.Drawing.Color.FromArgb(255, 153, 214),
        System.Drawing.Color.FromArgb(153, 61, 116),
        System.Drawing.Color.FromArgb(166, 66, 126),
        System.Drawing.Color.FromArgb(191, 77, 145),
        System.Drawing.Color.FromArgb(217, 87, 165),
        System.Drawing.Color.FromArgb(255, 102, 194),
        System.Drawing.Color.FromArgb(153, 31, 104),
        System.Drawing.Color.FromArgb(166, 33, 113),
        System.Drawing.Color.FromArgb(191, 38, 130),
        System.Drawing.Color.FromArgb(217, 43, 147),
        System.Drawing.Color.FromArgb(255, 51, 173),
        System.Drawing.Color.FromArgb(153, 0, 92),
        System.Drawing.Color.FromArgb(178, 0, 107),
        System.Drawing.Color.FromArgb(204, 0, 122),
        System.Drawing.Color.FromArgb(242, 0, 145),
        System.Drawing.Color.FromArgb(153, 92, 119),
        System.Drawing.Color.FromArgb(179, 107, 139),
        System.Drawing.Color.FromArgb(217, 130, 169),
        System.Drawing.Color.FromArgb(255, 153, 199),
        System.Drawing.Color.FromArgb(153, 61, 103),
        System.Drawing.Color.FromArgb(179, 71, 120),
        System.Drawing.Color.FromArgb(204, 82, 137),
        System.Drawing.Color.FromArgb(242, 97, 162),
        System.Drawing.Color.FromArgb(153, 31, 86),
        System.Drawing.Color.FromArgb(166, 33, 93),
        System.Drawing.Color.FromArgb(191, 38, 107),
        System.Drawing.Color.FromArgb(229, 46, 129),
        System.Drawing.Color.FromArgb(255, 51, 143),
        System.Drawing.Color.FromArgb(153, 0, 69),
        System.Drawing.Color.FromArgb(166, 0, 75),
        System.Drawing.Color.FromArgb(191, 0, 86),
        System.Drawing.Color.FromArgb(217, 0, 98),
        System.Drawing.Color.FromArgb(242, 0, 109),
        System.Drawing.Color.FromArgb(166, 99, 119),
        System.Drawing.Color.FromArgb(204, 122, 147),
        System.Drawing.Color.FromArgb(230, 138, 165),
        System.Drawing.Color.FromArgb(255, 153, 184),
        System.Drawing.Color.FromArgb(153, 61, 89),
        System.Drawing.Color.FromArgb(166, 66, 96),
        System.Drawing.Color.FromArgb(204, 82, 118),
        System.Drawing.Color.FromArgb(229, 92, 133),
        System.Drawing.Color.FromArgb(255, 102, 148),
        System.Drawing.Color.FromArgb(153, 31, 67),
        System.Drawing.Color.FromArgb(166, 33, 73),
        System.Drawing.Color.FromArgb(178, 36, 79),
        System.Drawing.Color.FromArgb(217, 43, 95),
        System.Drawing.Color.FromArgb(242, 48, 107),
        System.Drawing.Color.FromArgb(153, 0, 46),
        System.Drawing.Color.FromArgb(166, 0, 50),
        System.Drawing.Color.FromArgb(191, 0, 57),
        System.Drawing.Color.FromArgb(217, 0, 65),
        System.Drawing.Color.FromArgb(242, 0, 73),
        System.Drawing.Color.FromArgb(166, 99, 109),
        System.Drawing.Color.FromArgb(191, 115, 126),
        System.Drawing.Color.FromArgb(217, 130, 143),
        System.Drawing.Color.FromArgb(242, 145, 160),
        System.Drawing.Color.FromArgb(153, 61, 75),
        System.Drawing.Color.FromArgb(178, 71, 87),
        System.Drawing.Color.FromArgb(217, 87, 106),
        System.Drawing.Color.FromArgb(242, 97, 119),
        System.Drawing.Color.FromArgb(153, 31, 49),
        System.Drawing.Color.FromArgb(166, 33, 53),
        System.Drawing.Color.FromArgb(191, 38, 61),
        System.Drawing.Color.FromArgb(217, 43, 69),
        System.Drawing.Color.FromArgb(242, 48, 78),
        System.Drawing.Color.FromArgb(153, 0, 23),
        System.Drawing.Color.FromArgb(166, 0, 25),
        System.Drawing.Color.FromArgb(178, 0, 27),
        System.Drawing.Color.FromArgb(204, 0, 31),
        System.Drawing.Color.FromArgb(229, 0, 34),
        System.Drawing.Color.FromArgb(255, 0, 38)
      };
    }
  }
}
