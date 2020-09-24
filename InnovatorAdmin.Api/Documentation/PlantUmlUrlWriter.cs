using System;
using System.IO;
using System.IO.Compression;

namespace InnovatorAdmin.Documentation
{
  public class PlantUmlUrlWriter : IEntityWriter
  {
    public string Format { get; set; } = "svg";
    public Uri Url { get; set; } = new Uri("http://www.plantuml.com/plantuml/");
    public PlantUmlWriter Writer { get; set; }

    public void Write(EntityDiagram diagram, TextWriter writer)
    {
      var memStream = new MemoryStream();
      using (var deflate = new DeflateStream(memStream, CompressionLevel.Optimal))
      using (var compressWriter = new StreamWriter(deflate))
      {
        (Writer ?? new PlantUmlWriter()).Write(diagram, compressWriter);
      }

      const string base64Map = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
      const string plantUmlMap = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";
      var map = new char[128];
      for (var i = 0; i < base64Map.Length; i++)
        map[base64Map[i]] = plantUmlMap[i];

      var characters = Convert.ToBase64String(memStream.ToArray()).TrimEnd('=').ToCharArray();
      for (var i = 0; i < characters.Length; i++)
        characters[i] = map[characters[i]];

      writer.Write(Url.AbsoluteUri);
      if (!Url.AbsoluteUri.EndsWith("/"))
        writer.Write('/');
      writer.Write(Format);
      writer.Write('/');
      writer.Write(characters);
      writer.Flush();
    }
  }
}
