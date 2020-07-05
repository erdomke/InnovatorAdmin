using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Tests
{
  [TestClass]
  public class AmlDocumentationTests
  {
    [TestMethod]
    public void ParseMethod()
    {
      var doc = AmlDocumentation.Parse("TestMethod", @"/// <summary>Create a new version of a <see cref=""itemtype.Part""/> item</summary>
/// <param name=""@snapshot""><datatype path=""@snapshot"" type=""boolean"" />If <c>1</c>, create a minor 
/// revision if the current version is not released. Otherwise, follow the standard 
/// logic of the <c>version</c> action</param>
/// <param name=""Relationships/Item[@type='Part'][@id]"">
///   <datatype path=""@id"" type=""id"" />
///   The Part items to version
/// </param>

using (new Escalate(arasPlmIdent))
{
    var items = ToEnum(this.getRelationships())
        .Where(i => string.Equals(i.getType(), ""Part"", StringComparison.OrdinalIgnoreCase));
    if (!items.Any())");
      Assert.AreEqual("TestMethod", doc.Name);
      Assert.AreEqual("Create a new version of a Part item", doc.Summary);
      var attr = doc.Attributes.Single();
      Assert.AreEqual("snapshot", attr.Name);
      Assert.AreEqual(AmlDataType.Boolean, attr.ValueTypes.Single().Type);
      Assert.AreEqual("If 1, create a minor revision if the current version is not released. Otherwise, follow the standard logic of the version action", attr.Summary);
      var relationships = doc.Elements.Single();
      Assert.AreEqual("Relationships", relationships.Name);
      var item = relationships.Elements.Single();
      Assert.AreEqual("Item", item.Name);
      Assert.AreEqual("The Part items to version", item.Summary);
      var typeAttr = item.Attributes.ElementAt(0);
      var idAttr = item.Attributes.ElementAt(1);
      Assert.AreEqual(AmlDataType.Enum, typeAttr.ValueTypes.Single().Type);
      CollectionAssert.AreEqual(new[] { "Part" }, typeAttr.ValueTypes.Single().Values.ToArray());
      Assert.AreEqual(AmlDataType.Item, idAttr.ValueTypes.Single().Type);
    }
  }
}
