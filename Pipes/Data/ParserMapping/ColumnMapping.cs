using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Pipes.Data.ParserMapping
{
  public class ColumnMapping : IXmlSerializable
  {
    public CellAddress Address { get; set; }
    public string ConstantValue { get; set; }
    public IDestinationColumn Destination { get; set; }
    public CellRepeat Repeat { get; set; }
    public string StopValue { get; set; }

    System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
    {
      return null;
    }

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == typeof(ColumnMapping).Name)
      {
        XmlSerializer deserializer;
        string value;

        while (reader.Read() && reader.MoveToContent() == XmlNodeType.Element)
        {
          switch (reader.LocalName)
          {
            case "Address":
              if (!reader.IsEmptyElement)
              {
                reader.Read();  // Consume start element
                reader.MoveToContent();
                deserializer = new XmlSerializer(typeof(CellAddress));
                this.Address = (CellAddress)deserializer.Deserialize(reader);
                // Already positioned on address end element
              }
              break;
            case "ConstantValue":
              if (!reader.IsEmptyElement)
              {
                this.ConstantValue = reader.ReadString();
              }
              break;
            case "Destination":
              if (!reader.IsEmptyElement)
              {
                Type type;
                if (ParserMapping.TryGetType(reader["type"], reader["aqn"], out type))
                {
                  reader.Read();  // Consume start element
                  reader.MoveToContent();
                  deserializer = new XmlSerializer(type);
                  this.Destination = (IDestinationColumn)deserializer.Deserialize(reader);
                }
                else
                {
                  throw new InvalidOperationException("Cannot interpret type name " + reader["type"]);
                }
              }
              break;
            case "Repeat":
              if (!reader.IsEmptyElement)
              {
                value = reader.ReadString();
                this.Repeat = (CellRepeat)Enum.Parse(typeof(CellRepeat), value);
              }
              break;
            case "StopValue":
              if (!reader.IsEmptyElement)
              {
                this.StopValue = reader.ReadString();
              }
              break;
            default:
              throw new InvalidOperationException("Element " + reader.Name + " is unexpected in " + typeof(ColumnMapping).Name);
          }
        }
      }
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
      XmlSerializer serializer;

      if (this.Address != null)
      {
        writer.WriteStartElement("Address");
        serializer = new XmlSerializer(this.Address.GetType());
        serializer.Serialize(writer, this.Address);
        writer.WriteEndElement();
      }
      if (!string.IsNullOrEmpty(this.ConstantValue)) writer.WriteElementString("ConstantValue", this.ConstantValue);
      if (this.Destination != null)
      {
        writer.WriteStartElement("Destination");
        writer.WriteAttributeString("type", this.Destination.GetType().FullName);
        writer.WriteAttributeString("aqn", this.Destination.GetType().AssemblyQualifiedName);
        serializer = new XmlSerializer(this.Destination.GetType());
        serializer.Serialize(writer, this.Destination);
        writer.WriteEndElement();
      }
      writer.WriteElementString("Repeat", this.Repeat.ToString());
      if(!string.IsNullOrEmpty(this.StopValue)) writer.WriteElementString("StopValue", this.StopValue);
    }

    public override bool Equals(object obj)
    {
      var mapping = obj as ColumnMapping;
      if (mapping == null) return false;
      return this.Equals(mapping);
    }
    public bool Equals(ColumnMapping obj)
    {
      return Extension.IsEqual(this.Address, obj.Address) && 
             this.ConstantValue == obj.ConstantValue &&
             Extension.IsEqual(this.Destination, obj.Destination) &&
             this.Repeat == obj.Repeat && this.StopValue == obj.StopValue;
    }
    public override int GetHashCode()
    {
      return (this.Address == null ? 0 : this.Address.GetHashCode()) ^
             (this.ConstantValue ?? "").GetHashCode() ^
             (this.Destination == null ? 0 : this.Destination.GetHashCode()) ^
             this.Repeat.GetHashCode() ^
             (this.StopValue ?? "").GetHashCode();
    }
  }
}
