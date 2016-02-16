using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public class CsvTableWriter : DelimitedTableWriter
  {
    /// <summary>
    /// The delimiter which separates fields of data within a record
    /// </summary>
    [DisplayName("Delimiter"), Description("The delimiter which separates fields of data within a record.")]
    protected override string Delimiter
    {
      get { return ","; }
    }

    /// <summary>
    /// Whether or not field values are enclosed in quotes
    /// </summary>
    [DisplayName("Enclose Field in Quotes"), Description("Whether or not field values are enclosed in quotes.")]
    protected override bool EncloseFieldsInQuotes
    {
      get { return true; }
    }
  }
}
