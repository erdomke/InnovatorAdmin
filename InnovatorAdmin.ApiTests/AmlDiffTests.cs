using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;

namespace InnovatorAdmin.Tests
{
  [TestClass()]
  public class AmlDiffTests
  {
    [TestMethod()]
    public void GetMergeScriptTest()
    {
      var start = @"<AML>
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
      var dest = @"<AML>
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

      var diff = AmlDiff.GetMergeScript(XmlReader.Create(new StringReader(start)),
        XmlReader.Create(new StringReader(dest)));
      Assert.AreEqual(expected, diff.ToString());
    }
  }
}
