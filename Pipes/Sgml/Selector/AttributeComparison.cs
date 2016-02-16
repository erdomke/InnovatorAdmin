using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public enum AttributeComparison
  {
    Exists,
    Equals,
    NotEquals,
    WhitespaceListContains,
    StartsWith,
    EndsWith,
    Contains,
    HyphenatedListStartsWith
  }
}
