using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
    public interface ICriteria
    {
        /// <summary>
        /// The database field name or pipe-delimited list of field names
        /// </summary>
        string DbField { get; set; }
        Type FieldType { get; set; }
        ComparisonType Comparison { get; set; }
        object Value { get; set; }
    }
}
