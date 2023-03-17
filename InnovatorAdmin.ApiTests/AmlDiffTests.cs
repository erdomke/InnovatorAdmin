using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin.Tests
{
  [TestClass()]
  public class AmlDiffTests
  {
    [TestMethod()]
    public void GetMergeScriptTest()
    {
      var expected = @"<AML>
  <Item type=""RelationshipType"" id=""C92F589A534241B09EBD4FE0ECD903E2"" _keyed_name=""Activity Assignment"" action=""edit"">
    <relationship_id type=""ItemType"">
      <Item type=""ItemType"" id=""85924010F3184E77B24E9142FDBB481B"" action=""edit"">
        <close_icon>../images/InBasketTask.svg</close_icon>
        <large_icon>../images/InBasketTask.svg</large_icon>
        <manual_versioning is_null=""1"" />
        <open_icon>../images/InBasketTask.svg</open_icon>
        <Relationships>
          <Item type=""Server Event"" id=""13D8F0A86A6F4254AD7BFFD02591F119"" action=""edit"">
            <event_version>version_1</event_version>
            <is_required>0</is_required>
          </Item>
          <Item type=""TOC Access"" id=""C429A399247545299B1F389629CAA14F"" action=""edit"">
            <related_id type=""Identity"">
              <Item type=""Identity"" action=""get"">
                <name>Super User</name>
              </Item>
            </related_id>
          </Item>
        </Relationships>
        <structure_view>tabs on</structure_view>
      </Item>
    </relationship_id>
  </Item>
</AML>";

      var diff = AmlDiff.GetMergeScript(XmlReader.Create(new StringReader(_startAml)),
        XmlReader.Create(new StringReader(_destAml)));
      var actual = DiffAnnotation.ToString(diff, version: DiffVersion.Target);
      Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void DiffSort()
    {
      var start = @"<AML>
  <Item type='Life Cycle Map' id='760CDFD386E044AA9E7DB840876955E7' _keyed_name='ans_SimulationTask' action='merge'>
    <description />
    <start_state type='Life Cycle State'>A00A60BB830C48DA8DFC9E2E0D6BF316</start_state>
    <name>ans_SimulationTask</name>
    <Relationships>
      <Item type='Life Cycle State' id='AC2C6470090D42B0B5462EE62721B830' action='merge'>
        <image>../Images/ansys/State_Running.svg</image>
        <item_behavior>float</item_behavior>
        <set_is_released>0</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>1024</sort_order>
        <state_permission_id type='Permission'>0C4ED7DFBC1A4017AB0AA505C13C3403</state_permission_id>
        <x>188</x>
        <y>104</y>
        <name>Active</name>
      </Item>
      <Item type='Life Cycle State' id='745F4077B7EE4907BA3DC44636A4BE47' action='merge'>
        <image>../Images/ansys/State_Canceled.svg</image>
        <item_behavior>fixed</item_behavior>
        <set_is_released>1</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>896</sort_order>
        <state_permission_id type='Permission'>B19E470EAE6746359BE13686A14ED8E8</state_permission_id>
        <x>223</x>
        <y>201</y>
        <name>Canceled</name>
      </Item>
      <Item type='Life Cycle State' id='69B9678C4B0C431F8982D3F4B6E28A56' action='merge'>
        <image>../Images/ansys/State_Complete.svg</image>
        <item_behavior>fixed</item_behavior>
        <set_is_released>1</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>640</sort_order>
        <state_permission_id type='Permission'>B19E470EAE6746359BE13686A14ED8E8</state_permission_id>
        <x>554</x>
        <y>94</y>
        <name>Completed</name>
      </Item>
      <Item type='Life Cycle State' id='5422C709336F44A183A3C86A9FEA8EB7' action='merge'>
        <image>../Images/ansys/State_Failed.svg</image>
        <item_behavior>fixed</item_behavior>
        <set_is_released>1</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>768</sort_order>
        <state_permission_id type='Permission'>B19E470EAE6746359BE13686A14ED8E8</state_permission_id>
        <x>317</x>
        <y>195</y>
        <name>Failed</name>
      </Item>
      <Item type='Life Cycle State' id='B383E217D917452FB84F3EC1789A4441' action='merge'>
        <image>../Images/ansys/State_InReview.svg</image>
        <set_is_released>0</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>1280</sort_order>
        <x>366</x>
        <y>98</y>
        <name>In Review</name>
      </Item>
      <Item type='Life Cycle State' id='A00A60BB830C48DA8DFC9E2E0D6BF316' action='merge'>
        <image>../Images/ansys/State_New.svg</image>
        <item_behavior>float</item_behavior>
        <set_is_released>0</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>128</sort_order>
        <state_permission_id type='Permission'>0C4ED7DFBC1A4017AB0AA505C13C3403</state_permission_id>
        <x>28</x>
        <y>104</y>
        <name>New</name>
      </Item>
      <Item type='Life Cycle State' id='7C561D1E30814D19ABA25B3763A4442C' action='merge'>
        <image>../Images/ansys/State_InWork.svg</image>
        <item_behavior>float</item_behavior>
        <set_is_released>0</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>1152</sort_order>
        <state_permission_id type='Permission'>0C4ED7DFBC1A4017AB0AA505C13C3403</state_permission_id>
        <x>109</x>
        <y>104</y>
        <name>Preparing</name>
      </Item>
      <Item type='Life Cycle Transition' id='B1E5C6D80D8544F69CC4C0D5A8CF09DF' action='merge'>
        <from_state type='Life Cycle State'>7C561D1E30814D19ABA25B3763A4442C</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments />
        <sort_order>2304</sort_order>
        <to_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</to_state>
        <x>15</x>
        <y>-12</y>
      </Item>
      <Item type='Life Cycle Transition' id='47C632D3AB8A4C4FAD74D4525AFCAD11' action='merge'>
        <from_state type='Life Cycle State'>A00A60BB830C48DA8DFC9E2E0D6BF316</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>
          <Item type='Identity' action='get' select='id'>
            <name>Owner</name>
          </Item>
        </role>
        <segments>73,133</segments>
        <sort_order>2944</sort_order>
        <to_state type='Life Cycle State'>7C561D1E30814D19ABA25B3763A4442C</to_state>
        <x>16</x>
        <y>36</y>
      </Item>
      <Item type='Life Cycle Transition' id='A1E53F3FEFC64A84992A843583BFFEA8' action='merge'>
        <from_state type='Life Cycle State'>A00A60BB830C48DA8DFC9E2E0D6BF316</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments />
        <sort_order>128</sort_order>
        <to_state type='Life Cycle State'>7C561D1E30814D19ABA25B3763A4442C</to_state>
        <x>10</x>
        <y>-4</y>
      </Item>
      <Item type='Life Cycle Transition' id='CEAB11008DDF48D3825BA5243DAB3629' action='merge'>
        <from_state type='Life Cycle State'>A00A60BB830C48DA8DFC9E2E0D6BF316</from_state>
        <get_comment>1</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments>37,210</segments>
        <sort_order>1152</sort_order>
        <to_state type='Life Cycle State'>745F4077B7EE4907BA3DC44636A4BE47</to_state>
        <x>40</x>
        <y>91</y>
      </Item>
      <Item type='Life Cycle Transition' id='EC90F6CEAE314B168DB786B7FFA74140' action='merge'>
        <from_state type='Life Cycle State'>A00A60BB830C48DA8DFC9E2E0D6BF316</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>
          <Item type='Identity' action='get' select='id'>
            <name>Creator</name>
          </Item>
        </role>
        <segments>74,83</segments>
        <sort_order>2816</sort_order>
        <to_state type='Life Cycle State'>7C561D1E30814D19ABA25B3763A4442C</to_state>
        <x>16</x>
        <y>-41</y>
      </Item>
      <Item type='Life Cycle Transition' id='3ACAD8A14D2B4E3EADBB4C8F5C4A374A' action='merge'>
        <from_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments>199,65|119,65</segments>
        <sort_order>2688</sort_order>
        <to_state type='Life Cycle State'>7C561D1E30814D19ABA25B3763A4442C</to_state>
        <x>-66</x>
        <y>-53</y>
      </Item>
      <Item type='Life Cycle Transition' id='46C54231D5B747C7A7E56C946B6BDF0F' action='merge'>
        <from_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments>351,64</segments>
        <sort_order>1920</sort_order>
        <to_state type='Life Cycle State'>69B9678C4B0C431F8982D3F4B6E28A56</to_state>
        <x>67</x>
        <y>-60</y>
      </Item>
      <Item type='Life Cycle Transition' id='6C16335130594CF5BBEE24CB16E84FAE' action='merge'>
        <from_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments />
        <sort_order>3072</sort_order>
        <to_state type='Life Cycle State'>B383E217D917452FB84F3EC1789A4441</to_state>
        <x>53</x>
        <y>-7</y>
      </Item>
      <Item type='Life Cycle Transition' id='7E67634450D5486CA9E58B4EA37EB05B' action='merge'>
        <from_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</from_state>
        <get_comment>1</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments />
        <sort_order>2176</sort_order>
        <to_state type='Life Cycle State'>745F4077B7EE4907BA3DC44636A4BE47</to_state>
        <x>-37</x>
        <y>54</y>
      </Item>
      <Item type='Life Cycle Transition' id='A5C29E94B3E44785914478600F9F1728' action='merge'>
        <from_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments />
        <sort_order>2048</sort_order>
        <to_state type='Life Cycle State'>5422C709336F44A183A3C86A9FEA8EB7</to_state>
        <x>18</x>
        <y>22</y>
      </Item>
      <Item type='Life Cycle Transition' id='291AA9456301464FAF89447D5ADDE409' action='merge'>
        <from_state type='Life Cycle State'>B383E217D917452FB84F3EC1789A4441</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments>284,131</segments>
        <sort_order>3328</sort_order>
        <to_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</to_state>
        <x>-124</x>
        <y>36</y>
      </Item>
      <Item type='Life Cycle Transition' id='CCDBB9C85EF94E689F579FBAE79864FC' action='merge'>
        <from_state type='Life Cycle State'>B383E217D917452FB84F3EC1789A4441</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments />
        <sort_order>3200</sort_order>
        <to_state type='Life Cycle State'>69B9678C4B0C431F8982D3F4B6E28A56</to_state>
        <x>0</x>
        <y>0</y>
      </Item>
    </Relationships>
  </Item>
</AML>";
      var dest = @"<AML>
  <Item type='Life Cycle Map' id='760CDFD386E044AA9E7DB840876955E7' _keyed_name='ans_SimulationTask' action='merge'>
    <description />
    <start_state type='Life Cycle State'>A00A60BB830C48DA8DFC9E2E0D6BF316</start_state>
    <name>ans_SimulationTask</name>
    <Relationships>
      <Item type='Life Cycle State' id='AC2C6470090D42B0B5462EE62721B830' action='merge'>
        <image>../Images/ansys/State_Running.svg</image>
        <item_behavior>float</item_behavior>
        <set_is_released>0</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>1024</sort_order>
        <state_permission_id type='Permission'>0C4ED7DFBC1A4017AB0AA505C13C3403</state_permission_id>
        <x>188</x>
        <y>104</y>
        <name>Active</name>
      </Item>
      <Item type='Life Cycle State' id='745F4077B7EE4907BA3DC44636A4BE47' action='merge'>
        <image>../Images/ansys/State_Canceled.svg</image>
        <item_behavior>fixed</item_behavior>
        <set_is_released>1</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>896</sort_order>
        <state_permission_id type='Permission'>B19E470EAE6746359BE13686A14ED8E8</state_permission_id>
        <x>223</x>
        <y>201</y>
        <name>Canceled</name>
      </Item>
      <Item type='Life Cycle State' id='69B9678C4B0C431F8982D3F4B6E28A56' action='merge'>
        <image>../Images/ansys/State_Complete.svg</image>
        <item_behavior>fixed</item_behavior>
        <set_is_released>1</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>640</sort_order>
        <state_permission_id type='Permission'>B19E470EAE6746359BE13686A14ED8E8</state_permission_id>
        <x>554</x>
        <y>94</y>
        <name>Completed</name>
      </Item>
      <Item type='Life Cycle State' id='B383E217D917452FB84F3EC1789A4441' action='merge'>
        <image>../Images/ansys/State_InReview.svg</image>
        <set_is_released>0</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>1280</sort_order>
        <x>366</x>
        <y>98</y>
        <name>In Review</name>
      </Item>
      <Item type='Life Cycle State' id='A00A60BB830C48DA8DFC9E2E0D6BF316' action='merge'>
        <image>../Images/ansys/State_New.svg</image>
        <item_behavior>float</item_behavior>
        <set_is_released>0</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>128</sort_order>
        <state_permission_id type='Permission'>0C4ED7DFBC1A4017AB0AA505C13C3403</state_permission_id>
        <x>28</x>
        <y>104</y>
        <name>New</name>
      </Item>
      <Item type='Life Cycle State' id='7C561D1E30814D19ABA25B3763A4442C' action='merge'>
        <image>../Images/ansys/State_InWork.svg</image>
        <item_behavior>float</item_behavior>
        <set_is_released>0</set_is_released>
        <set_not_lockable>0</set_not_lockable>
        <sort_order>1152</sort_order>
        <state_permission_id type='Permission'>0C4ED7DFBC1A4017AB0AA505C13C3403</state_permission_id>
        <x>109</x>
        <y>104</y>
        <name>Preparing</name>
      </Item>
      <Item type='Life Cycle Transition' id='B1E5C6D80D8544F69CC4C0D5A8CF09DF' action='merge'>
        <from_state type='Life Cycle State'>7C561D1E30814D19ABA25B3763A4442C</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments />
        <sort_order>2304</sort_order>
        <to_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</to_state>
        <x>15</x>
        <y>-12</y>
      </Item>
      <Item type='Life Cycle Transition' id='47C632D3AB8A4C4FAD74D4525AFCAD11' action='merge'>
        <from_state type='Life Cycle State'>A00A60BB830C48DA8DFC9E2E0D6BF316</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>
          <Item type='Identity' action='get' select='id'>
            <name>Owner</name>
          </Item>
        </role>
        <segments>73,133</segments>
        <sort_order>2944</sort_order>
        <to_state type='Life Cycle State'>7C561D1E30814D19ABA25B3763A4442C</to_state>
        <x>16</x>
        <y>36</y>
      </Item>
      <Item type='Life Cycle Transition' id='A1E53F3FEFC64A84992A843583BFFEA8' action='merge'>
        <from_state type='Life Cycle State'>A00A60BB830C48DA8DFC9E2E0D6BF316</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments />
        <sort_order>128</sort_order>
        <to_state type='Life Cycle State'>7C561D1E30814D19ABA25B3763A4442C</to_state>
        <x>10</x>
        <y>-4</y>
      </Item>
      <Item type='Life Cycle Transition' id='CEAB11008DDF48D3825BA5243DAB3629' action='merge'>
        <from_state type='Life Cycle State'>A00A60BB830C48DA8DFC9E2E0D6BF316</from_state>
        <get_comment>1</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments>37,210</segments>
        <sort_order>1152</sort_order>
        <to_state type='Life Cycle State'>745F4077B7EE4907BA3DC44636A4BE47</to_state>
        <x>40</x>
        <y>91</y>
      </Item>
      <Item type='Life Cycle Transition' id='EC90F6CEAE314B168DB786B7FFA74140' action='merge'>
        <from_state type='Life Cycle State'>A00A60BB830C48DA8DFC9E2E0D6BF316</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>
          <Item type='Identity' action='get' select='id'>
            <name>Creator</name>
          </Item>
        </role>
        <segments>74,83</segments>
        <sort_order>2816</sort_order>
        <to_state type='Life Cycle State'>7C561D1E30814D19ABA25B3763A4442C</to_state>
        <x>16</x>
        <y>-41</y>
      </Item>
      <Item type='Life Cycle Transition' id='3ACAD8A14D2B4E3EADBB4C8F5C4A374A' action='merge'>
        <from_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments>199,65|119,65</segments>
        <sort_order>2688</sort_order>
        <to_state type='Life Cycle State'>7C561D1E30814D19ABA25B3763A4442C</to_state>
        <x>-66</x>
        <y>-53</y>
      </Item>
      <Item type='Life Cycle Transition' id='46C54231D5B747C7A7E56C946B6BDF0F' action='merge'>
        <from_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments>351,64</segments>
        <sort_order>1920</sort_order>
        <to_state type='Life Cycle State'>69B9678C4B0C431F8982D3F4B6E28A56</to_state>
        <x>67</x>
        <y>-60</y>
      </Item>
      <Item type='Life Cycle Transition' id='6C16335130594CF5BBEE24CB16E84FAE' action='merge'>
        <from_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments />
        <sort_order>3072</sort_order>
        <to_state type='Life Cycle State'>B383E217D917452FB84F3EC1789A4441</to_state>
        <x>53</x>
        <y>-7</y>
      </Item>
      <Item type='Life Cycle Transition' id='7E67634450D5486CA9E58B4EA37EB05B' action='merge'>
        <from_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</from_state>
        <get_comment>1</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments />
        <sort_order>2176</sort_order>
        <to_state type='Life Cycle State'>745F4077B7EE4907BA3DC44636A4BE47</to_state>
        <x>-37</x>
        <y>54</y>
      </Item>
      <Item type='Life Cycle Transition' id='291AA9456301464FAF89447D5ADDE409' action='merge'>
        <from_state type='Life Cycle State'>B383E217D917452FB84F3EC1789A4441</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments>284,131</segments>
        <sort_order>3328</sort_order>
        <to_state type='Life Cycle State'>AC2C6470090D42B0B5462EE62721B830</to_state>
        <x>-124</x>
        <y>36</y>
      </Item>
      <Item type='Life Cycle Transition' id='CCDBB9C85EF94E689F579FBAE79864FC' action='merge'>
        <from_state type='Life Cycle State'>B383E217D917452FB84F3EC1789A4441</from_state>
        <get_comment>0</get_comment>
        <role type='Identity'>56A96DA9E981481688563E2D14D5D878</role>
        <segments />
        <sort_order>3200</sort_order>
        <to_state type='Life Cycle State'>69B9678C4B0C431F8982D3F4B6E28A56</to_state>
        <x>0</x>
        <y>0</y>
      </Item>
    </Relationships>
  </Item>
</AML>";
      var diff = AmlDiff.GetMergeScript(XmlReader.Create(new StringReader(start)),
        XmlReader.Create(new StringReader(dest)));
      var actual = DiffAnnotation.ToString(diff, version: DiffVersion.Target);
      var expected = @"<AML>
  <Item type=""Life Cycle Map"" id=""760CDFD386E044AA9E7DB840876955E7"" _keyed_name=""ans_SimulationTask"">
    <Relationships>
      <Item type=""Life Cycle Transition"" id=""A5C29E94B3E44785914478600F9F1728"" action=""delete"" />
      <Item type=""Life Cycle State"" id=""5422C709336F44A183A3C86A9FEA8EB7"" action=""delete"" />
    </Relationships>
  </Item>
</AML>";
      Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void ItemTypeScript_DontDeleteSystemProperties()
    {
      var start = @"<AML>
  <Item type=""ItemType"" id=""2CFE6D5B341947668654545F031810ED"" action=""edit"" _scriptType=""1"">
    <Relationships>
      <Item type=""Property"" id=""0A6A4C8C27594767BE581CEB7684A961"" action=""merge"">
        <column_alignment>left</column_alignment>
        <column_width>80</column_width>
        <data_source>11F1E302DEFC469BADFD48150875A7AB</data_source>
        <data_type>sequence</data_type>
        <help_text>Server-assigned number identifying the task</help_text>
        <is_hidden>0</is_hidden>
        <is_hidden2>0</is_hidden2>
        <is_indexed>0</is_indexed>
        <is_keyed>1</is_keyed>
        <is_multi_valued>0</is_multi_valued>
        <is_required>0</is_required>
        <keyed_name_order>10</keyed_name_order>
        <label xml:lang=""en"">Task ID</label>
        <order_by>10</order_by>
        <range_inclusive>0</range_inclusive>
        <readonly>0</readonly>
        <sort_order>10</sort_order>
        <track_history>0</track_history>
        <name>item_number</name>
      </Item>
      <Item type=""Property"" action=""edit"" where=""source_id='2CFE6D5B341947668654545F031810ED' and name='classification'"">
        <column_alignment>left</column_alignment>
        <data_type>string</data_type>
        <help_text />
        <is_hidden>1</is_hidden>
        <is_hidden2>1</is_hidden2>
        <is_indexed>0</is_indexed>
        <is_keyed>0</is_keyed>
        <is_multi_valued>0</is_multi_valued>
        <is_required>0</is_required>
        <range_inclusive>0</range_inclusive>
        <readonly>0</readonly>
        <sort_order>128</sort_order>
        <stored_length>512</stored_length>
        <track_history>0</track_history>
        <name>classification</name>
      </Item>
    </Relationships>
  </Item>
</AML>";
      var dest = @"<AML>
  <Item type=""ItemType"" id=""2CFE6D5B341947668654545F031810ED"" action=""edit"" _scriptType=""1"">
    <Relationships>
      <Item type=""Property"" id=""0A6A4C8C27594767BE581CEB7684A961"" action=""merge"">
        <column_alignment>left</column_alignment>
        <column_width>80</column_width>
        <data_source>1DD29EE33CD44AAC9B1E6CB75219BD6D</data_source>
        <data_type>sequence</data_type>
        <help_text>Server-assigned number identifying the task</help_text>
        <is_hidden>0</is_hidden>
        <is_hidden2>0</is_hidden2>
        <is_indexed>0</is_indexed>
        <is_keyed>1</is_keyed>
        <is_multi_valued>0</is_multi_valued>
        <is_required>0</is_required>
        <keyed_name_order>10</keyed_name_order>
        <label xml:lang=""en"">Task ID</label>
        <order_by>10</order_by>
        <range_inclusive>0</range_inclusive>
        <readonly>0</readonly>
        <sort_order>10</sort_order>
        <track_history>0</track_history>
        <name>item_number</name>
      </Item>
    </Relationships>
  </Item>
</AML>";
      var expected = @"<AML>
  <Item type=""ItemType"" id=""2CFE6D5B341947668654545F031810ED"" _scriptType=""1"" action=""edit"">
    <Relationships>
      <Item type=""Property"" id=""0A6A4C8C27594767BE581CEB7684A961"" action=""edit"">
        <data_source>1DD29EE33CD44AAC9B1E6CB75219BD6D</data_source>
      </Item>
    </Relationships>
  </Item>
</AML>";
      var diff = AmlDiff.GetMergeScript(XmlReader.Create(new StringReader(start)),
        XmlReader.Create(new StringReader(dest)));
      Assert.AreEqual(expected, diff.ToString());
    }

    [TestMethod]
    public void HandleFileRename()
    {
      var baseDir = new MockDiffDirectory();
      baseDir.Add("User/Innovator Admin.xml", @"<AML>
 <Item type=""User"" id=""30B991F927274FA3829655F50C99472E"" _keyed_name=""Innovator Admin"" action=""merge"">
  <login_name>admin</login_name>
  <logon_enabled>1</logon_enabled>
  <first_name>Innovator</first_name>
  <last_name>Admin</last_name>
 </Item>
</AML>");
      var compareDir = new MockDiffDirectory();
      compareDir.Add("User/Software Admin.xml", @"<AML>
 <Item type=""User"" id=""30B991F927274FA3829655F50C99472E"" _keyed_name=""Software Admin"" action=""merge"">
  <login_name>admin</login_name>
  <logon_enabled>1</logon_enabled>
  <first_name>Software</first_name>
  <last_name>Admin</last_name>
 </Item>
</AML>");
      var scripts = new Dictionary<string, XDocument>(StringComparer.OrdinalIgnoreCase);
      new MergeProcessor().WriteAmlMergeScripts(baseDir, compareDir, (path, progress, deletedScript) =>
      {
        var root = new XDocument();
        scripts[path] = root;
        return root.CreateWriter();
      });
      Assert.AreEqual(1, scripts.Count);
      Assert.AreEqual("User/Software Admin.xml", scripts.Keys.Single().ToString());
      Assert.AreEqual(@"<AML>
  <Item type=""User"" id=""30B991F927274FA3829655F50C99472E"" _keyed_name=""Innovator Admin"" action=""edit"">
    <first_name>Software</first_name>
  </Item>
</AML>", scripts.Values.Single().ToString());
    }

    [TestMethod]
    public void HandleRelationshipDelete()
    {
      var baseDir = new MockDiffDirectory();
      baseDir.Add("PresentationConfiguration/Activity Assignment Presentation Configuration.xml", @"<AML>
  <Item type=""PresentationConfiguration"" id=""E1EED4609F744DE8BBE33DD6C99E2393"" _keyed_name=""Activity Assignment Presentation Configuration"" action=""merge"">
    <color>#ffcc80</color>
    <name>Activity Assignment Presentation Configuration</name>
    <Relationships>
      <Item type=""PresentationCommandBarSection"" id=""6B44F2C4839DFBD7BDEE61C462534D75"" action=""merge"">
        <related_id type=""CommandBarSection"">6B44F2C4839DFBD7618120D9625E4BB8</related_id>
        <role type=""Identity"">
          <Item type=""Identity"" action=""get"" select=""id"">
            <name>World</name>
          </Item>
        </role>
        <sort_order>256</sort_order>
      </Item>
    </Relationships>
  </Item>
</AML>");
      var compareDir = new MockDiffDirectory();
      compareDir.Add("PresentationConfiguration/Activity Assignment Presentation Configuration.xml", @"<AML>
  <Item type=""PresentationConfiguration"" id=""E1EED4609F744DE8BBE33DD6C99E2393"" _keyed_name=""Activity Assignment Presentation Configuration"" action=""merge"">
    <color>#ffcc80</color>
    <name>Activity Assignment Presentation Configuration</name>
  </Item>
</AML>");

      var scripts = new Dictionary<string, XDocument>(StringComparer.OrdinalIgnoreCase);
      new MergeProcessor().WriteAmlMergeScripts(baseDir, compareDir, (path, progress, deletedScript) =>
      {
        var root = new XDocument();
        scripts[path] = root;
        return root.CreateWriter();
      });

      Assert.AreEqual(1, scripts.Count);
      Assert.AreEqual("PresentationConfiguration/Activity Assignment Presentation Configuration.xml", scripts.Keys.Single().ToString());
      Assert.AreEqual(@"<AML>
  <Item type=""PresentationConfiguration"" id=""E1EED4609F744DE8BBE33DD6C99E2393"" _keyed_name=""Activity Assignment Presentation Configuration"" action=""edit"">
    <Relationships>
      <Item type=""PresentationCommandBarSection"" id=""6B44F2C4839DFBD7BDEE61C462534D75"" action=""delete"" />
    </Relationships>
  </Item>
</AML>", scripts.Values.Single().ToString());
    }

    [TestMethod]
    public void HandleFileShuffle()
    {
      var baseDir = new MockDiffDirectory();
      baseDir.Add("App/Wb.xml", @"﻿<AML>
  <Item type=""Application"" id=""C1AEEBAA77C541A2911BE86B6BFAED82"" _keyed_name=""Wb"" action=""merge"">
    <allow_custom_import>0</allow_custom_import>
    <arguments />
    <executable_path>${Agent.Cust.SecScript}</executable_path>
    <export_file_permissions>ReadWrite</export_file_permissions>
    <icon>../Images/Cust/Wb.svg</icon>
    <import_file type=""FileTransferDefinition"">9AFAE40E77B04F539A35EA26E19F2284</import_file>
    <job_template type=""ItemType"" name=""JobTemplate_Wb"">6B70C74779D84E808BC69A5A074644B6</job_template>
    <launch_local>0</launch_local>
    <solver_tag>Wb</solver_tag>
    <tags>Cust</tags>
    <type>batch</type>
    <name>Wb</name>
    <Relationships>
      <Item type=""Application_File"" id=""6BCAFC95169B466AB4D11FBB866C5614"" action=""merge"">
        <content>Update file content</content>
        <path>update-wbpz.wbjn</path>
        <sort_order>128</sort_order>
      </Item>
      <Item type=""Application_File"" id=""D2D4A8F306DC44A18B655ED9B26FFC7C"" action=""merge"">
        <content>Run multiple</content>
        <path>ans-wb-run-multiple-dps.wbjn</path>
        <sort_order>256</sort_order>
      </Item>
      <Item type=""Application_File"" id=""1C71B0D2E835472492C2B4D5CE4EC973"" action=""merge"">
        <content>server_utils</content>
        <path>Wb_server_utils.py</path>
        <sort_order>384</sort_order>
      </Item>	  
      <Item type=""Application_FileType"" id=""9E5C6BB96EF24BE9B40BFA723161592A"" action=""merge"">
        <related_id type=""FileType"">51EDE60C62664961A9D25E1A52CC0165</related_id>
        <sort_order>128</sort_order>
      </Item>
    </Relationships>
  </Item>
</AML>");
      var compareDir = new MockDiffDirectory();
      compareDir.Add("App/Wb.xml", @"﻿<AML>
  <Item type=""ans_Application"" id=""10C4994DCB93471C97C96E7012151C68"" _keyed_name=""Wb"" action=""merge"">
    <allow_custom_import>1</allow_custom_import>
    <arguments>-F ""${Job.File.0}""</arguments>
    <executable_path>${Agent.Cust.InstallDirectory}/Framework/bin/${Agent.Platform}/runwb2</executable_path>
    <export_file_permissions>ReadWrite</export_file_permissions>
    <icon>../Images/Cust/Wb.svg</icon>
    <job_template type=""ItemType"" name=""ans_JobTemplate_Local"">1362424A17364B50A9052E144751E360</job_template>
    <launch_local>1</launch_local>
    <tags>Cust</tags>
    <type>local</type>
    <name>Wb</name>
    <Relationships>
      <Item type=""ans_Application_FileType"" id=""DE2E9CBD3F674F77A38F0DECA4AAF24B"" action=""merge"">
        <related_id type=""FileType"">51EDE60C62664961A9D25E1A52CC0165</related_id>
        <sort_order>128</sort_order>
      </Item>
    </Relationships>
  </Item>
</AML>");
      compareDir.Add("App/Wb_C1AEEBAA77C541A2911BE86B6BFAED82.xml", @"﻿<AML>
  <Item type=""Application"" id=""C1AEEBAA77C541A2911BE86B6BFAED82"" _keyed_name=""Wb"" action=""merge"">
    <allow_custom_import>0</allow_custom_import>
    <arguments />
    <executable_path>${Agent.Cust.SecScript}</executable_path>
    <export_file_permissions>ReadWrite</export_file_permissions>
    <icon>../Images/Cust/Wb.svg</icon>
    <import_file type=""FileTransferDefinition"">9AFAE40E77B04F539A35EA26E19F2284</import_file>
    <job_template type=""ItemType"" name=""JobTemplate_Wb"">6B70C74779D84E808BC69A5A074644B6</job_template>
    <launch_local>0</launch_local>
    <solver_tag>Wb</solver_tag>
    <tags>Cust</tags>
    <type>batch</type>
    <name>Wb</name>
    <Relationships>
      <Item type=""Application_File"" id=""1C71B0D2E835472492C2B4D5CE4EC973"" action=""merge"">
        <content>server_utils</content>
        <path>Wb_server_utils.py</path>
        <sort_order>384</sort_order>
      </Item>
      <Item type=""Application_File"" id=""6BCAFC95169B466AB4D11FBB866C5614"" action=""merge"">
        <content>Update file content</content>
        <path>update-wbpz.wbjn</path>
        <sort_order>128</sort_order>
      </Item>
      <Item type=""Application_File"" id=""D2D4A8F306DC44A18B655ED9B26FFC7C"" action=""merge"">
        <content>Run multiple with change</content>
        <path>ans-wb-run-multiple-dps.wbjn</path>
        <sort_order>256</sort_order>
      </Item>
      <Item type=""Application_FileType"" id=""9E5C6BB96EF24BE9B40BFA723161592A"" action=""merge"">
        <related_id type=""FileType"">51EDE60C62664961A9D25E1A52CC0165</related_id>
        <sort_order>128</sort_order>
      </Item>
    </Relationships>
  </Item>
</AML>");
      var scripts = new Dictionary<string, XDocument>(StringComparer.OrdinalIgnoreCase);
      new MergeProcessor().WriteAmlMergeScripts(baseDir, compareDir, (path, progress, deletedScript) =>
      {
        var root = new XDocument();
        scripts[path] = root;
        return root.CreateWriter();
      });
      Assert.AreEqual(@"<AML>
  <Item type=""ans_Application"" id=""10C4994DCB93471C97C96E7012151C68"" _keyed_name=""Wb"" action=""merge"">
    <allow_custom_import>1</allow_custom_import>
    <arguments>-F ""${Job.File.0}""</arguments>
    <executable_path>${Agent.Cust.InstallDirectory}/Framework/bin/${Agent.Platform}/runwb2</executable_path>
    <export_file_permissions>ReadWrite</export_file_permissions>
    <icon>../Images/Cust/Wb.svg</icon>
    <job_template type=""ItemType"" name=""ans_JobTemplate_Local"">1362424A17364B50A9052E144751E360</job_template>
    <launch_local>1</launch_local>
    <tags>Cust</tags>
    <type>local</type>
    <name>Wb</name>
    <Relationships>
      <Item type=""ans_Application_FileType"" id=""DE2E9CBD3F674F77A38F0DECA4AAF24B"" action=""merge"">
        <related_id type=""FileType"">51EDE60C62664961A9D25E1A52CC0165</related_id>
        <sort_order>128</sort_order>
      </Item>
    </Relationships>
  </Item>
</AML>", scripts["App/Wb.xml"].ToString());
      Assert.AreEqual(@"<AML>
  <Item type=""Application"" id=""C1AEEBAA77C541A2911BE86B6BFAED82"" _keyed_name=""Wb"" action=""edit"">
    <Relationships>
      <Item type=""Application_File"" id=""D2D4A8F306DC44A18B655ED9B26FFC7C"" action=""edit"">
        <content>Run multiple with change</content>
      </Item>
    </Relationships>
  </Item>
</AML>", scripts["App/Wb_C1AEEBAA77C541A2911BE86B6BFAED82.xml"].ToString());
    }


    [TestMethod]
    public void HandleFileShuffle_Reverse()
    {
      var baseDir = new MockDiffDirectory();
      baseDir.Add("App/Wb.xml", @"﻿<AML>
  <Item type=""ans_Application"" id=""10C4994DCB93471C97C96E7012151C68"" _keyed_name=""Wb"" action=""merge"">
    <allow_custom_import>1</allow_custom_import>
    <arguments>-F ""${Job.File.0}""</arguments>
    <executable_path>${Agent.Cust.InstallDirectory}/Framework/bin/${Agent.Platform}/runwb2</executable_path>
    <export_file_permissions>ReadWrite</export_file_permissions>
    <icon>../Images/Cust/Wb.svg</icon>
    <job_template type=""ItemType"" name=""ans_JobTemplate_Local"">1362424A17364B50A9052E144751E360</job_template>
    <launch_local>1</launch_local>
    <tags>Cust</tags>
    <type>local</type>
    <name>Wb</name>
    <Relationships>
      <Item type=""ans_Application_FileType"" id=""DE2E9CBD3F674F77A38F0DECA4AAF24B"" action=""merge"">
        <related_id type=""FileType"">51EDE60C62664961A9D25E1A52CC0165</related_id>
        <sort_order>128</sort_order>
      </Item>
    </Relationships>
  </Item>
</AML>");
      baseDir.Add("App/Wb_C1AEEBAA77C541A2911BE86B6BFAED82.xml", @"﻿<AML>
  <Item type=""Application"" id=""C1AEEBAA77C541A2911BE86B6BFAED82"" _keyed_name=""Wb"" action=""merge"">
    <allow_custom_import>0</allow_custom_import>
    <arguments />
    <executable_path>${Agent.Cust.SecScript}</executable_path>
    <export_file_permissions>ReadWrite</export_file_permissions>
    <icon>../Images/Cust/Wb.svg</icon>
    <import_file type=""FileTransferDefinition"">9AFAE40E77B04F539A35EA26E19F2284</import_file>
    <job_template type=""ItemType"" name=""JobTemplate_Wb"">6B70C74779D84E808BC69A5A074644B6</job_template>
    <launch_local>0</launch_local>
    <solver_tag>Wb</solver_tag>
    <tags>Cust</tags>
    <type>batch</type>
    <name>Wb</name>
    <Relationships>
      <Item type=""Application_File"" id=""1C71B0D2E835472492C2B4D5CE4EC973"" action=""merge"">
        <content>server_utils</content>
        <path>Wb_server_utils.py</path>
        <sort_order>384</sort_order>
      </Item>
      <Item type=""Application_File"" id=""6BCAFC95169B466AB4D11FBB866C5614"" action=""merge"">
        <content>Update file content</content>
        <path>update-wbpz.wbjn</path>
        <sort_order>128</sort_order>
      </Item>
      <Item type=""Application_File"" id=""D2D4A8F306DC44A18B655ED9B26FFC7C"" action=""merge"">
        <content>Run multiple with change</content>
        <path>ans-wb-run-multiple-dps.wbjn</path>
        <sort_order>256</sort_order>
      </Item>
      <Item type=""Application_FileType"" id=""9E5C6BB96EF24BE9B40BFA723161592A"" action=""merge"">
        <related_id type=""FileType"">51EDE60C62664961A9D25E1A52CC0165</related_id>
        <sort_order>128</sort_order>
      </Item>
    </Relationships>
  </Item>
</AML>");
      
      var compareDir = new MockDiffDirectory();
      compareDir.Add("App/Wb.xml", @"﻿<AML>
  <Item type=""Application"" id=""C1AEEBAA77C541A2911BE86B6BFAED82"" _keyed_name=""Wb"" action=""merge"">
    <allow_custom_import>0</allow_custom_import>
    <arguments />
    <executable_path>${Agent.Cust.SecScript}</executable_path>
    <export_file_permissions>ReadWrite</export_file_permissions>
    <icon>../Images/Cust/Wb.svg</icon>
    <import_file type=""FileTransferDefinition"">9AFAE40E77B04F539A35EA26E19F2284</import_file>
    <job_template type=""ItemType"" name=""JobTemplate_Wb"">6B70C74779D84E808BC69A5A074644B6</job_template>
    <launch_local>0</launch_local>
    <solver_tag>Wb</solver_tag>
    <tags>Cust</tags>
    <type>batch</type>
    <name>Wb</name>
    <Relationships>
      <Item type=""Application_File"" id=""6BCAFC95169B466AB4D11FBB866C5614"" action=""merge"">
        <content>Update file content</content>
        <path>update-wbpz.wbjn</path>
        <sort_order>128</sort_order>
      </Item>
      <Item type=""Application_File"" id=""D2D4A8F306DC44A18B655ED9B26FFC7C"" action=""merge"">
        <content>Run multiple</content>
        <path>ans-wb-run-multiple-dps.wbjn</path>
        <sort_order>256</sort_order>
      </Item>
      <Item type=""Application_File"" id=""1C71B0D2E835472492C2B4D5CE4EC973"" action=""merge"">
        <content>server_utils</content>
        <path>Wb_server_utils.py</path>
        <sort_order>384</sort_order>
      </Item>	  
      <Item type=""Application_FileType"" id=""9E5C6BB96EF24BE9B40BFA723161592A"" action=""merge"">
        <related_id type=""FileType"">51EDE60C62664961A9D25E1A52CC0165</related_id>
        <sort_order>128</sort_order>
      </Item>
    </Relationships>
  </Item>
</AML>");
      var scripts = new Dictionary<string, XDocument>(StringComparer.OrdinalIgnoreCase);
      new MergeProcessor().WriteAmlMergeScripts(baseDir, compareDir, (path, progress, deletedScript) =>
      {
        var root = new XDocument();
        scripts[path] = root;
        return root.CreateWriter();
      });
      Assert.AreEqual(@"<AML>
  <Item type=""ans_Application"" id=""10C4994DCB93471C97C96E7012151C68"" _keyed_name=""Wb"" action=""delete"" />
</AML>", scripts["App/Wb.xml"].ToString());
      Assert.AreEqual(@"<AML>
  <Item type=""Application"" id=""C1AEEBAA77C541A2911BE86B6BFAED82"" _keyed_name=""Wb"" action=""edit"">
    <Relationships>
      <Item type=""Application_File"" id=""D2D4A8F306DC44A18B655ED9B26FFC7C"" action=""edit"">
        <content>Run multiple</content>
      </Item>
    </Relationships>
  </Item>
</AML>", scripts["App/Wb_C1AEEBAA77C541A2911BE86B6BFAED82.xml"].ToString());
    }

    private const string _startAml = @"<AML>
  <Item type='RelationshipType' id='C92F589A534241B09EBD4FE0ECD903E2' _keyed_name='Activity Assignment' action='merge'>
    <auto_search>1</auto_search>
    <behavior>float</behavior>
    <copy_permissions>0</copy_permissions>
    <create_related>0</create_related>
    <hide_in_all>0</hide_in_all>
    <inc_rel_key_name>1</inc_rel_key_name>
    <inc_related_key_name>1</inc_related_key_name>
    <is_list_type>0</is_list_type>
    <label xml:lang='en'>Assignments</label>
    <new_show_related>0</new_show_related>
    <related_id type='ItemType' name='Identity'>E582AB17663F4EF28460015B2BE9E094</related_id>
    <related_notnull>1</related_notnull>
    <related_option>0</related_option>
    <relationship_id type='ItemType' name='Activity Assignment'>
      <Item type='ItemType' id='85924010F3184E77B24E9142FDBB481B' action='merge'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x16_inbox.gif</close_icon>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <hide_where_used>0</hide_where_used>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_ASSIGNMENT</instance_data>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_versionable>0</is_versionable>
        <label xml:lang='en'>Activity Assignment</label>
        <label_plural xml:lang='en'>Project InBox</label_plural>
        <large_icon>../images/Icons/32x32/32x32_inbox2.gif</large_icon>
        <manual_versioning>0</manual_versioning>
        <open_icon>../images/Icons/16x16/16x16_inbox.gif</open_icon>
        <revisions type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Assignment</name>
        <Relationships>
          <Item type='Allowed Permission' id='7E7B3021770841CA96B794627AA74325' action='merge'>
            <is_default>1</is_default>
            <related_id type='Permission'>93AB06D37FC84328A314324DC45DB574</related_id>
            <sort_order>128</sort_order>
          </Item>
          <Item type='Can Add' id='290B587B1DC24AC68A36DC0BF5A7D151' action='merge'>
            <can_add>1</can_add>
            <related_id type='Identity'>
              <Item type='Identity' action='get' select='id'>
                <name>World</name>
              </Item>
            </related_id>
            <sort_order>256</sort_order>
          </Item>
          <Item type='Can Add' id='4319E0B5076A41BA93233908685EAB20' action='merge'>
            <can_add>1</can_add>
            <related_id type='Identity'>2618D6F5A90949BAA7E920D1B04C7EE1</related_id>
            <sort_order>384</sort_order>
          </Item>
          <Item type='Can Add' id='803240C9547B438CAF1C65A10FB556F4' action='merge'>
            <can_add>1</can_add>
            <related_id type='Identity'>
              <Item type='Identity' action='get' select='id'>
                <name>Super User</name>
              </Item>
            </related_id>
            <sort_order>128</sort_order>
          </Item>
          <Item type='Property' id='206FFEB5A06940AB93B2047AD6E5C8B5' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <data_source type='ItemType' name='Identity'>E582AB17663F4EF28460015B2BE9E094</data_source>
            <data_type>item</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>1</is_hidden>
            <is_hidden2>1</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <item_behavior>float</item_behavior>
            <label xml:lang='en' />
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>4608</sort_order>
            <track_history>0</track_history>
            <name>claimed_by</name>
          </Item>
          <Item type='Property' id='63038217D1E84A6684FF81081725DF83' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <column_width>120</column_width>
            <data_source type='ItemType' name='User'>45E899CD2859442982EB22BB2DF683E5</data_source>
            <data_type>item</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <item_behavior>float</item_behavior>
            <label xml:lang='en'>Closed By</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>4</sort_order>
            <track_history>0</track_history>
            <name>closed_by</name>
          </Item>
          <Item type='Property' id='010BA12B423B40479F015019902B6EB6' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <column_width>100</column_width>
            <data_type>date</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>1</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Closed On</label>
            <pattern>short_date_time</pattern>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>3</sort_order>
            <track_history>0</track_history>
            <name>closed_on</name>
          </Item>
          <Item type='Property' id='9C38B5BCA5D04716B8F7D916397AFC8C' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <column_width>240</column_width>
            <data_type>text</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Comments</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>5</sort_order>
            <track_history>0</track_history>
            <name>comments</name>
          </Item>
          <Item type='Property' id='739C43F3CDC842F792B5042BD876A665' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <data_type>integer</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>1</is_hidden>
            <is_hidden2>1</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Elapsed (ms)</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>4992</sort_order>
            <track_history>0</track_history>
            <name>elapsed_ms</name>
          </Item>
          <Item type='Property' id='09F7C22F718445D4B2165AC750DB11C0' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <data_source type='ItemType' name='Identity'>E582AB17663F4EF28460015B2BE9E094</data_source>
            <data_type>item</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <item_behavior>float</item_behavior>
            <label xml:lang='en'>Escalate To</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>4480</sort_order>
            <track_history>0</track_history>
            <name>escalate_to</name>
          </Item>
          <Item type='Property' id='85C001EBD7064D419D9AF2ABF3E17555' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>center</column_alignment>
            <column_width>80</column_width>
            <data_type>boolean</data_type>
            <default_value xml:lang='en'>0</default_value>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>For All Members</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>1</sort_order>
            <track_history>0</track_history>
            <name>for_all_members</name>
          </Item>
          <Item type='Property' id='82A7CB6D2F6A4D76852526110FD7EF33' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>center</column_alignment>
            <data_type>boolean</data_type>
            <default_value xml:lang='en'>0</default_value>
            <is_class_required>0</is_class_required>
            <is_hidden>1</is_hidden>
            <is_hidden2>1</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Disabled</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>128</sort_order>
            <track_history>0</track_history>
            <name>is_disabled</name>
          </Item>
          <Item type='Property' id='6532264A05D74855B9908B6CC0BED86D' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>center</column_alignment>
            <column_width>80</column_width>
            <data_type>boolean</data_type>
            <default_value xml:lang='en'>0</default_value>
            <is_class_required>0</is_class_required>
            <is_hidden>1</is_hidden>
            <is_hidden2>1</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Overdue</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>4</sort_order>
            <track_history>0</track_history>
            <name>is_overdue</name>
          </Item>
          <Item type='Property' id='6380838A9C7245CEA4D7682DE9D4FB1B' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>center</column_alignment>
            <column_width>80</column_width>
            <data_type>boolean</data_type>
            <default_value xml:lang='en'>0</default_value>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Required</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>0</sort_order>
            <track_history>0</track_history>
            <name>is_required</name>
          </Item>
          <Item type='Property' id='BC501CB8D09448FA827B7ABCB2A0D0C7' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <column_width>120</column_width>
            <data_type>string</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>1</is_hidden>
            <is_hidden2>1</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Path</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>99</sort_order>
            <stored_length>32</stored_length>
            <track_history>0</track_history>
            <name>path</name>
          </Item>
          <Item type='Property' id='4E0F6E38EBB1416891612974026C53BD' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>right</column_alignment>
            <column_width>80</column_width>
            <data_type>integer</data_type>
            <default_value xml:lang='en'>0</default_value>
            <is_class_required>0</is_class_required>
            <is_hidden>1</is_hidden>
            <is_hidden2>1</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Reminders Sent</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>3</sort_order>
            <track_history>0</track_history>
            <name>reminders_sent</name>
          </Item>
          <Item type='Property' id='DE47E38B4CAC4E649BC4945244C853EC' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>right</column_alignment>
            <column_width>80</column_width>
            <data_type>integer</data_type>
            <default_value xml:lang='en'>100</default_value>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>1</is_required>
            <label xml:lang='en'>Voting Weight</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>2</sort_order>
            <track_history>0</track_history>
            <name>voting_weight</name>
          </Item>
          <Item type='Server Event' id='13D8F0A86A6F4254AD7BFFD02591F119' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='6E72FE69A62E4931BF9E0823127E8241' where=""[Method].[config_id] = '6E72FE69A62E4931BF9E0823127E8241'"" /></related_id>
            <server_event>onAfterAdd</server_event>
            <sort_order>128</sort_order>
          </Item>
          <Item type='Server Event' id='8B7C5397C1EC47F1B483C6B28F39180E' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='4FF70B9A34424B42992C9DD19083C2CD' where=""[Method].[config_id] = '4FF70B9A34424B42992C9DD19083C2CD'"" /></related_id>
            <server_event>onAfterAdd</server_event>
            <sort_order>150</sort_order>
          </Item>
          <Item type='Server Event' id='9BD8C1A5434842BD8C7FE0A32A59FC31' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='4BBFF1DD8E094D139B58DCF7C20FFF54' where=""[Method].[config_id] = '4BBFF1DD8E094D139B58DCF7C20FFF54'"" /></related_id>
            <server_event>onAfterAdd</server_event>
            <sort_order>200</sort_order>
          </Item>
          <Item type='Server Event' id='6089B9C041814535962A7A3BB5C487BC' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='4FF70B9A34424B42992C9DD19083C2CD' where=""[Method].[config_id] = '4FF70B9A34424B42992C9DD19083C2CD'"" /></related_id>
            <server_event>onAfterUpdate</server_event>
            <sort_order>384</sort_order>
          </Item>
          <Item type='Server Event' id='6847E681E2DC46AD86ABE91D11411C61' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='AC3EEF912133498588D7DF8C06B814A3' where=""[Method].[config_id] = 'AC3EEF912133498588D7DF8C06B814A3'"" /></related_id>
            <server_event>onBeforeAdd</server_event>
            <sort_order>100</sort_order>
          </Item>
          <Item type='Server Event' id='496769BB12A04D5D8E7294A345233167' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='AC3EEF912133498588D7DF8C06B814A3' where=""[Method].[config_id] = 'AC3EEF912133498588D7DF8C06B814A3'"" /></related_id>
            <server_event>onBeforeDelete</server_event>
            <sort_order>100</sort_order>
          </Item>
          <Item type='Server Event' id='A5A19E8A26F04B97A4AE64701518F3D5' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='277CDBDC93AB446484808613BC0D754B' where=""[Method].[config_id] = '277CDBDC93AB446484808613BC0D754B'"" /></related_id>
            <server_event>onBeforeDelete</server_event>
            <sort_order>512</sort_order>
          </Item>
          <Item type='Server Event' id='FE59F0D96F4243C19DE75C8504B4E9D7' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='4BBFF1DD8E094D139B58DCF7C20FFF54' where=""[Method].[config_id] = '4BBFF1DD8E094D139B58DCF7C20FFF54'"" /></related_id>
            <server_event>onBeforeUpdate</server_event>
            <sort_order>10000</sort_order>
          </Item>
          <Item type='TOC Access' id='C429A399247545299B1F389629CAA14F' action='merge'>
            <related_id type='Identity'>
              <Item type='Identity' action='get' select='id'>
                <name>World</name>
              </Item>
            </related_id>
            <sort_order>128</sort_order>
            <category>Portfolio</category>
          </Item>
          <Item type='TOC View' id='DF069C7123F6442CB4EAEE72FCAC8138' action='merge'>
            <related_id type='Identity'>
              <Item type='Identity' action='get' select='id'>
                <name>World</name>
              </Item>
            </related_id>
            <sort_order>128</sort_order>
            <start_page>InBasket/InBasket.html</start_page>
          </Item>
        </Relationships>
      </Item>
    </relationship_id>
    <sort_order>128</sort_order>
    <source_id type='ItemType' name='Activity'>937CE47DE2854308BE6FF5AB1CFB19D4</source_id>
    <name>Activity Assignment</name>
  </Item>
</AML>";
    private const string _destAml = @"<AML>
  <Item type='RelationshipType' id='C92F589A534241B09EBD4FE0ECD903E2' _keyed_name='Activity Assignment' action='merge'>
    <auto_search>1</auto_search>
    <behavior>float</behavior>
    <copy_permissions>0</copy_permissions>
    <create_related>0</create_related>
    <hide_in_all>0</hide_in_all>
    <inc_rel_key_name>1</inc_rel_key_name>
    <inc_related_key_name>1</inc_related_key_name>
    <is_list_type>0</is_list_type>
    <label xml:lang='en'>Assignments</label>
    <new_show_related>0</new_show_related>
    <related_id type='ItemType' name='Identity'>E582AB17663F4EF28460015B2BE9E094</related_id>
    <related_notnull>1</related_notnull>
    <related_option>0</related_option>
    <relationship_id type='ItemType' name='Activity Assignment'>
      <Item type='ItemType' id='85924010F3184E77B24E9142FDBB481B' action='merge'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/InBasketTask.svg</close_icon>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <hide_where_used>0</hide_where_used>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_ASSIGNMENT</instance_data>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_versionable>0</is_versionable>
        <label xml:lang='en'>Activity Assignment</label>
        <label_plural xml:lang='en'>Project InBox</label_plural>
        <large_icon>../images/InBasketTask.svg</large_icon>
        <open_icon>../images/InBasketTask.svg</open_icon>
        <revisions type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <structure_view>tabs on</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Assignment</name>
        <Relationships>
          <Item type='Allowed Permission' id='7E7B3021770841CA96B794627AA74325' action='merge'>
            <is_default>1</is_default>
            <related_id type='Permission'>93AB06D37FC84328A314324DC45DB574</related_id>
            <sort_order>128</sort_order>
          </Item>
          <Item type='Can Add' id='290B587B1DC24AC68A36DC0BF5A7D151' action='merge'>
            <can_add>1</can_add>
            <related_id type='Identity'>
              <Item type='Identity' action='get' select='id'>
                <name>World</name>
              </Item>
            </related_id>
            <sort_order>256</sort_order>
          </Item>
          <Item type='Can Add' id='4319E0B5076A41BA93233908685EAB20' action='merge'>
            <can_add>1</can_add>
            <related_id type='Identity'>2618D6F5A90949BAA7E920D1B04C7EE1</related_id>
            <sort_order>384</sort_order>
          </Item>
          <Item type='Can Add' id='803240C9547B438CAF1C65A10FB556F4' action='merge'>
            <can_add>1</can_add>
            <related_id type='Identity'>
              <Item type='Identity' action='get' select='id'>
                <name>Super User</name>
              </Item>
            </related_id>
            <sort_order>128</sort_order>
          </Item>
          <Item type='Property' id='206FFEB5A06940AB93B2047AD6E5C8B5' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <data_source type='ItemType' name='Identity'>E582AB17663F4EF28460015B2BE9E094</data_source>
            <data_type>item</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>1</is_hidden>
            <is_hidden2>1</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <item_behavior>float</item_behavior>
            <label xml:lang='en' />
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>4608</sort_order>
            <track_history>0</track_history>
            <name>claimed_by</name>
          </Item>
          <Item type='Property' id='63038217D1E84A6684FF81081725DF83' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <column_width>120</column_width>
            <data_source type='ItemType' name='User'>45E899CD2859442982EB22BB2DF683E5</data_source>
            <data_type>item</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <item_behavior>float</item_behavior>
            <label xml:lang='en'>Closed By</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>4</sort_order>
            <track_history>0</track_history>
            <name>closed_by</name>
          </Item>
          <Item type='Property' id='010BA12B423B40479F015019902B6EB6' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <column_width>100</column_width>
            <data_type>date</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>1</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Closed On</label>
            <pattern>short_date_time</pattern>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>3</sort_order>
            <track_history>0</track_history>
            <name>closed_on</name>
          </Item>
          <Item type='Property' id='9C38B5BCA5D04716B8F7D916397AFC8C' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <column_width>240</column_width>
            <data_type>text</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Comments</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>5</sort_order>
            <track_history>0</track_history>
            <name>comments</name>
          </Item>
          <Item type='Property' id='739C43F3CDC842F792B5042BD876A665' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <data_type>integer</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>1</is_hidden>
            <is_hidden2>1</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Elapsed (ms)</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>4992</sort_order>
            <track_history>0</track_history>
            <name>elapsed_ms</name>
          </Item>
          <Item type='Property' id='09F7C22F718445D4B2165AC750DB11C0' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <data_source type='ItemType' name='Identity'>E582AB17663F4EF28460015B2BE9E094</data_source>
            <data_type>item</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <item_behavior>float</item_behavior>
            <label xml:lang='en'>Escalate To</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>4480</sort_order>
            <track_history>0</track_history>
            <name>escalate_to</name>
          </Item>
          <Item type='Property' id='85C001EBD7064D419D9AF2ABF3E17555' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>center</column_alignment>
            <column_width>80</column_width>
            <data_type>boolean</data_type>
            <default_value xml:lang='en'>0</default_value>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>For All Members</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>1</sort_order>
            <track_history>0</track_history>
            <name>for_all_members</name>
          </Item>
          <Item type='Property' id='82A7CB6D2F6A4D76852526110FD7EF33' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>center</column_alignment>
            <data_type>boolean</data_type>
            <default_value xml:lang='en'>0</default_value>
            <is_class_required>0</is_class_required>
            <is_hidden>1</is_hidden>
            <is_hidden2>1</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Disabled</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>128</sort_order>
            <track_history>0</track_history>
            <name>is_disabled</name>
          </Item>
          <Item type='Property' id='6532264A05D74855B9908B6CC0BED86D' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>center</column_alignment>
            <column_width>80</column_width>
            <data_type>boolean</data_type>
            <default_value xml:lang='en'>0</default_value>
            <is_class_required>0</is_class_required>
            <is_hidden>1</is_hidden>
            <is_hidden2>1</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Overdue</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>4</sort_order>
            <track_history>0</track_history>
            <name>is_overdue</name>
          </Item>
          <Item type='Property' id='6380838A9C7245CEA4D7682DE9D4FB1B' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>center</column_alignment>
            <column_width>80</column_width>
            <data_type>boolean</data_type>
            <default_value xml:lang='en'>0</default_value>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Required</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>0</sort_order>
            <track_history>0</track_history>
            <name>is_required</name>
          </Item>
          <Item type='Property' id='BC501CB8D09448FA827B7ABCB2A0D0C7' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>left</column_alignment>
            <column_width>120</column_width>
            <data_type>string</data_type>
            <is_class_required>0</is_class_required>
            <is_hidden>1</is_hidden>
            <is_hidden2>1</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Path</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>99</sort_order>
            <stored_length>32</stored_length>
            <track_history>0</track_history>
            <name>path</name>
          </Item>
          <Item type='Property' id='4E0F6E38EBB1416891612974026C53BD' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>right</column_alignment>
            <column_width>80</column_width>
            <data_type>integer</data_type>
            <default_value xml:lang='en'>0</default_value>
            <is_class_required>0</is_class_required>
            <is_hidden>1</is_hidden>
            <is_hidden2>1</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>0</is_required>
            <label xml:lang='en'>Reminders Sent</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>3</sort_order>
            <track_history>0</track_history>
            <name>reminders_sent</name>
          </Item>
          <Item type='Property' id='DE47E38B4CAC4E649BC4945244C853EC' action='merge'>
            <can_update_released>0</can_update_released>
            <column_alignment>right</column_alignment>
            <column_width>80</column_width>
            <data_type>integer</data_type>
            <default_value xml:lang='en'>100</default_value>
            <is_class_required>0</is_class_required>
            <is_hidden>0</is_hidden>
            <is_hidden2>0</is_hidden2>
            <is_indexed>0</is_indexed>
            <is_keyed>0</is_keyed>
            <is_multi_valued>0</is_multi_valued>
            <is_required>1</is_required>
            <label xml:lang='en'>Voting Weight</label>
            <range_inclusive>0</range_inclusive>
            <readonly>0</readonly>
            <sort_order>2</sort_order>
            <track_history>0</track_history>
            <name>voting_weight</name>
          </Item>
          <Item type='Server Event' id='13D8F0A86A6F4254AD7BFFD02591F119' action='merge'>
            <event_version>version_1</event_version>
            <is_required>0</is_required>
            <related_id type='Method'><Item type='Method' action='get' _config_id='6E72FE69A62E4931BF9E0823127E8241' where=""[Method].[config_id] = '6E72FE69A62E4931BF9E0823127E8241'"" /></related_id>
            <server_event>onAfterAdd</server_event>
            <sort_order>128</sort_order>
          </Item>
          <Item type='Server Event' id='8B7C5397C1EC47F1B483C6B28F39180E' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='4FF70B9A34424B42992C9DD19083C2CD' where=""[Method].[config_id] = '4FF70B9A34424B42992C9DD19083C2CD'"" /></related_id>
            <server_event>onAfterAdd</server_event>
            <sort_order>150</sort_order>
          </Item>
          <Item type='Server Event' id='9BD8C1A5434842BD8C7FE0A32A59FC31' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='4BBFF1DD8E094D139B58DCF7C20FFF54' where=""[Method].[config_id] = '4BBFF1DD8E094D139B58DCF7C20FFF54'"" /></related_id>
            <server_event>onAfterAdd</server_event>
            <sort_order>200</sort_order>
          </Item>
          <Item type='Server Event' id='6089B9C041814535962A7A3BB5C487BC' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='4FF70B9A34424B42992C9DD19083C2CD' where=""[Method].[config_id] = '4FF70B9A34424B42992C9DD19083C2CD'"" /></related_id>
            <server_event>onAfterUpdate</server_event>
            <sort_order>384</sort_order>
          </Item>
          <Item type='Server Event' id='6847E681E2DC46AD86ABE91D11411C61' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='AC3EEF912133498588D7DF8C06B814A3' where=""[Method].[config_id] = 'AC3EEF912133498588D7DF8C06B814A3'"" /></related_id>
            <server_event>onBeforeAdd</server_event>
            <sort_order>100</sort_order>
          </Item>
          <Item type='Server Event' id='496769BB12A04D5D8E7294A345233167' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='AC3EEF912133498588D7DF8C06B814A3' where=""[Method].[config_id] = 'AC3EEF912133498588D7DF8C06B814A3'"" /></related_id>
            <server_event>onBeforeDelete</server_event>
            <sort_order>100</sort_order>
          </Item>
          <Item type='Server Event' id='A5A19E8A26F04B97A4AE64701518F3D5' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='277CDBDC93AB446484808613BC0D754B' where=""[Method].[config_id] = '277CDBDC93AB446484808613BC0D754B'"" /></related_id>
            <server_event>onBeforeDelete</server_event>
            <sort_order>512</sort_order>
          </Item>
          <Item type='Server Event' id='FE59F0D96F4243C19DE75C8504B4E9D7' action='merge'>
            <related_id type='Method'><Item type='Method' action='get' _config_id='4BBFF1DD8E094D139B58DCF7C20FFF54' where=""[Method].[config_id] = '4BBFF1DD8E094D139B58DCF7C20FFF54'"" /></related_id>
            <server_event>onBeforeUpdate</server_event>
            <sort_order>10000</sort_order>
          </Item>
          <Item type='TOC Access' id='C429A399247545299B1F389629CAA14F' action='merge'>
            <related_id type='Identity'>
              <Item type='Identity' action='get' select='id'>
                <name>Super User</name>
              </Item>
            </related_id>
            <sort_order>128</sort_order>
            <category>Portfolio</category>
          </Item>
          <Item type='TOC View' id='DF069C7123F6442CB4EAEE72FCAC8138' action='merge'>
            <related_id type='Identity'>
              <Item type='Identity' action='get' select='id'>
                <name>World</name>
              </Item>
            </related_id>
            <sort_order>128</sort_order>
            <start_page>InBasket/InBasket.html</start_page>
          </Item>
        </Relationships>
      </Item>
    </relationship_id>
    <sort_order>128</sort_order>
    <source_id type='ItemType' name='Activity'>937CE47DE2854308BE6FF5AB1CFB19D4</source_id>
    <name>Activity Assignment</name>
  </Item>
</AML>";
  }
}
