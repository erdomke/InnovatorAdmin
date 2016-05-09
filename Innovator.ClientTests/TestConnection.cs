using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace Innovator.Client.Tests
{
  class TestConnection : IConnection
  {
    public ElementFactory AmlContext { get { return ElementFactory.Local; } }

    public string Database { get { return "Test"; } }

    public string UserId { get { return "2D246C5838644C1C8FD34F8D2796E327"; } }

    public UploadCommand CreateUploadCommand()
    {
      throw new NotImplementedException();
    }

    public string MapClientUrl(string relativeUrl)
    {
      return relativeUrl;
    }

    public Stream Process(Command request)
    {
      var elem = XElement.Parse(request.ToNormalizedAml(this.AmlContext.LocalizationContext));
      var result = @"<SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/'>
  <SOAP-ENV:Body>
    <SOAP-ENV:Fault xmlns:af='http://www.aras.com/InnovatorFault'>
      <faultcode>0</faultcode>
      <faultstring>No items of type found.</faultstring>
      <detail>
        <af:legacy_detail>No items of type found.</af:legacy_detail>
        <af:legacy_faultstring>No items of type '' found using the criteria: </af:legacy_faultstring>
      </detail>
    </SOAP-ENV:Fault>
  </SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
      if (AttrEquals(elem, "action", "get"))
      {
        switch (AttrValue(elem, "type"))
        {
          case "Company":
            if (AttrEquals(elem, "id", "0E086FFA6C4646F6939B74C43D094182"))
            {
              result = @"<Item type='Company' typeId='3E71E373FC2940B288760C915120AABE' id='0E086FFA6C4646F6939B74C43D094182'>
  <created_by_id keyed_name='First Last' type='User'>
    <Item type='User' typeId='45E899CD2859442982EB22BB2DF683E5' id='8227040ABF0A46A8AF06C18ABD3967B3'>
      <id keyed_name='First Last' type='User'>8227040ABF0A46A8AF06C18ABD3967B3</id>
      <first_name>First</first_name>
      <itemtype>45E899CD2859442982EB22BB2DF683E5</itemtype>
    </Item>
  </created_by_id>
  <id keyed_name='Another Company' type='Company'>0E086FFA6C4646F6939B74C43D094182</id>
  <permission_id keyed_name='Company' type='Permission'>
    <Item type='Permission' typeId='C6A89FDE1294451497801DF78341B473' id='A8FC3EC44ED0462B9A32D4564FAC0AD8'>
      <id keyed_name='Company' type='Permission'>A8FC3EC44ED0462B9A32D4564FAC0AD8</id>
      <name>Company</name>
    </Item>
  </permission_id>
  <itemtype>3E71E373FC2940B288760C915120AABE</itemtype>
</Item>";
            }
            break;
          case "User":
            if (AttrEquals(elem, "id", "8227040ABF0A46A8AF06C18ABD3967B3"))
            {
              result = @"<Result><Item type='User' typeId='45E899CD2859442982EB22BB2DF683E5' id='8227040ABF0A46A8AF06C18ABD3967B3'>
  <id keyed_name='First Last' type='User'>8227040ABF0A46A8AF06C18ABD3967B3</id>
  <first_name>First</first_name>
  <itemtype>45E899CD2859442982EB22BB2DF683E5</itemtype>
</Item></Result>";
            }
            break;
          case "Permission":
            if (AttrEquals(elem, "id", "A8FC3EC44ED0462B9A32D4564FAC0AD8"))
            {
              result = @"<Result><Item type='Permission' typeId='C6A89FDE1294451497801DF78341B473' id='A8FC3EC44ED0462B9A32D4564FAC0AD8'>
  <id keyed_name='Company' type='Permission'>A8FC3EC44ED0462B9A32D4564FAC0AD8</id>
  <name>Company</name>
</Item></Result>";
            }
            break;
        }
      }

      return new MemoryStream(Encoding.UTF8.GetBytes(result));
    }

    private string AttrValue(XElement elem, string name)
    {
      return elem.Attribute(name) == null ? null : elem.Attribute(name).Value;
    }
    private bool AttrEquals(XElement elem, string name, string value)
    {
      return elem.Attribute(name) != null && string.Equals(elem.Attribute(name).Value, value);
    }
  }
}
