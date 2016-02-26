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
using System.Windows.Media;

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
    public SecureToken Password { get; set; }
    [DisplayName("Url")]
    public string Url { get; set; }
    [DisplayName("User Name")]
    public string UserName { get; set; }
    [DisplayName("Color")]
    public Color Color { get; set; }
    [DisplayName("Confirm")]
    public bool Confirm { get; set; }
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

      if (this.Color == default(Color))
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
      writer.WriteElementString("Password", this.Password.UseString((ref string p) => EncryptWindows(p)));
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

    private string EncryptStatic(string password)
    {
      if (!password.IsGuid())
      {
        password = ConnectionDataExtensions.CalcMD5(password);
      }
      return RijndaelSimple.Encrypt(_encryptKey, password);
    }
    #endregion

    private Color FromHex(string hex)
    {
      if (hex.StartsWith("#"))
        hex = hex.Substring(1);

      if (hex.Length != 6) throw new Exception("Color not valid");

      return Color.FromRgb(
          byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
          byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
          byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
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
        Color.FromRgb(153, 92, 92),
        Color.FromRgb(166, 99, 99),
        Color.FromRgb(191, 115, 115),
        Color.FromRgb(229, 138, 138),
        Color.FromRgb(255, 153, 153),
        Color.FromRgb(153, 61, 61),
        Color.FromRgb(166, 66, 66),
        Color.FromRgb(191, 77, 77),
        Color.FromRgb(217, 87, 87),
        Color.FromRgb(242, 97, 97),
        Color.FromRgb(153, 31, 31),
        Color.FromRgb(166, 33, 33),
        Color.FromRgb(191, 38, 38),
        Color.FromRgb(229, 46, 46),
        Color.FromRgb(255, 51, 51),
        Color.FromRgb(153, 0, 0),
        Color.FromRgb(166, 0, 0),
        Color.FromRgb(178, 0, 0),
        Color.FromRgb(191, 0, 0),
        Color.FromRgb(217, 0, 0),
        Color.FromRgb(229, 0, 0),
        Color.FromRgb(255, 0, 0),
        Color.FromRgb(166, 109, 99),
        Color.FromRgb(204, 135, 122),
        Color.FromRgb(229, 151, 138),
        Color.FromRgb(255, 168, 153),
        Color.FromRgb(153, 75, 61),
        Color.FromRgb(166, 81, 66),
        Color.FromRgb(204, 100, 82),
        Color.FromRgb(229, 112, 92),
        Color.FromRgb(255, 125, 102),
        Color.FromRgb(153, 49, 31),
        Color.FromRgb(166, 53, 33),
        Color.FromRgb(191, 61, 38),
        Color.FromRgb(217, 69, 43),
        Color.FromRgb(242, 78, 48),
        Color.FromRgb(153, 23, 0),
        Color.FromRgb(166, 25, 0),
        Color.FromRgb(178, 27, 0),
        Color.FromRgb(204, 31, 0),
        Color.FromRgb(229, 34, 0),
        Color.FromRgb(255, 38, 0),
        Color.FromRgb(153, 110, 92),
        Color.FromRgb(178, 129, 107),
        Color.FromRgb(204, 147, 122),
        Color.FromRgb(229, 165, 138),
        Color.FromRgb(255, 184, 153),
        Color.FromRgb(166, 96, 66),
        Color.FromRgb(204, 118, 82),
        Color.FromRgb(229, 133, 92),
        Color.FromRgb(255, 148, 102),
        Color.FromRgb(153, 67, 31),
        Color.FromRgb(166, 73, 33),
        Color.FromRgb(191, 84, 38),
        Color.FromRgb(229, 101, 46),
        Color.FromRgb(255, 112, 51),
        Color.FromRgb(153, 46, 0),
        Color.FromRgb(166, 50, 0),
        Color.FromRgb(178, 54, 0),
        Color.FromRgb(217, 65, 0),
        Color.FromRgb(242, 73, 0),
        Color.FromRgb(166, 129, 99),
        Color.FromRgb(191, 149, 115),
        Color.FromRgb(217, 169, 130),
        Color.FromRgb(242, 189, 145),
        Color.FromRgb(153, 103, 61),
        Color.FromRgb(166, 111, 66),
        Color.FromRgb(191, 128, 77),
        Color.FromRgb(217, 145, 87),
        Color.FromRgb(255, 171, 102),
        Color.FromRgb(153, 86, 31),
        Color.FromRgb(178, 100, 36),
        Color.FromRgb(204, 114, 41),
        Color.FromRgb(229, 129, 46),
        Color.FromRgb(255, 143, 51),
        Color.FromRgb(153, 69, 0),
        Color.FromRgb(166, 75, 0),
        Color.FromRgb(191, 86, 0),
        Color.FromRgb(229, 103, 0),
        Color.FromRgb(255, 115, 0),
        Color.FromRgb(153, 129, 92),
        Color.FromRgb(166, 139, 99),
        Color.FromRgb(204, 171, 122),
        Color.FromRgb(229, 193, 138),
        Color.FromRgb(255, 214, 153),
        Color.FromRgb(153, 116, 61),
        Color.FromRgb(166, 126, 66),
        Color.FromRgb(178, 136, 71),
        Color.FromRgb(217, 165, 87),
        Color.FromRgb(242, 184, 97),
        Color.FromRgb(153, 104, 31),
        Color.FromRgb(166, 113, 33),
        Color.FromRgb(178, 121, 36),
        Color.FromRgb(204, 139, 41),
        Color.FromRgb(229, 156, 46),
        Color.FromRgb(255, 173, 51),
        Color.FromRgb(153, 92, 0),
        Color.FromRgb(178, 107, 0),
        Color.FromRgb(217, 130, 0),
        Color.FromRgb(242, 145, 0),
        Color.FromRgb(153, 138, 92),
        Color.FromRgb(166, 149, 99),
        Color.FromRgb(191, 172, 115),
        Color.FromRgb(229, 207, 138),
        Color.FromRgb(255, 230, 153),
        Color.FromRgb(153, 130, 61),
        Color.FromRgb(166, 141, 66),
        Color.FromRgb(178, 152, 71),
        Color.FromRgb(204, 173, 82),
        Color.FromRgb(229, 195, 92),
        Color.FromRgb(255, 217, 102),
        Color.FromRgb(153, 122, 31),
        Color.FromRgb(166, 133, 33),
        Color.FromRgb(191, 153, 38),
        Color.FromRgb(229, 184, 46),
        Color.FromRgb(255, 204, 51),
        Color.FromRgb(153, 115, 0),
        Color.FromRgb(166, 124, 0),
        Color.FromRgb(178, 134, 0),
        Color.FromRgb(204, 153, 0),
        Color.FromRgb(229, 172, 0),
        Color.FromRgb(255, 191, 0),
        Color.FromRgb(153, 147, 92),
        Color.FromRgb(166, 159, 99),
        Color.FromRgb(191, 184, 115),
        Color.FromRgb(229, 220, 138),
        Color.FromRgb(255, 245, 153),
        Color.FromRgb(153, 144, 61),
        Color.FromRgb(166, 156, 66),
        Color.FromRgb(178, 168, 71),
        Color.FromRgb(191, 180, 77),
        Color.FromRgb(229, 216, 92),
        Color.FromRgb(255, 240, 102),
        Color.FromRgb(153, 141, 31),
        Color.FromRgb(166, 152, 33),
        Color.FromRgb(178, 164, 36),
        Color.FromRgb(204, 188, 41),
        Color.FromRgb(242, 223, 48),
        Color.FromRgb(153, 138, 0),
        Color.FromRgb(166, 149, 0),
        Color.FromRgb(178, 161, 0),
        Color.FromRgb(191, 172, 0),
        Color.FromRgb(217, 195, 0),
        Color.FromRgb(255, 230, 0),
        Color.FromRgb(150, 153, 92),
        Color.FromRgb(162, 166, 99),
        Color.FromRgb(175, 179, 107),
        Color.FromRgb(200, 204, 122),
        Color.FromRgb(237, 242, 145),
        Color.FromRgb(148, 153, 61),
        Color.FromRgb(161, 166, 66),
        Color.FromRgb(173, 179, 71),
        Color.FromRgb(198, 204, 82),
        Color.FromRgb(223, 230, 92),
        Color.FromRgb(247, 255, 102),
        Color.FromRgb(147, 153, 31),
        Color.FromRgb(159, 166, 33),
        Color.FromRgb(171, 179, 36),
        Color.FromRgb(184, 191, 38),
        Color.FromRgb(208, 217, 43),
        Color.FromRgb(245, 255, 51),
        Color.FromRgb(145, 153, 0),
        Color.FromRgb(157, 166, 0),
        Color.FromRgb(170, 179, 0),
        Color.FromRgb(182, 191, 0),
        Color.FromRgb(206, 217, 0),
        Color.FromRgb(242, 255, 0),
        Color.FromRgb(141, 153, 92),
        Color.FromRgb(152, 166, 99),
        Color.FromRgb(176, 191, 115),
        Color.FromRgb(211, 230, 138),
        Color.FromRgb(235, 255, 153),
        Color.FromRgb(135, 153, 61),
        Color.FromRgb(146, 166, 66),
        Color.FromRgb(157, 179, 71),
        Color.FromRgb(168, 191, 77),
        Color.FromRgb(191, 217, 87),
        Color.FromRgb(213, 242, 97),
        Color.FromRgb(129, 153, 31),
        Color.FromRgb(139, 166, 33),
        Color.FromRgb(150, 179, 36),
        Color.FromRgb(161, 191, 38),
        Color.FromRgb(171, 204, 41),
        Color.FromRgb(193, 230, 46),
        Color.FromRgb(214, 255, 51),
        Color.FromRgb(122, 153, 0),
        Color.FromRgb(133, 166, 0),
        Color.FromRgb(143, 179, 0),
        Color.FromRgb(153, 191, 0),
        Color.FromRgb(163, 204, 0),
        Color.FromRgb(184, 230, 0),
        Color.FromRgb(204, 255, 0),
        Color.FromRgb(132, 153, 92),
        Color.FromRgb(143, 166, 99),
        Color.FromRgb(164, 191, 115),
        Color.FromRgb(197, 230, 138),
        Color.FromRgb(219, 255, 153),
        Color.FromRgb(121, 153, 61),
        Color.FromRgb(131, 166, 66),
        Color.FromRgb(141, 179, 71),
        Color.FromRgb(161, 204, 82),
        Color.FromRgb(181, 230, 92),
        Color.FromRgb(201, 255, 102),
        Color.FromRgb(110, 153, 31),
        Color.FromRgb(119, 166, 33),
        Color.FromRgb(129, 179, 36),
        Color.FromRgb(138, 191, 38),
        Color.FromRgb(147, 204, 41),
        Color.FromRgb(156, 217, 43),
        Color.FromRgb(174, 242, 48),
        Color.FromRgb(99, 153, 0),
        Color.FromRgb(108, 166, 0),
        Color.FromRgb(116, 179, 0),
        Color.FromRgb(124, 191, 0),
        Color.FromRgb(133, 204, 0),
        Color.FromRgb(149, 230, 0),
        Color.FromRgb(166, 255, 0),
        Color.FromRgb(122, 153, 92),
        Color.FromRgb(133, 166, 99),
        Color.FromRgb(143, 179, 107),
        Color.FromRgb(153, 191, 115),
        Color.FromRgb(173, 217, 130),
        Color.FromRgb(194, 242, 145),
        Color.FromRgb(107, 153, 61),
        Color.FromRgb(116, 166, 66),
        Color.FromRgb(125, 179, 71),
        Color.FromRgb(143, 204, 82),
        Color.FromRgb(161, 230, 92),
        Color.FromRgb(178, 255, 102),
        Color.FromRgb(92, 153, 31),
        Color.FromRgb(99, 166, 33),
        Color.FromRgb(107, 179, 36),
        Color.FromRgb(115, 191, 38),
        Color.FromRgb(122, 204, 41),
        Color.FromRgb(130, 217, 43),
        Color.FromRgb(145, 242, 48),
        Color.FromRgb(76, 153, 0),
        Color.FromRgb(83, 166, 0),
        Color.FromRgb(89, 179, 0),
        Color.FromRgb(96, 191, 0),
        Color.FromRgb(102, 204, 0),
        Color.FromRgb(108, 217, 0),
        Color.FromRgb(121, 242, 0),
        Color.FromRgb(113, 153, 92),
        Color.FromRgb(123, 166, 99),
        Color.FromRgb(142, 191, 115),
        Color.FromRgb(170, 230, 138),
        Color.FromRgb(189, 255, 153),
        Color.FromRgb(93, 153, 61),
        Color.FromRgb(101, 166, 66),
        Color.FromRgb(109, 179, 71),
        Color.FromRgb(117, 191, 77),
        Color.FromRgb(132, 217, 87),
        Color.FromRgb(148, 242, 97),
        Color.FromRgb(73, 153, 31),
        Color.FromRgb(80, 166, 33),
        Color.FromRgb(86, 179, 36),
        Color.FromRgb(92, 191, 38),
        Color.FromRgb(98, 204, 41),
        Color.FromRgb(104, 217, 43),
        Color.FromRgb(122, 255, 51),
        Color.FromRgb(54, 153, 0),
        Color.FromRgb(58, 166, 0),
        Color.FromRgb(62, 179, 0),
        Color.FromRgb(67, 191, 0),
        Color.FromRgb(76, 217, 0),
        Color.FromRgb(85, 242, 0),
        Color.FromRgb(121, 179, 107),
        Color.FromRgb(130, 191, 115),
        Color.FromRgb(147, 217, 130),
        Color.FromRgb(173, 255, 153),
        Color.FromRgb(106, 204, 82),
        Color.FromRgb(119, 230, 92),
        Color.FromRgb(133, 255, 102),
        Color.FromRgb(55, 153, 31),
        Color.FromRgb(60, 166, 33),
        Color.FromRgb(64, 179, 36),
        Color.FromRgb(69, 191, 38),
        Color.FromRgb(73, 204, 41),
        Color.FromRgb(78, 217, 43),
        Color.FromRgb(87, 242, 48),
        Color.FromRgb(41, 204, 0),
        Color.FromRgb(46, 230, 0),
        Color.FromRgb(51, 255, 0),
        Color.FromRgb(126, 204, 122),
        Color.FromRgb(142, 230, 138),
        Color.FromRgb(158, 255, 153),
        Color.FromRgb(66, 153, 61),
        Color.FromRgb(71, 166, 66),
        Color.FromRgb(77, 179, 71),
        Color.FromRgb(82, 191, 77),
        Color.FromRgb(93, 217, 87),
        Color.FromRgb(110, 255, 102),
        Color.FromRgb(61, 255, 51),
        Color.FromRgb(8, 153, 0),
        Color.FromRgb(92, 153, 98),
        Color.FromRgb(99, 166, 106),
        Color.FromRgb(115, 191, 122),
        Color.FromRgb(130, 217, 139),
        Color.FromRgb(153, 255, 163),
        Color.FromRgb(66, 166, 76),
        Color.FromRgb(71, 179, 82),
        Color.FromRgb(77, 191, 88),
        Color.FromRgb(82, 204, 94),
        Color.FromRgb(97, 242, 111),
        Color.FromRgb(33, 166, 46),
        Color.FromRgb(36, 179, 50),
        Color.FromRgb(38, 191, 54),
        Color.FromRgb(41, 204, 57),
        Color.FromRgb(43, 217, 61),
        Color.FromRgb(0, 166, 17),
        Color.FromRgb(0, 179, 18),
        Color.FromRgb(0, 191, 19),
        Color.FromRgb(0, 217, 22),
        Color.FromRgb(92, 153, 107),
        Color.FromRgb(99, 166, 116),
        Color.FromRgb(115, 191, 134),
        Color.FromRgb(130, 217, 152),
        Color.FromRgb(153, 255, 178),
        Color.FromRgb(61, 153, 84),
        Color.FromRgb(66, 166, 91),
        Color.FromRgb(71, 179, 98),
        Color.FromRgb(82, 204, 112),
        Color.FromRgb(92, 230, 126),
        Color.FromRgb(102, 255, 140),
        Color.FromRgb(33, 166, 66),
        Color.FromRgb(43, 217, 87),
        Color.FromRgb(51, 255, 102),
        Color.FromRgb(0, 153, 38),
        Color.FromRgb(0, 230, 57),
        Color.FromRgb(0, 255, 64),
        Color.FromRgb(92, 153, 116),
        Color.FromRgb(99, 166, 126),
        Color.FromRgb(107, 179, 136),
        Color.FromRgb(122, 204, 155),
        Color.FromRgb(138, 230, 174),
        Color.FromRgb(153, 255, 194),
        Color.FromRgb(61, 153, 98),
        Color.FromRgb(66, 166, 106),
        Color.FromRgb(71, 179, 114),
        Color.FromRgb(77, 191, 122),
        Color.FromRgb(92, 230, 147),
        Color.FromRgb(102, 255, 163),
        Color.FromRgb(31, 153, 80),
        Color.FromRgb(33, 166, 86),
        Color.FromRgb(36, 179, 93),
        Color.FromRgb(38, 191, 99),
        Color.FromRgb(43, 217, 113),
        Color.FromRgb(48, 242, 126),
        Color.FromRgb(0, 153, 61),
        Color.FromRgb(0, 179, 71),
        Color.FromRgb(0, 191, 77),
        Color.FromRgb(0, 204, 82),
        Color.FromRgb(0, 230, 92),
        Color.FromRgb(92, 153, 125),
        Color.FromRgb(107, 179, 146),
        Color.FromRgb(122, 204, 167),
        Color.FromRgb(138, 230, 188),
        Color.FromRgb(153, 255, 209),
        Color.FromRgb(61, 153, 112),
        Color.FromRgb(66, 166, 121),
        Color.FromRgb(71, 179, 130),
        Color.FromRgb(82, 204, 149),
        Color.FromRgb(92, 230, 168),
        Color.FromRgb(102, 255, 186),
        Color.FromRgb(31, 153, 98),
        Color.FromRgb(33, 166, 106),
        Color.FromRgb(36, 179, 114),
        Color.FromRgb(41, 204, 131),
        Color.FromRgb(48, 242, 155),
        Color.FromRgb(0, 204, 112),
        Color.FromRgb(0, 230, 126),
        Color.FromRgb(0, 255, 140),
        Color.FromRgb(92, 153, 135),
        Color.FromRgb(107, 179, 157),
        Color.FromRgb(122, 204, 180),
        Color.FromRgb(138, 230, 202),
        Color.FromRgb(153, 255, 224),
        Color.FromRgb(61, 153, 125),
        Color.FromRgb(66, 166, 136),
        Color.FromRgb(71, 179, 146),
        Color.FromRgb(82, 204, 167),
        Color.FromRgb(97, 242, 199),
        Color.FromRgb(31, 153, 116),
        Color.FromRgb(33, 166, 126),
        Color.FromRgb(36, 179, 136),
        Color.FromRgb(41, 204, 155),
        Color.FromRgb(46, 230, 174),
        Color.FromRgb(51, 255, 194),
        Color.FromRgb(0, 153, 107),
        Color.FromRgb(0, 166, 116),
        Color.FromRgb(0, 179, 125),
        Color.FromRgb(0, 204, 143),
        Color.FromRgb(0, 230, 161),
        Color.FromRgb(0, 255, 178),
        Color.FromRgb(92, 153, 144),
        Color.FromRgb(107, 179, 168),
        Color.FromRgb(122, 204, 192),
        Color.FromRgb(138, 230, 216),
        Color.FromRgb(153, 255, 240),
        Color.FromRgb(61, 153, 139),
        Color.FromRgb(66, 166, 151),
        Color.FromRgb(71, 179, 162),
        Color.FromRgb(82, 204, 186),
        Color.FromRgb(97, 242, 220),
        Color.FromRgb(31, 153, 135),
        Color.FromRgb(36, 179, 157),
        Color.FromRgb(43, 217, 191),
        Color.FromRgb(48, 242, 213),
        Color.FromRgb(0, 166, 141),
        Color.FromRgb(0, 191, 163),
        Color.FromRgb(0, 217, 184),
        Color.FromRgb(0, 255, 217),
        Color.FromRgb(92, 153, 153),
        Color.FromRgb(99, 166, 166),
        Color.FromRgb(115, 191, 191),
        Color.FromRgb(130, 217, 217),
        Color.FromRgb(145, 242, 242),
        Color.FromRgb(61, 153, 153),
        Color.FromRgb(66, 166, 166),
        Color.FromRgb(71, 179, 179),
        Color.FromRgb(82, 204, 204),
        Color.FromRgb(97, 242, 242),
        Color.FromRgb(41, 204, 204),
        Color.FromRgb(0, 153, 153),
        Color.FromRgb(0, 166, 166),
        Color.FromRgb(0, 179, 179),
        Color.FromRgb(0, 217, 217),
        Color.FromRgb(0, 242, 242),
        Color.FromRgb(92, 144, 153),
        Color.FromRgb(99, 156, 166),
        Color.FromRgb(122, 192, 204),
        Color.FromRgb(138, 216, 230),
        Color.FromRgb(153, 240, 255),
        Color.FromRgb(66, 151, 166),
        Color.FromRgb(77, 174, 191),
        Color.FromRgb(87, 197, 217),
        Color.FromRgb(97, 220, 242),
        Color.FromRgb(36, 157, 179),
        Color.FromRgb(43, 191, 217),
        Color.FromRgb(48, 213, 242),
        Color.FromRgb(0, 130, 153),
        Color.FromRgb(0, 152, 179),
        Color.FromRgb(0, 173, 204),
        Color.FromRgb(0, 195, 230),
        Color.FromRgb(0, 217, 255),
        Color.FromRgb(92, 135, 153),
        Color.FromRgb(115, 168, 191),
        Color.FromRgb(138, 202, 230),
        Color.FromRgb(153, 224, 255),
        Color.FromRgb(66, 136, 166),
        Color.FromRgb(77, 157, 191),
        Color.FromRgb(92, 188, 230),
        Color.FromRgb(102, 209, 255),
        Color.FromRgb(33, 126, 166),
        Color.FromRgb(41, 155, 204),
        Color.FromRgb(46, 174, 230),
        Color.FromRgb(51, 194, 255),
        Color.FromRgb(0, 107, 153),
        Color.FromRgb(0, 116, 166),
        Color.FromRgb(0, 134, 191),
        Color.FromRgb(0, 161, 230),
        Color.FromRgb(0, 179, 255),
        Color.FromRgb(99, 136, 166),
        Color.FromRgb(122, 167, 204),
        Color.FromRgb(138, 188, 230),
        Color.FromRgb(153, 209, 255),
        Color.FromRgb(61, 112, 153),
        Color.FromRgb(71, 130, 179),
        Color.FromRgb(87, 158, 217),
        Color.FromRgb(102, 186, 255),
        Color.FromRgb(31, 98, 153),
        Color.FromRgb(36, 114, 179),
        Color.FromRgb(43, 139, 217),
        Color.FromRgb(51, 163, 255),
        Color.FromRgb(0, 91, 166),
        Color.FromRgb(0, 105, 191),
        Color.FromRgb(0, 126, 230),
        Color.FromRgb(0, 140, 255),
        Color.FromRgb(92, 116, 153),
        Color.FromRgb(107, 136, 179),
        Color.FromRgb(122, 155, 204),
        Color.FromRgb(138, 174, 230),
        Color.FromRgb(153, 194, 255),
        Color.FromRgb(66, 106, 166),
        Color.FromRgb(77, 122, 191),
        Color.FromRgb(87, 139, 217),
        Color.FromRgb(97, 155, 242),
        Color.FromRgb(31, 80, 153),
        Color.FromRgb(36, 93, 179),
        Color.FromRgb(41, 106, 204),
        Color.FromRgb(46, 119, 230),
        Color.FromRgb(51, 133, 255),
        Color.FromRgb(0, 61, 153),
        Color.FromRgb(0, 71, 179),
        Color.FromRgb(0, 87, 217),
        Color.FromRgb(0, 102, 255),
        Color.FromRgb(99, 116, 166),
        Color.FromRgb(115, 134, 191),
        Color.FromRgb(130, 152, 217),
        Color.FromRgb(145, 170, 242),
        Color.FromRgb(66, 91, 166),
        Color.FromRgb(77, 105, 191),
        Color.FromRgb(87, 119, 217),
        Color.FromRgb(102, 140, 255),
        Color.FromRgb(33, 66, 166),
        Color.FromRgb(38, 76, 191),
        Color.FromRgb(46, 92, 230),
        Color.FromRgb(0, 41, 166),
        Color.FromRgb(0, 45, 179),
        Color.FromRgb(0, 51, 204),
        Color.FromRgb(0, 57, 230),
        Color.FromRgb(0, 64, 255),
        Color.FromRgb(92, 98, 153),
        Color.FromRgb(107, 114, 179),
        Color.FromRgb(122, 131, 204),
        Color.FromRgb(138, 147, 230),
        Color.FromRgb(153, 163, 255),
        Color.FromRgb(61, 70, 153),
        Color.FromRgb(71, 82, 179),
        Color.FromRgb(87, 100, 217),
        Color.FromRgb(97, 111, 242),
        Color.FromRgb(31, 43, 153),
        Color.FromRgb(38, 54, 191),
        Color.FromRgb(43, 61, 217),
        Color.FromRgb(48, 68, 242),
        Color.FromRgb(0, 15, 153),
        Color.FromRgb(0, 17, 166),
        Color.FromRgb(0, 18, 179),
        Color.FromRgb(0, 19, 191),
        Color.FromRgb(0, 22, 217),
        Color.FromRgb(0, 25, 255),
        Color.FromRgb(95, 92, 153),
        Color.FromRgb(103, 99, 166),
        Color.FromRgb(119, 115, 191),
        Color.FromRgb(134, 130, 217),
        Color.FromRgb(158, 153, 255),
        Color.FromRgb(66, 61, 153),
        Color.FromRgb(71, 66, 166),
        Color.FromRgb(77, 71, 179),
        Color.FromRgb(88, 82, 204),
        Color.FromRgb(99, 92, 230),
        Color.FromRgb(110, 102, 255),
        Color.FromRgb(37, 31, 153),
        Color.FromRgb(43, 36, 179),
        Color.FromRgb(49, 41, 204),
        Color.FromRgb(55, 46, 230),
        Color.FromRgb(61, 51, 255),
        Color.FromRgb(8, 0, 166),
        Color.FromRgb(9, 0, 179),
        Color.FromRgb(10, 0, 204),
        Color.FromRgb(12, 0, 230),
        Color.FromRgb(113, 99, 166),
        Color.FromRgb(130, 115, 191),
        Color.FromRgb(147, 130, 217),
        Color.FromRgb(165, 145, 242),
        Color.FromRgb(80, 61, 153),
        Color.FromRgb(86, 66, 166),
        Color.FromRgb(93, 71, 179),
        Color.FromRgb(106, 82, 204),
        Color.FromRgb(126, 97, 242),
        Color.FromRgb(60, 33, 166),
        Color.FromRgb(69, 38, 191),
        Color.FromRgb(78, 43, 217),
        Color.FromRgb(92, 51, 255),
        Color.FromRgb(31, 0, 153),
        Color.FromRgb(38, 0, 191),
        Color.FromRgb(43, 0, 217),
        Color.FromRgb(48, 0, 242),
        Color.FromRgb(113, 92, 153),
        Color.FromRgb(123, 99, 166),
        Color.FromRgb(142, 115, 191),
        Color.FromRgb(170, 138, 230),
        Color.FromRgb(189, 153, 255),
        Color.FromRgb(93, 61, 153),
        Color.FromRgb(101, 66, 166),
        Color.FromRgb(117, 77, 191),
        Color.FromRgb(132, 87, 217),
        Color.FromRgb(148, 97, 242),
        Color.FromRgb(73, 31, 153),
        Color.FromRgb(80, 33, 166),
        Color.FromRgb(86, 36, 179),
        Color.FromRgb(98, 41, 204),
        Color.FromRgb(116, 48, 242),
        Color.FromRgb(54, 0, 153),
        Color.FromRgb(58, 0, 166),
        Color.FromRgb(62, 0, 179),
        Color.FromRgb(67, 0, 191),
        Color.FromRgb(71, 0, 204),
        Color.FromRgb(80, 0, 230),
        Color.FromRgb(89, 0, 255),
        Color.FromRgb(122, 92, 153),
        Color.FromRgb(143, 107, 179),
        Color.FromRgb(163, 122, 204),
        Color.FromRgb(184, 138, 230),
        Color.FromRgb(204, 153, 255),
        Color.FromRgb(107, 61, 153),
        Color.FromRgb(116, 66, 166),
        Color.FromRgb(134, 77, 191),
        Color.FromRgb(161, 92, 230),
        Color.FromRgb(179, 102, 255),
        Color.FromRgb(92, 31, 153),
        Color.FromRgb(99, 33, 166),
        Color.FromRgb(107, 36, 179),
        Color.FromRgb(122, 41, 204),
        Color.FromRgb(145, 48, 242),
        Color.FromRgb(77, 0, 153),
        Color.FromRgb(83, 0, 166),
        Color.FromRgb(89, 0, 179),
        Color.FromRgb(96, 0, 191),
        Color.FromRgb(108, 0, 217),
        Color.FromRgb(128, 0, 255),
        Color.FromRgb(132, 92, 153),
        Color.FromRgb(154, 107, 179),
        Color.FromRgb(175, 122, 204),
        Color.FromRgb(208, 145, 242),
        Color.FromRgb(121, 61, 153),
        Color.FromRgb(131, 66, 166),
        Color.FromRgb(141, 71, 179),
        Color.FromRgb(161, 82, 204),
        Color.FromRgb(181, 92, 230),
        Color.FromRgb(201, 102, 255),
        Color.FromRgb(110, 31, 153),
        Color.FromRgb(119, 33, 166),
        Color.FromRgb(129, 36, 179),
        Color.FromRgb(147, 41, 204),
        Color.FromRgb(165, 46, 230),
        Color.FromRgb(184, 51, 255),
        Color.FromRgb(99, 0, 153),
        Color.FromRgb(108, 0, 166),
        Color.FromRgb(116, 0, 179),
        Color.FromRgb(124, 0, 191),
        Color.FromRgb(133, 0, 204),
        Color.FromRgb(149, 0, 230),
        Color.FromRgb(166, 0, 255),
        Color.FromRgb(141, 92, 153),
        Color.FromRgb(152, 99, 166),
        Color.FromRgb(176, 115, 191),
        Color.FromRgb(211, 138, 230),
        Color.FromRgb(235, 153, 255),
        Color.FromRgb(135, 61, 153),
        Color.FromRgb(146, 66, 166),
        Color.FromRgb(168, 77, 191),
        Color.FromRgb(191, 87, 217),
        Color.FromRgb(213, 97, 242),
        Color.FromRgb(129, 31, 153),
        Color.FromRgb(139, 33, 166),
        Color.FromRgb(150, 36, 179),
        Color.FromRgb(161, 38, 191),
        Color.FromRgb(182, 43, 217),
        Color.FromRgb(203, 48, 242),
        Color.FromRgb(122, 0, 153),
        Color.FromRgb(133, 0, 166),
        Color.FromRgb(143, 0, 179),
        Color.FromRgb(153, 0, 191),
        Color.FromRgb(173, 0, 217),
        Color.FromRgb(204, 0, 255),
        Color.FromRgb(150, 92, 153),
        Color.FromRgb(162, 99, 166),
        Color.FromRgb(187, 115, 191),
        Color.FromRgb(225, 138, 230),
        Color.FromRgb(250, 153, 255),
        Color.FromRgb(148, 61, 153),
        Color.FromRgb(161, 66, 166),
        Color.FromRgb(173, 71, 179),
        Color.FromRgb(186, 77, 191),
        Color.FromRgb(210, 87, 217),
        Color.FromRgb(247, 102, 255),
        Color.FromRgb(147, 31, 153),
        Color.FromRgb(159, 33, 166),
        Color.FromRgb(171, 36, 179),
        Color.FromRgb(196, 41, 204),
        Color.FromRgb(220, 46, 230),
        Color.FromRgb(245, 51, 255),
        Color.FromRgb(145, 0, 153),
        Color.FromRgb(157, 0, 166),
        Color.FromRgb(170, 0, 179),
        Color.FromRgb(194, 0, 204),
        Color.FromRgb(218, 0, 230),
        Color.FromRgb(242, 0, 255),
        Color.FromRgb(153, 92, 147),
        Color.FromRgb(179, 107, 171),
        Color.FromRgb(204, 122, 196),
        Color.FromRgb(230, 138, 220),
        Color.FromRgb(255, 153, 245),
        Color.FromRgb(153, 61, 144),
        Color.FromRgb(166, 66, 156),
        Color.FromRgb(179, 71, 168),
        Color.FromRgb(217, 87, 204),
        Color.FromRgb(255, 102, 240),
        Color.FromRgb(153, 31, 141),
        Color.FromRgb(166, 33, 152),
        Color.FromRgb(179, 36, 164),
        Color.FromRgb(217, 43, 199),
        Color.FromRgb(255, 51, 235),
        Color.FromRgb(153, 0, 138),
        Color.FromRgb(166, 0, 149),
        Color.FromRgb(191, 0, 172),
        Color.FromRgb(230, 0, 207),
        Color.FromRgb(153, 92, 138),
        Color.FromRgb(166, 99, 149),
        Color.FromRgb(204, 122, 184),
        Color.FromRgb(242, 145, 218),
        Color.FromRgb(153, 61, 130),
        Color.FromRgb(179, 71, 152),
        Color.FromRgb(204, 82, 173),
        Color.FromRgb(230, 92, 195),
        Color.FromRgb(255, 102, 217),
        Color.FromRgb(153, 31, 122),
        Color.FromRgb(166, 33, 133),
        Color.FromRgb(179, 36, 143),
        Color.FromRgb(204, 41, 163),
        Color.FromRgb(230, 46, 184),
        Color.FromRgb(255, 51, 204),
        Color.FromRgb(153, 0, 115),
        Color.FromRgb(166, 0, 124),
        Color.FromRgb(191, 0, 143),
        Color.FromRgb(217, 0, 163),
        Color.FromRgb(242, 0, 182),
        Color.FromRgb(166, 99, 139),
        Color.FromRgb(204, 122, 171),
        Color.FromRgb(230, 138, 193),
        Color.FromRgb(255, 153, 214),
        Color.FromRgb(153, 61, 116),
        Color.FromRgb(166, 66, 126),
        Color.FromRgb(191, 77, 145),
        Color.FromRgb(217, 87, 165),
        Color.FromRgb(255, 102, 194),
        Color.FromRgb(153, 31, 104),
        Color.FromRgb(166, 33, 113),
        Color.FromRgb(191, 38, 130),
        Color.FromRgb(217, 43, 147),
        Color.FromRgb(255, 51, 173),
        Color.FromRgb(153, 0, 92),
        Color.FromRgb(178, 0, 107),
        Color.FromRgb(204, 0, 122),
        Color.FromRgb(242, 0, 145),
        Color.FromRgb(153, 92, 119),
        Color.FromRgb(179, 107, 139),
        Color.FromRgb(217, 130, 169),
        Color.FromRgb(255, 153, 199),
        Color.FromRgb(153, 61, 103),
        Color.FromRgb(179, 71, 120),
        Color.FromRgb(204, 82, 137),
        Color.FromRgb(242, 97, 162),
        Color.FromRgb(153, 31, 86),
        Color.FromRgb(166, 33, 93),
        Color.FromRgb(191, 38, 107),
        Color.FromRgb(229, 46, 129),
        Color.FromRgb(255, 51, 143),
        Color.FromRgb(153, 0, 69),
        Color.FromRgb(166, 0, 75),
        Color.FromRgb(191, 0, 86),
        Color.FromRgb(217, 0, 98),
        Color.FromRgb(242, 0, 109),
        Color.FromRgb(166, 99, 119),
        Color.FromRgb(204, 122, 147),
        Color.FromRgb(230, 138, 165),
        Color.FromRgb(255, 153, 184),
        Color.FromRgb(153, 61, 89),
        Color.FromRgb(166, 66, 96),
        Color.FromRgb(204, 82, 118),
        Color.FromRgb(229, 92, 133),
        Color.FromRgb(255, 102, 148),
        Color.FromRgb(153, 31, 67),
        Color.FromRgb(166, 33, 73),
        Color.FromRgb(178, 36, 79),
        Color.FromRgb(217, 43, 95),
        Color.FromRgb(242, 48, 107),
        Color.FromRgb(153, 0, 46),
        Color.FromRgb(166, 0, 50),
        Color.FromRgb(191, 0, 57),
        Color.FromRgb(217, 0, 65),
        Color.FromRgb(242, 0, 73),
        Color.FromRgb(166, 99, 109),
        Color.FromRgb(191, 115, 126),
        Color.FromRgb(217, 130, 143),
        Color.FromRgb(242, 145, 160),
        Color.FromRgb(153, 61, 75),
        Color.FromRgb(178, 71, 87),
        Color.FromRgb(217, 87, 106),
        Color.FromRgb(242, 97, 119),
        Color.FromRgb(153, 31, 49),
        Color.FromRgb(166, 33, 53),
        Color.FromRgb(191, 38, 61),
        Color.FromRgb(217, 43, 69),
        Color.FromRgb(242, 48, 78),
        Color.FromRgb(153, 0, 23),
        Color.FromRgb(166, 0, 25),
        Color.FromRgb(178, 0, 27),
        Color.FromRgb(204, 0, 31),
        Color.FromRgb(229, 0, 34),
        Color.FromRgb(255, 0, 38)
      };
    }
  }
}
