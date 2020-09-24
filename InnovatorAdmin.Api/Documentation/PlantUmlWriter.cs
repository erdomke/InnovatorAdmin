using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InnovatorAdmin.Documentation
{
  public class PlantUmlWriter : IEntityWriter
  {
    public Dictionary<string, string> SkipParams { get; } = new Dictionary<string, string>()
    {
      { "monochrome", "true" },
      { "shadowing", "false" }
    };

    public void Write(EntityDiagram diagram, TextWriter writer)
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
          writer.Write("package ");
          writer.Write(package.Key);
          writer.WriteLine(" <<Rectangle>> {");
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
  }
}
