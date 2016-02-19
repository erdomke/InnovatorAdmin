using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
      _assembly16 = new IconInfo("assembly-16", LoadBitmapFromResource("Resources/TreeImages/assembly_16.png"));
      _attribute16 = new IconInfo("attribute-16", LoadBitmapFromResource("Resources/TreeImages/attribute_16.png"));
      _class16 = new IconInfo("class-16", LoadBitmapFromResource("Resources/TreeImages/class_16.png"));
      _enumValue16 = new IconInfo("enum-value-16", LoadBitmapFromResource("Resources/TreeImages/enum_value_16.png"));
      _field16 = new IconInfo("field-16", LoadBitmapFromResource("Resources/TreeImages/field_16.png"));
      _folder16 = new IconInfo("folder-16", LoadBitmapFromResource("Resources/TreeImages/folder_16.png"));
      _folderSpecial16 = new IconInfo("folder-special-16", LoadBitmapFromResource("Resources/TreeImages/folder_special_16.png"));
      _method16 = new IconInfo("method-16", LoadBitmapFromResource("Resources/TreeImages/method_16.png"));
      _methodFriend16 = new IconInfo("method-friend-16", LoadBitmapFromResource("Resources/TreeImages/method_friend_16.png"));
      _namespace16 = new IconInfo("namespace-16", LoadBitmapFromResource("Resources/TreeImages/namespace_16.png"));
      _operator16 = new IconInfo("operator-16", LoadBitmapFromResource("Resources/TreeImages/operator_16.png"));
      _property16 = new IconInfo("property-16", LoadBitmapFromResource("Resources/TreeImages/property_16.png"));
      _xmlTag16 = new IconInfo("xml-tag-16", LoadBitmapFromResource("Resources/TreeImages/xml_tag_16.png"));

      _all = new IconInfo[] { _assembly16, _attribute16, _class16, _enumValue16, _field16, _folder16, _folderSpecial16, _method16, _methodFriend16, _namespace16, _operator16, _property16, _xmlTag16 };
    }

    /// <summary>
    /// Load a resource WPF-BitmapImage (png, bmp, ...) from embedded resource defined as 'Resource' not as 'Embedded resource'.
    /// </summary>
    /// <param name="pathInApplication">Path without starting slash</param>
    /// <param name="assembly">Usually 'Assembly.GetExecutingAssembly()'. If not mentionned, I will use the calling assembly</param>
    /// <returns></returns>
    public static BitmapImage LoadBitmapFromResource(string pathInApplication, Assembly assembly = null)
    {
      if (assembly == null)
      {
        assembly = Assembly.GetCallingAssembly();
      }

      if (pathInApplication[0] == '/')
      {
        pathInApplication = pathInApplication.Substring(1);
      }
      return new BitmapImage(new Uri(@"pack://application:,,,/" + assembly.GetName().Name + ";component/" + pathInApplication, UriKind.Absolute));
    }
  }

  public struct IconInfo
  {
    private string _key;
    private BitmapImage _wpf;

    public string Key { get { return _key; } set { _key = value; } }
    public BitmapImage Wpf { get { return _wpf; } set { _wpf = value; } }

    public IconInfo(string key, BitmapImage img)
    {
      _key = key;
      _wpf = img;
    }
  }
}
