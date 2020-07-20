using Innovator.Client;
using InnovatorAdmin.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Plugins
{
  class AmlToRest : IPluginMethod
  {
    public Task<IPluginResult> Execute(IPluginContext arg)
    {
      return Task.FromResult<IPluginResult>(new PluginResult(writer =>
      {
        writer.WriteLine("<!--");
        writer.WriteLine("// POST /odata/method." + arg.Item.Attribute("action").Value);
        using (var json = new JsonTextWriter(writer, false)
        {
          IndentChars = "  "
        })
        {
          RenderJson(json, arg.Item, true);
        }
        writer.WriteLine("-->");
      }, 1));
    }

    private void RenderJson(JsonTextWriter json, IReadOnlyElement elem, bool root)
    {
      if (!elem.Attributes().Any() && !elem.Elements().Any())
      {
        json.WriteValue(elem.Value);
      }
      else
      {
        json.WriteStartObject();
        if (root && elem.Attribute("type").Exists)
          json.WriteProperty("@odata.type", "http://host/odata/$metadata#" + elem.Attribute("type").Value);
        foreach (var attr in elem.Attributes())
        {
          if (!(root && (attr.Name == "type" || attr.Name == "action")))
            json.WriteProperty("@" + attr.Name, attr.Value);
        }

        if (elem.Elements().Any())
        {
          foreach (var group in elem.Elements().GroupBy(e => e.Name))
          {
            json.WritePropertyName(group.Key);
            var needsArray = group.Skip(1).Any();
            if (needsArray)
              json.WriteStartArray();
            foreach (var child in group)
              RenderJson(json, child, false);
            if (needsArray)
              json.WriteEndArray();
          }
        }
        else if (!string.IsNullOrEmpty(elem.Value))
        {
          json.WriteProperty("#text", elem.Value);
        }


        json.WriteEndObject();
      }
    }
  }
}
