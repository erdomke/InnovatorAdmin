using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Xml.Schema;

namespace InnovatorAdmin.Editor
{
  public static class XmlSchemas
  {
    public static XmlSchemaSet SchemasFromDescrip(params ServiceDescription[] descriptions)
    {
      return SchemasFromDescrip((IEnumerable<ServiceDescription>)descriptions);
    }
    public static XmlSchemaSet SchemasFromDescrip(IEnumerable<ServiceDescription> descriptions)
    {
      var schemaSet = new XmlSchemaSet();
      var soap = new StringReader(string.Format(Properties.Resources.SoapSchema, descriptions.First().TargetNamespace));
      schemaSet.Add(XmlSchema.Read(soap, new ValidationEventHandler(Validation)));
      foreach (var schema in descriptions.SelectMany(d => d.Types.Schemas.OfType<XmlSchema>()))
      {
        schemaSet.Add(schema);
      }
      schemaSet.Compile();
      return schemaSet;
    }

    private static void Validation(object sender, ValidationEventArgs e)
    {
      // Do nothing
    }
  }
}
