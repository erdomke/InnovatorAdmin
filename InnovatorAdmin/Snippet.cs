using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  public class Snippet
  {
    private static char[] _newLineChars = new char[] { '\r', '\n' };

    public string Action { get; set; }
    public string Text { get; set; }

    public Snippet() { }
    public Snippet(string fileContent)
    {
      var newLine = fileContent.IndexOfAny(_newLineChars);
      if (newLine < 0) return;
      this.Action = fileContent.Substring(0, newLine).TrimEnd(_newLineChars);
      this.Text = fileContent.Substring(newLine).TrimStart(_newLineChars);
    }

    public bool IsEmpty()
    {
      return string.IsNullOrWhiteSpace(this.Action) && string.IsNullOrWhiteSpace(this.Text);
    }

    public override string ToString()
    {
      return this.Action + Environment.NewLine + this.Text;
    }
  }
}
