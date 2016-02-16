using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public class XmlWriterSettings : Sgml.ISgmlTextWriterSettings
  {
    private System.Xml.XmlWriterSettings _settings;

    public XmlWriterSettings()
    {
      _settings = new System.Xml.XmlWriterSettings();
    }
    public XmlWriterSettings(System.Xml.XmlWriterSettings settings)
    {
      _settings = settings;
    }

    public System.Xml.ConformanceLevel ConformanceLevel 
    {
      get
      {
        return _settings.ConformanceLevel;
      }
      set
      {
        _settings.ConformanceLevel = value;
      }
    }
    public bool Indent
    {
      get
      {
        return _settings.Indent;
      }
      set
      {
        _settings.Indent = value;
      }
    }
    public string IndentChars
    {
      get
      {
        return _settings.IndentChars;
      }
      set
      {
        _settings.IndentChars = value;
      }
    }
    public string NewLineChars
    {
      get
      {
        return _settings.NewLineChars;
      }
      set
      {
        _settings.NewLineChars = value;
      }
    }
    public bool NewLineOnAttributes
    {
      get
      {
        return _settings.NewLineOnAttributes;
      }
      set
      {
        _settings.NewLineOnAttributes = value;
      }
    }
    public bool OmitXmlDeclaration
    {
      get
      {
        return _settings.OmitXmlDeclaration;
      }
      set
      {
        _settings.OmitXmlDeclaration = value;
      }
    }

    public System.Xml.XmlWriterSettings GetInnerSettings()
    {
      return _settings;
    }
  }
}
