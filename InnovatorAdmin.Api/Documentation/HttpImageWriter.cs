using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public class HttpImageWriter : IDiagramWriter<Stream>
  {
    public string Format { get; set; } = "svg";
    public Uri Url { get; set; } = new Uri("http://www.plantuml.com/plantuml/");
    public PlantUmlWriter Writer { get; set; }
    public HttpClient HttpClient { get; set; }

    public Task WriteAsync(EntityDiagram diagram, Stream stream)
    {
      return WriteInternal(diagram, stream);
    }

    public Task WriteAsync(StateDiagram diagram, Stream stream)
    {
      return WriteInternal(diagram, stream);
    }

    public async Task<string> GetUrl(IDiagram diagram)
    {
      var memStream = new MemoryStream();
      using (var deflate = new DeflateStream(memStream, CompressionLevel.Optimal))
      using (var compressWriter = new StreamWriter(deflate))
      {
        await diagram.WriteAsync(Writer ?? new PlantUmlWriter(), compressWriter);
      }

      const string base64Map = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
      const string plantUmlMap = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";
      var map = new char[128];
      for (var i = 0; i < base64Map.Length; i++)
        map[base64Map[i]] = plantUmlMap[i];

      var characters = Convert.ToBase64String(memStream.ToArray()).TrimEnd('=').ToCharArray();
      for (var i = 0; i < characters.Length; i++)
        characters[i] = map[characters[i]];

      var url = Url.AbsoluteUri;
      if (!url.EndsWith("/"))
        url += "/";
      url += Format + "/" + new string(characters);
      return url;
    }

    private async Task WriteInternal(IDiagram diagram, Stream stream)
    {
      var url = await GetUrl(diagram);
      HttpClient = HttpClient ?? new HttpClient();
      using (var readStream = await HttpClient.GetStreamAsync(url))
      {
        await readStream.CopyToAsync(stream);
      }
    }
  }
}
