using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Innovator.Client.Connection.Tests
{
  /// <summary>
  /// Tests from http://greenbytes.de/tech/tc/httpauth/#simplebasictok and interpretation of http://greenbytes.de/tech/webdav/draft-ietf-httpbis-p7-auth-latest.html#imported.abnf
  /// </summary>
  [TestClass()]
  public class AuthenticationSchemeTests
  {
    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasic()
    {
      var schemes = AuthenticationScheme.Parse("Basic realm=\"foo\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("foo", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasiclf()
    {
      var schemes = AuthenticationScheme.Parse("Basic\r\n realm=\"foo\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("foo", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasicucase()
    {
      var schemes = AuthenticationScheme.Parse("BASIC REALM=\"foo\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("foo", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasictok()
    {
      var schemes = AuthenticationScheme.Parse("Basic realm=foo").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("foo", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasictok_trailingws()
    {
      var schemes = AuthenticationScheme.Parse("Basic realm=foo  ").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("foo", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasictokbs()
    {
      try
      {
        var schemes = AuthenticationScheme.Parse(@"Basic realm=\f\o\o").ToList();
        Assert.Fail();
      }
      catch (FormatException) { }
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasicsq()
    {
      var schemes = AuthenticationScheme.Parse("Basic realm='foo'").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("'foo'", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasicpct()
    {
      var schemes = AuthenticationScheme.Parse("Basic realm=foo%20bar").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("foo%20bar", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasiccomma()
    {
      var schemes = AuthenticationScheme.Parse("Basic , realm=\"foo\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("foo", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasiccomma2()
    {
      try
      {
        var schemes = AuthenticationScheme.Parse("Basic, realm=\"foo\"").ToList();
        Assert.Fail();
      }
      catch (FormatException) { }
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasicnorealm()
    {
      var schemes = AuthenticationScheme.Parse("Basic").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(0, schemes[0].Parameters.Count);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasic2realms()
    {
      try
      {
        var schemes = AuthenticationScheme.Parse("Basic realm=\"foo\", realm=\"bar\"").ToList();
        Assert.Fail();
      }
      catch (ArgumentException) { }
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasicwsrealm()
    {
      var schemes = AuthenticationScheme.Parse("Basic realm = \"foo\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("foo", schemes[0].Parameters["realm"]);
    }


    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasicrealmsqc()
    {
      var schemes = AuthenticationScheme.Parse(@"Basic realm=""\f\o\o""").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("foo", schemes[0].Parameters["realm"]);
    }


    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasicrealmsqc2()
    {
      var schemes = AuthenticationScheme.Parse(@"Basic realm=""\""foo\""""").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("\"foo\"", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasicnewparam1()
    {
      var schemes = AuthenticationScheme.Parse("Basic realm=\"foo\", bar=\"xyz\",, a=b,,,c=d").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(4, schemes[0].Parameters.Count);
      Assert.AreEqual("foo", schemes[0].Parameters["realm"]);
      Assert.AreEqual("xyz", schemes[0].Parameters["bar"]);
      Assert.AreEqual("b", schemes[0].Parameters["a"]);
      Assert.AreEqual("d", schemes[0].Parameters["c"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasicnewparam2()
    {
      var schemes = AuthenticationScheme.Parse("Basic bar=\"xyz\", realm=\"foo\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(2, schemes[0].Parameters.Count);
      Assert.AreEqual("foo", schemes[0].Parameters["realm"]);
      Assert.AreEqual("xyz", schemes[0].Parameters["bar"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasicrealmiso88591()
    {
      var schemes = AuthenticationScheme.Parse("Basic realm=\"foo-ä\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("foo-ä", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasicrealmutf8()
    {
      var schemes = AuthenticationScheme.Parse("Basic realm=\"foo-Ã¤\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("foo-Ã¤", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_simplebasicrealmrfc2047()
    {
      var schemes = AuthenticationScheme.Parse("Basic realm=\"=?ISO-8859-1?Q?foo-=E4?=\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("=?ISO-8859-1?Q?foo-=E4?=", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_multibasicunknown()
    {
      var schemes = AuthenticationScheme.Parse("Basic realm=\"basic\", Newauth realm=\"newauth\"").ToList();
      Assert.AreEqual(2, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("basic", schemes[0].Parameters["realm"]);
      Assert.AreEqual("newauth", schemes[1].Name);
      Assert.AreEqual(1, schemes[1].Parameters.Count);
      Assert.AreEqual("newauth", schemes[1].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_multibasicunknown2()
    {
      var schemes = AuthenticationScheme.Parse("Newauth realm=\"newauth\", Basic realm=\"basic\"").ToList();
      Assert.AreEqual(2, schemes.Count);
      Assert.AreEqual("basic", schemes[1].Name);
      Assert.AreEqual(1, schemes[1].Parameters.Count);
      Assert.AreEqual("basic", schemes[1].Parameters["realm"]);
      Assert.AreEqual("newauth", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("newauth", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_multibasicempty()
    {
      var schemes = AuthenticationScheme.Parse(",Basic realm=\"basic\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("basic", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_multibasicqs()
    {
      var schemes = AuthenticationScheme.Parse("Newauth realm=\"apps\", type=1, title=\"Login to \\\"apps\\\"\", Basic realm=\"simple\"").ToList();
      Assert.AreEqual(2, schemes.Count);
      Assert.AreEqual("basic", schemes[1].Name);
      Assert.AreEqual(1, schemes[1].Parameters.Count);
      Assert.AreEqual("simple", schemes[1].Parameters["realm"]);
      Assert.AreEqual("newauth", schemes[0].Name);
      Assert.AreEqual(3, schemes[0].Parameters.Count);
      Assert.AreEqual("apps", schemes[0].Parameters["realm"]);
      Assert.AreEqual("1", schemes[0].Parameters["type"]);
      Assert.AreEqual("Login to \"apps\"", schemes[0].Parameters["title"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_multidisgscheme()
    {
      var schemes = AuthenticationScheme.Parse("Newauth realm=\"Newauth Realm\", basic=foo, Basic realm=\"Basic Realm\"").ToList();
      Assert.AreEqual(2, schemes.Count);
      Assert.AreEqual("basic", schemes[1].Name);
      Assert.AreEqual(1, schemes[1].Parameters.Count);
      Assert.AreEqual("Basic Realm", schemes[1].Parameters["realm"]);
      Assert.AreEqual("newauth", schemes[0].Name);
      Assert.AreEqual(2, schemes[0].Parameters.Count);
      Assert.AreEqual("Newauth Realm", schemes[0].Parameters["realm"]);
      Assert.AreEqual("foo", schemes[0].Parameters["basic"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_unknown()
    {
      var schemes = AuthenticationScheme.Parse("Newauth realm=\"newauth\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("newauth", schemes[0].Name);
      Assert.AreEqual(1, schemes[0].Parameters.Count);
      Assert.AreEqual("newauth", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_disguisedrealm()
    {
      var schemes = AuthenticationScheme.Parse("Basic foo=\"realm=nottherealm\", realm=\"basic\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(2, schemes[0].Parameters.Count);
      Assert.AreEqual("realm=nottherealm", schemes[0].Parameters["foo"]);
      Assert.AreEqual("basic", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_disguisedrealm2()
    {
      var schemes = AuthenticationScheme.Parse("Basic nottherealm=\"nottherealm\", realm=\"basic\"").ToList();
      Assert.AreEqual(1, schemes.Count);
      Assert.AreEqual("basic", schemes[0].Name);
      Assert.AreEqual(2, schemes[0].Parameters.Count);
      Assert.AreEqual("nottherealm", schemes[0].Parameters["nottherealm"]);
      Assert.AreEqual("basic", schemes[0].Parameters["realm"]);
    }

    [TestMethod()]
    public void AuthenticationSchemeParse_missingquote()
    {
      try
      {
        var schemes = AuthenticationScheme.Parse("Basic, realm=\"basic").ToList();
        Assert.Fail();
      }
      catch (FormatException) { }
    }
  }
}
