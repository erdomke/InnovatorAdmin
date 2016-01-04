using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace InnovatorAdmin.Editor
{
  public static class WpfImages
  {
    private static BitmapImage _assembly16;
    private static BitmapImage _attribute16;
    private static BitmapImage _class16;
    private static BitmapImage _enumValue16;
    private static BitmapImage _method16;
    private static BitmapImage _methodFriend16;
    private static BitmapImage _namespace16;
    private static BitmapImage _operator16;
    private static BitmapImage _property16;
    private static BitmapImage _xmlTag16;

    public static BitmapImage Assembly16
    {
      get { return _assembly16; }
    }
    public static BitmapImage Attribute16
    {
      get { return _attribute16; }
    }
    public static BitmapImage Class16
    {
      get { return _class16; }
    }
    public static BitmapImage EnumValue16
    {
      get { return _enumValue16; }
    }
    public static BitmapImage Method16
    {
      get { return _method16; }
    }
    public static BitmapImage MethodFriend16
    {
      get { return _methodFriend16; }
    }
    public static BitmapImage Namespace16
    {
      get { return _namespace16; }
    }
    public static BitmapImage Operator16
    {
      get { return _operator16; }
    }
    public static BitmapImage Property16
    {
      get { return _property16; }
    }
    public static BitmapImage XmlTag16
    {
      get { return _xmlTag16; }
    }

    static WpfImages()
    {
      _assembly16 = resources.TreeImages.assembly_16.ToWpf();
      _attribute16 = resources.TreeImages.attribute_16.ToWpf();
      _class16 = resources.TreeImages.class_16.ToWpf();
      _enumValue16 = resources.TreeImages.enum_value_16.ToWpf();
      _method16 = resources.TreeImages.method_16.ToWpf();
      _methodFriend16 = resources.TreeImages.method_friend_16.ToWpf();
      _namespace16 = resources.TreeImages.namespace_16.ToWpf();
      _operator16 = resources.TreeImages.operator_16.ToWpf();
      _property16 = resources.TreeImages.property_16.ToWpf();
      _xmlTag16 = resources.TreeImages.xml_tag_16.ToWpf();
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
}
