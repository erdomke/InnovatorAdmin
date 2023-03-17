using System.Xml.Linq;

namespace InnovatorAdmin.Merge
{
  internal class DiffScript
  {
    public string Path { get; }
    public XElement Script { get; }

    public DiffScript(string path, XElement script)
    {
      Path = path;
      Script = script;
    }
  }
}
