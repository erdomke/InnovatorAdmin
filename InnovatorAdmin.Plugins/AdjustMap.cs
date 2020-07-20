using Innovator.Client;
using InnovatorAdmin.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin
{
  public class AdjustMap : IPluginMethod
  {
    public int XOffset { get; set; }
    public int YOffset { get; set; }

    public async Task<IPluginResult> Execute(IPluginContext arg)
    {
      var conn = (IAsyncConnection)arg.Conn;
      var elements = default(MapElement[]);

      XOffset = arg.Item.Attribute("xoffset").AsInt(0);
      YOffset = arg.Item.Attribute("yoffset").AsInt(0);

      switch (arg.Item.TypeName())
      {
        case "Life Cycle Map":
          elements = (await conn.ApplyAsync(@"<Item type='Life Cycle Map' action='get' id='@0' select='id'>
  <Relationships>
    <Item type='Life Cycle State' action='get' select='x,y' />
    <Item type='Life Cycle Transition' action='get' select='segments,x,y' />
  </Relationships>
</Item>", true, false, arg.Item.Id()).ConfigureAwait(false))
            .AssertItem()
            .Relationships()
            .Select(i => new MapElement(i))
            .ToArray();
          break;
        case "Workflow Process":
          elements = (await conn.ApplyAsync(@"<Item type='Workflow Process Activity' action='get' select='related_id'>
  <related_id>
    <Item type='Activity' action='get' select='x,y'>
      <Relationships>
        <Item type='Workflow Process Path' action='get' select='x,y,segments' />
      </Relationships>
    </Item>
  </related_id>
  <source_id>@0</source_id>
</Item>", true, false, arg.Item.Id()).ConfigureAwait(false))
            .Items()
            .SelectMany(i => i.RelatedItem().Relationships().Concat(new[] { i.RelatedItem() }))
            .Select(i => new MapElement(i))
            .ToArray();
          break;
        case "Workflow Map":
          elements = (await conn.ApplyAsync(@"<Item type='Workflow Map Activity' action='get' select='related_id'>
  <related_id>
    <Item type='Activity Template' action='get' select='x,y'>
      <Relationships>
        <Item type='Workflow Map Path' action='get' select='x,y,segments' />
      </Relationships>
    </Item>
  </related_id>
  <source_id>@0</source_id>
</Item>", true, false, arg.Item.Id()).ConfigureAwait(false))
            .Items()
            .SelectMany(i => i.RelatedItem().Relationships().Concat(new[] { i.RelatedItem() }))
            .Select(i => new MapElement(i))
            .ToArray();
          break;
        default:
          throw new NotSupportedException($"Items of type `{arg.Item.TypeName()}` are not supported.");
      }

      foreach (var element in elements)
      {
        if (!element.IsRelative)
        {
          element.Position.X += XOffset;
          element.Position.Y += YOffset;
        }
        foreach (var segment in element.Segments)
        {
          segment.X += XOffset;
          segment.Y += YOffset;
        }
      }

      var query = new Command(elements);
      var result = await conn.ApplyAsync(query, true, false).ConfigureAwait(false);
      return new PluginResult(result);
    }

    private class MapElement : IAmlNode
    {
      private IServerContext _context;

      public IEnumerable<Point> Segments { get; }
      public Point Position { get; }
      public string Type { get; }
      public string Id { get; }
      public bool IsRelative { get; }

      public MapElement(IReadOnlyItem item)
      {
        Type = item.TypeName();
        Id = item.Id();
        IsRelative = item.Property("segments").Exists;

        Position = new Point()
        {
          X = item.Property("x").AsInt(0),
          Y = item.Property("y").AsInt(0)
        };

        if (item.Property("segments").HasValue())
        {
          Segments = item.Property("segments").Value
            .Split('|')
            .Select(p => p.Split(','))
            .Select(p => new Point()
            {
              X = int.Parse(p[0]),
              Y = int.Parse(p[1])
            })
            .ToArray();
        }
        else
        {
          Segments = Enumerable.Empty<Point>();
        }

        _context = item.AmlContext.LocalizationContext;
      }

      public void ToAml(XmlWriter writer, AmlWriterSettings settings)
      {
        writer.WriteStartElement("Item");
        writer.WriteAttributeString("type", Type);
        writer.WriteAttributeString("id", Id);
        writer.WriteAttributeString("action", "edit");
        writer.WriteAttributeString("doGetItem", "0");
        writer.WriteElementString("x", _context.Format(Position.X));
        writer.WriteElementString("y", _context.Format(Position.Y));
        if (Segments.Any())
          writer.WriteElementString("segments", Segments.GroupConcat("|", p => $"{p.X},{p.Y}"));
        writer.WriteEndElement();
      }
    }

    private class Point
    {
      public int X { get; set; }
      public int Y { get; set; }
    }
  }
}
