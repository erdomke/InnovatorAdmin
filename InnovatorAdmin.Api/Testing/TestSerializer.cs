using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin.Testing
{
  public static class TestSerializer
  {

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
              reader.Read();
              break;
            case "Init":
              using (var r = reader.ReadSubtree())
              {
                ReadItems<ICommand>(result.Init, r, ReadCommand);
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
                      ReadItems<ITestCommand>(test.Commands, r, ReadTestCommand);
                    }
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
                ReadItems<ICommand>(result.Cleanup, r, ReadCommand);
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

    private static void ReadItems<T>(IList<T> items, XmlReader reader, Func<XmlReader, string, T> itemReader)
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
          item = itemReader(reader, lastComment);
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

    private static ICommand ReadCommand(System.Xml.XmlReader reader, string comment)
    {
      switch (reader.LocalName)
      {
        case "Login":
          return new Login()
          {
            Comment = comment,
            Database = reader.GetAttribute("database"),
            Password = reader.GetAttribute("password"),
            Url = reader.GetAttribute("url"),
            UserName = reader.GetAttribute("username")
          };
        case "Logout":
          return new Logout()
          {
            Comment = comment
          };
        case "sql":
        case "SQL":
        case "Item":
        case "AML":
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
      isXml = false;
      if (string.IsNullOrWhiteSpace(value))
        return value;
      if (value.TrimStart()[0] == '<')
      {
        isXml = true;
        return value;
      }

      using (var reader = new StringReader("<a>" + value + "</a>"))
      using (var xml = XmlReader.Create(reader))
      {
        xml.ReadToDescendant("a");
        xml.Read();
        return xml.Value;
      }
    }

    private static ITestCommand ReadTestCommand(System.Xml.XmlReader reader, string comment)
    {
      var cmd = ReadCommand(reader, comment);
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
      }
      return null;
    }

    public static void Write(this AssertMatch match, XmlWriter writer)
    {
      if (!string.IsNullOrWhiteSpace(match.Comment))
        writer.WriteComment(match.Comment);
      writer.WriteStartElement("AssertMatch");
      if (!string.IsNullOrWhiteSpace(match.Match))
        writer.WriteAttributeString("match", match.Match);
      if (!match.RemoveSystemProperties)
        writer.WriteAttributeString("removeSysProps", "0");
      foreach (var remove in match.Removes)
      {
        writer.WriteStartElement("Remove");
        writer.WriteAttributeString("match", remove);
        writer.WriteEndElement();
      }
      if (string.IsNullOrWhiteSpace(match.Expected))
      {
        if (!string.IsNullOrEmpty(match.Actual))
        {
          writer.WriteStartElement("Actual");
          WriteFormatted(match.Actual, match.IsXml, writer);
          writer.WriteEndElement();
        }
      }
      else
      {
        writer.WriteStartElement("Expected");
        WriteFormatted(match.Expected, match.IsXml, writer);
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }
    public static void Write(this Login login, XmlWriter writer)
    {
      if (!string.IsNullOrWhiteSpace(login.Comment))
        writer.WriteComment(login.Comment);
      writer.WriteStartElement("Login");

      writer.WriteEndElement();
    }
    public static void Write(this ParamAssign param, XmlWriter writer)
    {
      if (!string.IsNullOrWhiteSpace(param.Comment))
        writer.WriteComment(param.Comment);
      writer.WriteStartElement("Param");
      if (!string.IsNullOrWhiteSpace(param.Name))
        writer.WriteAttributeString("name", param.Name);
      if (!string.IsNullOrWhiteSpace(param.Select))
        writer.WriteAttributeString("select", param.Select);
      if (string.IsNullOrEmpty(param.Value))
      {
        if (!string.IsNullOrEmpty(param.ActualValue)) writer.WriteComment(param.ActualValue);
      }
      else
      {
        WriteFormatted(param.Value, param.IsXml, writer);
      }
      writer.WriteEndElement();
    }
    public static void Write(this Query query, XmlWriter writer)
    {
      if (!string.IsNullOrWhiteSpace(query.Comment))
        writer.WriteComment(query.Comment);
      WriteFormatted(query.Text, true, writer);
    }
    public static void Write(this Test test, XmlWriter writer)
    {
      if (!string.IsNullOrWhiteSpace(test.Comment))
        writer.WriteComment(test.Comment);
      writer.WriteStartElement("Test");
      if (!string.IsNullOrWhiteSpace(test.Name))
        writer.WriteAttributeString("name", test.Name);
      foreach (var cmd in test.Commands)
        Write(cmd, writer);
      writer.WriteEndElement();
    }
    public static void Write(this TestRun run, XmlWriter writer)
    {
      writer.WriteStartElement("Run");
      if (!string.IsNullOrWhiteSpace(run.Name))
        writer.WriteAttributeString("name", run.Name);
      writer.WriteAttributeString("result", run.Result.ToString());
      if (run.ElapsedMilliseconds > 0)
        writer.WriteAttributeString("elapsedMs", run.ElapsedMilliseconds.ToString());
      writer.WriteAttributeString("start", run.Start.ToString("s"));
      if (run.ErrorLine > 0)
        writer.WriteAttributeString("errorLine", run.ErrorLine.ToString());
      if (!string.IsNullOrWhiteSpace(run.Message))
        writer.WriteAttributeString("message", run.Message);
      writer.WriteEndElement();
    }
    public static void Write(this TestSuite suite, TextWriter writer)
    {
      using (var xml = new XmlTextWriter(writer))
      {
        xml.Indentation = 2;
        xml.IndentChar = ' ';
        xml.Formatting = Formatting.Indented;
        xml.QuoteChar = '\'';

        suite.Write(xml);
        xml.Flush();
      }
    }
    public static void Write(this TestSuite suite, XmlWriter writer)
    {
      if (!string.IsNullOrWhiteSpace(suite.Comment))
        writer.WriteComment(suite.Comment);
      writer.WriteStartElement("TestSuite");
      if (suite.Results.Any())
      {
        writer.WriteStartElement("Results");
        foreach (var cmd in suite.Results)
          Write(cmd, writer);
        writer.WriteEndElement();
      }
      if (suite.Output.Any())
      {
        writer.WriteStartElement("Out");
        foreach (var cmd in suite.Output)
          Write(cmd, writer);
        writer.WriteEndElement();
      }

      if (suite.Init.Any())
      {
        writer.WriteStartElement("Init");
        foreach (var cmd in suite.Init)
          Write(cmd, writer);
        writer.WriteEndElement();
      }
      if (suite.Tests.Any())
      {
        writer.WriteStartElement("Tests");
        foreach (var cmd in suite.Tests)
          Write(cmd, writer);
        writer.WriteEndElement();
      }
      if (suite.Cleanup.Any())
      {
        writer.WriteStartElement("Cleanup");
        foreach (var cmd in suite.Cleanup)
          Write(cmd, writer);
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
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
    private static void Write(ITestCommand cmd, XmlWriter writer)
    {
      var assert = cmd as AssertMatch;
      if (assert != null)
        assert.Write(writer);
      else
        Write((ICommand)cmd, writer);
    }
    private static void Write(ICommand cmd, XmlWriter writer)
    {
      var param = cmd as ParamAssign;
      if (param != null)
        param.Write(writer);

      var query = cmd as Query;
      if (query != null)
        query.Write(writer);
    }
  }
}
