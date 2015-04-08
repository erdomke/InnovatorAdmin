using Aras.IOM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin.Connections
{
  public class DatabaseConverter : StringConverter
  {
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
      //true means show a combobox
      return true;
    }

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
    {
      //true will limit to list. false will show the list, but allow free-form entry
      return false;
    }

    public override System.ComponentModel.TypeConverter.StandardValuesCollection
           GetStandardValues(ITypeDescriptorContext context)
    {
      var connData = context.Instance as ConnectionData;
      if (connData != null && !string.IsNullOrEmpty(connData.Url))
      {
        var conn = IomFactory.CreateHttpServerConnection(connData.Url, string.Empty, string.Empty, string.Empty);
        return new StandardValuesCollection(conn.GetDatabases());
      }
      else
      {
        return new StandardValuesCollection(new String[] { });
      }
    }
  }
}
