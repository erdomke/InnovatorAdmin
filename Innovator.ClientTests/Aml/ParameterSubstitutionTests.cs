using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.Tests
{
  [TestClass()]
  public class ParameterSubstitutionTests
  {
    [TestMethod()]
    public void CSharpSubstitute_SimpleTest()
    {
      var sub = new ParameterSubstitution()
      {
        Style = ParameterStyle.CSharp
      };
      sub.AddIndexedParameters(new object[] { "first & second > third", true, new DateTime(2015, 1, 1) });
      var newQuery = sub.Substitute("<Item><name>{0}</name><is_current>{1}</is_current><date>{2:yyyy-MM-dd}</date></Item>", ElementFactory.Local.LocalizationContext);

      Assert.AreEqual("<Item><name>first &amp; second &gt; third</name><is_current>1</is_current><date>2015-01-01T00:00:00</date></Item>", newQuery);
    }

#if NET46
    [TestMethod()]
    public void InterpolatedString_SimpleTest()
    {
      var name = "first & second > third";
      var isCurrent = true;
      var date = new DateTime(2015, 1, 1);

      Assert.AreEqual("<Item><name>first &amp; second &gt; third</name><is_current>1</is_current><date>2015-01-01T00:00:00</date></Item>",
        new Command((FormattableString)$"<Item><name>{name}</name><is_current>{isCurrent}</is_current><date>{date:yyyy-MM-dd}</date></Item>").ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }
#endif
  }
}
