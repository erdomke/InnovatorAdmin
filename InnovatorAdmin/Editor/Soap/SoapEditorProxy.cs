using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Schema;

namespace InnovatorAdmin.Editor
{
  public class SoapEditorProxy
  {
    public static void ParseWsdl(string url)
    {
      //http://www.border.gov.au/_vti_bin/Lists.asmx?wsdl
      var reader = XmlReader.Create(url);
      var wsdl = ServiceDescription.Read(reader);
      var schemaSet = new XmlSchemaSet();

      var soap = new StringReader(string.Format(Properties.Resources.SoapSchema, wsdl.TargetNamespace));
      schemaSet.Add(XmlSchema.Read(soap, new ValidationEventHandler(Validation)));
      foreach (var schema in wsdl.Types.Schemas.OfType<XmlSchema>())
      {
        schemaSet.Add(schema);
      }
      schemaSet.Compile();
      var thing = 2 + 2;
    }

    private static void Validation(object sender, ValidationEventArgs e)
    {
      // Do nothing
    }
  }
}
