using System;
using System.Collections.Generic;
using System.Text;

namespace orm.attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UniqueAttribute : Attribute
    {
        public string? Group { get; set; }
    }
}
