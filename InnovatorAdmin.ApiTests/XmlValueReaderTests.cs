using InnovatorAdmin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin.Tests
{
  [TestClass()]
  public class XmlValueReaderTests
  {
    public void RunTest(string expected)
    {
      var stringReader = new StringReader(expected);
      var reader = XmlReader.Create(stringReader);
      reader.Read();
      reader.Read();
      var valueReader = new XmlValueReader(reader);
      var content = valueReader.ReadToEnd();
      var actual = "<value>" + content + "</value>";
      Assert.AreEqual(expected, actual.Replace("\n", "\r\n"));
    }

    [TestMethod()]
    public void XmlValueReaderTest()
    {
      RunTest("<value>   </value>");
      RunTest("<value> a thing </value>");
      RunTest(_xml);
    }

    private const string _xml = @"<value>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus porta risus ac dapibus ullamcorper. Vivamus facilisis tellus a elit ornare, at pulvinar lacus facilisis. Curabitur cursus convallis rutrum. Quisque pretium malesuada luctus. Aenean in nibh sollicitudin, blandit ipsum eget, posuere nulla. Fusce massa mauris, cursus non hendrerit eu, venenatis tincidunt justo. In id augue eget est scelerisque bibendum. In placerat velit nisi, sed lobortis metus pellentesque sed.

Morbi et pharetra nulla. Praesent vitae justo odio. Quisque ac ornare enim. Duis semper est in viverra facilisis. Sed accumsan, diam ac eleifend maximus, leo turpis posuere metus, pretium consectetur nisi augue eget neque. Nunc accumsan felis a felis dignissim, eget tincidunt augue aliquet. Mauris pulvinar volutpat sem. Aenean porttitor at libero dapibus pulvinar. Sed lobortis, mi in dapibus vehicula, sem erat aliquam justo, non maximus elit elit at metus. Vivamus at vulputate risus, nec porta ante. Mauris magna turpis, cursus sed metus quis, feugiat rutrum felis. Donec id odio nibh. Aliquam ornare dictum bibendum. Sed tempus ligula vitae nibh imperdiet, ac maximus augue vestibulum. Maecenas eget risus sit amet ex efficitur facilisis. Aliquam in aliquet sem, ut mattis justo.

Vivamus congue eleifend lacinia. Morbi laoreet sem non scelerisque placerat. Quisque a vehicula enim. Sed auctor a orci a laoreet. Nulla consectetur tincidunt odio id interdum. Donec elit lacus, sodales in vehicula ac, tempor id ipsum. In vulputate congue orci id congue. Ut sit amet commodo nisl. Sed iaculis felis scelerisque tincidunt tempor. Proin et enim mollis tellus egestas consequat. Vivamus fermentum efficitur consequat. Aenean mollis justo a risus pharetra eleifend. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae;

Suspendisse hendrerit ex massa, sit amet condimentum lectus ultricies eget. In egestas sapien congue, ullamcorper nibh quis, aliquam dui. Pellentesque mollis, ligula id ultrices auctor, tortor purus tincidunt justo, vitae dictum nisi ligula pharetra felis. Nulla facilisi. Nullam viverra viverra elit eget faucibus. Curabitur tempus ullamcorper sapien, quis tempor enim ultrices tincidunt. Curabitur convallis lacus massa, in pellentesque lacus consectetur at. Aenean porttitor congue pellentesque. Nulla sed magna facilisis, egestas lacus vel, imperdiet elit. Integer lobortis consectetur dui id elementum. Aliquam imperdiet, est eu molestie semper, risus mi posuere ligula, id semper orci ante nec dui. Vestibulum tellus ipsum, pretium nec consectetur et, iaculis ac nisl. Cras felis massa, convallis et purus a, rutrum maximus mauris. Donec quis mi in dolor lobortis pellentesque.

Nulla sit amet quam eu nulla tristique placerat. Nullam porttitor velit non augue tincidunt, sit amet ultrices augue dapibus. Aliquam quam ante, gravida in pulvinar eget, venenatis id mi. Cras ornare maximus ante sit amet aliquam. Nunc lobortis lorem sed tempor malesuada. In hac habitasse platea dictumst. Nullam consequat ut lorem ut posuere. Aliquam consectetur eu magna quis sollicitudin. Nulla facilisi.

Fusce scelerisque placerat lacus. Aenean vel eros viverra, faucibus neque eget, facilisis est. In sed odio venenatis, efficitur ex sit amet, laoreet felis. Donec ullamcorper, mi vel efficitur hendrerit, est augue blandit est, ultrices commodo elit neque sit amet ligula. Morbi viverra eros ut tristique pellentesque. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Duis sit amet sem quam. Fusce porttitor, sem sed fringilla laoreet, risus libero commodo felis, vitae ullamcorper enim lorem quis nulla. Mauris in fringilla odio.

Donec quis velit ante. Phasellus id magna vulputate, tincidunt tortor sodales, rutrum urna. Nam condimentum magna sed ligula vehicula, in volutpat velit laoreet. Mauris ut augue at odio bibendum malesuada. In sollicitudin commodo nisl, sit amet egestas justo auctor at. Nam molestie sit amet erat a lacinia. Duis sem ante, pharetra vehicula odio porttitor, gravida hendrerit leo. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Vestibulum ut lacinia mauris. Pellentesque molestie consectetur est, nec sodales nisi pretium ac. Aliquam rhoncus molestie sem, sed aliquet ante ornare at. Vestibulum eu viverra magna. Vestibulum luctus hendrerit dolor et faucibus. Pellentesque fringilla nulla ac urna vulputate eleifend. Etiam ullamcorper commodo diam, non euismod enim sagittis aliquam. Mauris ligula massa, gravida sed sapien sit amet, placerat venenatis velit.

In porta sollicitudin orci. Maecenas non imperdiet justo, a euismod nisl. Morbi ut ipsum vel urna vehicula malesuada. In viverra viverra nisi et mattis. Pellentesque sodales, justo sed ornare tincidunt, velit nibh pharetra eros, ut viverra purus eros eget nisi. Quisque ac eleifend quam, id fringilla tortor. Mauris non aliquam libero. Maecenas feugiat sed ante ac convallis.

Donec venenatis eros eget accumsan tempus. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc pellentesque enim sed tortor finibus, ut tincidunt tortor facilisis. Aliquam fermentum accumsan porta. Nam hendrerit est non eros efficitur, vel condimentum leo suscipit. Nulla condimentum orci at ligula dapibus, quis mattis mi euismod. Integer a fringilla libero. Pellentesque mattis, arcu sed mollis facilisis, tortor leo luctus turpis, in ullamcorper ante magna nec augue. Curabitur interdum, ligula non placerat tincidunt, neque neque lobortis mi, ut efficitur justo sem vitae ex. Nam elementum tincidunt ante ac congue. Suspendisse euismod in nulla eget eleifend. Nulla placerat non sapien sit amet venenatis. Donec et tincidunt est, id dapibus justo. Aenean rhoncus metus nisi, at posuere lacus dictum vel. Nunc pulvinar sodales semper.

Vestibulum nulla nisl, consectetur id ex eu, varius ultrices turpis. Aliquam rhoncus elementum faucibus. Integer at ullamcorper ipsum, in lacinia elit. Quisque at nisl sed sem pellentesque vehicula quis vitae risus. Praesent iaculis vulputate nisi, a tincidunt quam placerat nec. Sed quis tempor nulla. Aliquam tempus eros dolor. Vestibulum tincidunt tempor diam eu lacinia. Ut rutrum, purus ac dapibus feugiat, neque justo tincidunt erat, eget varius arcu nisl aliquam felis. Aliquam vitae leo nisl. Ut laoreet erat vitae condimentum vestibulum. Quisque metus nisl, elementum vel tortor ut, convallis sodales magna. Proin nisl ante, pharetra nec enim id, sagittis placerat enim.

Nunc quis lectus ut arcu laoreet volutpat. Vivamus id dui tellus. Nunc ut odio augue. Ut varius sodales tempus. Morbi facilisis nec arcu in tincidunt. Praesent venenatis molestie faucibus. In pellentesque arcu in ante sollicitudin accumsan sed et neque. Ut fermentum convallis dui, vel vehicula ipsum venenatis a. Nam magna tellus, suscipit non egestas id, tempor sit amet lorem.

Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Donec a elit quam. Quisque at dolor ac enim egestas dictum. Vivamus nec lorem placerat, mattis mi sed, imperdiet urna. Duis lacinia non diam et lacinia. Proin feugiat sit amet purus commodo iaculis. Nullam non pellentesque ligula. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Nunc aliquet lectus non urna euismod, eget pulvinar libero ullamcorper. Nullam ut enim eu dui convallis tempus ut at purus. Quisque lacinia et sem in dignissim. Maecenas tincidunt sapien accumsan orci ultrices, non condimentum dui aliquam.

In pulvinar dolor sed mauris interdum, eget lacinia quam mollis. Nunc luctus diam sit amet fermentum interdum. Cras dapibus nunc ac orci tincidunt vulputate. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Mauris volutpat malesuada neque ut ultrices. Duis at congue lacus, in fermentum elit. Proin pulvinar magna nisi, non ornare arcu semper sed. Aenean convallis erat sed nibh euismod tincidunt. Nunc bibendum interdum sem convallis pellentesque. Nunc placerat eros id nibh vulputate dignissim non ut lacus. Cras vel finibus quam. Nulla finibus nisi at turpis vehicula cras amet.</value>";
  }
}
