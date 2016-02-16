using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
    public class ComparisonItem : ICriteria
    {
        /// <summary>
        /// The database field name or pipe-delimited list of field names
        /// </summary>
        public string DbField { get; set; }
        public Type FieldType { get; set; }
        public ComparisonType Comparison { get; set; }
        /// <summary>
        /// The name of the field for displaying in the user interface
        /// </summary>
        public string UiField { get; set; }
        public object Value { get; set; }

        public ComparisonItem()
        {
            FieldType = typeof(string);
        }
        public ComparisonItem(string fieldName, ComparisonType comparison, object value)
            : base()
        {
            this.DbField = fieldName;
            this.Comparison = comparison;
            this.Value = value;
        }
        public ComparisonItem(string fieldName, Type fieldType, ComparisonType comparison, object value)
            : this(fieldName, comparison, value)
        {
            this.FieldType = fieldType;
        }

        public override string ToString()
        {
            string returnVal = string.Empty;
            switch (Comparison)
            {
                case ComparisonType.GreaterThan:
                    returnVal += ">";
                    break;
                case ComparisonType.GreaterThanEqual:
                    returnVal += ">=";
                    break;
                case ComparisonType.LessThan:
                    returnVal += "<";
                    break;
                case ComparisonType.LessThanEqual:
                    returnVal += "<=";
                    break;
                case ComparisonType.NotEqual:
                    returnVal += "<>";
                    break;
            }
            if (Value != null)
            {
                returnVal += Value.ToString();
            }
            return returnVal;
        }
    }
}
