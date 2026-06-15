using System;
using System.Collections.Generic;
using System.Text;

namespace orm.attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DefaultAttribute : Attribute
    {
        public object? Value { get; }
        public string? Raw { get; set; }

        public DefaultAttribute() { }
        public DefaultAttribute(object value) { Value = value; }

        public string ToSql()
        {
            if (!string.IsNullOrWhiteSpace(Raw)) return Raw!;

            return Value switch
            {
                null => "NULL",
                bool b => b ? "TRUE" : "FALSE",
                string s => $"'{s.Replace("'", "''")}'",
                char c => $"'{c.ToString().Replace("'", "''")}'",
                Enum e => Convert.ToInt64(e).ToString(),
                IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
                _ => $"'{Value.ToString()!.Replace("'", "''")}'"
            };
        }
    }
}
