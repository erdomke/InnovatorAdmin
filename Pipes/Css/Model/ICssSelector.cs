using Pipes.Sgml.Selector;

namespace Pipes.Css.Model
{
  interface ISupportsSelector
  {
    ISelector Selector { get; set; }
  }
}