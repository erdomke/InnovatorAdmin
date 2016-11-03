using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Innovator.Client.Tests
{
  [TestClass()]
  public class UploadCommandTests
  {
    /*[TestMethod()]
    public void AddFileQuery_DontModifyIfComplete()
    {
      var query = @"<AML>
  <Item type='Document' action='merge' id='36FCA6E1D57F4C1C9DBFF44B89098A97'>
    <copyright is_null='1' />
    <is_active_rev>1</is_active_rev>
    <lab_controlled_document is_null='1' />
    <name>heart-1239269 v3</name>
    <spec_regulation is_null='1' />
    <classification>Part-Rev Document</classification>
    <created_by_id>2D246C5838644C1C8FD34F8D2796E327</created_by_id>
    <locked_by_id>2D246C5838644C1C8FD34F8D2796E327</locked_by_id>
    <team_id>2DEF50D558B44ECD9A603759D0B2D0DF</team_id>
    <id>36FCA6E1D57F4C1C9DBFF44B89098A97</id>
    <Relationships>
      <Item type='Document File' action='merge' id='082AF828369D4818930E2EE9C8C0E585'>
        <is_shared>1</is_shared>
        <related_id>
          <Item type='File' action='add' id='EAD5E1CD373D483FBE1084455E810ECB'>
            <actual_filename>C:\Users\eric.domke\AppData\Roaming\Gentex Corporation\Component Tracker\eric.domke\REFRESH\ead5e1cd373d483fbe1084455e810ecb.jpg</actual_filename>
            <checkedout_path>C:\Users\eric.domke\AppData\Roaming\Gentex Corporation\Component Tracker\eric.domke\REFRESH</checkedout_path>
            <checksum>EA78CB3CD0DB5E99F15EFF5C353B9A16</checksum>
            <filename>heart-1239269.jpg</filename>
            <file_size>55986</file_size>
            <created_by_id>2D246C5838644C1C8FD34F8D2796E327</created_by_id>
            <id>EAD5E1CD373D483FBE1084455E810ECB</id>
            <keyed_name>heart-1239269.jpg</keyed_name>
            <new_version>1</new_version>
            <Relationships>
              <Item type='Located' action='add' id='EF8FDEE863764F4694F3BEB9B7387125'>
                <file_version>1</file_version>
                <related_id>67BBB9204FE84A8981ED8313049BA06C</related_id>
                <source_id>EAD5E1CD373D483FBE1084455E810ECB</source_id>
              </Item>
            </Relationships>
          </Item>
        </related_id>
        <source_id>36FCA6E1D57F4C1C9DBFF44B89098A97</source_id>
        <created_by_id>2D246C5838644C1C8FD34F8D2796E327</created_by_id>
        <id>082AF828369D4818930E2EE9C8C0E585</id>
      </Item>
    </Relationships>
  </Item>
</AML>";
      var aml = ElementFactory.Local;
      var upload = new UploadCommand(Vault.GetVault(aml.Item(aml.Id("67BBB9204FE84A8981ED8313049BA06C"), aml.Property("vault_url", "asdf"))));
      upload.AddFileQuery(query);
      Assert.AreEqual(query, upload.Aml.Replace('"', '\''));
    }

    [TestMethod()]
    public void AddFileQuery_ModifyIfIncomplete()
    {
      var query = @"<AML>
  <Item type='Document' action='merge' id='36FCA6E1D57F4C1C9DBFF44B89098A97'>
    <copyright is_null='1' />
    <is_active_rev>1</is_active_rev>
    <lab_controlled_document is_null='1' />
    <name>heart-1239269 v3</name>
    <spec_regulation is_null='1' />
    <classification>Part-Rev Document</classification>
    <created_by_id>2D246C5838644C1C8FD34F8D2796E327</created_by_id>
    <locked_by_id>2D246C5838644C1C8FD34F8D2796E327</locked_by_id>
    <team_id>2DEF50D558B44ECD9A603759D0B2D0DF</team_id>
    <id>36FCA6E1D57F4C1C9DBFF44B89098A97</id>
    <Relationships>
      <Item type='Document File' action='merge' id='082AF828369D4818930E2EE9C8C0E585'>
        <is_shared>1</is_shared>
        <related_id>
          <Item type='File' action='add' id='EAD5E1CD373D483FBE1084455E810ECB'>
            <actual_filename>C:\Users\eric.domke\AppData\Roaming\Gentex Corporation\Component Tracker\eric.domke\REFRESH\ead5e1cd373d483fbe1084455e810ecb.jpg</actual_filename>
          </Item>
        </related_id>
        <source_id>36FCA6E1D57F4C1C9DBFF44B89098A97</source_id>
        <created_by_id>2D246C5838644C1C8FD34F8D2796E327</created_by_id>
        <id>082AF828369D4818930E2EE9C8C0E585</id>
      </Item>
    </Relationships>
  </Item>
</AML>";
      var aml = ElementFactory.Local;
      var upload = new UploadCommand(Vault.GetVault(aml.Item(aml.Id("67BBB9204FE84A8981ED8313049BA06C"), aml.Property("vault_url", "asdf"))));
      upload.AddFileQuery(query);
      var uploadAml = XElement.Parse(upload.Aml);
      var fileItem = uploadAml.Element("Item").Element("Relationships").Element("Item").Element("related_id").Element("Item");
      var located = fileItem.Element("Relationships").Element("Item");
      Assert.AreEqual(@"C:\Users\eric.domke\AppData\Roaming\Gentex Corporation\Component Tracker\eric.domke\REFRESH", fileItem.Element("checkedout_path").Value);
      Assert.AreEqual("ead5e1cd373d483fbe1084455e810ecb.jpg", fileItem.Element("filename").Value);
      Assert.AreEqual("1", located.Element("file_version").Value);
      Assert.AreEqual("67BBB9204FE84A8981ED8313049BA06C", located.Element("related_id").Value);
      Assert.AreEqual("EAD5E1CD373D483FBE1084455E810ECB", located.Element("source_id").Value);
    }*/
  }
}
