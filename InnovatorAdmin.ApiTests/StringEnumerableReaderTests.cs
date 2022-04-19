using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
namespace InnovatorAdmin.Tests
{
  [TestClass()]
  public class StringEnumerableReaderTests
  {
    public string ReadChunked(TextReader reader, int chunkSize)
    {
      var builder = new StringBuilder();
      var buffer = new char[chunkSize];
      var cnt = reader.Read(buffer, 0, chunkSize);
      while (cnt > 0)
      {
        builder.Append(buffer, 0, cnt);
        cnt = reader.Read(buffer, 0, chunkSize);
      }
      return builder.ToString();
    }

    [TestMethod()]
    public void StringEnumerableReaderTest()
    {
      var strings = new string[] { "first", "another item", "third thing", "stuff", "1234", "a really long string"};
      var expected = "firstanother itemthird thingstuff1234a really long string";
      Assert.AreEqual(expected, ReadChunked(new StringEnumerableReader(strings), 1));
      Assert.AreEqual(expected, ReadChunked(new StringEnumerableReader(strings), 3));
      Assert.AreEqual(expected, ReadChunked(new StringEnumerableReader(strings), 7));
      Assert.AreEqual(expected, ReadChunked(new StringEnumerableReader(strings), 19));
      Assert.AreEqual(expected, ReadChunked(new StringEnumerableReader(strings), 38));
      Assert.AreEqual(expected, ReadChunked(new StringEnumerableReader(strings), 999));
    }
  }
}
