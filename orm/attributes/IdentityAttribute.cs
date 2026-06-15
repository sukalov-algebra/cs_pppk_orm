using System;
using System.Collections.Generic;
using System.Text;

namespace orm.attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IdentityAttribute : Attribute
    {
        public bool Always { get; set; } = false;
    }
}
