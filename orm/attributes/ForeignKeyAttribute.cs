using System;
using System.Collections.Generic;
using System.Text;

namespace orm.attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ForeignKeyAttribute : Attribute
    {
        public Type References { get; }

        public string Column { get; set; } = "Id";

        public string? OnDelete { get; set; }

        public string? OnUpdate { get; set; }

        public ForeignKeyAttribute(Type references) => References = references;
    }
}
