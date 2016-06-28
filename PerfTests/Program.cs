using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;

namespace PerfTests
{
  class Program
  {
    static void Main(string[] args)
    {
      for (var i = 0; i < 100; i++)
      {
        ElementFactory.Local.FromXml(_itemTypeAml);
      }
    }

    private static string _itemTypeAml = @"<SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/'>
  <SOAP-ENV:Body>
    <Result>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='AEFCD3D2DC1D4E3EA126D49D68041EB6'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_properties.gif</close_icon>
        <config_id keyed_name='Access' type='ItemType' name='Access'>AEFCD3D2DC1D4E3EA126D49D68041EB6</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:42:51</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Access' type='ItemType'>AEFCD3D2DC1D4E3EA126D49D68041EB6</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACCESS</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Access</keyed_name>
        <label xml:lang='en'>Access</label>
        <label_plural xml:lang='en'>Permissions</label_plural>
        <large_icon>../images/Icons/32x32/32x32_check_list.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2012-09-19T13:22:46</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_properties.gif</open_icon>
        <permission_id keyed_name='A6EF49D3EAD24F0EAA6E6359448BC2DE' type='Permission'>A6EF49D3EAD24F0EAA6E6359448BC2DE</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Access</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='483228BE6B9A4C0E99ACD55FDF328DEC'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_methods.gif</close_icon>
        <config_id keyed_name='Action' type='ItemType' name='Action'>483228BE6B9A4C0E99ACD55FDF328DEC</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-09-02T02:50:14</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Actions.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Action' type='ItemType'>483228BE6B9A4C0E99ACD55FDF328DEC</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTION</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Action</keyed_name>
        <label xml:lang='en'>Action</label>
        <label_plural xml:lang='en'>Actions</label_plural>
        <large_icon>../images/Icons/32x32/32x32_action.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2008-09-04T05:10:20</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_action.gif</open_icon>
        <permission_id keyed_name='DC7C1DED9942449F93E511F7EA5D0F4A' type='Permission'>DC7C1DED9942449F93E511F7EA5D0F4A</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Action</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='937CE47DE2854308BE6FF5AB1CFB19D4'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x16_text.gif</close_icon>
        <config_id keyed_name='Activity' type='ItemType' name='Activity'>937CE47DE2854308BE6FF5AB1CFB19D4</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:32:09</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity' type='ItemType'>937CE47DE2854308BE6FF5AB1CFB19D4</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>1</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity</keyed_name>
        <label xml:lang='en'>Activity</label>
        <label_plural xml:lang='en'>Activities</label_plural>
        <large_icon>../images/Icons/32x32/32x32_documenting.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2015-10-13T11:20:13</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_text.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Activity</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='85924010F3184E77B24E9142FDBB481B'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x16_inbox.gif</close_icon>
        <config_id keyed_name='Activity Assignment' type='ItemType' name='Activity Assignment'>85924010F3184E77B24E9142FDBB481B</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:33:30</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity Assignment' type='ItemType'>85924010F3184E77B24E9142FDBB481B</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_ASSIGNMENT</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity Assignment</keyed_name>
        <label xml:lang='en'>Activity Assignment</label>
        <label_plural xml:lang='en'>Project InBox</label_plural>
        <large_icon>../images/Icons/32x32/32x32_inbox2.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2016-03-08T14:26:30</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_inbox.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Assignment</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='90D8880C29CD45C3AA8DAFF1DAEBC60E'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Activity EMail' type='ItemType' name='Activity EMail'>90D8880C29CD45C3AA8DAFF1DAEBC60E</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:34:21</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity EMail' type='ItemType'>90D8880C29CD45C3AA8DAFF1DAEBC60E</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_EMAIL</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity EMail</keyed_name>
        <label xml:lang='en'>Activity EMail</label>
        <label_plural xml:lang='en'>Activity EMails</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_on>2004-02-27T10:34:33</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity EMail</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='BF3EAD31AAA2403592CAAC2446FF7797'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Activity Method' type='ItemType' name='Activity Method'>BF3EAD31AAA2403592CAAC2446FF7797</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:34:10</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity Method' type='ItemType'>BF3EAD31AAA2403592CAAC2446FF7797</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_METHOD</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity Method</keyed_name>
        <label xml:lang='en'>Activity Method</label>
        <label_plural xml:lang='en'>Activity Methods</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:53:37</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Method</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='46BDE53304404C28B5C45610E41C1DD5'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x16_had_up.gif</close_icon>
        <config_id keyed_name='Activity Task' type='ItemType' name='Activity Task'>46BDE53304404C28B5C45610E41C1DD5</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:33:17</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity Task' type='ItemType'>46BDE53304404C28B5C45610E41C1DD5</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_TASK</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity Task</keyed_name>
        <label xml:lang='en'>Activity Task</label>
        <label_plural xml:lang='en'>Activity Tasks</label_plural>
        <large_icon>../images/Icons/32x32/32x32_check_list.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:53:47</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_had_up.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Task</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='BD4A250787A742A484C7B174A4AED1E2'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Activity Task Value' type='ItemType' name='Activity Task Value'>BD4A250787A742A484C7B174A4AED1E2</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:33:57</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity Task Value' type='ItemType'>BD4A250787A742A484C7B174A4AED1E2</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_TASK_VALUE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity Task Value</keyed_name>
        <label xml:lang='en'>Activity Task Value</label>
        <label_plural xml:lang='en'>Activity Task Values</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_on>2004-02-27T10:34:09</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Task Value</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='7A3EFE7242DB4403965890C053A57A0B'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_lifecycle.gif</close_icon>
        <config_id keyed_name='Activity Template' type='ItemType' name='Activity Template'>7A3EFE7242DB4403965890C053A57A0B</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:29:37</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity Template' type='ItemType'>7A3EFE7242DB4403965890C053A57A0B</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_TEMPLATE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>1</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity Template</keyed_name>
        <label xml:lang='en'>Activity Template</label>
        <label_plural xml:lang='en'>Actvity Templates</label_plural>
        <large_icon>../images/Icons/32x32/32x32_lifecycle.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:45:27</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_lifecycle.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Activity Template</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='96C238700CD840DEBE512EE85D440AF3'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_text.gif</close_icon>
        <config_id keyed_name='Activity Template Assignment' type='ItemType' name='Activity Template Assignment'>96C238700CD840DEBE512EE85D440AF3</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:28:50</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity Template Assignment' type='ItemType'>96C238700CD840DEBE512EE85D440AF3</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_TEMPLATE_ASSIGNMENT</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity Template Assignment</keyed_name>
        <label xml:lang='en'>Activity Template Assignment</label>
        <label_plural xml:lang='en'>Activity Template Assignments</label_plural>
        <large_icon>../images/Icons/32x32/32x32_documenting.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2005-07-15T11:30:34</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_text.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Template Assignment</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='67735DF455F54736A9D51CB53AB129E3'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Activity Template EMail' type='ItemType' name='Activity Template EMail'>67735DF455F54736A9D51CB53AB129E3</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:29:02</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity Template EMail' type='ItemType'>67735DF455F54736A9D51CB53AB129E3</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_TEMPLATE_EMAIL</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity Template EMail</keyed_name>
        <label xml:lang='en'>Activity Template EMail</label>
        <label_plural xml:lang='en'>Activity Template EMails</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_on>2004-02-27T10:29:12</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Template EMail</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='05A68B0BC74C47A6A2FD4404A73C815F'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Activity Template Method' type='ItemType' name='Activity Template Method'>05A68B0BC74C47A6A2FD4404A73C815F</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:29:13</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity Template Method' type='ItemType'>05A68B0BC74C47A6A2FD4404A73C815F</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_TEMPLATE_METHOD</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity Template Method</keyed_name>
        <label xml:lang='en'>Activity Template Method</label>
        <label_plural xml:lang='en'>Activity Template Methods</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:53:55</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Template Method</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='E32DB4E6E3B64F98A12B550C578E6A01'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x16_had_up.gif</close_icon>
        <config_id keyed_name='Activity Template Task' type='ItemType' name='Activity Template Task'>E32DB4E6E3B64F98A12B550C578E6A01</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:28:39</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity Template Task' type='ItemType'>E32DB4E6E3B64F98A12B550C578E6A01</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_TEMPLATE_TASK</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity Template Task</keyed_name>
        <label xml:lang='en'>Activity Template Task</label>
        <label_plural xml:lang='en'>Activity Template Tasks</label_plural>
        <large_icon>../images/Icons/32x32/32x32_check_list.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:54:04</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_had_up.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Template Task</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='FA1755A31ACF4EDFBBFFCD6A8C6F7AF8'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <config_id keyed_name='Activity Template Variable' type='ItemType' name='Activity Template Variable'>FA1755A31ACF4EDFBBFFCD6A8C6F7AF8</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:28:26</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity Template Variable' type='ItemType'>FA1755A31ACF4EDFBBFFCD6A8C6F7AF8</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_TEMPLATE_VARIABLE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity Template Variable</keyed_name>
        <label xml:lang='en'>Activity Template Variable</label>
        <label_plural xml:lang='en'>Activity Template Variables</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:54:23</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Template Variable</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='8B58025F8E504DBDADF0E1176D3CE178'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <config_id keyed_name='Activity Variable' type='ItemType' name='Activity Variable'>8B58025F8E504DBDADF0E1176D3CE178</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:33:04</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity Variable' type='ItemType'>8B58025F8E504DBDADF0E1176D3CE178</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_VARIABLE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity Variable</keyed_name>
        <label xml:lang='en'>Activity Variable</label>
        <label_plural xml:lang='en'>Activity Variables</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:54:42</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Variable</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='CB7696CC33EC4D1BB98716465F1AD580'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <config_id keyed_name='Activity Variable Value' type='ItemType' name='Activity Variable Value'>CB7696CC33EC4D1BB98716465F1AD580</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:33:46</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Activity Variable Value' type='ItemType'>CB7696CC33EC4D1BB98716465F1AD580</id>
        <implementation_type>table</implementation_type>
        <instance_data>ACTIVITY_VARIABLE_VALUE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Activity Variable Value</keyed_name>
        <label xml:lang='en'>Activity Variable Value</label>
        <label_plural xml:lang='en'>Activity Variable Values</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_on>2004-02-27T10:33:56</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Activity Variable Value</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='4E355E04444B4676AE723B43DECA37DC'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_profiles.gif</close_icon>
        <config_id keyed_name='Alias' type='ItemType' name='Alias'>4E355E04444B4676AE723B43DECA37DC</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:42:37</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Alias' type='ItemType'>4E355E04444B4676AE723B43DECA37DC</id>
        <implementation_type>table</implementation_type>
        <instance_data>ALIAS</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Alias</keyed_name>
        <label xml:lang='en'>Alias</label>
        <label_plural xml:lang='en'>Aliases</label_plural>
        <large_icon>../images/Icons/32x32/32x32_profile.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2012-02-06T14:19:47</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_profiles.gif</open_icon>
        <permission_id keyed_name='5623B4984F344C5ABBF8A6DFEBA990A5' type='Permission'>5623B4984F344C5ABBF8A6DFEBA990A5</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Alias</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='DB54505FA3E9419DA3C1E1AFB7A48C1C'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Allowed Permission' type='ItemType' name='Allowed Permission'>DB54505FA3E9419DA3C1E1AFB7A48C1C</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-08-04T17:11:47</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Allowed Permission' type='ItemType'>DB54505FA3E9419DA3C1E1AFB7A48C1C</id>
        <implementation_type>table</implementation_type>
        <instance_data>ALLOWED_PERMISSION</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Allowed Permission</keyed_name>
        <label xml:lang='en'>Allowed Permission</label>
        <label_plural xml:lang='en'>Allowed Permissions</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_on>2004-08-04T17:12:05</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Allowed Permission</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='10B8BB84EEE9413AAD071C8341BBAB04'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_lifecycle.gif</close_icon>
        <config_id keyed_name='Allowed Workflow' type='ItemType' name='Allowed Workflow'>10B8BB84EEE9413AAD071C8341BBAB04</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-09-09T12:23:19</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Allowed Workflow' type='ItemType'>10B8BB84EEE9413AAD071C8341BBAB04</id>
        <implementation_type>table</implementation_type>
        <instance_data>ALLOWED_WORKFLOW</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Allowed Workflow</keyed_name>
        <label xml:lang='en'>Allowed Workflow</label>
        <label_plural xml:lang='en'>Allowed Workflows</label_plural>
        <large_icon>../images/Icons/32x32/32x32_lifecycle.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:02:43</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_lifecycle.gif</open_icon>
        <permission_id keyed_name='D3A88BF525C64EFD8D9D02F5404C5E2C' type='Permission'>D3A88BF525C64EFD8D9D02F5404C5E2C</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Allowed Workflow</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='6B9057491021453ABA0A425570CC10D2'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Applied Updates' type='ItemType' name='Applied Updates'>6B9057491021453ABA0A425570CC10D2</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-08-04T17:26:30</created_on>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Applied_Updates.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Applied Updates' type='ItemType'>6B9057491021453ABA0A425570CC10D2</id>
        <implementation_type>table</implementation_type>
        <instance_data>APPLIED_UPDATES</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Applied Updates</keyed_name>
        <label xml:lang='en'>Applied Updates</label>
        <label_plural xml:lang='en'>Applied Updates</label_plural>
        <large_icon>../images/Icons/32x32/32x32_databaseUpdates.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:46:10</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_databaseUpdates.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Applied Updates</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='BF60433C7E924BE6B78D901809F8FEF6'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_properties.gif</close_icon>
        <config_id keyed_name='Body' type='ItemType' name='Body'>BF60433C7E924BE6B78D901809F8FEF6</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:43:31</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Body' type='ItemType'>BF60433C7E924BE6B78D901809F8FEF6</id>
        <implementation_type>table</implementation_type>
        <instance_data>BODY</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Body</keyed_name>
        <label xml:lang='en'>Body</label>
        <label_plural xml:lang='en'>Bodies</label_plural>
        <large_icon>../images/Icons/32x32/32x32_check_list.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:03:13</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_properties.gif</open_icon>
        <permission_id keyed_name='4AE93FA4D51540F8989D712722143D35' type='Permission'>4AE93FA4D51540F8989D712722143D35</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Body</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='3A65F41FF1FC42518A702FDA164AF420'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Can Add' type='ItemType' name='Can Add'>3A65F41FF1FC42518A702FDA164AF420</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-08-04T17:11:33</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Can Add' type='ItemType'>3A65F41FF1FC42518A702FDA164AF420</id>
        <implementation_type>table</implementation_type>
        <instance_data>CAN_ADD</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Can Add</keyed_name>
        <label xml:lang='en'>Can Add</label>
        <label_plural xml:lang='en'>Can Add</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2011-12-27T15:30:14</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Can Add</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='432E29895A994D0DBC9DF9B0918E189F'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Client Event' type='ItemType' name='Client Event'>432E29895A994D0DBC9DF9B0918E189F</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2005-07-15T11:32:53</created_on>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Client Event' type='ItemType'>432E29895A994D0DBC9DF9B0918E189F</id>
        <implementation_type>table</implementation_type>
        <instance_data>CLIENT_EVENT</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Client Event</keyed_name>
        <label xml:lang='en'>Client Event</label>
        <label_plural xml:lang='en'>Client Events</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:55:10</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Client Event</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='0EA142227FAE40148A4BDD8CF1D450EF'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_collection.gif</close_icon>
        <config_id keyed_name='Column Event' type='ItemType' name='Column Event'>0EA142227FAE40148A4BDD8CF1D450EF</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2005-07-15T11:34:26</created_on>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Column Event' type='ItemType'>0EA142227FAE40148A4BDD8CF1D450EF</id>
        <implementation_type>table</implementation_type>
        <instance_data>COLUMN_EVENT</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Column Event</keyed_name>
        <label xml:lang='en'>Column Event</label>
        <label_plural xml:lang='en'>Column Events</label_plural>
        <large_icon>../images/Icons/32x32/32x32_collection.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2006-08-03T14:21:11</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_collection.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <show_parameters_tab>1</show_parameters_tab>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Column Event</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='4762D42F2B774C2B958D170477EAF695'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/20x20/21x14_bom.gif</close_icon>
        <config_id keyed_name='Configuration' type='ItemType' name='Configuration'>4762D42F2B774C2B958D170477EAF695</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-08-09T09:40:07</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Configuration' type='ItemType'>4762D42F2B774C2B958D170477EAF695</id>
        <implementation_type>table</implementation_type>
        <instance_data>CONFIGURATION</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Configuration</keyed_name>
        <label xml:lang='en'>Configuration</label>
        <label_plural xml:lang='en'>Saved Cache</label_plural>
        <large_icon>../images/Icons/32x32/28x32_structure.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:46:35</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/20x20/21x14_bom.gif</open_icon>
        <permission_id keyed_name='949C36ACD4674671B6CA0EB80C9CF5BD' type='Permission'>949C36ACD4674671B6CA0EB80C9CF5BD</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Configuration</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='789104C736664FAA9748352CBBF86BCA'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x14_favorites.gif</close_icon>
        <config_id keyed_name='Desktop' type='ItemType' name='Desktop'>789104C736664FAA9748352CBBF86BCA</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Innovator Admin' type='User'>30B991F927274FA3829655F50C99472E</created_by_id>
        <created_on>2003-01-03T14:19:05</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Desktop' type='ItemType'>789104C736664FAA9748352CBBF86BCA</id>
        <implementation_type>table</implementation_type>
        <instance_data>DESKTOP</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Desktop</keyed_name>
        <label xml:lang='en'>Desktop</label>
        <label_plural xml:lang='en'>My Desktop</label_plural>
        <large_icon>../images/Icons/32x32/34x34_desktop.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:53:08</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x14_favorites.gif</open_icon>
        <permission_id keyed_name='5B02D2FFF93A49A58A3B035009652F00' type='Permission'>5B02D2FFF93A49A58A3B035009652F00</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Desktop</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='F91DE6AE038A4CC8B53D47D5A7FA49FC'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/icons/16x16/19x16_envelope.gif</close_icon>
        <config_id keyed_name='EMail Message' type='ItemType' name='EMail Message'>F91DE6AE038A4CC8B53D47D5A7FA49FC</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2003-11-06T11:23:11</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='EMail Message' type='ItemType'>F91DE6AE038A4CC8B53D47D5A7FA49FC</id>
        <implementation_type>table</implementation_type>
        <instance_data>EMAIL_MESSAGE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>EMail Message</keyed_name>
        <label xml:lang='en'>EMail Message</label>
        <label_plural xml:lang='en'>E-Mail Message</label_plural>
        <large_icon>../images/Icons/32x32/32x32_email.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2014-09-18T17:41:57</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/icons/16x16/19x16_envelope.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>EMail Message</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='1549FB139D6B4AD99FDC1ED4B8011DB9'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_had_up.gif</close_icon>
        <config_id keyed_name='Exclusion' type='ItemType' name='Exclusion'>1549FB139D6B4AD99FDC1ED4B8011DB9</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:45:37</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Exclusion' type='ItemType'>1549FB139D6B4AD99FDC1ED4B8011DB9</id>
        <implementation_type>table</implementation_type>
        <instance_data>EXCLUSION</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Exclusion</keyed_name>
        <label xml:lang='en'>Exclusion</label>
        <label_plural xml:lang='en'>Exclusions</label_plural>
        <large_icon>../images/Icons/32x32/32x32_check_list.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:05:36</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_had_up.gif</open_icon>
        <permission_id keyed_name='56E23C7FDDED4BF99E947751BD9B10C4' type='Permission'>56E23C7FDDED4BF99E947751BD9B10C4</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Exclusion</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='0BB5B81FEB37475BB9C779408080DB61'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_properties.gif</close_icon>
        <config_id keyed_name='Field' type='ItemType' name='Field'>0BB5B81FEB37475BB9C779408080DB61</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:43:40</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Field' type='ItemType'>0BB5B81FEB37475BB9C779408080DB61</id>
        <implementation_type>table</implementation_type>
        <instance_data>FIELD</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Field</keyed_name>
        <label xml:lang='en'>Field</label>
        <label_plural xml:lang='en'>Fields</label_plural>
        <large_icon>../images/Icons/32x32/32x32_check_list.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:59:24</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_properties.gif</open_icon>
        <permission_id keyed_name='2F3872148EB543D69AE5C6C47063C5FD' type='Permission'>2F3872148EB543D69AE5C6C47063C5FD</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Field</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='3572C4D1479445D9959C413624A9FFF6'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_arrow_right.gif</close_icon>
        <config_id keyed_name='Field Event' type='ItemType' name='Field Event'>3572C4D1479445D9959C413624A9FFF6</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:44:56</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Field Event' type='ItemType'>3572C4D1479445D9959C413624A9FFF6</id>
        <implementation_type>table</implementation_type>
        <instance_data>FIELD_EVENT</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Field Event</keyed_name>
        <label xml:lang='en'>Field Event</label>
        <label_plural xml:lang='en'>Field Events</label_plural>
        <large_icon>../images/Icons/32x32/32x32_method.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T17:00:45</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_arrow_right.gif</open_icon>
        <permission_id keyed_name='D3C4F9B6132F4220ACC305DCFCBA317C' type='Permission'>D3C4F9B6132F4220ACC305DCFCBA317C</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Field Event</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='8052A558B9084D41B9F11805E464F443'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/icons/16x16/16x16_file.gif</close_icon>
        <config_id keyed_name='File' type='ItemType' name='File'>8052A558B9084D41B9F11805E464F443</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:45:11</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Files.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='File' type='ItemType'>8052A558B9084D41B9F11805E464F443</id>
        <implementation_type>table</implementation_type>
        <instance_data>FILE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>1</is_versionable>
        <keyed_name>File</keyed_name>
        <label xml:lang='en'>File</label>
        <label_plural xml:lang='en'>Files</label_plural>
        <large_icon>../images/icons/32x32/32x32_file.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2014-11-17T15:15:59</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_file3.gif</open_icon>
        <permission_id keyed_name='AAC67329352649E585527F1DB1FBA748' type='Permission'>AAC67329352649E585527F1DB1FBA748</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>File</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='13EC84A626F1457BB5F60A13DA03580B'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_file.gif</close_icon>
        <config_id keyed_name='FileType' type='ItemType' name='FileType'>13EC84A626F1457BB5F60A13DA03580B</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-08-23T09:22:09</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_FileTypes.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='FileType' type='ItemType'>13EC84A626F1457BB5F60A13DA03580B</id>
        <implementation_type>table</implementation_type>
        <instance_data>FILETYPE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>FileType</keyed_name>
        <label xml:lang='en'>FileType</label>
        <label_plural xml:lang='en'>FileTypes</label_plural>
        <large_icon>../images/Icons/32x32/32x32_pc.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:47:39</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_computer.gif</open_icon>
        <permission_id keyed_name='CF9B4A0F6803405DB9C68DF884DF1329' type='Permission'>CF9B4A0F6803405DB9C68DF884DF1329</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>FileType</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='BCC8053D365143A18B033850EFE56F3C'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_lists.gif</close_icon>
        <config_id keyed_name='Filter Value' type='ItemType' name='Filter Value'>BCC8053D365143A18B033850EFE56F3C</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-10-03T23:24:29</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Filter Value' type='ItemType'>BCC8053D365143A18B033850EFE56F3C</id>
        <implementation_type>table</implementation_type>
        <instance_data>FILTER_VALUE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Filter Value</keyed_name>
        <label xml:lang='en'>Filter Value</label>
        <label_plural xml:lang='en'>Filter Values</label_plural>
        <large_icon>../images/Icons/32x32/32x32_lists.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2016-03-03T16:12:45</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_lists.gif</open_icon>
        <permission_id keyed_name='3A12758B5BF54F46A2D1480F93120876' type='Permission'>3A12758B5BF54F46A2D1480F93120876</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Filter Value</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='47573682FB7549F59ADECD4BFE04F1DE'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_item.gif</close_icon>
        <config_id keyed_name='Form' type='ItemType' name='Form'>47573682FB7549F59ADECD4BFE04F1DE</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:43:15</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Forms.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Form' type='ItemType'>47573682FB7549F59ADECD4BFE04F1DE</id>
        <implementation_type>table</implementation_type>
        <instance_data>FORM</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Form</keyed_name>
        <label xml:lang='en'>Form</label>
        <label_plural xml:lang='en'>Forms</label_plural>
        <large_icon>../images/icons/32x32/32x32_form.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2014-03-11T08:06:14</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_forms.gif</open_icon>
        <permission_id keyed_name='5654AEADEAE7498AA069965B1CD5BF13' type='Permission'>5654AEADEAE7498AA069965B1CD5BF13</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Form</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='96730C617F4C40729D730D97395FD620'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_arrow_right.gif</close_icon>
        <config_id keyed_name='Form Event' type='ItemType' name='Form Event'>96730C617F4C40729D730D97395FD620</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:44:41</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Form Event' type='ItemType'>96730C617F4C40729D730D97395FD620</id>
        <implementation_type>table</implementation_type>
        <instance_data>FORM_EVENT</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Form Event</keyed_name>
        <label xml:lang='en'>Form Event</label>
        <label_plural xml:lang='en'>Form Events</label_plural>
        <large_icon>../images/Icons/32x32/32x32_method.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:55:32</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_arrow_right.gif</open_icon>
        <permission_id keyed_name='B972F1DCFF9148F5BD3E00C75DC96E80' type='Permission'>B972F1DCFF9148F5BD3E00C75DC96E80</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Form Event</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='AC218DF3AE43488C9E64E1AA551D2522'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_item.gif</close_icon>
        <config_id keyed_name='Frame' type='ItemType' name='Frame'>AC218DF3AE43488C9E64E1AA551D2522</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:44:22</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Frame' type='ItemType'>AC218DF3AE43488C9E64E1AA551D2522</id>
        <implementation_type>table</implementation_type>
        <instance_data>FRAME</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Frame</keyed_name>
        <label xml:lang='en'>Frame</label>
        <label_plural xml:lang='en'>Frames</label_plural>
        <large_icon>../images/Icons/32x32/32x32_form.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:08:23</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_item.gif</open_icon>
        <permission_id keyed_name='B5451BB6D23B4C3CA3C8FD5B36FB558B' type='Permission'>B5451BB6D23B4C3CA3C8FD5B36FB558B</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Frame</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='CA289ED4F1A84A9EB6CDD822846FD745'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_item.gif</close_icon>
        <config_id keyed_name='Frameset' type='ItemType' name='Frameset'>CA289ED4F1A84A9EB6CDD822846FD745</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:44:03</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Frameset' type='ItemType'>CA289ED4F1A84A9EB6CDD822846FD745</id>
        <implementation_type>table</implementation_type>
        <instance_data>FRAMESET</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Frameset</keyed_name>
        <label xml:lang='en'>Frameset</label>
        <label_plural xml:lang='en'>Framesets</label_plural>
        <large_icon>../images/Icons/32x32/32x32_form.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:08:39</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_item.gif</open_icon>
        <permission_id keyed_name='94F2E8F6ED144F8183BFE2A2B5420659' type='Permission'>94F2E8F6ED144F8183BFE2A2B5420659</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Frameset</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='8EABD91B465443F0A4995418F483DC51'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_collection.gif</close_icon>
        <config_id keyed_name='Grid' type='ItemType' name='Grid'>8EABD91B465443F0A4995418F483DC51</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2005-07-15T11:34:21</created_on>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Grid' type='ItemType'>8EABD91B465443F0A4995418F483DC51</id>
        <implementation_type>table</implementation_type>
        <instance_data>GRID</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Grid</keyed_name>
        <label xml:lang='en'>Grid</label>
        <label_plural xml:lang='en'>Grids</label_plural>
        <large_icon>../images/Icons/32x32/32x32_collection.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2006-08-03T14:20:58</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_collection.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <show_parameters_tab>1</show_parameters_tab>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Grid</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='C24A87B33E3740B3B01254DC776F1EFE'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_collection.gif</close_icon>
        <config_id keyed_name='Grid Column' type='ItemType' name='Grid Column'>C24A87B33E3740B3B01254DC776F1EFE</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2005-07-15T11:34:23</created_on>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Grid Column' type='ItemType'>C24A87B33E3740B3B01254DC776F1EFE</id>
        <implementation_type>table</implementation_type>
        <instance_data>GRID_COLUMN</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Grid Column</keyed_name>
        <label xml:lang='en'>Grid Column</label>
        <label_plural xml:lang='en'>Grid Columns</label_plural>
        <large_icon>../images/Icons/32x32/32x32_collection.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2008-09-04T05:10:21</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_collection.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <show_parameters_tab>1</show_parameters_tab>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Grid Column</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='E2DECCA6300E4815B466C62C66E9D3AF'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_arrow_right.gif</close_icon>
        <config_id keyed_name='Grid Event' type='ItemType' name='Grid Event'>E2DECCA6300E4815B466C62C66E9D3AF</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2003-01-26T13:59:49</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Grid Event' type='ItemType'>E2DECCA6300E4815B466C62C66E9D3AF</id>
        <implementation_type>table</implementation_type>
        <instance_data>GRID_EVENT</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Grid Event</keyed_name>
        <label xml:lang='en'>Grid Event</label>
        <label_plural xml:lang='en'>Grid Events</label_plural>
        <large_icon>../images/Icons/32x32/32x32_method.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:59:36</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_arrow_right.gif</open_icon>
        <permission_id keyed_name='61F992AC2A5348148AA5D05931665C28' type='Permission'>61F992AC2A5348148AA5D05931665C28</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Grid Event</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='1718DC9FB25043B9A3F0B76DB5DC6637'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Help' type='ItemType' name='Help'>1718DC9FB25043B9A3F0B76DB5DC6637</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-12-08T14:35:15</created_on>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Help' type='ItemType'>1718DC9FB25043B9A3F0B76DB5DC6637</id>
        <implementation_type>table</implementation_type>
        <instance_data>HELP</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Help</keyed_name>
        <label xml:lang='en'>Help</label>
        <label_plural xml:lang='en'>Help Topics</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_help.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Help</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='ACD5116613FF40C9BC5EF7447CBEBBCB'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Help See Also' type='ItemType' name='Help See Also'>ACD5116613FF40C9BC5EF7447CBEBBCB</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-12-08T14:35:15</created_on>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Help See Also' type='ItemType'>ACD5116613FF40C9BC5EF7447CBEBBCB</id>
        <implementation_type>table</implementation_type>
        <instance_data>HELP_SEE_ALSO</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Help See Also</keyed_name>
        <label xml:lang='en'>Help See Also</label>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-12-08T14:42:08</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Help See Also</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='213B74E721ED457C9BE735C038C7CB95'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_redx.gif</close_icon>
        <config_id keyed_name='Hide In' type='ItemType' name='Hide In'>213B74E721ED457C9BE735C038C7CB95</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-09-03T13:32:30</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Hide In' type='ItemType'>213B74E721ED457C9BE735C038C7CB95</id>
        <implementation_type>table</implementation_type>
        <instance_data>HIDE_IN</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Hide In</keyed_name>
        <label xml:lang='en'>Hide In</label>
        <label_plural xml:lang='en'>Hide In</label_plural>
        <large_icon>../images/Icons/32x32/32x32_ref_x.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:09:05</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_redx.gif</open_icon>
        <permission_id keyed_name='51BBCE8C962349E2AE8A4DB0C834CD09' type='Permission'>51BBCE8C962349E2AE8A4DB0C834CD09</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Hide In</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='B70DE4074ABC49C69B0D8729D9212982'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_redx.gif</close_icon>
        <config_id keyed_name='Hide Related In' type='ItemType' name='Hide Related In'>B70DE4074ABC49C69B0D8729D9212982</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-09-03T13:34:08</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Hide Related In' type='ItemType'>B70DE4074ABC49C69B0D8729D9212982</id>
        <implementation_type>table</implementation_type>
        <instance_data>HIDE_RELATED_IN</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Hide Related In</keyed_name>
        <label xml:lang='en'>Hide Related In</label>
        <label_plural xml:lang='en'>Hide Related In</label_plural>
        <large_icon>../images/Icons/32x32/32x32_ref_x.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:09:47</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_redx.gif</open_icon>
        <permission_id keyed_name='5B7CCC6AAFB347DBBE93E0324AB2F7E5' type='Permission'>5B7CCC6AAFB347DBBE93E0324AB2F7E5</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Hide Related In</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='6323EE2C82E94CC4BB83779DDFDDD6F5'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_tearoff.gif</close_icon>
        <config_id keyed_name='History' type='ItemType' name='History'>6323EE2C82E94CC4BB83779DDFDDD6F5</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-09-02T02:03:28</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='History' type='ItemType'>6323EE2C82E94CC4BB83779DDFDDD6F5</id>
        <implementation_type>table</implementation_type>
        <instance_data>HISTORY</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>History</keyed_name>
        <label xml:lang='en'>History</label>
        <label_plural xml:lang='en'>History</label_plural>
        <large_icon>../images/Icons/32x32/32x32_folder_search.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T17:24:50</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_tearoff.gif</open_icon>
        <permission_id keyed_name='A0D86DBC5F0B4B64B386317AF74DBF27' type='Permission'>A0D86DBC5F0B4B64B386317AF74DBF27</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>History</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='06E0660816FE40A2BF1411B2280062B3'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='History Container' type='ItemType' name='History Container'>06E0660816FE40A2BF1411B2280062B3</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-08-04T17:12:37</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='History Container' type='ItemType'>06E0660816FE40A2BF1411B2280062B3</id>
        <implementation_type>table</implementation_type>
        <instance_data>HISTORY_CONTAINER</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>History Container</keyed_name>
        <label xml:lang='en'>History</label>
        <large_icon>../images/Item-large/books1.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2011-09-21T14:19:57</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>History Container</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='E582AB17663F4EF28460015B2BE9E094'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <class_structure><![CDATA[<class id='E582AB17663F4EF28460015B2BE9E094'><class id='6109991B724B4214865317F18FE189F1' name='Team' /><class id='6134A59154C74319B8F8B00729C9DD1E' name='System' /></class>]]></class_structure>
        <close_icon>../images/Icons/16x16/16x16_users.gif</close_icon>
        <config_id keyed_name='Identity' type='ItemType' name='Identity'>E582AB17663F4EF28460015B2BE9E094</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:42:20</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Identities.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Identity' type='ItemType'>E582AB17663F4EF28460015B2BE9E094</id>
        <implementation_type>table</implementation_type>
        <instance_data>IDENTITY</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Identity</keyed_name>
        <label xml:lang='en'>Identity</label>
        <label_plural xml:lang='en'>Identities</label_plural>
        <large_icon>../images/icons/32x32/32x32_members.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2014-03-29T12:25:41</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_members.gif</open_icon>
        <permission_id keyed_name='FEC86F601F7B40D1A3610ACE84CC4321' type='Permission'>FEC86F601F7B40D1A3610ACE84CC4321</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Identity</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='6C280692633D4498BD9CFEA7989138AB'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_action.gif</close_icon>
        <config_id keyed_name='Item Action' type='ItemType' name='Item Action'>6C280692633D4498BD9CFEA7989138AB</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-09-02T02:53:36</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Item Action' type='ItemType'>6C280692633D4498BD9CFEA7989138AB</id>
        <implementation_type>table</implementation_type>
        <instance_data>ITEM_ACTION</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Item Action</keyed_name>
        <label xml:lang='en'>Item Action</label>
        <label_plural xml:lang='en'>Item Actions</label_plural>
        <large_icon>../images/Icons/32x32/32x32_action.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:10:47</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_action.gif</open_icon>
        <permission_id keyed_name='896E27B2DE8B45A89119DF9F35769EB2' type='Permission'>896E27B2DE8B45A89119DF9F35769EB2</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Item Action</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='5BD6BED0CD794A078AA42476F47ECF46'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Item Report' type='ItemType' name='Item Report'>5BD6BED0CD794A078AA42476F47ECF46</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-08-04T17:25:13</created_on>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Item Report' type='ItemType'>5BD6BED0CD794A078AA42476F47ECF46</id>
        <implementation_type>table</implementation_type>
        <instance_data>ITEM_REPORT</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Item Report</keyed_name>
        <label xml:lang='en'>Item Report</label>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2012-10-02T16:10:24</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Item Report</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='450906E86E304F55A34B3C0D65C097EA'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_itemtype.gif</close_icon>
        <config_id keyed_name='ItemType' type='ItemType' name='ItemType'>450906E86E304F55A34B3C0D65C097EA</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:41:51</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_ItemTypes.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='ItemType' type='ItemType'>450906E86E304F55A34B3C0D65C097EA</id>
        <implementation_type>table</implementation_type>
        <instance_data>ITEMTYPE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>ItemType</keyed_name>
        <label xml:lang='en'>ItemType</label>
        <label_plural xml:lang='en'>ItemTypes</label_plural>
        <large_icon>../images/icons/32x32/32x32_itemtype.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Emanuel Chiaburu' type='User'>C42E964EC71E412CA678A1856A95B82A</modified_by_id>
        <modified_on>2016-03-29T15:42:20</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_itemtype.gif</open_icon>
        <permission_id keyed_name='8D591B8ED2EB4ABC80301D8B488DC045' type='Permission'>8D591B8ED2EB4ABC80301D8B488DC045</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>ItemType</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='AC32527D85604A4D9FC9107C516AEF47'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x16_lifecycle.gif</close_icon>
        <config_id keyed_name='Life Cycle Map' type='ItemType' name='Life Cycle Map'>AC32527D85604A4D9FC9107C516AEF47</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:49:33</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>100</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Lifecycles.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Life Cycle Map' type='ItemType'>AC32527D85604A4D9FC9107C516AEF47</id>
        <implementation_type>table</implementation_type>
        <instance_data>LIFE_CYCLE_MAP</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Life Cycle Map</keyed_name>
        <label xml:lang='en'>Life Cycle Map</label>
        <label_plural xml:lang='en'>Life Cycle Maps</label_plural>
        <large_icon>../images/icons/32x32/32x32_lifecycle.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2014-09-11T10:36:51</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_lifecycle.gif</open_icon>
        <permission_id keyed_name='197EEA64EC1F4E358B4804046AB4339D' type='Permission'>197EEA64EC1F4E358B4804046AB4339D</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Life Cycle Map</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='5EFB53D35BAE468B851CD388BEA46B30'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/20x20/20x15_lifecycle.gif</close_icon>
        <config_id keyed_name='Life Cycle State' type='ItemType' name='Life Cycle State'>5EFB53D35BAE468B851CD388BEA46B30</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:49:16</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Life Cycle State' type='ItemType'>5EFB53D35BAE468B851CD388BEA46B30</id>
        <implementation_type>table</implementation_type>
        <instance_data>LIFE_CYCLE_STATE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Life Cycle State</keyed_name>
        <label xml:lang='en'>Life Cycle State</label>
        <label_plural xml:lang='en'>Life Cycle States</label_plural>
        <large_icon>../images/Icons/Other/42x32_lifecycle.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:55:50</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/20x20/20x15_lifecycle.gif</open_icon>
        <permission_id keyed_name='91C1F80AFBD64275B28351733724472C' type='Permission'>91C1F80AFBD64275B28351733724472C</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Life Cycle State</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='1E764495A5134823B30060D83FD6A2F9'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_properties.gif</close_icon>
        <config_id keyed_name='Life Cycle Transition' type='ItemType' name='Life Cycle Transition'>1E764495A5134823B30060D83FD6A2F9</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:49:34</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Life Cycle Transition' type='ItemType'>1E764495A5134823B30060D83FD6A2F9</id>
        <implementation_type>table</implementation_type>
        <instance_data>LIFE_CYCLE_TRANSITION</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Life Cycle Transition</keyed_name>
        <label xml:lang='en'>Life Cycle Transition</label>
        <label_plural xml:lang='en'>Life Cycle Transitions</label_plural>
        <large_icon>../images/Icons/32x32/32x32_check_list.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2014-03-31T17:47:26</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_properties.gif</open_icon>
        <permission_id keyed_name='D2664329CFC346BFB3438E4FF03B287D' type='Permission'>D2664329CFC346BFB3438E4FF03B287D</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Life Cycle Transition</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='5736C479A8CB49BCA20138514C637266'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_lists.gif</close_icon>
        <config_id keyed_name='List' type='ItemType' name='List'>5736C479A8CB49BCA20138514C637266</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:41:14</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Lists.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='List' type='ItemType'>5736C479A8CB49BCA20138514C637266</id>
        <implementation_type>table</implementation_type>
        <instance_data>LIST</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>List</keyed_name>
        <label xml:lang='en'>List</label>
        <label_plural xml:lang='en'>Lists</label_plural>
        <large_icon>../images/customer/images/List32.png</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2016-03-03T14:35:37</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/customer/images/List16.png</open_icon>
        <permission_id keyed_name='A6E9ABE7F628487BA10B7E0D83F8E6D9' type='Permission'>A6E9ABE7F628487BA10B7E0D83F8E6D9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>List</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='5698BACD2A7A45D6AC3FA60EAB3E6566'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_vault.gif</close_icon>
        <config_id keyed_name='Located' type='ItemType' name='Located'>5698BACD2A7A45D6AC3FA60EAB3E6566</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-08-19T12:26:53</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Located' type='ItemType'>5698BACD2A7A45D6AC3FA60EAB3E6566</id>
        <implementation_type>table</implementation_type>
        <instance_data>LOCATED</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Located</keyed_name>
        <label xml:lang='en'>Located</label>
        <label_plural xml:lang='en'>Located</label_plural>
        <large_icon>../images/Icons/32x32/32x32_vault.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:56:14</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_vault.gif</open_icon>
        <permission_id keyed_name='F9339B68E7CD4D9FAC3D9D0BFFB87012' type='Permission'>F9339B68E7CD4D9FAC3D9D0BFFB87012</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Located</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='8EF041900FE2428AABD404063AB979B6'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/17x17/17x17_locked.gif</close_icon>
        <config_id keyed_name='LockedItems' type='ItemType' name='LockedItems'>8EF041900FE2428AABD404063AB979B6</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2003-11-06T11:20:23</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='LockedItems' type='ItemType'>8EF041900FE2428AABD404063AB979B6</id>
        <implementation_type>table</implementation_type>
        <instance_data>LOCKEDITEMS</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>LockedItems</keyed_name>
        <label xml:lang='en'>LockedItems</label>
        <label_plural xml:lang='en'>LockedItems</label_plural>
        <large_icon>../images/Icons/17x17/17x17_locked.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:13:41</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/17x17/17x17_locked.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>LockedItems</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='7348E620D27E40D1868C54247B5DE8D1'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_users.gif</close_icon>
        <config_id keyed_name='Member' type='ItemType' name='Member'>7348E620D27E40D1868C54247B5DE8D1</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:42:44</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Member' type='ItemType'>7348E620D27E40D1868C54247B5DE8D1</id>
        <implementation_type>table</implementation_type>
        <instance_data>MEMBER</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Member</keyed_name>
        <label xml:lang='en'>Member</label>
        <label_plural xml:lang='en'>Members</label_plural>
        <large_icon>../images/Icons/32x32/32x32_identities.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2015-02-25T12:05:20</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_users.gif</open_icon>
        <permission_id keyed_name='C99F1148BCB44FF7A84EDC441D9F2096' type='Permission'>C99F1148BCB44FF7A84EDC441D9F2096</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Member</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='87879A09B8044DE380D59DF22DE1867F'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_arrow_right.gif</close_icon>
        <config_id keyed_name='Method' type='ItemType' name='Method'>87879A09B8044DE380D59DF22DE1867F</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:43:01</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>45</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Methods.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Method' type='ItemType'>87879A09B8044DE380D59DF22DE1867F</id>
        <implementation_type>table</implementation_type>
        <instance_data>METHOD</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>1</is_versionable>
        <keyed_name>Method</keyed_name>
        <label xml:lang='en'>Method</label>
        <label_plural xml:lang='en'>Methods</label_plural>
        <large_icon>../images/Icons/32x32/32x32_method.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2014-04-11T16:36:04</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_method.gif</open_icon>
        <permission_id keyed_name='F0E486ABB76F415181595E0DC22635D6' type='Permission'>F0E486ABB76F415181595E0DC22635D6</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Method</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='5ED4936039F64E9E99F703F8F46A1DB1'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Morphae' type='ItemType' name='Morphae'>5ED4936039F64E9E99F703F8F46A1DB1</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2005-07-15T11:30:03</created_on>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Morphae' type='ItemType'>5ED4936039F64E9E99F703F8F46A1DB1</id>
        <implementation_type>table</implementation_type>
        <instance_data>MORPHAE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Morphae</keyed_name>
        <label xml:lang='en'>Morphae</label>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Morphae</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='C6A89FDE1294451497801DF78341B473'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_permissions.gif</close_icon>
        <config_id keyed_name='Permission' type='ItemType' name='Permission'>C6A89FDE1294451497801DF78341B473</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-08-04T17:11:17</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Permissions.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Permission' type='ItemType'>C6A89FDE1294451497801DF78341B473</id>
        <implementation_type>table</implementation_type>
        <instance_data>PERMISSION</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Permission</keyed_name>
        <label xml:lang='en'>Permission</label>
        <label_plural xml:lang='en'>Permissions</label_plural>
        <large_icon>../images/Icons/32x32/32x32_permissions.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2005-07-15T11:31:32</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_permissions.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Permission</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='26D7CD4E033242148E2724D3D054B4D3'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_properties.gif</close_icon>
        <config_id keyed_name='Property' type='ItemType' name='Property'>26D7CD4E033242148E2724D3D054B4D3</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:42:14</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Property' type='ItemType'>26D7CD4E033242148E2724D3D054B4D3</id>
        <implementation_type>table</implementation_type>
        <instance_data>PROPERTY</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Property</keyed_name>
        <label xml:lang='en'>Property</label>
        <label_plural xml:lang='en'>Properties</label_plural>
        <large_icon>../images/Icons/32x32/32x32_check_list.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Emanuel Chiaburu' type='User'>C42E964EC71E412CA678A1856A95B82A</modified_by_id>
        <modified_on>2016-05-06T10:39:21</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_properties.gif</open_icon>
        <permission_id keyed_name='5D0CB63EB95E439197B2ED7B76934BDB' type='Permission'>5D0CB63EB95E439197B2ED7B76934BDB</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Property</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='FF5EFC69BB8F4E56839A43A8477AE58F'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_methods.gif</close_icon>
        <config_id keyed_name='Relationship Grid Event' type='ItemType' name='Relationship Grid Event'>FF5EFC69BB8F4E56839A43A8477AE58F</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2003-01-28T14:06:31</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Relationship Grid Event' type='ItemType'>FF5EFC69BB8F4E56839A43A8477AE58F</id>
        <implementation_type>table</implementation_type>
        <instance_data>RELATIONSHIP_GRID_EVENT</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Relationship Grid Event</keyed_name>
        <label xml:lang='en'>Relationship Grid Event</label>
        <label_plural xml:lang='en'>Relationship Grid Events</label_plural>
        <large_icon>../images/Icons/32x32/32x32_action.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T17:00:03</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_methods.gif</open_icon>
        <permission_id keyed_name='1098E180841F4881B304D52FA717BD62' type='Permission'>1098E180841F4881B304D52FA717BD62</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Relationship Grid Event</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='C57EC6AAFE22490082F06FD8DCBE2E1C'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_item.gif</close_icon>
        <config_id keyed_name='Relationship View' type='ItemType' name='Relationship View'>C57EC6AAFE22490082F06FD8DCBE2E1C</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2003-11-06T11:21:28</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Relationship View' type='ItemType'>C57EC6AAFE22490082F06FD8DCBE2E1C</id>
        <implementation_type>table</implementation_type>
        <instance_data>RELATIONSHIP_VIEW</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Relationship View</keyed_name>
        <label xml:lang='en'>Relationship View</label>
        <label_plural xml:lang='en'>Relationship Views</label_plural>
        <large_icon>../images/Icons/32x32/32x32_form.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T17:00:15</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_item.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Relationship View</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='471932C33B604C3099070F4106EE5024'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_relationship.gif</close_icon>
        <config_id keyed_name='RelationshipType' type='ItemType' name='RelationshipType'>471932C33B604C3099070F4106EE5024</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:42:06</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_RelationshipTypes.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='RelationshipType' type='ItemType'>471932C33B604C3099070F4106EE5024</id>
        <implementation_type>table</implementation_type>
        <instance_data>RELATIONSHIPTYPE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>RelationshipType</keyed_name>
        <label xml:lang='en'>RelationshipType</label>
        <label_plural xml:lang='en'>RelationshipTypes</label_plural>
        <large_icon>../images/icons/32x32/32x32_relationship.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2014-03-11T08:09:51</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_relationship.gif</open_icon>
        <permission_id keyed_name='6F455466756A438C8955F3DC8AA55D82' type='Permission'>6F455466756A438C8955F3DC8AA55D82</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>RelationshipType</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='F0834BBA6FB64394B78DF5BB725532DD'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <config_id keyed_name='Report' type='ItemType' name='Report'>F0834BBA6FB64394B78DF5BB725532DD</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-08-04T17:24:44</created_on>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Reports.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Report' type='ItemType'>F0834BBA6FB64394B78DF5BB725532DD</id>
        <implementation_type>table</implementation_type>
        <instance_data>REPORT</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Report</keyed_name>
        <label xml:lang='en'>Report</label>
        <label_plural xml:lang='en'>Reports</label_plural>
        <large_icon>../images/Icons/32x32/32x32_reports.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2014-09-11T10:37:20</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_reports.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Report</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='F81CDEF9FE324D01947CC9023BC38317'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/20x20/20x20_revision.gif</close_icon>
        <config_id keyed_name='Revision' type='ItemType' name='Revision'>F81CDEF9FE324D01947CC9023BC38317</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-05-18T16:53:08</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Revisions.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Revision' type='ItemType'>F81CDEF9FE324D01947CC9023BC38317</id>
        <implementation_type>table</implementation_type>
        <instance_data>REVISION</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Revision</keyed_name>
        <label xml:lang='en'>Revision</label>
        <label_plural xml:lang='en'>Revisions</label_plural>
        <large_icon>../images/icons/32x32/32x32_revision.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2011-08-09T16:44:21</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/20x20/20x20_revision.gif</open_icon>
        <permission_id keyed_name='8EAA5F5725A94070ABEBAE084761EFFD' type='Permission'>8EAA5F5725A94070ABEBAE084761EFFD</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Revision</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='E5BC8090E82D4F8D8E3F389C95316433'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_calendar.gif</close_icon>
        <config_id keyed_name='Scheduled Task' type='ItemType' name='Scheduled Task'>E5BC8090E82D4F8D8E3F389C95316433</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2003-01-28T22:51:38</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Scheduled_Tasks.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Scheduled Task' type='ItemType'>E5BC8090E82D4F8D8E3F389C95316433</id>
        <implementation_type>table</implementation_type>
        <instance_data>SCHEDULED_TASK</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Scheduled Task</keyed_name>
        <label xml:lang='en'>Scheduled Task</label>
        <label_plural xml:lang='en'>Scheduled Tasks</label_plural>
        <large_icon>../images/Icons/32x32/32x32_schedule3.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2005-07-15T11:31:47</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_calendar.gif</open_icon>
        <permission_id keyed_name='3D61D401EE97453D8ECE536AA94295E3' type='Permission'>3D61D401EE97453D8ECE536AA94295E3</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Scheduled Task</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='DA581F2EAD1641CF976A1F1211E1ADBA'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Process/search.gif</close_icon>
        <config_id keyed_name='Search' type='ItemType' name='Search'>DA581F2EAD1641CF976A1F1211E1ADBA</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Innovator Admin' type='User'>30B991F927274FA3829655F50C99472E</created_by_id>
        <created_on>2003-01-31T16:54:34</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Search' type='ItemType'>DA581F2EAD1641CF976A1F1211E1ADBA</id>
        <implementation_type>table</implementation_type>
        <instance_data>SEARCH</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Search</keyed_name>
        <label xml:lang='en'>Search</label>
        <label_plural xml:lang='en'>Searches</label_plural>
        <large_icon>../images/Process/search.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:17:07</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Process/search.gif</open_icon>
        <permission_id keyed_name='0DEE64C36FD84574A9C76070F8E7ABDC' type='Permission'>0DEE64C36FD84574A9C76070F8E7ABDC</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Search</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='D2ECF1B2A7FC45DB9D916996CE9BE9C9'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_properties.gif</close_icon>
        <config_id keyed_name='Search Criteria' type='ItemType' name='Search Criteria'>D2ECF1B2A7FC45DB9D916996CE9BE9C9</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Innovator Admin' type='User'>30B991F927274FA3829655F50C99472E</created_by_id>
        <created_on>2003-01-31T17:04:32</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Search Criteria' type='ItemType'>D2ECF1B2A7FC45DB9D916996CE9BE9C9</id>
        <implementation_type>table</implementation_type>
        <instance_data>SEARCH_CRITERIA</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Search Criteria</keyed_name>
        <label xml:lang='en'>Search Criteria</label>
        <label_plural xml:lang='en'>Search Criteria</label_plural>
        <large_icon>../images/Icons/32x32/32x32_check_list.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:17:24</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_properties.gif</open_icon>
        <permission_id keyed_name='8F958A9D0F0A40ED8FFB0364962414E0' type='Permission'>8F958A9D0F0A40ED8FFB0364962414E0</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Search Criteria</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='2B46201802CE46708C269667DB4798AC'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_sequence.gif</close_icon>
        <config_id keyed_name='Sequence' type='ItemType' name='Sequence'>2B46201802CE46708C269667DB4798AC</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:45:51</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Sequences.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Sequence' type='ItemType'>2B46201802CE46708C269667DB4798AC</id>
        <implementation_type>table</implementation_type>
        <instance_data>SEQUENCE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Sequence</keyed_name>
        <label xml:lang='en'>Sequence</label>
        <label_plural xml:lang='en'>Sequences</label_plural>
        <large_icon>../images/Icons/32x32/32x32_sequence.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2014-03-11T08:10:50</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_sequence.gif</open_icon>
        <permission_id keyed_name='E3FEC2C3458D4ACDA03E3AFBACCB5703' type='Permission'>E3FEC2C3458D4ACDA03E3AFBACCB5703</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Sequence</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='8214ECDE53F04AFC95243E10B2C7BBD4'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_arrow_right.gif</close_icon>
        <config_id keyed_name='Server Event' type='ItemType' name='Server Event'>8214ECDE53F04AFC95243E10B2C7BBD4</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:43:07</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Server Event' type='ItemType'>8214ECDE53F04AFC95243E10B2C7BBD4</id>
        <implementation_type>table</implementation_type>
        <instance_data>SERVER_EVENT</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Server Event</keyed_name>
        <label xml:lang='en'>Server Event</label>
        <label_plural xml:lang='en'>Server Events</label_plural>
        <large_icon>../images/Icons/32x32/32x32_method.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:56:54</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_arrow_right.gif</open_icon>
        <permission_id keyed_name='765AFC23A5B443089BD97395EE6A0368' type='Permission'>765AFC23A5B443089BD97395EE6A0368</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Server Event</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='D2794EA7FB7B4B52BA2CE4681E2D9DFB'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_users.gif</close_icon>
        <config_id keyed_name='State Distribution' type='ItemType' name='State Distribution'>D2794EA7FB7B4B52BA2CE4681E2D9DFB</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2003-11-06T11:24:43</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='State Distribution' type='ItemType'>D2794EA7FB7B4B52BA2CE4681E2D9DFB</id>
        <implementation_type>table</implementation_type>
        <instance_data>STATE_DISTRIBUTION</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>State Distribution</keyed_name>
        <label xml:lang='en'>State Distribution</label>
        <label_plural xml:lang='en'>State Distributions</label_plural>
        <large_icon>../images/Icons/32x32/32x32_identities.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:18:25</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_users.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>State Distribution</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='F0B2EAE5414249748F2986CC1EE78340'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/19x16_envelope.gif</close_icon>
        <config_id keyed_name='State EMail' type='ItemType' name='State EMail'>F0B2EAE5414249748F2986CC1EE78340</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2003-11-06T11:23:44</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='State EMail' type='ItemType'>F0B2EAE5414249748F2986CC1EE78340</id>
        <implementation_type>table</implementation_type>
        <instance_data>STATE_EMAIL</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>State EMail</keyed_name>
        <label xml:lang='en'>State EMail</label>
        <label_plural xml:lang='en'>State EMails</label_plural>
        <large_icon>../images/Icons/16x16/19x16_envelope.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:18:43</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/19x16_envelope.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>State EMail</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='F0CC0E7AEC0A401A94E8A63C9AC4F4D3'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/19x16_envelope.gif</close_icon>
        <config_id keyed_name='State Notification' type='ItemType' name='State Notification'>F0CC0E7AEC0A401A94E8A63C9AC4F4D3</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2003-06-13T09:39:11</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='State Notification' type='ItemType'>F0CC0E7AEC0A401A94E8A63C9AC4F4D3</id>
        <implementation_type>table</implementation_type>
        <instance_data>STATE_NOTIFICATION</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>State Notification</keyed_name>
        <label xml:lang='en'>State Notification</label>
        <label_plural xml:lang='en'>State Notifications</label_plural>
        <large_icon>../images/Icons/32x32/32x32_documenting.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:19:02</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/19x16_envelope.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>State Notification</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='38C9CE2A4E06401DABF942E1D0224E87'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_properties.gif</close_icon>
        <config_id keyed_name='TOC Access' type='ItemType' name='TOC Access'>38C9CE2A4E06401DABF942E1D0224E87</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-09-03T21:23:50</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='TOC Access' type='ItemType'>38C9CE2A4E06401DABF942E1D0224E87</id>
        <implementation_type>table</implementation_type>
        <instance_data>TOC_ACCESS</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>TOC Access</keyed_name>
        <label xml:lang='en'>TOC Access</label>
        <label_plural xml:lang='en'>TOC Access</label_plural>
        <large_icon>../images/Icons/32x32/32x32_check_list.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:57:20</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_properties.gif</open_icon>
        <permission_id keyed_name='1B7A4AD632DA4980B72A423682472431' type='Permission'>1B7A4AD632DA4980B72A423682472431</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>TOC Access</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='F356C4CED1584EBF812912F2D926066B'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_item.gif</close_icon>
        <config_id keyed_name='TOC View' type='ItemType' name='TOC View'>F356C4CED1584EBF812912F2D926066B</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2003-11-06T11:22:09</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='TOC View' type='ItemType'>F356C4CED1584EBF812912F2D926066B</id>
        <implementation_type>table</implementation_type>
        <instance_data>TOC_VIEW</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>TOC View</keyed_name>
        <label xml:lang='en'>TOC View</label>
        <label_plural xml:lang='en'>TOC View</label_plural>
        <large_icon>../images/Icons/32x32/32x32_form.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:57:33</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_item.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>TOC View</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='1DA22B3D6F12458290F8549165B490EC'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_users.gif</close_icon>
        <config_id keyed_name='Transition Distribution' type='ItemType' name='Transition Distribution'>1DA22B3D6F12458290F8549165B490EC</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2003-11-06T11:26:07</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Transition Distribution' type='ItemType'>1DA22B3D6F12458290F8549165B490EC</id>
        <implementation_type>table</implementation_type>
        <instance_data>TRANSITION_DISTRIBUTION</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Transition Distribution</keyed_name>
        <label xml:lang='en'>Transition Distribution</label>
        <label_plural xml:lang='en'>Transition Distributions</label_plural>
        <large_icon>../images/Icons/32x32/32x32_identities.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:20:16</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_users.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Transition Distribution</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='8E5FA57F5168436BA998A70CB2C7F259'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/19x16_envelope.gif</close_icon>
        <config_id keyed_name='Transition EMail' type='ItemType' name='Transition EMail'>8E5FA57F5168436BA998A70CB2C7F259</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2003-11-06T11:25:21</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Transition EMail' type='ItemType'>8E5FA57F5168436BA998A70CB2C7F259</id>
        <implementation_type>table</implementation_type>
        <instance_data>TRANSITION_EMAIL</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Transition EMail</keyed_name>
        <label xml:lang='en'>Transition EMail</label>
        <label_plural xml:lang='en'>Transition EMails</label_plural>
        <large_icon>../images/Icons/16x16/19x16_envelope.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2004-02-27T09:20:32</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/19x16_envelope.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Transition EMail</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='45E899CD2859442982EB22BB2DF683E5'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x16_user.gif</close_icon>
        <config_id keyed_name='User' type='ItemType' name='User'>45E899CD2859442982EB22BB2DF683E5</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:42:25</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Users.htm</help_url>
        <hide_where_used>1</hide_where_used>
        <id keyed_name='User' type='ItemType'>45E899CD2859442982EB22BB2DF683E5</id>
        <implementation_type>table</implementation_type>
        <instance_data>USER</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>User</keyed_name>
        <label xml:lang='en'>User</label>
        <label_plural xml:lang='en'>Users</label_plural>
        <large_icon>../images/Icons/32x32/32x32_user.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2013-06-11T15:55:16</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_user.gif</open_icon>
        <permission_id keyed_name='DCA0BC0221FA48399DB079F3B88A3909' type='Permission'>DCA0BC0221FA48399DB079F3B88A3909</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>User</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='122BF06C8B8E423A9931604DD939172F'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='UserMessage' type='ItemType' name='UserMessage'>122BF06C8B8E423A9931604DD939172F</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2005-07-15T11:29:05</created_on>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='UserMessage' type='ItemType'>122BF06C8B8E423A9931604DD939172F</id>
        <implementation_type>table</implementation_type>
        <instance_data>USERMESSAGE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>UserMessage</keyed_name>
        <label xml:lang='en'>UserMessage</label>
        <label_plural xml:lang='en'>User Messages</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:51:16</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>UserMessage</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='8651DCAB4D714EF6AA747BB8F50719BA'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_lists.gif</close_icon>
        <config_id keyed_name='Value' type='ItemType' name='Value'>8651DCAB4D714EF6AA747BB8F50719BA</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:41:15</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Value' type='ItemType'>8651DCAB4D714EF6AA747BB8F50719BA</id>
        <implementation_type>table</implementation_type>
        <instance_data>VALUE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Value</keyed_name>
        <label xml:lang='en'>Value</label>
        <label_plural xml:lang='en'>Values</label_plural>
        <large_icon>../images/Icons/32x32/32x32_lists.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2016-03-03T16:10:04</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_lists.gif</open_icon>
        <permission_id keyed_name='F77018634C7946EA8624629256DA8152' type='Permission'>F77018634C7946EA8624629256DA8152</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Value</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='6DAB4ACC09E6471DB4BDD15F36C3482B'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x16_variable.gif</close_icon>
        <config_id keyed_name='Variable' type='ItemType' name='Variable'>6DAB4ACC09E6471DB4BDD15F36C3482B</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:41:26</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Variables.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Variable' type='ItemType'>6DAB4ACC09E6471DB4BDD15F36C3482B</id>
        <implementation_type>table</implementation_type>
        <instance_data>VARIABLE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Variable</keyed_name>
        <label xml:lang='en'>Variable</label>
        <label_plural xml:lang='en'>Variables</label_plural>
        <large_icon>../images/icons/32x32/32x32_variable.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2014-09-11T10:37:43</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_variable.gif</open_icon>
        <permission_id keyed_name='28F75689077F4BCCB2185BB7DDC28553' type='Permission'>28F75689077F4BCCB2185BB7DDC28553</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Variable</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='8FC29FEF933641A09CEE13A604A9DC74'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_vault.gif</close_icon>
        <config_id keyed_name='Vault' type='ItemType' name='Vault'>8FC29FEF933641A09CEE13A604A9DC74</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-08-19T12:01:38</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Vaults.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Vault' type='ItemType'>8FC29FEF933641A09CEE13A604A9DC74</id>
        <implementation_type>table</implementation_type>
        <instance_data>VAULT</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Vault</keyed_name>
        <label xml:lang='en'>Vault</label>
        <label_plural xml:lang='en'>Vaults</label_plural>
        <large_icon>../images/icons/32x32/32x32_vault.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:51:46</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/icons/16x16/16x16_vault.gif</open_icon>
        <permission_id keyed_name='125634D4813641B39D5CF61BF6C4131A' type='Permission'>125634D4813641B39D5CF61BF6C4131A</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Vault</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='8EFCE40BCB74478B8254CEB594CE8774'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_file.gif</close_icon>
        <config_id keyed_name='View' type='ItemType' name='View'>8EFCE40BCB74478B8254CEB594CE8774</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-04-24T09:43:24</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='View' type='ItemType'>8EFCE40BCB74478B8254CEB594CE8774</id>
        <implementation_type>table</implementation_type>
        <instance_data>VIEW</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>View</keyed_name>
        <label xml:lang='en'>View</label>
        <label_plural xml:lang='en'>Item Views</label_plural>
        <large_icon>../images/Icons/32x32/32x32_file.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2006-08-03T14:20:25</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_file.gif</open_icon>
        <permission_id keyed_name='5BD12F13D13743C6971A6D9D015083DE' type='Permission'>5BD12F13D13743C6971A6D9D015083DE</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>View</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='2D700440AC084B99AD123528BAE67D29'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_had_up.gif</close_icon>
        <config_id keyed_name='View With' type='ItemType' name='View With'>2D700440AC084B99AD123528BAE67D29</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-09-17T23:29:27</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_View_With.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='View With' type='ItemType'>2D700440AC084B99AD123528BAE67D29</id>
        <implementation_type>table</implementation_type>
        <instance_data>VIEW_WITH</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>View With</keyed_name>
        <label xml:lang='en'>View With</label>
        <label_plural xml:lang='en'>View With</label_plural>
        <large_icon>../images/Icons/32x32/32x32_file_view1.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:57:42</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_had_up.gif</open_icon>
        <permission_id keyed_name='55E02448F0BF4F16B6882410AF4B88B4' type='Permission'>55E02448F0BF4F16B6882410AF4B88B4</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>View With</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='602D9828174C48EBA648B1D261C54E43'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_drawing.gif</close_icon>
        <config_id keyed_name='Viewer' type='ItemType' name='Viewer'>602D9828174C48EBA648B1D261C54E43</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-09-17T23:27:29</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Viewers.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Viewer' type='ItemType'>602D9828174C48EBA648B1D261C54E43</id>
        <implementation_type>table</implementation_type>
        <instance_data>VIEWER</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Viewer</keyed_name>
        <label xml:lang='en'>Viewer</label>
        <label_plural xml:lang='en'>Viewers</label_plural>
        <large_icon>../images/Icons/32x32/32x32_drawing.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2005-07-15T11:32:07</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_drawing.gif</open_icon>
        <permission_id keyed_name='5A34C130F9784D38B3CD97C533B2E087' type='Permission'>5A34C130F9784D38B3CD97C533B2E087</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Viewer</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='9E212D4ED3C64493B631EE15D0A62AF7'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Workflow' type='ItemType' name='Workflow'>9E212D4ED3C64493B631EE15D0A62AF7</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:31:17</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow' type='ItemType'>9E212D4ED3C64493B631EE15D0A62AF7</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Workflow</keyed_name>
        <label xml:lang='en'>Workflow</label>
        <label_plural xml:lang='en'>Workflows</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2014-08-18T08:34:47</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Workflow</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='B19D349CC6FC44BC97D50A6D70AE79CB'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x16_lifecycle.gif</close_icon>
        <config_id keyed_name='Workflow Map' type='ItemType' name='Workflow Map'>B19D349CC6FC44BC97D50A6D70AE79CB</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2002-09-09T11:56:14</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>75</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Workflow_Maps.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow Map' type='ItemType'>B19D349CC6FC44BC97D50A6D70AE79CB</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW_MAP</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>0</is_released>
        <is_versionable>1</is_versionable>
        <keyed_name>Workflow Map</keyed_name>
        <label xml:lang='en'>Workflow Map</label>
        <label_plural xml:lang='en'>Workflow Maps</label_plural>
        <large_icon>../images/Icons/32x32/32x32_lifecycle.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2014-09-11T10:40:11</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_lifecycle.gif</open_icon>
        <permission_id keyed_name='457A9E12377D43CF9BDD8451CDF9B4B7' type='Permission'>457A9E12377D43CF9BDD8451CDF9B4B7</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Workflow Map</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='42A80AD3F88443F785C005BAF2121E01'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x16_had_up.gif</close_icon>
        <config_id keyed_name='Workflow Map Activity' type='ItemType' name='Workflow Map Activity'>42A80AD3F88443F785C005BAF2121E01</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:27:56</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow Map Activity' type='ItemType'>42A80AD3F88443F785C005BAF2121E01</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW_MAP_ACTIVITY</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Workflow Map Activity</keyed_name>
        <label xml:lang='en'>Workflow Map Activity</label>
        <label_plural xml:lang='en'>Activities</label_plural>
        <large_icon>../images/Icons/32x32/32x32_check_list.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_on>2004-02-27T10:28:05</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_had_up.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Workflow Map Activity</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='97F00180EC8442B3A1CB67E6349D7BDE'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_lifecycle.gif</close_icon>
        <config_id keyed_name='Workflow Map Path' type='ItemType' name='Workflow Map Path'>97F00180EC8442B3A1CB67E6349D7BDE</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:29:23</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow Map Path' type='ItemType'>97F00180EC8442B3A1CB67E6349D7BDE</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW_MAP_PATH</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Workflow Map Path</keyed_name>
        <label xml:lang='en'>Workflow Map Path</label>
        <label_plural xml:lang='en'>Workflow Map Paths</label_plural>
        <large_icon>../images/Icons/32x32/32x32_lifecycle.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2015-02-18T15:16:16</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_lifecycle.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Workflow Map Path</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='AE7AC22E64D746B69F970EA1EC65DB05'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Workflow Map Path Post' type='ItemType' name='Workflow Map Path Post'>AE7AC22E64D746B69F970EA1EC65DB05</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:28:16</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow Map Path Post' type='ItemType'>AE7AC22E64D746B69F970EA1EC65DB05</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW_MAP_PATH_POST</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Workflow Map Path Post</keyed_name>
        <label xml:lang='en'>Workflow Map Path Post</label>
        <label_plural xml:lang='en'>Workflow Map Path Post Methods</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_on>2004-02-27T10:28:25</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Workflow Map Path Post</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='7937E5F0640240FDBBF9B158F45F4F6C'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Workflow Map Path Pre' type='ItemType' name='Workflow Map Path Pre'>7937E5F0640240FDBBF9B158F45F4F6C</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:28:06</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow Map Path Pre' type='ItemType'>7937E5F0640240FDBBF9B158F45F4F6C</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW_MAP_PATH_PRE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Workflow Map Path Pre</keyed_name>
        <label xml:lang='en'>Workflow Map Path Pre</label>
        <label_plural xml:lang='en'>Workflow Map Path Pre Methods</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_on>2004-02-27T10:28:15</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Workflow Map Path Pre</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='FE597F427BF84EC783435F4471520403'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <config_id keyed_name='Workflow Map Variable' type='ItemType' name='Workflow Map Variable'>FE597F427BF84EC783435F4471520403</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:27:45</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow Map Variable' type='ItemType'>FE597F427BF84EC783435F4471520403</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW_MAP_VARIABLE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Workflow Map Variable</keyed_name>
        <label xml:lang='en'>Workflow Map Variable</label>
        <label_plural xml:lang='en'>Workflow Map Variables</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:58:08</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Workflow Map Variable</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='261EAC08AE9144FC95C49182ACE0D3FE'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x16_lifecycle.gif</close_icon>
        <config_id keyed_name='Workflow Process' type='ItemType' name='Workflow Process'>261EAC08AE9144FC95C49182ACE0D3FE</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:31:28</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <help_url xml:lang='en'>mergedProjects/Just Ask Innovator/About_Workflow_Processes.htm</help_url>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow Process' type='ItemType'>261EAC08AE9144FC95C49182ACE0D3FE</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW_PROCESS</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>0</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Workflow Process</keyed_name>
        <label xml:lang='en'>Workflow Process</label>
        <label_plural xml:lang='en'>Workflow Processes</label_plural>
        <large_icon>../images/Icons/32x32/32x32_lifecycle.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Eric Domke' type='User'>2D246C5838644C1C8FD34F8D2796E327</modified_by_id>
        <modified_on>2014-07-01T08:33:08</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_lifecycle.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>0</use_src_access>
        <name>Workflow Process</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='F3F07BFCCDDF48E79ED239F0111E4710'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <close_icon>../images/Icons/16x16/16x16_had_up.gif</close_icon>
        <config_id keyed_name='Workflow Process Activity' type='ItemType' name='Workflow Process Activity'>F3F07BFCCDDF48E79ED239F0111E4710</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:31:59</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow Process Activity' type='ItemType'>F3F07BFCCDDF48E79ED239F0111E4710</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW_PROCESS_ACTIVITY</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Workflow Process Activity</keyed_name>
        <label xml:lang='en'>Workflow Process Activity</label>
        <label_plural xml:lang='en'>Workflow Process Activities</label_plural>
        <large_icon>../images/Icons/32x32/32x32_check_list.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_on>2004-02-27T10:32:08</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_had_up.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Workflow Process Activity</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='34682D3EB66141ECACC8796C9D3A42B8'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <close_icon>../images/Icons/16x16/16x16_lifecycle.gif</close_icon>
        <config_id keyed_name='Workflow Process Path' type='ItemType' name='Workflow Process Path'>34682D3EB66141ECACC8796C9D3A42B8</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:32:27</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow Process Path' type='ItemType'>34682D3EB66141ECACC8796C9D3A42B8</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW_PROCESS_PATH</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Workflow Process Path</keyed_name>
        <label xml:lang='en'>Workflow Process Path</label>
        <label_plural xml:lang='en'>Workflow Process Paths</label_plural>
        <large_icon>../images/Icons/32x32/32x32_lifecycle.gif</large_icon>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='Kenton Van Klompenberg' type='User'>68BA4B969D43484F8A933DC73AF84AD2</modified_by_id>
        <modified_on>2015-02-18T15:13:18</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <open_icon>../images/Icons/16x16/16x16_lifecycle.gif</open_icon>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Workflow Process Path</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='4245947FE6244E37982F46D2FA46D74E'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Workflow Process Path Post' type='ItemType' name='Workflow Process Path Post'>4245947FE6244E37982F46D2FA46D74E</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:32:53</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow Process Path Post' type='ItemType'>4245947FE6244E37982F46D2FA46D74E</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW_PROCESS_PATH_POST</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Workflow Process Path Post</keyed_name>
        <label xml:lang='en'>Workflow Process Path Post</label>
        <label_plural xml:lang='en'>Workflow Process Path Post Methods</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_on>2004-02-27T10:33:02</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Workflow Process Path Post</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='30F30E1181BA43FE99706038200EFEBF'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>0</auto_search>
        <config_id keyed_name='Workflow Process Path Pre' type='ItemType' name='Workflow Process Path Pre'>30F30E1181BA43FE99706038200EFEBF</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:32:42</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow Process Path Pre' type='ItemType'>30F30E1181BA43FE99706038200EFEBF</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW_PROCESS_PATH_PRE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Workflow Process Path Pre</keyed_name>
        <label xml:lang='en'>Workflow Process Path Pre</label>
        <label_plural xml:lang='en'>Workflow Process Path Pre</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_on>2004-02-27T10:32:51</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <structure_view>tab view</structure_view>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Workflow Process Path Pre</name>
      </Item>
      <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='40F45DEED2D84BA58B223F556FA25617'>
        <allow_private_permission>1</allow_private_permission>
        <auto_search>1</auto_search>
        <config_id keyed_name='Workflow Process Variable' type='ItemType' name='Workflow Process Variable'>40F45DEED2D84BA58B223F556FA25617</config_id>
        <core>1</core>
        <created_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
        <created_on>2004-02-27T10:31:44</created_on>
        <current_state name='Released' keyed_name='Released' type='Life Cycle State'>C363ABDADF8D485393BB89877DBDCFD0</current_state>
        <default_page_size>25</default_page_size>
        <enforce_discovery>0</enforce_discovery>
        <full_text_index>0</full_text_index>
        <generation>1</generation>
        <hide_where_used>0</hide_where_used>
        <id keyed_name='Workflow Process Variable' type='ItemType'>40F45DEED2D84BA58B223F556FA25617</id>
        <implementation_type>table</implementation_type>
        <instance_data>WORKFLOW_PROCESS_VARIABLE</instance_data>
        <is_cached>0</is_cached>
        <is_current>1</is_current>
        <is_dependent>0</is_dependent>
        <is_relationship>1</is_relationship>
        <is_released>1</is_released>
        <is_versionable>0</is_versionable>
        <keyed_name>Workflow Process Variable</keyed_name>
        <label xml:lang='en'>Workflow Process Variable</label>
        <label_plural xml:lang='en'>Process Variables</label_plural>
        <major_rev>A</major_rev>
        <manual_versioning>0</manual_versioning>
        <modified_by_id keyed_name='_Super User' type='User'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</modified_by_id>
        <modified_on>2011-08-03T16:58:37</modified_on>
        <new_version>0</new_version>
        <not_lockable>0</not_lockable>
        <permission_id keyed_name='ItemType' type='Permission'>102D29B8CD9948BFB5F558341DF4C0F9</permission_id>
        <revisions keyed_name='Default' type='Revision'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
        <show_parameters_tab>1</show_parameters_tab>
        <state>Released</state>
        <unlock_on_logout>0</unlock_on_logout>
        <use_src_access>1</use_src_access>
        <name>Workflow Process Variable</name>
      </Item>
    </Result>
  </SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
  }
}
