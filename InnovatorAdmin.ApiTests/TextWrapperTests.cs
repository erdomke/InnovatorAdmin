using Innovator.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace InnovatorAdmin.Tests
{
  [TestClass]
  public class TextWrapperTests
  {
    private void RunTest(string input, int maxWidth, int indent, string expected)
    {
      var writer = new StringWriter();
      var wrapper = new TextWrapper(writer)
      {
        MaxWidth = maxWidth
      };
      wrapper.IncreaseIndent(indent);
      wrapper.Write(input);
      Assert.AreEqual(expected, wrapper.ToString());
    }

    private void JoinList(string separator, IEnumerable<string> items, int maxWidth)
    {
      var writer = new StringWriter();
      var wrapper = new TextWrapper(writer)
      {
        MaxWidth = maxWidth
      };

      var first = true;
      foreach (var item in items)
      {
        if (first)
          first = false;
        else
          wrapper.Write(separator);

        wrapper.Write(item);
      }

      Assert.AreEqual(string.Join(separator, items), wrapper.ToString());
    }

    [TestMethod]
    public void WriteInLoop1()
    {
      JoinList(", ", new[] { "item", "thing", "another" }, 80);
    }

    [TestMethod]
    public void WriteInLoop2()
    {
      JoinList(" | ", new[] { "item", "thing", "another" }, 80);
    }

    [TestMethod]
    public void ExtraSpacesAreTreatedAsNonBreaking()
    {
      RunTest("here is some text                                                          with some extra spacing"
          , 20, 0
          , @"here is some text
with some extra
spacing");
    }

    [TestMethod]
    public void IndentWorksCorrectly()
    {
      RunTest(@"line1
line2"
          , int.MaxValue, 2
          , @"  line1
  line2");
    }

    [TestMethod]
    public void LongWordsAreBroken()
    {
      RunTest(@"here is some text that contains a veryLongWordThatWontFitOnASingleLine"
          , 20, 0
          , @"here is some text
that contains a
veryLongWordThatWont
FitOnASingleLine");
    }

    [TestMethod]
    public void LongWordsAreBrokenOnHyphen()
    {
      RunTest("here is some text that contains a veryLongWordThat-WontFitOnASingle-LineVeryWell"
          , 20, 0
          , @"here is some text
that contains a
veryLongWordThat-
WontFitOnASingle-
LineVeryWell");
    }

    [TestMethod]
    public void LongWordsAreBrokenOnSpace()
    {
      RunTest("here is some text that contains a veryLongWordThat\xA0WontFitOnASingleLine"
          , 20, 0
          , @"here is some text
that contains a
veryLongWordThat
WontFitOnASingleLine");
    }

    [TestMethod]
    public void NegativeColumnWidthStillProducesOutput()
    {
      RunTest("test"
          , -1, 0
          , @"t
e
s
t");
    }

    [TestMethod]
    public void SimpleWrappingIsAsExpected()
    {
      RunTest(@"here is some text that needs wrapping"
          , 10, 0
          , @"here is
some text
that needs
wrapping");
    }

    [TestMethod]
    public void SimpleWrappingWithPrefix()
    {
      var writer = new StringWriter();
      var wrapper = new TextWrapper(writer)
      {
        MaxWidth = 12
      };
      wrapper.IncreaseIndent("# ");
      wrapper.Write(@"here is some text that needs wrapping");
      Assert.AreEqual(@"# here is
# some text
# that needs
# wrapping", wrapper.ToString());
    }

    [TestMethod]
    public void SingleColumnStillProducesOutputForSubIndentation()
    {
      RunTest(@"test
    ind"
          , -1, 0
          , @"t
e
s
t
i
n
d");
    }

    [TestMethod]
    public void SpacesWithinStringAreRespected()
    {
      RunTest("here     is some text with some extra spacing"
          , 20, 0
          , @"here     is some
text with some extra
spacing");
    }

    [TestMethod]
    public void SubIndentationCorrectlyWrapsWhenColumnWidthRequiresIt()
    {
      RunTest(@"test
    indented"
          , 6, 0
          , @"test
    in
    de
    nt
    ed");
    }

    [TestMethod]
    public void SubIndentationIsPreservedWhenBreakingWords()
    {
      RunTest("here is some text that contains \n  a veryLongWordThatWontFitOnASingleLine"
          , 20, 0
          , @"here is some text
that contains
  a
  veryLongWordThatWo
  ntFitOnASingleLine");
    }

    [TestMethod]
    public void WrappingAvoidsBreakingWords()
    {
      RunTest(@"here hippopotamus is some text that needs wrapping"
          , 15, 0
          , @"here
hippopotamus is
some text that
needs wrapping");
    }

    [TestMethod]
    public void WrappingExtraSpacesObeySubIndent()
    {
      RunTest("here is some\n   text                                                          with some extra spacing"
          , 20, 0
          , @"here is some
   text
   with some extra
   spacing");
    }

    [TestMethod]
    public void WrappingObeysLineBreaksOfAllStyles()
    {
      RunTest("here is some text\nthat needs\r\nwrapping"
          , 20, 0
          , @"here is some text
that needs
wrapping");
    }

    [TestMethod]
    public void WrappingPreservesSubIndentation()
    {
      RunTest("here is some text\n   that needs wrapping where we want the wrapped part to preserve indentation\nand this part to not be indented"
          , 20, 0
          , @"here is some text
   that needs
   wrapping where we
   want the wrapped
   part to preserve
   indentation
and this part to not
be indented");
    }
  }
}
