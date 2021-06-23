using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public class PlantUmlWriter : IDiagramWriter<TextWriter>
  {
    public Dictionary<string, string> SkipParams { get; } = new Dictionary<string, string>()
    {
      { "monochrome", "true" },
      { "shadowing", "false" }
    };
    
    public Task WriteAsync(EntityDiagram diagram, TextWriter writer)
    {
      writer.Write(@"@startuml ");
      writer.WriteLine(diagram.Entities.FirstOrDefault(e => !string.IsNullOrEmpty(e.Package))?.Package ?? "Entities");
      writer.WriteLine();

      foreach (var skinParam in SkipParams)
      {
        writer.Write("skinparam ");
        writer.Write(skinParam.Key);
        writer.Write(" ");
        writer.WriteLine(skinParam.Value);
      }
      writer.WriteLine();
      foreach (var package in diagram.Entities
        .GroupBy(e => e.Package ?? "")
        .OrderBy(p => string.IsNullOrEmpty(p.Key) ? 1 : 0))
      {
        if (!string.IsNullOrEmpty(package.Key))
        {
          writer.Write("package \"");
          writer.Write(package.Key);
          writer.WriteLine("\" <<Rectangle>> {");
        }
        foreach (var entity in package)
        {
          writer.Write(entity.Type.ToString().ToLowerInvariant());
          writer.Write(" \"");
          var parts = entity.Label.Split('\n');
          parts[0] = "**" + parts[0] + "**";
          writer.Write(string.Join("\\n", parts));
          writer.Write("\" as ");
          writer.Write(entity.Id);
          if (!string.IsNullOrEmpty(entity.Stereotype))
          {
            writer.Write(" << ");
            writer.Write(entity.Stereotype);
            writer.Write(" >>");
          }
          writer.WriteLine();
          writer.WriteLine("{");
          foreach (var attribute in entity.Attributes)
          {
            switch (attribute.Visiblity)
            {
              case AttributeVisibility.Package:
                writer.Write(" ~");
                break;
              case AttributeVisibility.Private:
                writer.Write(" -");
                break;
              case AttributeVisibility.Protected:
                writer.Write(" #");
                break;
              case AttributeVisibility.Public:
                writer.Write(" +");
                break;
              default:
                writer.Write("  ");
                break;
            }
            writer.Write(attribute.Name);
            if (!string.IsNullOrEmpty(attribute.DataType))
            {
              writer.Write(" : ");
              writer.Write(attribute.DataType);
            }
            writer.WriteLine();
          }
          writer.WriteLine("}");
          if (!string.IsNullOrEmpty(entity.Note))
          {
            writer.WriteLine("note right");
            writer.WriteLine(entity.Note);
            writer.WriteLine("end note");
          }
        }
        if (!string.IsNullOrEmpty(package.Key))
          writer.WriteLine("}");
      }

      foreach (var association in diagram.Associations)
      {
        writer.Write(association.Source.Entity.Id);
        writer.Write(AssociationEndLabel(association.Source));
        writer.Write(" ");
        writer.Write(AssociationSymbol(association.Source.Type, true));
        writer.Write(association.Dashed ? ".." : "--");
        writer.Write(AssociationSymbol(association.Related.Type, false));
        writer.Write(AssociationEndLabel(association.Related));
        writer.Write(" ");
        writer.Write(association.Related.Entity.Id);
        if (!string.IsNullOrEmpty(association.Label))
        {
          writer.Write(" : ");
          writer.Write(association.Label);
        }
        writer.WriteLine();
      }

      writer.WriteLine("@enduml");
      writer.Flush();
      return Task.FromResult(true);
    }

    private string AssociationEndLabel(AssociationEnd associationEnd)
    {
      if (!string.IsNullOrEmpty(associationEnd.Label))
      {
        return $" \"{associationEnd.Label}\"";
      }
      else if (associationEnd.Multiplicity.HasValue)
      {
        if (associationEnd.Multiplicity == int.MaxValue)
          return " \"many\"";
        return $" \"{associationEnd.Multiplicity}\"";
      }
      return "";
    }

    private string AssociationSymbol(AssociationType associationType, bool start)
    {
      switch (associationType)
      {
        case AssociationType.Aggregation:
          return "o";
        case AssociationType.Associated:
          return start ? "<" : ">";
        case AssociationType.Composition:
          return "*";
        case AssociationType.Inheritance:
          return start ? "<|" : "|>";
        case AssociationType.Many:
          return start ? "}" : "{";
        default: // AssociationType.Unknown
          return "";
      }
    }
    
    public Task WriteAsync(StateDiagram diagram, TextWriter writer)
    {
      const string terminalNode = "[*]";
      writer.WriteLine("@startuml");
      foreach (var skinParam in SkipParams)
      {
        writer.Write("skinparam ");
        writer.Write(skinParam.Key);
        writer.Write(" ");
        writer.WriteLine(skinParam.Value);
      }

      var nodesByName = new Dictionary<string, StateNode>(StringComparer.OrdinalIgnoreCase);
      var namesById = new Dictionary<string, string>();
      var hasOneStartNode = diagram.Nodes.Values.Count(DisplayAsStartNode) == 1;
      if (!hasOneStartNode)
        nodesByName[terminalNode] = null;
      foreach (var node in diagram.Nodes.Values)
      {
        if (hasOneStartNode && DisplayAsStartNode(node))
        {
          nodesByName[terminalNode] = node;
          namesById[node.Id] = terminalNode;
        }
        else
        {
          var baseName = node.Name.Replace(" ", "").Replace("-", "");
          var name = baseName;
          var index = 1;
          while (nodesByName.ContainsKey(name))
            name = baseName + (++index).ToString();
          nodesByName[name] = node;
          namesById[node.Id] = name;
          var label = node.Label ?? node.Name;
          if (label != name)
          {
            writer.Write("state \"");
            writer.Write(label);
            writer.Write("\" as ");
            writer.WriteLine(name);
          }
        }
      }

      var links = new List<StateLink>();
      if (nodesByName[terminalNode] == null)
      {
        links.AddRange(diagram.Nodes.Values
          .Where(n => n.Type == NodeType.Start)
          .OrderBy(n => Math.Pow(n.Location.X, 2) + Math.Pow(n.Location.Y, 2))
          .Select(n => new StateLink(null, n, "Start")));
      }
      links.AddRange(diagram.Links
        .OrderBy(l => l.Source.Type == NodeType.Start ? 0
          : (l.Related.Type == NodeType.End ? 2 : 1))
        .ThenBy(l => Math.Pow(l.Source.Location.X, 2) + Math.Pow(l.Source.Location.Y, 2))
        .ThenBy(l => Math.Pow(l.Related.Location.X, 2) + Math.Pow(l.Related.Location.Y, 2)));

      foreach (var link in links)
      {
        if (link.Source == null)
          writer.Write(terminalNode);
        else
          writer.Write(namesById[link.Source.Id]);

        writer.Write(" -");
        var direction = GetDirection(link);
        writer.Write(direction);
        var styles = new List<string>();
        if (link.IsDefault)
          styles.Add("bold");
        if (link.IsOverride)
          styles.Add("dashed");
        if (styles.Count > 0)
        {
          writer.Write('[');
          writer.Write(string.Join(",", styles));
          writer.Write(']');
        }
        writer.Write("-> ");

        if (link.Related == null)
          writer.Write(terminalNode);
        else
          writer.Write(namesById[link.Related.Id]);
        var label = link.Label ?? link.Name;
        if (!string.IsNullOrEmpty(label))
        {
          writer.Write(" : ");
          writer.Write(label);
        }
        writer.WriteLine();
      }

      writer.WriteLine("@enduml");
      writer.Flush();
      return Task.FromResult(true);
    }

    private static string GetDirection(StateLink link)
    {
      if (link.Source != null && link.Related != null)
      {
        var angle = Math.Atan2(link.Related.Location.Y - link.Source.Location.Y
          , link.Related.Location.X - link.Source.Location.X);
        if (angle >= Math.PI / 12 && angle <= Math.PI * 11 / 12)
          return "down";
        else if (angle <= -Math.PI / 12 && angle >= -Math.PI * 11 / 12)
          return "up";
        else if (Math.Abs(angle) > Math.PI * 11 / 12)
          return "left";
      }
      return "right";
    }

    private static bool DisplayAsStartNode(StateNode node)
    {
      return node.Type == NodeType.Start
        && node.IsAutomatic
        && string.Equals(node.Label ?? node.Name, "Start", System.StringComparison.OrdinalIgnoreCase);
    }
  }
}
