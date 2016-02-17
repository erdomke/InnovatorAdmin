using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin.Testing
{
  public static class TestSerializer
  {
    // TODO: Figure out login/logout handling
    // TODO: Implement some sort of file download
    // TODO: Create tests that expect an error

    private static RNGCryptoServiceProvider _secureRandom = new RNGCryptoServiceProvider();

    public static TestSuite ReadTestSuite(TextReader reader)
    {
      using (var xml = new XmlTextReader(reader))
      {
        xml.WhitespaceHandling = WhitespaceHandling.All;
        return ReadTestSuite(xml);
      }
    }
    public static TestSuite ReadTestSuite(XmlReader reader)
    {
      var result = new TestSuite();
      string lastComment = null;
      string sessionId = null;

      while (!reader.EOF)
      {
        if (reader.NodeType == XmlNodeType.Comment)
        {
          lastComment = reader.Value;
          reader.Read();
        }
        else if (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.LocalName)
          {
            case "TestSuite":
              result.Comment = lastComment;
              lastComment = null;
              sessionId = reader.GetAttribute("sessionId");
              reader.Read();
              break;
            case "Init":
              using (var r = reader.ReadSubtree())
              {
                ReadItems<ICommand>(result.Init, r, sessionId, ReadCommand);
              }
              break;
            case "Tests":
              lastComment = null;
              while (!(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Tests"))
              {
                if (reader.NodeType == XmlNodeType.Comment)
                {
                  lastComment = reader.Value;
                  reader.Read();
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                  if (reader.LocalName == "Test")
                  {
                    var test = new Test()
                    {
                      Comment = lastComment,
                      Name = reader.GetAttribute("name")
                    };
                    using (var r = reader.ReadSubtree())
                    {
                      ReadItems<ITestCommand>(test.Commands, r, sessionId, ReadTestCommand);
                    }
                    AddPrints(test.Commands);
                    result.Tests.Add(test);
                  }
                  reader.Read();
                  lastComment = null;
                }
                else
                {
                  reader.Read();
                }
              }
              reader.Read();
              break;
            case "Cleanup":
              using (var r = reader.ReadSubtree())
              {
                ReadItems<ICommand>(result.Cleanup, r, sessionId, ReadCommand);
              }
              break;
            default:
              using (var r = reader.ReadSubtree())
              {
                while (!r.EOF)
                  r.Read();
              }
              break;
          }
        }
        else
        {
          reader.Read();
        }
      }
      return result;
    }

    private static void AddPrints(IList<ITestCommand> commands)
    {
      var foundAssert = false;
      for (var i = commands.Count - 1; i >= 0; i--)
      {
        if (commands[i] is Query)
        {
          if (!foundAssert)
            commands.Insert(i + 1, new PrintError());
          foundAssert = false;
        }
        else if (commands[i] is AssertMatch)
        {
          foundAssert = true;
        }
      }
    }

    private static void ReadItems<T>(IList<T> items, XmlReader reader, string sessionId, Func<XmlReader, string, string, T> itemReader)
    {
      reader.Read();
      reader.Read();
      T item;
      string lastComment = null;
      while (!reader.EOF)
      {
        if (reader.NodeType == XmlNodeType.Comment)
        {
          lastComment = reader.Value;
          reader.Read();
        }
        else if (reader.NodeType == XmlNodeType.Element)
        {
          item = itemReader(reader, lastComment, sessionId);
          if (item == null)
            reader.Read();
          else
            items.Add(item);
          lastComment = null;
        }
        else
        {
          reader.Read();
        }
      }
    }

    private static ICommand ReadCommand(XmlReader reader, string comment, string sessionId)
    {
      switch (reader.LocalName)
      {
        case "Login":
          var login = new Login()
          {
            Comment = comment,
            Database = reader.GetAttribute("database"),
            Url = reader.GetAttribute("url"),
            UserName = reader.GetAttribute("username"),
          }
          .SetType(reader.GetAttribute("type"))
          .SetPassword(reader.GetAttribute("password"), sessionId);
          reader.Read();
          return login;
        case "Logout":
          var logout = new Logout()
          {
            Comment = comment
          };
          reader.Read();
          return logout;
        case "Delay":
          var delay = new Delay()
          {
            BySeconds = reader.GetAttribute("by"),
            From = reader.GetAttribute("from"),
            Comment = comment
          };
          reader.Read();
          return delay;
        case "sql":
        case "SQL":
        case "Item":
        case "AML":
        case "GetNextSequence":
          return new Query()
          {
            Comment = comment,
            Text = reader.ReadOuterXml()
          };
        case "Param":
          bool isXml;
          var result = new ParamAssign()
          {
            Comment = comment,
            Name = reader.GetAttribute("name"),
            Select = reader.GetAttribute("select"),
            Value = ProcessXmlValue(reader.ReadInnerXml(), out isXml)
          };
          result.IsXml = isXml;
          return result;
      }
      return null;
    }

    private static string ProcessXmlValue(string value, out bool isXml)
    {
      isXml = IsXml(value);
      if (isXml || string.IsNullOrWhiteSpace(value))
        return value;

      using (var reader = new StringReader("<a>" + value + "</a>"))
      using (var xml = XmlReader.Create(reader))
      {
        xml.ReadToDescendant("a");
        xml.Read();
        return string.IsNullOrWhiteSpace(xml.Value) ? null : xml.Value;
      }
    }
    internal static bool IsXml(string value)
    {
      if (value == null)
        return false;

      var i = 0;
      while (true)
      {
        while (i < value.Length && char.IsWhiteSpace(value[i]))
          i++;
        if ((i + 3) >= value.Length)
          return false;
        if (value[i] == '<')
        {
          if (value[i + 1] == '!' && value[i + 2] == '-' && value[i + 3] == '-')
          {
            i = value.IndexOf("-->", i + 3);
            if (i < 0)
              return false;
            else
              i += 3;
          }
          else
          {
            return true;
          }
        }
        else
        {
          return false;
        }
      }
    }

    private static ITestCommand ReadTestCommand(System.Xml.XmlReader reader, string comment, string sessionId)
    {
      var cmd = ReadCommand(reader, comment, sessionId);
      if (cmd != null)
        return cmd;

      switch (reader.LocalName)
      {
        case "AssertMatch":
          var result = new AssertMatch()
          {
            Comment = comment,
            Match = reader.GetAttribute("match")
          };
          var removeSys = reader.GetAttribute("removeSysProps");
          result.RemoveSystemProperties = (removeSys != "0");

          using (var subReader = reader.ReadSubtree())
          {
            subReader.Read();
            while (!subReader.EOF)
            {
              if (reader.NodeType == XmlNodeType.Element)
              {
                switch (reader.LocalName)
                {
                  case "Remove":
                    result.Removes.Add(reader.GetAttribute("match"));
                    subReader.Read();
                    break;
                  case "Expected":
                    bool isXml;
                    result.Expected = ProcessXmlValue(reader.ReadInnerXml(), out isXml);
                    result.IsXml = isXml;
                    break;
                  default:
                    subReader.Read();
                    break;
                }
              }
              else
              {
                subReader.Read();
              }
            }
          }
          reader.Read();
          return result;
        case "DownloadFile":
          return new DownloadFile()
          {
            Comment = comment,
            Text = reader.ReadInnerXml()
          };
      }
      return null;
    }

    public static void Write(this TestSuite suite, TextWriter writer)
    {
      using (var visitor = new TestWriter(writer))
      {
        visitor.Visit(suite);
      }
    }

    private class TestWriter : ITestVisitor, IDisposable
    {
      private XmlWriter _xml;
      private string _sessionId;

      private TestWriter()
      {
        var data = new byte[24];
        _secureRandom.GetNonZeroBytes(data);
        _sessionId = Convert.ToBase64String(data);
      }
      public TestWriter(XmlWriter writer) : this()
      {
        _xml = writer;
      }
      public TestWriter(TextWriter writer) : this()
      {
        var xml = new XmlTextWriter(writer);
        xml.Indentation = 2;
        xml.IndentChar = ' ';
        xml.Formatting = Formatting.Indented;
        xml.QuoteChar = '\'';
        _xml = xml;
      }

      public void Visit(AssertMatch match)
      {
        if (!string.IsNullOrWhiteSpace(match.Comment))
          _xml.WriteComment(match.Comment);
        _xml.WriteStartElement("AssertMatch");
        if (!string.IsNullOrWhiteSpace(match.Match))
          _xml.WriteAttributeString("match", match.Match);
        if (!match.RemoveSystemProperties)
          _xml.WriteAttributeString("removeSysProps", "0");
        foreach (var remove in match.Removes)
        {
          _xml.WriteStartElement("Remove");
          _xml.WriteAttributeString("match", remove);
          _xml.WriteEndElement();
        }
        if (string.IsNullOrWhiteSpace(match.Expected))
        {
          if (!string.IsNullOrEmpty(match.Actual))
          {
            _xml.WriteStartElement("Actual");
            WriteFormatted(match.Actual, match.IsXml, _xml);
            _xml.WriteEndElement();
          }
        }
        else
        {
          _xml.WriteStartElement("Expected");
          WriteFormatted(match.Expected, match.IsXml, _xml);
          _xml.WriteEndElement();
        }
        _xml.WriteEndElement();
      }
      public void Visit(Delay delay)
      {
        if (!string.IsNullOrWhiteSpace(delay.Comment))
          _xml.WriteComment(delay.Comment);
        _xml.WriteStartElement("Delay");
        if (!string.IsNullOrEmpty(delay.From))
          _xml.WriteAttributeString("from", delay.From);
        if (!string.IsNullOrEmpty(delay.BySeconds))
          _xml.WriteAttributeString("by", delay.BySeconds);
        _xml.WriteEndElement();
      }
      public void Visit(DownloadFile download)
      {
        if (!string.IsNullOrWhiteSpace(download.Comment))
          _xml.WriteComment(download.Comment);
        _xml.WriteStartElement("DownloadFile");
        WriteFormatted(download.Text, true, _xml);
        _xml.WriteEndElement();
      }
      public void Visit(Login login)
      {
        if (!string.IsNullOrWhiteSpace(login.Comment))
          _xml.WriteComment(login.Comment);
        _xml.WriteStartElement("Login");
        _xml.WriteAttributeString("type", login.Type.ToString());
        if (!string.IsNullOrEmpty(login.Url))
          _xml.WriteAttributeString("url", login.Url);
        if (!string.IsNullOrEmpty(login.Database))
          _xml.WriteAttributeString("database", login.Database);
        if (!string.IsNullOrEmpty(login.UserName))
          _xml.WriteAttributeString("username", login.UserName);
        if (login.HasPassword)
          _xml.WriteAttributeString("password", login.GetEncryptedPassword(_sessionId));
        _xml.WriteEndElement();
      }
      public void Visit(Logout logout)
      {
        if (!string.IsNullOrWhiteSpace(logout.Comment))
          _xml.WriteComment(logout.Comment);
        _xml.WriteStartElement("Logout");
        _xml.WriteEndElement();
      }
      public void Visit(ParamAssign param)
      {
        if (!string.IsNullOrWhiteSpace(param.Comment))
          _xml.WriteComment(param.Comment);
        _xml.WriteStartElement("Param");
        if (!string.IsNullOrWhiteSpace(param.Name))
          _xml.WriteAttributeString("name", param.Name);
        if (!string.IsNullOrWhiteSpace(param.Select))
          _xml.WriteAttributeString("select", param.Select);
        if (string.IsNullOrEmpty(param.Value))
        {
          if (!string.IsNullOrEmpty(param.ActualValue)) _xml.WriteComment(param.ActualValue);
        }
        else
        {
          WriteFormatted(param.Value, param.IsXml, _xml);
        }
        _xml.WriteEndElement();
      }
      public void Visit(PrintError print)
      {
        if (!string.IsNullOrWhiteSpace(print.Comment))
          _xml.WriteComment(print.Comment);
        if (print.ErrorNode != null)
          print.ErrorNode.WriteTo(_xml);
      }
      public void Visit(Query query)
      {
        if (!string.IsNullOrWhiteSpace(query.Comment))
          _xml.WriteComment(query.Comment);
        WriteFormatted(query.Text, true, _xml);
      }
      public void Visit(Test test)
      {
        if (!string.IsNullOrWhiteSpace(test.Comment))
          _xml.WriteComment(test.Comment);
        _xml.WriteStartElement("Test");
        if (!string.IsNullOrWhiteSpace(test.Name))
          _xml.WriteAttributeString("name", test.Name);
        foreach (var cmd in test.Commands)
          cmd.Visit(this);
        _xml.WriteEndElement();
      }
      public void Visit(TestRun run)
      {
        _xml.WriteStartElement("Run");
        if (!string.IsNullOrWhiteSpace(run.Name))
          _xml.WriteAttributeString("name", run.Name);
        _xml.WriteAttributeString("result", run.Result.ToString());
        if (run.ElapsedMilliseconds > 0)
          _xml.WriteAttributeString("elapsedMs", run.ElapsedMilliseconds.ToString());
        _xml.WriteAttributeString("start", run.Start.ToString("s"));
        if (run.ErrorLine > 0)
          _xml.WriteAttributeString("errorLine", run.ErrorLine.ToString());
        if (!string.IsNullOrWhiteSpace(run.Message))
          _xml.WriteAttributeString("message", run.Message);
        _xml.WriteEndElement();
      }
      public void Visit(TestSuite suite)
      {
        if (!string.IsNullOrWhiteSpace(suite.Comment))
          _xml.WriteComment(suite.Comment);
        _xml.WriteStartElement("TestSuite");
        _xml.WriteAttributeString("sessionId", _sessionId);
        if (suite.Results.Any())
        {
          _xml.WriteStartElement("Results");
          foreach (var cmd in suite.Results)
            cmd.Visit(this);
          _xml.WriteEndElement();
        }
        if (suite.Output.Any())
        {
          _xml.WriteStartElement("Out");
          foreach (var cmd in suite.Output)
            cmd.Visit(this);
          _xml.WriteEndElement();
        }

        if (suite.Init.Any())
        {
          _xml.WriteStartElement("Init");
          foreach (var cmd in suite.Init)
            cmd.Visit(this);
          _xml.WriteEndElement();
        }
        if (suite.Tests.Any())
        {
          _xml.WriteStartElement("Tests");
          foreach (var cmd in suite.Tests)
            cmd.Visit(this);
          _xml.WriteEndElement();
        }
        if (suite.Cleanup.Any())
        {
          _xml.WriteStartElement("Cleanup");
          foreach (var cmd in suite.Cleanup)
            cmd.Visit(this);
          _xml.WriteEndElement();
        }
        _xml.WriteEndElement();
      }
      private static void WriteFormatted(string value, bool isXml, XmlWriter writer)
      {
        if (!string.IsNullOrWhiteSpace(value))
        {
          if (isXml)
          {
            char[] writeNodeBuffer = null;
            using (var reader = new XmlTextReader(value, XmlNodeType.Element, null))
            {
              reader.WhitespaceHandling = WhitespaceHandling.Significant;

              bool canReadValueChunk = reader.CanReadValueChunk;
              while (reader.Read())
              {
                switch (reader.NodeType)
                {
                  case XmlNodeType.Element:
                    writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                    writer.WriteAttributes(reader, false);
                    if (reader.IsEmptyElement)
                    {
                      writer.WriteEndElement();
                    }
                    break;
                  case XmlNodeType.Text:
                    if (canReadValueChunk)
                    {
                      if (writeNodeBuffer == null)
                      {
                        writeNodeBuffer = new char[1024];
                      }
                      int count;
                      while ((count = reader.ReadValueChunk(writeNodeBuffer, 0, 1024)) > 0)
                      {
                        writer.WriteChars(writeNodeBuffer, 0, count);
                      }
                    }
                    else
                    {
                      value = reader.Value;
                      writer.WriteString(value);
                    }
                    break;
                  case XmlNodeType.CDATA:
                    writer.WriteCData(reader.Value);
                    break;
                  case XmlNodeType.EntityReference:
                    writer.WriteEntityRef(reader.Name);
                    break;
                  case XmlNodeType.ProcessingInstruction:
                  case XmlNodeType.XmlDeclaration:
                    writer.WriteProcessingInstruction(reader.Name, reader.Value);
                    break;
                  case XmlNodeType.Comment:
                    writer.WriteComment(reader.Value);
                    break;
                  case XmlNodeType.DocumentType:
                    writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                    break;
                  //case XmlNodeType.Whitespace:
                  case XmlNodeType.SignificantWhitespace:
                    writer.WriteWhitespace(reader.Value);
                    break;
                  case XmlNodeType.EndElement:
                    writer.WriteFullEndElement();
                    break;
                }
              }
            }
          }
          else
          {
            writer.WriteString(value);
          }
        }
      }

      public void Dispose()
      {
        _xml.Flush();
        _xml.Dispose();
      }
    }
  }
}
