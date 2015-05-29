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
            if (result.isError()) return result.dom.DocumentElement.OuterXml;

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
      var lease = (ILease)base.InitializeLifetimeService();
      if (lease.CurrentState == LeaseState.Initial)
      {
        lease.InitialLeaseTime = TimeSpan.FromMinutes(60);
        lease.SponsorshipTimeout = TimeSpan.FromMinutes(2);
        lease.RenewOnCallTime = TimeSpan.FromMinutes(30);
      }
      return lease;
    }
  }
}
