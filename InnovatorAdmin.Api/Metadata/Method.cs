using Innovator.Client;
using InnovatorAdmin.Documentation;

namespace InnovatorAdmin
{
  public class Method : ItemReference
  {
    public bool IsCore { get; }
    public OperationElement Documentation { get; }
    public string ExecutionAllowedTo { get; }
    public bool IsServerMethod { get; }
    public bool IsServerEvent { get; set; }

    public Method(IReadOnlyItem elem, bool isCore = false)
    {
      FillItemRef(this, elem, false);
      this.KeyedName = elem.Property("name").AsString("");
      this.IsCore = elem.Property("core").AsBoolean(isCore);
      this.Documentation = OperationElement.Parse(this.KeyedName, elem.Property("method_code").AsString(""));
      if (this.Documentation.Summary.Count < 1 && elem.Property("comments").HasValue())
        this.Documentation.Summary.Add(new TextRun(elem.Property("comments").Value));
      var execution = elem.Property("execution_allowed_to");
      ExecutionAllowedTo = execution.KeyedName().Value
        ?? execution.AsItem().KeyedName().Value
        ?? execution.AsItem().Property("name").Value;
      IsServerMethod = elem.Property("method_type").Value != "JavaScript";
    }
  }
}
