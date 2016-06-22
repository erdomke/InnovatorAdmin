using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  /// <summary>
  /// Represents a node of AML structure that can be rendered to AML
  /// </summary>
  public interface IAmlNode
  {
    /// <summary>
    /// Write the node to the specified <c>XmlWriter</c>
    /// </summary>
    /// <param name="writer"><c>XmlWriter</c> to write the node to</param>
    void ToAml(XmlWriter writer, AmlWriterSettings settings);
  }
}
