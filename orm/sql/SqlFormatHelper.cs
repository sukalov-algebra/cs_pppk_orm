using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace orm.sql
{
    public class SqlFormatHelper
    {
        public static string GetColumnName(PropertyInfo property)
        {
            return $"\"{property.Name}\"";
        }

        public static string GetAssignment(PropertyInfo property, object entity)
        {
            var column = $"\"{property.Name}\"";
            var value = GetSqlValue(property.GetValue(entity));

            return $"{column} = {value}";
        }

        public static string GetSqlValue(object? value)
        {
            if (value == null)
                return "NULL";

            return value switch
            {
                string s => $"'{s.Replace("'", "''")}'",

                char c => $"'{c.ToString().Replace("'", "''")}'",

                bool b => b ? "TRUE" : "FALSE",

                DateTime dt => $"'{dt:yyyy-MM-dd HH:mm:ss.ffffff}'",

                DateTimeOffset dto => $"'{dto:yyyy-MM-dd HH:mm:ss.ffffff zzz}'",

                Guid g => $"'{g}'",

                Enum e => Convert.ToInt32(e).ToString(),

                decimal d => d.ToString(CultureInfo.InvariantCulture),

                double d => d.ToString(CultureInfo.InvariantCulture),

                float f => f.ToString(CultureInfo.InvariantCulture),

                byte or sbyte or short or ushort or int or uint or long or ulong
                    => Convert.ToString(value, CultureInfo.InvariantCulture)!,

                _ => $"'{value.ToString()?.Replace("'", "''")}'"
            };
        }
    }
}
