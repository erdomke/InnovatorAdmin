#region MethodTemplate

#r "C:\Users\eric.domke\Documents\Code\InnovatorAdmin\IOM\11_5\IOM.dll"
#r "System.Xml"
#r "System.Data"

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Globalization;
using Aras.IOM;


  public class ItemMethod : Item
  {
    public ItemMethod(IServerConnection arg) : base(arg)
    {
    }

    public Item methodCode()
    {
      //Aras.Server.Core.CallContext CCO = ((Aras.Server.Core.IOMConnection) serverConnection).CCO;
#endregion
/*****``METHOD_CODE``*****/
#region MethodTemplate
    }
  }
  
  public static class MethodCaller
  {
    public static string Exec(
      IServerConnection InnovatorServerASP,
      string aml
    )
    {
      ItemMethod inItem = null;
      Item outItem = null;
      var inDom = new XmlDocument();
      inDom.LoadXml(aml);
      inItem = new ItemMethod(InnovatorServerASP);
      inItem.dom = inDom;
      XmlNodeList nodes = inDom.SelectNodes("//Item[not(ancestor::node()[local-name()='Item'])]");
      if (nodes.Count == 1)
        inItem.node = (XmlElement)nodes[0];
      else
      {
        inItem.node = null;
        inItem.nodeList = nodes;
      }

      outItem = inItem.methodCode();
      return outItem.dom.OuterXml;
    }
  }

/*****``URL_PARAMS``*****/
var conn = IomFactory.CreateHttpServerConnection(__param_url, __param_db, __param_user, __param_pass);
MethodCaller.Exec(conn, "<Item/>").Dump();

#endregion