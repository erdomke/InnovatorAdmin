using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public sealed class ParamControlAttribute : Attribute
  {
    public Type ControlType { get; set; }
    public string Options { get; set; }

    public ParamControlAttribute(Type type, string options = null)
    {
      this.ControlType = type;
      this.Options = options;
    }
  }
  public interface IOptionsControl
  {
    void SetOptions(string options);
  }
}
