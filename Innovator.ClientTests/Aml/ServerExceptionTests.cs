using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Innovator.Client.Tests
{
  [TestClass()]
  public class ServerExceptionTests
  {
    [TestMethod()]
    public void VerifySerialization()
    {
      var factory = ElementFactory.Local;

      var ex = new ServerException(null, "A bad exception");
      var expected = ex.ToString();
      ex = SerializeException<ServerException>(ex);
      Assert.AreEqual(expected, ex.ToString());

      ex = factory.NoItemsFoundException("Part", "<query />");
      expected = ex.ToString();
      ex = SerializeException<ServerException>(ex);
      Assert.AreEqual(expected, ex.ToString());

      ex = factory.ValidationException("Missing Properties", factory.Item(factory.Type("Method")), "owned_by_id", "managed_by_id");
      expected = ex.ToString();
      ex = SerializeException<ServerException>(ex);
      Assert.AreEqual(expected, ex.ToString());
    }

    private T SerializeException<T>(Exception ex) where T : Exception
    {
      var bf = new BinaryFormatter();
      using (var ms = new MemoryStream())
      {
        // "Save" object state
        bf.Serialize(ms, ex);

        // Re-use the same stream for de-serialization
        ms.Seek(0, 0);

        // Replace the original exception with de-serialized one
        return (T)bf.Deserialize(ms);
      }
    }
  }
}
