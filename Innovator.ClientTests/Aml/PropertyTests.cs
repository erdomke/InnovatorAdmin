using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.Tests
{
  [TestClass()]
  public class PropertyTests
  {
    [TestMethod()]
    public void IsCurrentPropertyAccess()
    {
      var aml = ElementFactory.Local;
      var item = aml.FromXml(@"<Item type='Part' typeId='4F1AC04A2B484F3ABA4E20DB63808A88' id='A8F945913D58433A98D4E8DE57D4B008'>
  <classification>Electronic/Transistor/Other</classification>
  <current_state name='Implemented' keyed_name='Implemented' type='Life Cycle State'>1FA687F2F75F4BCCAF978684B3E6B482</current_state>
  <effective_date>2010-08-16T15:41:31</effective_date>
  <generation>2</generation>
  <id keyed_name='asdf' type='Part'>A8F945913D58433A98D4E8DE57D4B008</id>
  <is_active_rev>1</is_active_rev>
  <is_current>1</is_current>
  <is_released>1</is_released>
  <keyed_name>asdf</keyed_name>
  <major_rev>DAB</major_rev>
  <permission_id keyed_name='Released Part' type='Permission'>95475AE006E7415794BDC93808DC04D2</permission_id>
  <plm_only>0</plm_only>
  <state>Implemented</state>
  <state_image>../images/customer/images/Part_Implemented16.png</state_image>
  <team_id keyed_name='Part: General' type='Team'>EC30D51403344F689242E451481286D8</team_id>
  <unit>EA</unit>
  <itemtype>4F1AC04A2B484F3ABA4E20DB63808A88</itemtype>
</Item>", "query", null).AssertItem();

      Assert.AreEqual(true, item.IsCurrent().AsBoolean(false));
    }

    [TestMethod()]
    public void IsNullPropertyAsString()
    {
      var aml = ElementFactory.Local;
      var item = aml.FromXml(@"<Item type='Life Cycle State' typeId='5EFB53D35BAE468B851CD388BEA46B30' id='E7A383494C724B338518A4DD1EB867FA'>
  <id keyed_name='Superseded' type='Life Cycle State'>E7A383494C724B338518A4DD1EB867FA</id>
  <keyed_name>Superseded</keyed_name>
  <label is_null='1' />
  <name>Superseded</name>
</Item>").AssertItem();
      Assert.AreEqual("Superseded", item.Property("name").AsString("stuff"));
      Assert.AreEqual("Superseded", item.Property("label").AsString(item.Property("name").Value));
    }
  }
}
