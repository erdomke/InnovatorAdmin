using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Aras.Tools.InnovatorAdmin
{
  public static class DataExtractorFactory
  {
    public static IDataExtractor Get(IEnumerable<string> paths, ImportType type)
    {
      switch (type)
      {
        case ImportType.Files:
          return new FolderExtractor(paths);
        case ImportType.DataFile:
          return new ExcelExtractor(paths.Single());
        default:
          throw new NotImplementedException();
      }
    }

    public static IDataExtractor Deserialize(string data)
    {
      var concreteTypes = typeof(DataExtractorFactory).Assembly.GetTypes().Where(t => typeof(IDataExtractor).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);
      var serializer = concreteTypes.Select(t => new XmlSerializer(t))
        .FirstOrDefault(s => {
          using (var reader = new StringReader(data))
          {
            using (var xml = XmlReader.Create(reader))
            {
              return s.CanDeserialize(xml);
            }
          }
        });

      if (serializer == null) return null;
      using (var reader = new StringReader(data))
      {
        return (IDataExtractor)serializer.Deserialize(reader);
      }
    }

    public static string Serialize(this IDataExtractor extractor)
    {
      using (var writer = new StringWriter())
      {
        var serializer = new XmlSerializer(extractor.GetType());
        serializer.Serialize(writer, extractor);
        return writer.ToString();
      }
    }
  }
}
