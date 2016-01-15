using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace InnovatorAdmin.Editor
{
  public static class Icons
  {
    private static IconInfo _assembly16;
    private static IconInfo _attribute16;
    private static IconInfo _class16;
    private static IconInfo _enumValue16;
    private static IconInfo _field16;
    private static IconInfo _folder16;
    private static IconInfo _folderSpecial16;
    private static IconInfo _method16;
    private static IconInfo _methodFriend16;
    private static IconInfo _namespace16;
    private static IconInfo _operator16;
    private static IconInfo _property16;
    private static IconInfo _xmlTag16;
    private static IEnumerable<IconInfo> _all;

    public static IEnumerable<IconInfo> All
    {
      get { return _all; }
    }

    public static IconInfo Assembly16
    {
      get { return _assembly16; }
    }
    public static IconInfo Attribute16
    {
      get { return _attribute16; }
    }
    public static IconInfo Class16
    {
      get { return _class16; }
    }
    public static IconInfo EnumValue16
    {
      get { return _enumValue16; }
    }
    public static IconInfo Field16
    {
      get { return _field16; }
    }
    public static IconInfo Folder16
    {
      get { return _folder16; }
    }
    public static IconInfo FolderSpecial16
    {
      get { return _folderSpecial16; }
    }
    public static IconInfo Method16
    {
      get { return _method16; }
    }
    public static IconInfo MethodFriend16
    {
      get { return _methodFriend16; }
    }
    public static IconInfo Namespace16
    {
      get { return _namespace16; }
    }
    public static IconInfo Operator16
    {
      get { return _operator16; }
    }
    public static IconInfo Property16
    {
      get { return _property16; }
    }
    public static IconInfo XmlTag16
    {
      get { return _xmlTag16; }
    }

    static Icons()
    {
      _assembly16 = new IconInfo("assembly-16", resources.TreeImages.assembly_16);
      _attribute16 = new IconInfo("attribute-16", resources.TreeImages.attribute_16);
      _class16 = new IconInfo("class-16", resources.TreeImages.class_16);
      _enumValue16 = new IconInfo("enum-value-16", resources.TreeImages.enum_value_16);
      _field16 = new IconInfo("field-16", resources.TreeImages.field_16);
      _folder16 = new IconInfo("folder-16", resources.TreeImages.folder_16);
      _folderSpecial16 = new IconInfo("folder-special-16", resources.TreeImages.folder_special_16);
      _method16 = new IconInfo("method-16", resources.TreeImages.method_16);
      _methodFriend16 = new IconInfo("method-friend-16", resources.TreeImages.method_friend_16);
      _namespace16 = new IconInfo("namespace-16", resources.TreeImages.namespace_16);
      _operator16 = new IconInfo("operator-16", resources.TreeImages.operator_16);
      _property16 = new IconInfo("property-16", resources.TreeImages.property_16);
      _xmlTag16 = new IconInfo("xml-tag-16", resources.TreeImages.xml_tag_16);

      _all = new IconInfo[] { _assembly16, _attribute16, _class16, _enumValue16, _field16, _folder16, _folderSpecial16, _method16, _methodFriend16, _namespace16, _operator16, _property16, _xmlTag16 };
    }


    public static BitmapImage ToWpf(this System.Drawing.Image img)
    {
      var ms = new MemoryStream();
      img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
      var result = new BitmapImage();
      result.BeginInit();
      ms.Seek(0, SeekOrigin.Begin);
      result.StreamSource = ms;
      result.EndInit();

      return result;
    }
  }

  public struct IconInfo
  {
    private System.Drawing.Image _gdi;
    private string _key;
    private BitmapImage _wpf;

    public System.Drawing.Image Gdi { get { return _gdi; } set { _gdi = value; } }
    public string Key { get { return _key; } set { _key = value; } }
    public BitmapImage Wpf { get { return _wpf; } set { _wpf = value; } }

    public IconInfo(string key, System.Drawing.Image img)
    {
      _gdi = img;
      _key = key;
      _wpf = img.ToWpf();
    }
  }
}
