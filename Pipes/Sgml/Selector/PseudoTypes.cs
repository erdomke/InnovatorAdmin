using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public enum PseudoTypes
  {
    FunctionNthchild,
    FunctionNthlastchild,
    FunctionNthOfType,
    FunctionNthLastOfType,
    Root,
    FirstOfType,
    Lastoftype,
    Onlychild,
    OnlyOfType,
    Firstchild,
    Lastchild,
    Empty,
    Link,
    Visited,
    Active,
    Hover,
    Focus,
    Target,
    Enabled,
    Disabled,
    Checked,
    Unchecked,
    Indeterminate,
    Default,
    Valid,
    Invalid,
    Required,
    Inrange,
    Outofrange,
    Optional,
    Readonly,
    Readwrite,
    FunctionDir,
    FunctionNot,
    FunctionLang,
    FunctionContains
  }
}
