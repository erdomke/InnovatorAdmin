using InnovatorAdmin.Documentation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace InnovatorAdmin.Tests
{
  [TestClass]
  public class AmlDocumentationTests
  {
    [TestMethod]
    public void ParseMethod()
    {
      var doc = OperationElement.Parse("TestMethod", @"/// <summary>Create a new version of a <see cref=""itemtype.Part""/> item</summary>
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

    [TestMethod]
    public void ParseComplexRelationships()
    {
      var doc = OperationElement.Parse("TestMethod", @"/// <summary>Create a new version of a <see cref=""itemtype.Part""/> item</summary>
/// <param name=""Relationships/Item[@type='File' or @type='Folder'][@url][@id][filename][directoryname][content[@encoding='text' or @encoding='base64']][url]"">
///   The files/folders to version
/// </param>

using (new Escalate(arasPlmIdent))
{
    var items = ToEnum(this.getRelationships())
        .Where(i => string.Equals(i.getType(), ""Part"", StringComparison.OrdinalIgnoreCase));
    if (!items.Any())");
      Assert.AreEqual("TestMethod", doc.Name);
      Assert.AreEqual("Create a new version of a Part item", doc.Summary);
      var relationships = doc.Elements.Single();
      Assert.AreEqual("Relationships", relationships.Name);
      var item = relationships.Elements.Single();
      Assert.AreEqual("Item", item.Name);
      Assert.AreEqual("The files/folders to version", item.Summary);
      CollectionAssert.AreEqual(new[] { "type", "url", "id" }, item.Attributes.Select(a => a.Name).ToArray());
      var typeAttr = item.Attributes.ElementAt(0);
      Assert.AreEqual(AmlDataType.Enum, typeAttr.ValueTypes.Single().Type);
      CollectionAssert.AreEqual(new[] { "File", "Folder" }, typeAttr.ValueTypes.Single().Values.ToArray());
      CollectionAssert.AreEqual(new[] { "filename", "directoryname", "content", "url" }, item.Elements.Select(a => a.Name).ToArray());
      var encodingAttr = item.Elements.ElementAt(2).Attributes.First();
      Assert.AreEqual(AmlDataType.Enum, encodingAttr.ValueTypes.Single().Type);
      CollectionAssert.AreEqual(new[] { "text", "base64" }, encodingAttr.ValueTypes.Single().Values.ToArray());
    }
  }
}
