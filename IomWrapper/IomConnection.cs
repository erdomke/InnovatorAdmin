using Aras.IOM;
using Aras.Tools.InnovatorAdmin;
using Mvp.Xml.Common.XPath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Xml;

namespace IomWrapper
{
  public class IomConnection : MarshalByRefObject, IArasConnection
  {
    const string faultXPath = "/*[local-name()='Envelope' and (namespace-uri()='http://schemas.xmlsoap.org/soap/envelope/' or namespace-uri()='')]/*[local-name()='Body' and (namespace-uri()='http://schemas.xmlsoap.org/soap/envelope/' or namespace-uri()='')]/*[local-name()='Fault' and (namespace-uri()='http://schemas.xmlsoap.org/soap/envelope/' or namespace-uri()='')]";

    private Innovator _inn;
    private Item _userInfo;

    public IomConnection(Innovator inn)
    {
      _inn = inn;
    }

    public string CallAction(string action, string input)
    {
      return CallAction(action, input, null);
    }
    public string CallAction(string action, string input, IProgressCallback progressReporter)
    {
      XmlNode fault;
      XmlDocument outputDoc = null;
      var inputDoc = new XmlDocument();
      inputDoc.LoadXml(input);
      if (_userInfo == null)
        _userInfo = _inn.applyAML(string.Format("<AML><Item type='User' action='get' select='default_vault' expand='1'><id>{0}</id><Relationships><Item type='ReadPriority' action='get' select='priority, related_id' expand='1' orderBy='priority'/></Relationships></Item></AML>", _inn.getUserID()));

      if (action == "ApplyItem" || action == "ApplyAML")
      {
        var fileNodes = XPathCache.SelectNodes("descendant-or-self::Item[@type='File' and (@action='add' or @action='update' or @action='create') and actual_filename]", inputDoc.DocumentElement);
        XmlNode locatedNode;
        if (fileNodes.Count > 0)
        {
          Item fileItem = _inn.newItem();
          foreach (var fileNode in fileNodes.OfType<XmlElement>())
          {
            if (string.IsNullOrEmpty(fileNode.Attribute("id"))) fileNode.Attr("id", _inn.getNewID());
            fileNode.Elem("checkedout_path", Path.GetDirectoryName(fileNode.Element("actual_filename", "")));
            fileNode.Elem("filename", Path.GetFileName(fileNode.Element("actual_filename", "")));
            locatedNode = XPathCache.SelectSingleNode("Relationships/Item[@type='Located']/related_id", fileNode);
            if (locatedNode == null)
            {
              fileItem.dom = inputDoc;
              fileItem.node = (XmlElement)fileNode;
              fileItem.nodeList = null;
              fileItem.attachPhysicalFile(fileNode.Element("actual_filename", ""), _userInfo.getProperty("default_vault"));
            }
          }

          var firstItem = XPathCache.SelectSingleNode("//Item[1]", inputDoc.DocumentElement);
          IList<XmlElement> items;
          if (firstItem.ParentNode == null)
          {
            items = new XmlElement[] { (XmlElement)firstItem };
          }
          else
          {
            items = firstItem.Parent().Elements("Item").ToList();
          }

          Item result;
          XmlElement resultNode = null;

          for (var i = 0; i < items.Count; i++)
          {
            fileItem.dom = items[i].OwnerDocument;
            fileItem.node = items[i];
            fileItem.nodeList = null;
            result = fileItem.apply();
            fault = XPathCache.SelectSingleNode(faultXPath, result.dom.DocumentElement);
            if (fault != null)
            {
              fault.AppendChild(result.dom.CreateElement("original_query")).InnerText = input;
              return result.dom.DocumentElement.OuterXml;
            }
            else if (result.isError())
            {
              throw new InvalidOperationException();
            }

            if (outputDoc == null)
            {
              outputDoc = result.dom;
              resultNode = XPathCache.SelectSingleNode("//Item[1]", outputDoc.DocumentElement).Parent() as XmlElement;
            }
            else
            {
              resultNode.AppendChild(outputDoc.ImportNode(result.node, true));
            }

            if (progressReporter != null) progressReporter.ReportProgress(i + 1, items.Count);
          }

          return outputDoc.OuterXml;
        }
      }

      outputDoc = new XmlDocument();
      outputDoc.Elem("Empty");
      _inn.getConnection().CallAction(action, inputDoc, outputDoc);
      fault = XPathCache.SelectSingleNode(faultXPath, outputDoc.DocumentElement);
      if (fault != null)
      {
        fault.AppendChild(outputDoc.CreateElement("original_query")).InnerText = input;
      }
      return outputDoc.DocumentElement.OuterXml;
    }

    public string GetDatabaseName()
    {
      return _inn.getConnection().GetDatabaseName();
    }

    public string GetIomVersion()
    {
      return typeof(Aras.IOM.IomFactory).Assembly.GetName().Version.ToString();
    }

    public string GetUserId()
    {
      return _inn.getUserID();
    }

    public override object InitializeLifetimeService()
    {
      return null;
    }
  }
}
