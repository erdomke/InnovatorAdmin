using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public class DocumentationWriter
  {
    public DiagramFormat Format { get; set; }
    public DocumentOutput Output { get; set; }
    public HttpImageWriter ImageWriter { get; set; }

    public async Task WriteAsync(PackageMetadataProvider metadata, Stream stream)
    {
      var entityDiagram = EntityDiagram.FromTypes(metadata.ItemTypes, metadata.Title);
      ImageWriter = ImageWriter ?? new HttpImageWriter();
      ImageWriter.Format = Format.ToString().ToLowerInvariant();
      ImageWriter.Writer = ImageWriter.Writer ?? new PlantUmlWriter();
      
      if (Output == DocumentOutput.Markdown)
      {
        using (var writer = new StreamWriter(stream))
        {
          writer.WriteLine("# Data Model");
          writer.WriteLine();
          await WriteDiagramMarkdown(entityDiagram, writer);
          writer.WriteLine();

          var mdWriter = new MarkdownVisitor(writer);
          var itemTypes = OrderTypes(metadata.ItemTypes.Where(i => !i.IsUiOnly).OrderBy(i => i.Name)).ToList();
          foreach (var itemType in itemTypes)
          {
            mdWriter.Visit(Document.FromItemType(itemType, new DocumentOptions()
            {
              IncludeCrossReferenceLinks = false,
              IncludeCoreProperties = false
            }, metadata));
          }

          if (metadata.Lists.Any())
          {
            writer.WriteLine();
            writer.WriteLine("# Lists");
            foreach (var list in metadata.Lists.OrderBy(l => l.Name))
            {
              writer.WriteLine();
              writer.WriteLine($"## {MarkdownVisitor.Escape(list.Name)} ({MarkdownVisitor.Escape(list.Label)})");
              writer.WriteLine();
              foreach (var value in list.Values.OrderBy(l => l.Filter ?? "").ThenBy(l => l.Value))
              {
                writer.Write("- **");
                MarkdownVisitor.Escape(value.Value, writer);
                writer.Write("** (");
                MarkdownVisitor.Escape(value.Label, writer);
                writer.Write(")");
                if (!string.IsNullOrEmpty(value.Filter))
                {
                  writer.Write(": Filter = ");
                  MarkdownVisitor.Escape(value.Filter, writer);
                }
                writer.WriteLine();
              }
            }
          }

          writer.WriteLine();
          foreach (var group in metadata.Diagrams.GroupBy(d => d.Type))
          {
            writer.WriteLine("# " + group.Key);
            writer.WriteLine();
            foreach (var diagram in group)
            {
              var name = diagram.Name;
              if (!string.IsNullOrEmpty(diagram.Label) && !string.Equals(diagram.Label, diagram.Name, StringComparison.OrdinalIgnoreCase))
                name += $" ({diagram.Label})";
              writer.WriteLine("## " + name);
              writer.WriteLine();
              await WriteDiagramMarkdown(diagram, writer);
              writer.WriteLine();
            }
          }
        }
      }
      else
      {
        if (Format == DiagramFormat.PlantUml)
        {
          await entityDiagram.WriteAsync(ImageWriter.Writer, new StreamWriter(stream));
        }
        else
        {
          ImageWriter.Format = Format.ToString().ToLowerInvariant();
          await entityDiagram.WriteAsync(ImageWriter, stream);
        }
      }
    }

    private IEnumerable<ItemType> OrderTypes(IEnumerable<ItemType> itemTypes)
    {
      if (!itemTypes.Any())
        return Enumerable.Empty<ItemType>();

      var initial = itemTypes.Where(i => !i.IsUiOnly).OrderBy(i => i.Name).ToList();
      var result = new List<ItemType>();
      var stack = new Stack<ItemType>();
      stack.Push(initial.FirstOrDefault(i => !i.IsRelationship)
        ?? initial.First());
      while (stack.Count > 0)
      {
        var curr = stack.Peek();
        result.Add(curr);
        initial.Remove(curr);

        var next = initial.FirstOrDefault(i => curr.Relationships.Contains(i));
        while (stack.Count > 0 && next == null)
        {
          stack.Pop();
          if (stack.Count == 0)
            next = initial.FirstOrDefault(i => !i.IsRelationship);
          else
            next = initial.FirstOrDefault(i => stack.Peek().Relationships.Contains(i));
        }
        if (next != null)
          stack.Push(next);
      }
      return result;
    }

    private async Task WriteDiagramMarkdown(IDiagram diagram, TextWriter writer)
    {
      if (Format == DiagramFormat.PlantUml)
      {
        writer.WriteLine("```plantuml");
        await diagram.WriteAsync(ImageWriter.Writer, writer);
        writer.WriteLine();
        writer.WriteLine("```");
      }
      else
      {
        writer.Write("![");
        writer.Write(diagram.GetType().Name);
        writer.Write("](");
        writer.Write(await ImageWriter.GetUrl(diagram));
        writer.Write(")");
      }
    }
  }
}
