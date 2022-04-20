using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin.Tests
{
  [TestClass]
  public class DependencyTests
  {
    [TestMethod]
    public void DetectStandardDependencies()
    {
      var installItem = CreateInstallItem(@"<Item type=""ItemType"" id=""4AE047D8E67A4FD59DB03CD74D64C430"" _keyed_name=""Ans_SimulationRequest"" action=""edit"" _dependencies_analyzed=""1"">
  <class_structure><![CDATA[<class id=""4AE047D8E67A4FD59DB03CD74D64C430"">
  <class id=""A44BB5F2D06448B5817915AF75CA9E5D"" name=""Antenna"" />
  <class id=""DF39B587008843EC8322E69918B6EDC6"" name=""Board"" />
</class>]]></class_structure>
  <default_page_size>250</default_page_size>
  <history_template type=""History Template"">3BC16EF9E52B4F9792AB76BCE0492F29</history_template>
  <label_plural xml:lang=""en"">WorkRequests</label_plural>
</Item>");
      var dependencies = GetDependencies(installItem).ToList();
      Assert.IsTrue(dependencies.Any(d => d.Type == "History Template" && d.Unique == "3BC16EF9E52B4F9792AB76BCE0492F29"));
    }

    [TestMethod]
    public void DetectFeedTemplateDependencies()
    {
      var installItem = CreateInstallItem(@"<Item type='FeedTemplate' typeId='4697179F1FC94E25A8274E586EEF2F39' id='23604D58C4CD4EB6958B08420FC4B8C2'>
  <id keyed_name='23604D58C4CD4EB6958B08420FC4B8C2' type='FeedTemplate'>23604D58C4CD4EB6958B08420FC4B8C2</id>
  <reference>&lt;Item type='ECN' id='${id}' action='VC_GetAffectedControlItems'/&gt;</reference>
</Item>");
      var dependencies = GetDependencies(installItem).ToList();
      Assert.IsTrue(dependencies.Any(d => d.Type == "ItemType" && d.KeyedName == "ECN"));

      installItem = CreateInstallItem(@"<Item type='FeedTemplate' typeId='4697179F1FC94E25A8274E586EEF2F39' id='7D4B86FC634E47F98B63F63F2BA06CEE'>
  <id keyed_name='7D4B86FC634E47F98B63F63F2BA06CEE' type='FeedTemplate'>7D4B86FC634E47F98B63F63F2BA06CEE</id>
  <reference>Part CAD(related_id)</reference>
</Item>");
      dependencies = GetDependencies(installItem).ToList();
      Assert.IsTrue(dependencies.Any(d => d.Type == "ItemType" && d.KeyedName == "Part CAD"));

      installItem = CreateInstallItem(@"<Item type='FeedTemplate' typeId='4697179F1FC94E25A8274E586EEF2F39' id='7D4B86FC634E47F98B63F63F2BA06CEE'>
  <id keyed_name='7D4B86FC634E47F98B63F63F2BA06CEE' type='FeedTemplate'>7D4B86FC634E47F98B63F63F2BA06CEE</id>
  <reference>Part CAD/Another Rel(related_id)</reference>
</Item>");
      dependencies = GetDependencies(installItem).ToList();
      Assert.IsTrue(dependencies.Any(d => d.Type == "ItemType" && d.KeyedName == "Part CAD"));
      Assert.IsTrue(dependencies.Any(d => d.Type == "ItemType" && d.KeyedName == "Another Rel"));

      // Just looking for no errors here
      installItem = CreateInstallItem(@"<Item type='FeedTemplate' typeId='4697179F1FC94E25A8274E586EEF2F39' id='7D4B86FC634E47F98B63F63F2BA06CEE'>
  <id keyed_name='7D4B86FC634E47F98B63F63F2BA06CEE' type='FeedTemplate'>7D4B86FC634E47F98B63F63F2BA06CEE</id>
  <reference>this</reference>
</Item>");
      dependencies = GetDependencies(installItem).ToList();

      installItem = CreateInstallItem(@"<Item type='FeedTemplate' typeId='4697179F1FC94E25A8274E586EEF2F39' id='7D4B86FC634E47F98B63F63F2BA06CEE'>
  <id keyed_name='7D4B86FC634E47F98B63F63F2BA06CEE' type='FeedTemplate'>7D4B86FC634E47F98B63F63F2BA06CEE</id>
  <reference>affected_item</reference>
</Item>");
      dependencies = GetDependencies(installItem).ToList();
    }

    private IEnumerable<ItemReference> GetDependencies(InstallItem installItem)
    {
      var analyzer = new DependencyAnalyzer(new MockMetadata());
      analyzer.AddReferenceAndDependencies(installItem);
      analyzer.FinishAdding();
      return analyzer.GetDependencies(installItem.Reference);
    }

    private InstallItem CreateInstallItem(string aml)
    {
      var doc = new XmlDocument();
      doc.LoadXml(aml);
      return InstallItem.FromScript(doc.DocumentElement);
    }
  }
}
