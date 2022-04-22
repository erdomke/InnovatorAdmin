using System;
using System.Collections.Generic;

namespace InnovatorAdmin.Cmd
{
  class ExportAllType
  {
    public string Name { get; }
    public Version MinVersion { get; }
    public Version MaxVersion { get; }

    public ExportAllType(string itemType, Version minVersion = null, Version maxVersion = null)
    {
      Name = itemType;
      MinVersion = minVersion;
      MaxVersion = maxVersion;
    }

    public bool Applies(Version version)
    {
      return ((MinVersion?.Major ?? 0) < 1 || version >= MinVersion)
        && ((MaxVersion?.Major ?? 0) < 1 || version <= MaxVersion);
    }

    public override string ToString()
    {
      if (string.Equals(Name, "Identity", StringComparison.OrdinalIgnoreCase))
        return "<Item type='Identity' action='get' select='config_id'><or><is_alias>0</is_alias><id condition='in'>'DBA5D86402BF43D5976854B8B48FCDD1','E73F43AD85CD4A95951776D57A4D517B'</id></or></Item>";
      return $"<Item type='{Name}' action='get' select='config_id'></Item>";
    }

    public static IEnumerable<ExportAllType> Types { get; } = new List<ExportAllType>()
    {
      new ExportAllType("Action"),
      new ExportAllType("Chart"),
      new ExportAllType("cmf_ContentType", new Version(11, 0, 5)),
      new ExportAllType("cmf_Style", new Version(11, 0, 5)),
      new ExportAllType("cmf_TabularView", new Version(11, 0, 5)),
      new ExportAllType("cmf_TabularViewHeaderRow", new Version(11, 0, 5)),
      new ExportAllType("CommandBarButton", new Version(11, 0, 5)),
      new ExportAllType("CommandBarDropDown", new Version(11, 0, 5)),
      new ExportAllType("CommandBarEdit", new Version(11, 0, 9)),
      new ExportAllType("CommandBarMenu", new Version(11, 0, 9)),
      new ExportAllType("CommandBarMenuButton", new Version(11, 0, 9)),
      new ExportAllType("CommandBarMenuCheckbox", new Version(11, 0, 9)),
      new ExportAllType("CommandBarMenuSeparator", new Version(11, 0, 9)),
      new ExportAllType("CommandBarSection", new Version(11, 0, 5)),
      new ExportAllType("CommandBarSeparator", new Version(11, 0, 5)),
      new ExportAllType("CommandBarShortcut", new Version(11, 0, 9)),
      new ExportAllType("ConversionRule", new Version(11, 0, 5)),
      new ExportAllType("ConversionServer", new Version(11, 0, 0)),
      new ExportAllType("ConverterType", new Version(11, 0, 5)),
      new ExportAllType("cui_Control", new Version(12, 0, 0)),
      new ExportAllType("cui_Event", new Version(12, 0, 0)),
      new ExportAllType("cui_Location", new Version(12, 0, 0)),
      new ExportAllType("cui_WindowSection", new Version(12, 0, 0)),
      new ExportAllType("Dashboard"),
      new ExportAllType("EMail Message"),
      new ExportAllType("FileType"),
      new ExportAllType("Form"),
      new ExportAllType("GlobalPresentationConfig"),
      new ExportAllType("Grid"),
      new ExportAllType("History Action"),
      new ExportAllType("History Template"),
      new ExportAllType("Identity"),
      new ExportAllType("ItemType"),
      new ExportAllType("Language"),
      new ExportAllType("Life Cycle Map"),
      new ExportAllType("List"),
      new ExportAllType("Locale"),
      new ExportAllType("Measurement Unit", new Version(11, 0, 5)),
      new ExportAllType("Method"),
      new ExportAllType("Metric"),
      new ExportAllType("mp_MacPolicy", new Version(11, 0, 12)),
      new ExportAllType("mp_PolicyAccessEnvAttribute", new Version(11, 0, 12)),
      new ExportAllType("Permission"),
      new ExportAllType("Permission_ExplicitDefine", new Version(11, 0, 12)),
      new ExportAllType("Permission_ItemClassification", new Version(11, 0, 12)),
      new ExportAllType("Permission_PropertyValue", new Version(11, 0, 12)),
      new ExportAllType("PreferenceTypes"),
      new ExportAllType("PresentationConfiguration", new Version(11, 0)),
      new ExportAllType("qry_QueryDefinition", new Version(11, 0, 12)),
      new ExportAllType("rb_TreeGridViewDefinition", new Version(11, 0, 12)),
      new ExportAllType("Report"),
      new ExportAllType("Revision"),
      new ExportAllType("SearchMode"),
      new ExportAllType("SecureMessageViewTemplate", new Version(11, 0)),
      new ExportAllType("SelfServiceReportHelp", new Version(11, 0, 5)),
      new ExportAllType("Sequence"),
      new ExportAllType("SQL"),
      new ExportAllType("SSVCPresentationConfiguration", new Version(11, 0)),
      new ExportAllType("SystemFileContainer", new Version(11, 0)),
      new ExportAllType("tp_XmlSchema", new Version(11, 0, 5)),
      new ExportAllType("UserMessage"),
      new ExportAllType("Variable"),
      new ExportAllType("Vault"),
      new ExportAllType("Viewer"),
      new ExportAllType("Workflow Map"),
      new ExportAllType("xClassificationTree", new Version(11, 0, 12)),
      new ExportAllType("xPropertyDefinition", new Version(11, 0, 12)),

      new ExportAllType("ES_IndexedConfiguration"),
      new ExportAllType("MSO_CommonSettings"),
      new ExportAllType("MSO_Preferences"),
      new ExportAllType("MSO_Reference")
    };
  }
}
