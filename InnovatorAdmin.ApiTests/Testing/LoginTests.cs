using Microsoft.VisualStudio.TestTools.UnitTesting;
using InnovatorAdmin.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Testing.Tests
{
  [TestClass()]
  public class LoginTests
  {
    [TestMethod()]
    public void GetEncryptedPasswordTest()
    {
      var password = "pass123";
      var sessionId = "session123";
      var login = new Login();
      login.SetPassword(password, sessionId);
      var encrypted = login.GetEncryptedPassword(sessionId);
      login.SetPassword(encrypted, sessionId);
      var newEncrypted = login.GetEncryptedPassword(sessionId);
      Assert.AreEqual(encrypted, newEncrypted);
    }
  }
}
